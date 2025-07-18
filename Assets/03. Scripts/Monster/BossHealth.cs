using System.Collections;
using UnityEngine;

public class BossHealth : MonsterHealth
{
    private BossMoving _bossMoving;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        StartCoroutine(TempDotDamage());
    }

    // Update is called once per frame
    void Update()
    {
        if (_currentHp <= 0 && !_isDead)
        {
            Die();
        }
    }

    protected override void Awake()
    {
        _spawnManager = GameObject.Find("Spawn Manager").GetComponent<SpawnManager>(); // SpawnManager script
        _bossMoving = GetComponent<BossMoving>();
        _animator = GetComponent<Animator>();
        _isDead = false;
    }

    public override void Initialize(int wave)
    {
        _wave = wave;
        _maxHp = _wave * 1000;
        _currentHp = _maxHp;
    }

    protected override void Die()
    {
        _isDead = true;
        _bossMoving.NoticeMonsterDeath();
        StartCoroutine(DestroyAfterDeath());
    }

    protected override IEnumerator DestroyAfterDeath()
    {
        _animator.SetTrigger(DeathClipId);
        yield return new WaitForSeconds(_deathAnimDuration);
        
        _spawnManager.OnMonsterDeath(1);
        Destroy(gameObject);
    }
}
