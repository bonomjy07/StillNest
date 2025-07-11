using System.Collections;
using System.Diagnostics.CodeAnalysis;
using UnityEditor;
using UnityEngine;
using UnityEngine.Rendering;

public partial class HeroUnit : MonoBehaviour
{
    [Header("[Movement]")]
    [SerializeField] protected float _moveSpeed = 2f;
    
    [Header("[Attack]")]
    [SerializeField] protected int _damageAmount = 10;
    [SerializeField] protected float _attackCooldown = 1.0f;
    [SerializeField] protected float _attackRange = 10f;
    [SerializeField] protected float AttackSpeedMultiplier = 1.0f;
    [SerializeField] protected LayerMask _monsterLayer;
    
    [Header("[Animation]")]
    [SerializeField] protected AnimationEventHandler _animationEventHandler;

    protected HeroState State
    {
        get => _state;
        private set
        {
            _state = value;
            _animator.SetInteger(HeroStateParamId, (int)value);
        }
    }
    private HeroState _state = HeroState.Idle;
    
    // Components
    protected Animator _animator;
    protected SpriteRenderer _spriteRenderer;
    
    // Movement
    protected const float STOP_THRESHOLD = 0.05f;
    protected Vector3 _destination;
    
    // Attack
    protected Transform _currentTarget;
    protected float _cooldownTimer; 
    
    protected int _currentAttackClipId = AttackClipId;
    
    protected static readonly int HeroStateParamId = Animator.StringToHash("HeroState");
    protected static readonly int SpeedClipId = Animator.StringToHash("Speed");
    protected static readonly int AttackClipId = Animator.StringToHash("IsAttacking");
    private static readonly int SpeedMultiplier = Animator.StringToHash("AttackSpeedMultiplier");

    protected virtual void Awake()
    {
        _animator = GetComponent<Animator>();
        _spriteRenderer = GetComponent<SpriteRenderer>();
        _destination = transform.position;
        
        _animationEventHandler.onAttackAnimationEnd += OnAttackAnimEnd;
    }

    private void Start()
    {
        _animator.SetFloat(SpeedMultiplier, AttackSpeedMultiplier);
    }

    protected virtual void Update()
    {
        UpdateCooldown();
        
        switch (_state)
        {
            case HeroState.Idle:
            {
                if (TryFindTarget(out var target) && _cooldownTimer <= 0f)
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
        }
    }

    private void UpdateCooldown()
    {
        if (_cooldownTimer <= 0f)
        {
            return;
        }

        _cooldownTimer -= Time.deltaTime;
        if (_cooldownTimer < 0f)
        {
            _cooldownTimer = 0f;
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
        State = HeroState.Attacking;
        _currentTarget = target;

        Vector3 dir = (target.position - transform.position).normalized;
        UpdateFacing(dir);
    }

    protected void OnAttackAnimEnd()
    {
        if (State != HeroState.Attacking)
        {
            return;
        }

        Attack();

        EndAttack();
    }

    protected virtual void Attack()
    {
        if (_currentTarget && _currentTarget.TryGetComponent(out MonsterHealth monster))
        {
            monster.TakeDamage(_damageAmount);
        }
    }

    protected void EndAttack()
    {
        State = HeroState.Idle;
        _currentTarget = null;
        _cooldownTimer = _attackCooldown;
    }
    
    protected void CancelAttack()
    {
        _currentTarget = null;
        State = HeroState.Idle;
    }

    private void UpdateMovement()
    {
        Vector3 direction = transform.position - _destination;
        float distance = direction.magnitude;

        if (distance < STOP_THRESHOLD)
        {
            transform.position = _destination;
            State = HeroState.Idle;
            return;
        }

        transform.position += -direction.normalized * (_moveSpeed * Time.deltaTime);
    }

    public void MoveTo(Vector3 worldPosition)
    {
        if (State == HeroState.Attacking)
        {
            CancelAttack();
        }

        State = HeroState.Moving;
        _destination = worldPosition;

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
        Handles.Label(transform.position + Vector3.up * 1.5f, $"스테이트: {State}");
#endif
    }
}

public partial class HeroUnit
{
    protected enum HeroState
    {
        Idle       = 0,
        Moving     = 1,
        Attacking  = 2,
    }
}
 