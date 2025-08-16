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
        if (_currentHp.Value <= 0 && !_isDead.Value)
        {
            Die();
        }
    }

    protected override void Awake()
    {
        _spawnManager = GameObject.Find("Spawn Manager").GetComponent<SpawnManager>(); // SpawnManager script
        _bossMoving = GetComponent<BossMoving>();
        _animator = GetComponent<Animator>();
    }

    public override void Initialize(int wave)
    {
        _wave = wave;
        _maxHp.Value = _wave * 1000;
        _currentHp.Value = _maxHp.Value;
    }

    protected override void Die()
    {
        _isDead.Value = true;
        StartCoroutine(DestroyAfterDeath());
    }

    protected override IEnumerator DestroyAfterDeath()
    {
        _animator.SetTrigger(DeathClipId);
        yield return new WaitForSeconds(_deathAnimDuration);
        
        Monster mob = GetComponent<Monster>();
        _spawnManager.OnMonsterDeath((int)mob.MobType);
        Destroy(gameObject);
    }
}
