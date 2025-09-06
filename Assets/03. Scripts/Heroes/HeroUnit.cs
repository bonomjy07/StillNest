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
    [SerializeField] protected int _damageAmount = 30;
    [SerializeField] protected float _attackCooldown = 1.0f;
    [SerializeField] protected float _attackRange = 10f;
    [SerializeField] protected float AttackSpeedMultiplier = 1.0f;
    [SerializeField] protected LayerMask _monsterLayer;
    
    [Header("[Animation]")]
    [SerializeField] protected AnimationEventHandler _animationEventHandler;

    protected HeroState State
    {
        get => _syncState.Value;
        private set
        {
            if (IsServerInitialized)
            {
                _syncState.Value = value;
            }
            else if (IsOwner)
            {
                SetSyncStateServerRpc(value);
            }
        }
    }

    private readonly SyncVar<HeroState> _syncState = new();

    // Components
    protected Animator _animator;
    protected SpriteRenderer _spriteRenderer;
    
    // Movement
    private const float STOP_THRESHOLD = 0.05f;

    private Vector3 _destination;
    private bool _hasDestination;
    
    // Attack
    protected Transform _currentTarget;
    private float _cooldownTimer; 
    
    protected int _currentAttackClipId = AttackClipId;
    
    protected static readonly int HeroStateParamId = Animator.StringToHash("HeroState");
    protected static readonly int SpeedClipId = Animator.StringToHash("Speed");
    protected static readonly int AttackClipId = Animator.StringToHash("IsAttacking");
    private static readonly int SpeedMultiplier = Animator.StringToHash("AttackSpeedMultiplier");

    protected virtual void Awake()
    {
        _animator = GetComponentInChildren<Animator>();
        _spriteRenderer = GetComponentInChildren<SpriteRenderer>();
        
        _animationEventHandler.onAttackAnimationEnd += OnAttackWindUp;
        _syncState.OnChange += OnStateChange;
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
    
    private void OnStateChange(HeroState prev, HeroState next, bool asServer)
    {
        // 서버에서만 처리
        if (asServer)
        {
            //SetAnimatorParam(next);
        }
        // 
        else
        {
            SetAnimatorParam(next);
        }
    }

    [ServerRpc]
    private void SetSyncStateServerRpc(HeroState newState)
    {
        _syncState.Value = newState;
    }

    private void TimeManager_OnTick()
    {
        RunInputs(CreateReplicateData());

        if (IsServerInitialized)
        {
            CreateReconcile();
        }

        if (IsServerInitialized)
        {
            Tick_Attack();
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
        
        Vector3 toDestination = data.Destination - transform.position;
        float remainingDistance = toDestination.magnitude;
        float moveStep = _moveSpeed * (float)TimeManager.TickDelta;

        if (remainingDistance < STOP_THRESHOLD || moveStep >= remainingDistance)
        {
            transform.position = data.Destination;
            _hasDestination = false;
            State = HeroState.Idle;
            return;
        }

        transform.position += (toDestination / remainingDistance) * moveStep;
        
        SetFacing(toDestination.normalized);
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

    [Server]
    private void Tick_Attack()
    {
        if (State != HeroState.Idle)
        {
            return;
        }

        if (_cooldownTimer > 0f)
        {
            _cooldownTimer -= (float)TimeManager.TickDelta;
            return;
        }

        if (!TryFindTarget(out _currentTarget) || _currentTarget == null)
        {
            return;
        }
        
        StartAttack(_currentTarget);
    }

    private bool TryFindTarget(out Transform target)
    {
        Collider2D[] hits = Physics2D.OverlapCircleAll(transform.position, _attackRange, _monsterLayer);
        target = null;
    
        float minDist = float.MaxValue;
    
        foreach (Collider2D hit in hits)
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
    
    [Server]
    protected virtual void StartAttack(Transform target)
    {
        State = HeroState.Attacking;
        _currentTarget = target;
        
        Vector3 dir = (target.position - transform.position).normalized;
        StartAttackClientRpc(dir);
    }

    [ObserversRpc]
    private void StartAttackClientRpc(Vector3 dir)
    {
        SetFacing(dir);
    }
    
    private void OnAttackWindUp()
    {
        // 서버만
        if (!IsServerInitialized)
        {
            return;
        }
        
        if (State != HeroState.Attacking)
        {
            return;
        }

        ApplyDamage();

        State = HeroState.Idle;
        
        _currentTarget = null;
        _cooldownTimer = _attackCooldown;
    }

    protected virtual void ApplyDamage()
    {
        if (_currentTarget && _currentTarget.TryGetComponent(out MonsterHealth monster))
        {
            monster.TakeDamage(_damageAmount);
        }
    }

    public void MoveTo(Vector3 worldPosition)
    {
        Debug.Log($"[unit][{name}]Moveto() worldPosition:{worldPosition}");

        State = HeroState.Moving;
        
        _destination = worldPosition;
        _hasDestination = true;

        _currentTarget = null;
    }

    private void SetFacing(Vector3 moveDir)
    {
        if (moveDir.x == 0f) 
        {
            return;
        }
        _spriteRenderer.flipX = moveDir.x < 0;
    }

    private void SetAnimatorParam(HeroState state)
    {
        if (_animator == null)
        {
            Debug.LogError($"[{name}] animator is null");
            return;
        }
        
        _animator.SetInteger(HeroStateParamId, (int)state);
    }

    public void UpgradeDamage(int level)
    {
        _damageAmount = 50 + (20 * level);
    }

    public void UpgradeAttackSpeed(int level)
    {
        _attackCooldown = Math.Max((float)0.3, (float)(0.8 - (0.05 * level)));
    }
}

public partial class HeroUnit
{
    public enum HeroState
    {
        Idle = 0,
        Moving = 1,
        Attacking = 2,
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

        public void Dispose()
        {
        }

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

        public void Dispose()
        {
        }

        public uint GetTick() => _tick;
        public void SetTick(uint value) => _tick = value;
    }
}

public partial class HeroUnit
{
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
 