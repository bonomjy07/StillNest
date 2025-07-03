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
                if (_currentTarget)
                {
                    Vector3 direction = (_currentTarget.position - transform.position).normalized;
                    UpdateFacing(direction);
                }
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

        transform.position += -direction.normalized * (_moveSpeed * Time.deltaTime);
        _animator.SetFloat(SpeedClipId, _moveSpeed);
    }

    private void UpdateFacing(Vector3 moveDir)
    {
        _spriteRenderer.flipX = moveDir.x < 0;
    }

    public void MoveTo(Vector3 worldPosition)
    {
        if (_state == WizardState.Attacking)
        {
            CancelAttack();
        }

        _destination = worldPosition;
        _state = WizardState.Moving;

        Vector3 moveDir = worldPosition - transform.position;
        UpdateFacing(moveDir);
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

        _animator.SetTrigger(AttackClipId);
        Vector3 dir = (target.position - transform.position).normalized;
        UpdateFacing(dir);
    }

    public void OnAttackCastingFinish()
    {
        if (_state != WizardState.Attacking)
        {
            return;
        }
        
        // Spawn Fire
        Transform fireballPoint = _spriteRenderer.flipX ? _fireballPointLeft : _fireballPointRight;
        Fireball fireball = Instantiate(_fireballPrefab, fireballPoint.position, Quaternion.identity);
        fireball.Initialize(_currentTarget.GetComponent<MonsterController>());

        _state = WizardState.Idle;
        _currentTarget = null;
    }
    
    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, _attackRange);
    }
 
}
