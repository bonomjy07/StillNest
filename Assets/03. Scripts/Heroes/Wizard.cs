using System.Collections;
using UnityEngine;

public class Wizard : MonoBehaviour
{
    [Header("[Movement]")]
    [SerializeField] private float _moveSpeed = 2f;

    [Header("[Fireball]")]
    [SerializeField] private Fireball _fireballPrefab;
    [SerializeField] private Transform _fireballPointRight;
    [SerializeField] private Transform _fireballPointLeft;

    [Header("[Attack]")]
    [SerializeField] private float _attackDuration = 0.11f + 0.02f; // 애니메이션 타이밍
    [SerializeField] private float _attackCooldown = 1.0f;
    [SerializeField] private float _attackRange = 10f;
    [SerializeField] private LayerMask _monsterLayer;

    private Animator _animator;
    private SpriteRenderer _spriteRenderer;

    private Vector3 _destination;
    private Transform _currentTarget;

    private const float STOP_THRESHOLD = 0.05f;

    private enum WizardState
    {
        Idle,
        Moving,
        Attacking
    }

    private WizardState _state = WizardState.Idle;

    private static readonly int SpeedClipId = Animator.StringToHash("Speed");
    private static readonly int AttackClipId = Animator.StringToHash("Attack");

    private void Start()
    {
        _animator = GetComponent<Animator>();
        _spriteRenderer = GetComponent<SpriteRenderer>();
        _destination = transform.position;
    }

    private void Update()
    {
        switch (_state)
        {
            case WizardState.Idle:
            {
                if (TryFindTarget(out var target))
                {
                    StartAttack(target);
                }
            }
                break;
            case WizardState.Moving:
            {
                UpdateMovement();
            }
                break;

            case WizardState.Attacking:
            {
                // Do nothing
            }
                break;
        }
    }

    private void UpdateMovement()
    {
        Vector3 direction = transform.position - _destination;
        float distance = direction.magnitude;

        if (distance < STOP_THRESHOLD)
        {
            transform.position = _destination;
            _state = WizardState.Idle;
            _animator.SetFloat(SpeedClipId, 0f);
            return;
        }

        Vector3 moveDir = -direction.normalized;
        UpdateFacing(moveDir);
        transform.position += moveDir * (_moveSpeed * Time.deltaTime);
        _animator.SetFloat(SpeedClipId, _moveSpeed);
    }

    private void UpdateFacing(Vector3 moveDir)
    {
        _spriteRenderer.flipX = moveDir.x < 0;
    }

    public void MoveTo(Vector3 worldPosition)
    {
        _destination = worldPosition;
        _state = WizardState.Moving;
        
        if (_state == WizardState.Attacking)
        {
            CancelAttack();
        }
    }
    
    private void CancelAttack()
    {
        _currentTarget = null;
        _animator.ResetTrigger(AttackClipId);
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
    
    private void StartAttack(Transform target)
    {
        _state = WizardState.Attacking;
        _currentTarget = target;

        UpdateFacing(target.position - transform.position);
        _animator.SetTrigger(AttackClipId);
    }

    public void OnAttackCastingFinish()
    {
        if (_state != WizardState.Attacking)
        {
            return;
        }
        
        // Spawn Fire
        Transform fireballPoint = _spriteRenderer.flipX ? _fireballPointLeft : _fireballPointRight;
        Fireball fireball = Instantiate(_fireballPrefab, fireballPoint.position, Quaternion.identity, transform);
        fireball.Initialize(_currentTarget.GetComponent<MonsterController>());

        _state = WizardState.Idle;
    }
    
    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, _attackRange);
    }
 
}
