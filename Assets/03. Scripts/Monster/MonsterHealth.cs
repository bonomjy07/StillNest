using System.Collections;
using UnityEngine;

public class MonsterHealth : MonoBehaviour
{
    [SerializeField] private float maxHp = 100;
    [SerializeField] private float currentHp;
    [SerializeField] private float _deathAnimDuration = 0.22f; // Death 애니메이션 실행시간
    private bool _isDead;
    private int _wave;

    private SpawnManager _spawnManager;
    private Animator _animator;
    private MonsterMoving _monsterMoving;

    private HealthBar _healthBar;

    private static readonly int TakeHitClipId = Animator.StringToHash("TakeHit");
    private static readonly int DeathClipId = Animator.StringToHash("Death");

    public bool IsDead => currentHp <= 0;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        _spawnManager = GameObject.Find("Spawn Manager").GetComponent<SpawnManager>(); // SpawnManager script
        _monsterMoving = GetComponent<MonsterMoving>();
        _animator = GetComponent<Animator>();
        _isDead = false;

        //StartCoroutine(TempDotDamage()); // For Test
    }

    // Update is called once per frame
    void Update()
    {
        if (currentHp <= 0 && !_isDead)
        {
            Die();
            //_spawnManager.RemoveMonster(); // SpawnManager쪽에 몬스터 죽을때 알리는건데 미완성
        }
    }

    public void Initialize(int wave)
    {
        _wave = wave;
        currentHp = _wave * 100;
    }
    public void TakeDamage(float dmg)
    {
        if(!_isDead)
        {
            currentHp -= dmg;
            _animator.ResetTrigger(TakeHitClipId);
            _animator.SetTrigger(TakeHitClipId);
        }
        
        ShowHealthBar();
    }

    private void ShowHealthBar()
    {
        if (!_healthBar) 
        {
            _healthBar = HealthBarManager.Instance.GetHealthBar();
        }
        
        if (!_healthBar)
        {
            return;
        }
        
        float healthPercentage = currentHp / maxHp;
        _healthBar.SetBar(healthPercentage, transform);
    }

    IEnumerator TempDotDamage()
    {
        // 임시로 유닛을 사용하지 않고 몬스터의 Die로직을 처리하기위해 초당 도트템 적용
        while(currentHp > 0)
        {
            yield return new WaitForSeconds(2f);
            currentHp -= 20f;
            //_animator.SetTrigger("TakeHit");
            _animator.ResetTrigger(TakeHitClipId);
            _animator.SetTrigger(TakeHitClipId);
        }
    }

    private void Die()
    {
        _isDead = true;
        // Notice to MonsterMoving
        _monsterMoving.NoticeMonsterDeath();
        StartCoroutine(DestroyAfterDeath());
    }

    IEnumerator DestroyAfterDeath()
    {
        _animator.SetTrigger(DeathClipId);
        yield return new WaitForSeconds(_deathAnimDuration);
        // Notice to Spawn Manager
        _spawnManager.OnMonsterDeath();
        Destroy(gameObject);
        
        HideHealthBar();
    }
    
    private void HideHealthBar()
    {
        if (_healthBar)
        {
            _healthBar.Hide();
        }
    }
}
