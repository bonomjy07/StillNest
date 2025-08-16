using System;
using System.Collections;
using System.Diagnostics.CodeAnalysis;
using FishNet.Object;
using FishNet.Object.Prediction;
using FishNet.Object.Synchronizing;
using FishNet.Transporting;
using UnityEditor;
using UnityEngine;
using UnityEngine.Rendering;


public partial class HeroUnit : NetworkBehaviour
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
        get => _syncState;
        private set
        {
            {
                _syncState = value;
            }
        }
    }

    private HeroState _syncState;
    //private readonly SyncVar<HeroState> _syncState = new(); 
    
    // Components
    protected Animator _animator;
    protected SpriteRenderer _spriteRenderer;
    
    // Movement
    protected const float STOP_THRESHOLD = 0.05f;
    
    [SerializeField]
    protected Vector3 _destination;
    [SerializeField]
    private bool _hasDestination;
    
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
        _animator = GetComponentInChildren<Animator>();
        _spriteRenderer = GetComponentInChildren<SpriteRenderer>();
        
        _animationEventHandler.onAttackAnimationEnd += OnAttackAnimEnd;
    }

    private void Start()
    {
        _destination = transform.position;
    }

    public override void OnStartNetwork()
    {
        name += $",id:{ObjectId}, owner:{OwnerId}";
        
        TimeManager.OnTick += TimeManager_OnTick;
    }

    public override void OnStopNetwork()
    {
        TimeManager.OnTick -= TimeManager_OnTick;
    }

    private void TimeManager_OnTick()
    {
        //if (IsOwner)
        {
            RunInputs(CreateReplicateData());
        }

        if (IsServerInitialized)
        {
            CreateReconcile();
        }
    }
    
    private ReplicateData CreateReplicateData()
    {
        if (!IsOwner)
        {
            return default;
        }
        return new ReplicateData(_destination, _hasDestination);
    }
    
    [Replicate]
    private void RunInputs(ReplicateData data, ReplicateState state = ReplicateState.Invalid, Channel channel = Channel.Unreliable)
    {
        if (!data.HasDestination)
        {
            return;
        }
        
        Vector3 dir = data.Destination - transform.position;
        float dist = dir.magnitude;

        if (dist < STOP_THRESHOLD)
        {
            transform.position = data.Destination;
            _hasDestination = false;
            return;
        }

        float step = _moveSpeed * (float)TimeManager.TickDelta;
        if (step >= dist)
        {
            transform.position = data.Destination;
            _hasDestination = false;
            return;
        }
        
        transform.position += (dir / dist) * step;
    }

    public override void CreateReconcile()
    {
        ReconcileData rd = new ReconcileData(transform.position);
        PerformReconcile(rd);
    }
    
    [Reconcile]
    private void PerformReconcile(ReconcileData rd, Channel channel = Channel.Unreliable)
    {
        transform.position = rd.Position;
    }

    protected virtual void Update()
    {
        UpdateCooldown();
        
        // 예측 이동 테스트 중
        //UpdateMovement();
        
        switch (State)
        {
            case HeroState.Idle:
            {
                if (TryFindTarget(out var target) && _cooldownTimer <= 0f)
                {
                    //StartAttack(target);
                }
                break;
            } 
            
            case HeroState.Moving:
            {
                // 굳이 이때만 업데이트할 ㅣㅍㄹ요가 있을까 ??
                break;
            }
            
            case HeroState.Attacking:
            {
                if (_currentTarget)
                {
                    Vector3 direction = (_currentTarget.position - transform.position).normalized;
                    //UpdateFacing(direction);
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
        // 예측이동 허용
        if (!IsServerInitialized && !IsOwner)
        {
            return;
        }
        
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

    public bool moveToControl;
    
    public void MoveTo(Vector3 worldPosition)
    {
        State = HeroState.Moving;
        
        _destination = worldPosition;
        _hasDestination = true;
        
        Debug.Log($"[unit][{name}]Moveto() worldPosition:{worldPosition}");

        Vector3 moveDir = worldPosition - transform.position;
        UpdateFacing(moveDir);
    }

    private void UpdateFacing(Vector3 moveDir)
    {
        _spriteRenderer.flipX = moveDir.x < 0;
    }
}

public partial class HeroUnit
{
    public enum HeroState
    {
        Idle       = 0,
        Moving     = 1,
        Attacking  = 2,
    }
    
    public struct ReplicateData : IReplicateData
    {
        public Vector3 Destination;
        public bool HasDestination;

        public ReplicateData(Vector3 destination, bool hasDestination) : this()
        {
            Destination = destination;
            HasDestination = hasDestination;
        }
        
        private uint _tick;
        public void Dispose() { }
        public uint GetTick() => _tick;
        public void SetTick(uint value) => _tick = value;
    }

    public struct ReconcileData : IReconcileData
    {
        public Vector3 Position;
        
        public ReconcileData(Vector3 position) : this()
        {
            Position = position;
        }

        private uint _tick;
        public void Dispose() { }
        public uint GetTick() => _tick;
        public void SetTick(uint value) => _tick = value;
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
 