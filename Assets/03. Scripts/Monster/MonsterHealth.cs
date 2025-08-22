using System.Collections;
using UnityEngine;

public class MonsterHealth : MonoBehaviour
{
    [SerializeField] protected float _maxHp;
    [SerializeField] protected float _currentHp;
    [SerializeField] protected float _deathAnimDuration = 0.22f; // Death 애니메이션 실행시간
    protected int _money = 20;
    protected bool _isDead;
    protected int _wave;

    protected SpawnManager _spawnManager;
    protected Animator _animator;
    private MonsterMoving _monsterMoving;

    private HealthBar _healthBar;

    protected static readonly int TakeHitClipId = Animator.StringToHash("TakeHit");
    protected static readonly int DeathClipId = Animator.StringToHash("Death");

    public GameBalanceData balanceData;

    public bool IsDead => _currentHp <= 0;
    public int Money => _money;

    void Start()
    {
        
        //StartCoroutine(TempDotDamage()); // For Test
    }

    void Update()
    {
        if (_currentHp <= 0 && !_isDead)
        {
            Die();
        }
    }
    protected virtual void Awake()
    {
        _spawnManager = GameObject.Find("Spawn Manager").GetComponent<SpawnManager>();
        _monsterMoving = GetComponent<MonsterMoving>();
        _animator = GetComponent<Animator>();
        _isDead = false;
        balanceData = Resources.Load<GameBalanceData>("ScriptableObjects/GameBalanceData");
        _money = balanceData.monsterKillGold;
    }

    public virtual void Initialize(int wave)
    {
        _wave = wave;
        
        _maxHp = 50 + (_wave - 1) * 20;
        _currentHp = _maxHp;
    }
    public void TakeDamage(float dmg)
    {
        if(!_isDead)
        {
            _currentHp -= dmg;
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
        
        float healthPercentage = _currentHp / _maxHp;
        _healthBar.SetBar(healthPercentage, transform);
    }

    protected IEnumerator TempDotDamage()
    {
        while(_currentHp > 0)
        {
            yield return new WaitForSeconds(2f);
            _currentHp -= 20f;
            
            _animator.ResetTrigger(TakeHitClipId);
            _animator.SetTrigger(TakeHitClipId);
        }
    }

    protected virtual void Die()
    {
        _isDead = true;
        
        _monsterMoving.NoticeMonsterDeath();
        StartCoroutine(DestroyAfterDeath());
    }

    protected virtual IEnumerator DestroyAfterDeath()
    {
        _animator.SetTrigger(DeathClipId);

        ShowMoneyText();
        
        yield return new WaitForSeconds(_deathAnimDuration);
        
        _spawnManager.OnMonsterDeath(gameObject, MonsterType.General);
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
