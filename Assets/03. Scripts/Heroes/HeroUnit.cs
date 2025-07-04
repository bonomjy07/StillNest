using System.Collections;
using System.Diagnostics.CodeAnalysis;
using UnityEditor;
using UnityEngine;

public partial class HeroUnit : MonoBehaviour
{
    [Header("[Movement]")]
    [SerializeField] protected float _moveSpeed = 2f;
    
    [Header("[Attack]")]
    [SerializeField] protected float _attackCooldown = 1.0f;
    [SerializeField] protected float _attackRange = 10f;
    [SerializeField] protected LayerMask _monsterLayer;
    
    [Header("[Animation]")]
    [SerializeField] protected AnimationEventHandler _animationEventHandler;
    
    // Components
    protected Animator _animator;
    protected SpriteRenderer _spriteRenderer;
    
    // State
    protected HeroState _state = HeroState.Idle;

    // Movement
    protected const float STOP_THRESHOLD = 0.05f;
    protected Vector3 _destination;
    
    // Attack
    protected Transform _currentTarget;
    
    protected int _currentAttackClipId = AttackClipId;
    
    protected static readonly int SpeedClipId = Animator.StringToHash("Speed");
    protected static readonly int AttackClipId = Animator.StringToHash("Attack");
    
    protected virtual void Awake()
    {
        _animator = GetComponent<Animator>();
        _spriteRenderer = GetComponent<SpriteRenderer>();
        _destination = transform.position;
        
        _animationEventHandler.onAttackAnimationEnd += OnAttackAnimEnd;
    }
    
    protected virtual void Update()
    {
        switch (_state)
        {
            case HeroState.Idle:
            {
                if (TryFindTarget(out var target))
                {
                    StartAttack(target);
                }
                break;
            } 
            
            case HeroState.Moving:
            {
                UpdateMovement();
                break;
            }
            
            case HeroState.Attacking:
            {
                if (_currentTarget)
                {
                    Vector3 direction = (_currentTarget.position - transform.position).normalized;
                    UpdateFacing(direction);
                }
                break;
            }
            case HeroState.AttackCooldown:
            {
                // Do nothing, waiting for cooldown to finish
                break;
            }
        }
    }
    
    private bool TryFindTarget(out Transform target)
    {
        Collider2D[] hits = Physics2D.OverlapCircleAll(transform.position, _attackRange, _monsterLayer);
        target = null;
    
        float minDist = float.MaxValue;
    
        foreach (var hit in hits)
        {
            float dist = Vector2.Distance(transform.position, hit.transform.position);
            if (dist < minDist)
            {
                minDist = dist;
                target = hit.transform;
            }
        }
    
        return target;
    }
   
    protected virtual void StartAttack(Transform target)
    {
        _state = HeroState.Attacking;
        _currentTarget = target;

        _animator.SetTrigger(_currentAttackClipId);

        Vector3 dir = (target.position - transform.position).normalized;
        UpdateFacing(dir);
    }

    protected void OnAttackAnimEnd()
    {
        if (_state != HeroState.Attacking)
        {
            return;
        }

        Attack();

        EndAttack();
    }

    protected virtual void Attack()
    {
        if (_currentTarget && _currentTarget.TryGetComponent(out MonsterController monster))
        {
            monster.TakeDamage(10);
        }
    }

    protected void EndAttack()
    {
        _state = HeroState.Idle;
        _currentTarget = null;

        StopCoroutine(nameof(Cor_AttackCooldown));
        StartCoroutine(nameof(Cor_AttackCooldown));
    }
    
   private IEnumerator Cor_AttackCooldown()
    {
        _state = HeroState.AttackCooldown;
        yield return new WaitForSeconds(_attackCooldown);
        _state = HeroState.Idle;
    }

    protected void CancelAttack()
    {
        _currentTarget = null;
        _animator.ResetTrigger(_currentAttackClipId);
    }

    private void UpdateMovement()
    {
        Vector3 direction = transform.position - _destination;
        float distance = direction.magnitude;

        if (distance < STOP_THRESHOLD)
        {
            transform.position = _destination;
            _state = HeroState.Idle;
            _animator.SetFloat(SpeedClipId, 0f);
            return;
        }

        transform.position += -direction.normalized * (_moveSpeed * Time.deltaTime);
        _animator.SetFloat(SpeedClipId, _moveSpeed);
    }

    public void MoveTo(Vector3 worldPosition)
    {
        if (_state == HeroState.Attacking)
        {
            CancelAttack();
        }

        _destination = worldPosition;
        _state = HeroState.Moving;

        Vector3 moveDir = worldPosition - transform.position;
        UpdateFacing(moveDir);
    }
 
    private void UpdateFacing(Vector3 moveDir)
    {
        _spriteRenderer.flipX = moveDir.x < 0;
    }
    
    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, _attackRange);
        
#if UNITY_EDITOR
        Handles.color = Color.white;
        Handles.Label(transform.position + Vector3.up * 1.5f, $"스테이트: {_state}");
#endif
    }
}

public partial class HeroUnit
{
    protected enum HeroState
    {
        Idle,
        Moving,
        Attacking,
        AttackCooldown,
    }
}
 