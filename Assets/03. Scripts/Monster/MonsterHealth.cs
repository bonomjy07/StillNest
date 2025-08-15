using System.Collections;
using FishNet.Object;
using FishNet.Object.Synchronizing;
using UnityEngine;
using UnityEngine.Events;

public class MonsterHealth : NetworkBehaviour
{
    [SerializeField] protected float _deathAnimDuration = 0.22f; // Death 애니메이션 실행시간
    [SerializeField] protected int _money = 20;
    public int _wave;

    protected readonly SyncVar<float> _maxHp = new();
    protected readonly SyncVar<float> _currentHp = new();
    protected readonly SyncVar<bool> _isDead = new();

    protected SpawnManager _spawnManager;
    protected Animator _animator;
    private MonsterMoving _monsterMoving;

    private HealthBar _healthBar;

    protected static readonly int TakeHitClipId = Animator.StringToHash("TakeHit");
    protected static readonly int DeathClipId = Animator.StringToHash("Death");

    public bool IsDead => _currentHp.Value <= 0;
    public int Money => _money; // or _wave * 10;

    public event SyncVar<bool>.OnChanged OnDeadChange
    {
        add => _isDead.OnChange += value;
        remove => _isDead.OnChange -= value;
    }
    
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        

        //StartCoroutine(TempDotDamage()); // For Test
    }

    // Update is called once per frame
    void Update()
    {
        if (_currentHp.Value <= 0 && !_isDead.Value)
        {
            Die();
            //_spawnManager.RemoveMonster(); // SpawnManager쪽에 몬스터 죽을때 알리는건데 미완성
        }
    }
    protected virtual void Awake()
    {
        _spawnManager = GameObject.Find("Spawn Manager").GetComponent<SpawnManager>(); // SpawnManager script
        _monsterMoving = GetComponent<MonsterMoving>();
        _animator = GetComponent<Animator>();
    }

    public virtual void Initialize(int wave)
    {
        _wave = wave;
        _maxHp.Value = _wave * 50;
        _currentHp.Value = _maxHp.Value;
    }
    public void TakeDamage(float dmg)
    {
        if (!_isDead.Value)
        {
            _currentHp.Value -= dmg;
            _animator.ResetTrigger(TakeHitClipId);
            _animator.SetTrigger(TakeHitClipId);
        }
        
        ShowHealthBar();
    }

    private void ShowHealthBar()
    {
        if (!_healthBar) 
        {
            _healthBar = HealthBarManager.Instance.Spawn();
        }
        
        if (!_healthBar)
        {
            return;
        }
        
        float healthPercentage = _currentHp.Value / _maxHp.Value;
        _healthBar.SetBar(healthPercentage, transform);
    }

    protected IEnumerator TempDotDamage()
    {
        // 임시로 유닛을 사용하지 않고 몬스터의 Die로직을 처리하기위해 초당 도트템 적용
        while(_currentHp.Value > 0)
        {
            yield return new WaitForSeconds(2f);
            _currentHp.Value -= 20f;
            //_animator.SetTrigger("TakeHit");
            _animator.ResetTrigger(TakeHitClipId);
            _animator.SetTrigger(TakeHitClipId);
        }
    }

    protected virtual void Die()
    {
        _isDead.Value = true;
        // Notice to MonsterMoving
        StartCoroutine(DestroyAfterDeath());
    }

    protected virtual IEnumerator DestroyAfterDeath()
    {
        _animator.SetTrigger(DeathClipId);

        ShowMoneyText();
        
        yield return new WaitForSeconds(_deathAnimDuration);
        // Notice to Spawn Manager
        _spawnManager.OnMonsterDeath(0);
        Destroy(gameObject);
        
        HideHealthBar();
    }

    private void ShowMoneyText()
    {
        FloatingMoneyText floatingMoneyText = FloatingMoneyTextManager.Instance.Spawn();
        if (!floatingMoneyText)
        {
            return;
        }
        
        floatingMoneyText.SetData(transform, Money);
        floatingMoneyText._onTweenComplete = FloatingMoneyTextManager.Instance.Despawn;
    }

    private void HideHealthBar()
    {
        if (!_healthBar)
        {
            return;
        }
        
        HealthBarManager.Instance.Despawn(_healthBar);
    }
}
