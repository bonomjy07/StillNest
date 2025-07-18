using System.Collections;
using UnityEngine;

public class MonsterHealth : MonoBehaviour
{
    [SerializeField] protected float _maxHp;
    [SerializeField] protected float _currentHp;
    [SerializeField] protected float _deathAnimDuration = 0.22f; // Death 애니메이션 실행시간
    [SerializeField] protected int _money = 20;
    protected bool _isDead;
    protected int _wave;

    protected SpawnManager _spawnManager;
    protected Animator _animator;
    private MonsterMoving _monsterMoving;

    private HealthBar _healthBar;

    protected static readonly int TakeHitClipId = Animator.StringToHash("TakeHit");
    protected static readonly int DeathClipId = Animator.StringToHash("Death");

    public bool IsDead => _currentHp <= 0;
    public int Money => _money; // or _wave * 10;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        

        StartCoroutine(TempDotDamage()); // For Test
    }

    // Update is called once per frame
    void Update()
    {
        if (_currentHp <= 0 && !_isDead)
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
        _isDead = false;
    }

    public virtual void Initialize(int wave)
    {
        _wave = wave;
        _maxHp = _wave * 50;
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
        // 임시로 유닛을 사용하지 않고 몬스터의 Die로직을 처리하기위해 초당 도트템 적용
        while(_currentHp > 0)
        {
            yield return new WaitForSeconds(2f);
            _currentHp -= 20f;
            //_animator.SetTrigger("TakeHit");
            _animator.ResetTrigger(TakeHitClipId);
            _animator.SetTrigger(TakeHitClipId);
        }
    }

    protected virtual void Die()
    {
        _isDead = true;
        // Notice to MonsterMoving
        _monsterMoving.NoticeMonsterDeath();
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
