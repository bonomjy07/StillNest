using System.Collections;
using UnityEngine;

public class MonsterHealth : MonoBehaviour
{
    [SerializeField] private float hp;
    [SerializeField] private float _deathAnimDuration = 0.22f; // Death 애니메이션 실행시간
    private bool _isDead;
    private int _wave;

    private SpawnManager _spawnManager;
    private Animator _animator;
    private MonsterMoving _monsterMoving;

    private static readonly int TakeHitClipId = Animator.StringToHash("TakeHit");
    private static readonly int DeathClipId = Animator.StringToHash("Death");
    

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
        if (hp <= 0 && !_isDead)
        {
            Die();
            //_spawnManager.RemoveMonster(); // SpawnManager쪽에 몬스터 죽을때 알리는건데 미완성
        }
    }

    public void Initialize(int wave)
    {
        _wave = wave;
        hp = _wave * 100;
    }
    public void TakeDamage(float dmg)
    {
        if(!_isDead)
        {
            hp -= dmg;
            _animator.ResetTrigger(TakeHitClipId);
            _animator.SetTrigger(TakeHitClipId);
        }
    }

    IEnumerator TempDotDamage()
    {
        // 임시로 유닛을 사용하지 않고 몬스터의 Die로직을 처리하기위해 초당 도트템 적용
        while(hp > 0)
        {
            yield return new WaitForSeconds(2f);
            hp -= 20f;
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
    }
}
