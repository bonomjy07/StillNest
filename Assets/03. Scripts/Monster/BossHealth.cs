using System.Collections;
using UnityEngine;

public class BossHealth : MonsterHealth
{
    private BossMoving _bossMoving;
    
    void Start()
    {

    }

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
        balanceData = Resources.Load<GameBalanceData>("ScriptableObjects/GameBalanceData");
        _money = balanceData.bossKillGold;
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

        ShowMoneyText();
        yield return new WaitForSeconds(_deathAnimDuration);
        
        Monster mob = GetComponent<Monster>();
        _spawnManager.OnMonsterDeath(mob.gameObject, mob.MobType);
        Destroy(gameObject);

        HideHealthBar();
    }
}
