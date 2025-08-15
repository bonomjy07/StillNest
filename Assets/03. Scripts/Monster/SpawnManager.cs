using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Tilemaps;

public class SpawnManager : Singleton<SpawnManager>
{
    [SerializeField] private GameObject[] _monsterPrefabs;
    [SerializeField] private GameObject[] _bossPrefabs;
    [SerializeField] private Transform _monsterRoot;
    [SerializeField] private Transform _bossRoot;
    [SerializeField] private Tilemap _tilemap;

    private Tilemap _spawnTilemap1;
    private Tilemap _spawnTilemap2;

    private GameInfoUIManager gameInfoUIManager;
    private Vector2 _generalSpawnPosition1;
    private Vector2 _generalSpawnPosition2;
    private Vector2 _bossSpawnPosition1;
    private Vector2 _bossSpawnPosition2;
    private float _generalMonsterOffsetY = 0.14f;
    private float _bossMonsterOffsetY = 0.64f;

    private int _monsterCount = 0;
    private int _monsterLimit = 100;
    private int _bossCount = 0;

    private int _currentWave = 1;
    [SerializeField] private int _endWave = 50; // 50웨이브까지
    [SerializeField] private int _monsterPerWave = 60; // 웨이브 당 소환되는 몬스터의 수
    [SerializeField] private int _timePerWave = 40; // 웨이브 당 주어지는 시간
    [SerializeField] private int _timePerBoss = 90; // 웨이브 당 주어지는 시간
    [SerializeField] private float _spawnTerms = 0.5f; // 몬스터 스폰텀
    private float _playerTerms = 0.25f; // 플레이어간의 스폰텀

    public UnityAction<int /*Money*/> onMonsterDeath;
    private List<GameObject> _spawnedMonsterList = new List<GameObject>();
    private List<GameObject> _spawnedBossList = new List<GameObject>();
    public GameBalanceData balanceData;

    void Start()
    {
        _spawnTilemap1 = DuoMap.Inst.GetSpawnTileMap();
        _spawnTilemap2 = DuoMap.Inst.GetSpawnTileMap(1);
        
        _generalSpawnPosition1 = GetSpawnPosition(_spawnTilemap1, MonsterType.General);
        _generalSpawnPosition2 = GetSpawnPosition(_spawnTilemap2, MonsterType.General);
        _bossSpawnPosition1 = GetSpawnPosition(_spawnTilemap1, MonsterType.Boss);
        _bossSpawnPosition2 = GetSpawnPosition(_spawnTilemap2, MonsterType.Boss);
        Debug.Log($"General Spawn Position 1 : ({_generalSpawnPosition1.x}, {_generalSpawnPosition1.y})");
        Debug.Log($"General Spawn Position 2 : ({_generalSpawnPosition2.x}, {_generalSpawnPosition2.y})");
        Debug.Log($"Boss Spawn Position 1 : ({_bossSpawnPosition1.x}, {_bossSpawnPosition1.y})");
        Debug.Log($"Boss Spawn Position 2 : ({_bossSpawnPosition2.x}, {_bossSpawnPosition2.y})");

        gameInfoUIManager = GameObject.Find("Game Manager").GetComponent<GameInfoUIManager>();
        balanceData = Resources.Load<GameBalanceData>("ScriptableObjects/GameBalanceData");

        StartCoroutine(SpawnLoop());
    }

    void Update()
    {
        if (_monsterCount >= _monsterLimit && GameManager.Instance.CurrentState == GameState.Playing)
        {
            GameManager.Instance.GameOver();
        }

    }

    private Vector2 GetSpawnPosition(Tilemap tilemap, MonsterType type)
    {
        Vector3 worldPos = new Vector3();
        Vector2 spawnPos2D = new Vector2();
        BoundsInt bound = tilemap.cellBounds;
        foreach (Vector3Int cellPos in bound.allPositionsWithin)
        {
            if (tilemap.HasTile(cellPos))
                worldPos = tilemap.GetCellCenterWorld(cellPos);
        }

        if (type == MonsterType.General)
            spawnPos2D = new Vector2(worldPos.x, worldPos.y + _generalMonsterOffsetY);
        else if(type == MonsterType.Boss)
            spawnPos2D = new Vector2(worldPos.x, worldPos.y + _bossMonsterOffsetY);

        return spawnPos2D;
    }

    IEnumerator SpawnLoop()
    {
        while (_currentWave <= _endWave)
        {
            while(GameManager.Instance.CurrentState != GameState.Playing)
            {
                yield return null; // 1 frame wait
            }

            gameInfoUIManager.UpdateWave(_currentWave);
            gameInfoUIManager.AlertWave(_currentWave);

            if (_currentWave % 5 == 0) // Boss Wave
            {
                //Debug.Log($"Boss Wave({_currentWave}) Start!");
                int bossIndex = Random.Range(0, _bossPrefabs.Length);
                SpawnBossWave(_currentWave, PlayerNumber.Player1, bossIndex);
                yield return new WaitForSeconds(_playerTerms);
                SpawnBossWave(_currentWave, PlayerNumber.Player2, bossIndex);
                yield return StartCoroutine(BossCountDown(_timePerBoss));
            }
            else
            {
                //Debug.Log($"{_currentWave} Wave Start!");
                int monsterIndex = Random.Range(0, _monsterPrefabs.Length);
                StartCoroutine(SpawnWave(_currentWave, PlayerNumber.Player1, monsterIndex));
                yield return new WaitForSeconds(_playerTerms);
                StartCoroutine(SpawnWave(_currentWave, PlayerNumber.Player2, monsterIndex));
                yield return StartCoroutine(WaveCountDown(_timePerWave));
            }

            _currentWave++;
        }
    }

    IEnumerator SpawnWave(int wave, PlayerNumber playerNum, int monsterIndex)
    {
        Vector2 pos = new Vector2();
        if (playerNum == PlayerNumber.Player1)
            pos = _generalSpawnPosition1;
        else if (playerNum == PlayerNumber.Player2)
            pos = _generalSpawnPosition2;

        for (int i = 0; i < _monsterPerWave; i++)
        {
            GameObject mob = Instantiate(_monsterPrefabs[monsterIndex], pos, Quaternion.identity, _monsterRoot);
            mob.GetComponent<MonsterMoving>().Initialize(playerNum, MonsterType.General);
            mob.GetComponent<MonsterHealth>().Initialize(wave);
            _spawnedMonsterList.Add(mob);
            _monsterCount++;

            gameInfoUIManager.UpdateMonsterCount(_monsterCount, _monsterLimit);
            yield return new WaitForSeconds(_spawnTerms);
        }
    }
    void SpawnBossWave(int wave, PlayerNumber playerNum, int bossIndex)
    {
        Vector2 pos = new Vector2();
        if (playerNum == PlayerNumber.Player1)
            pos = _bossSpawnPosition1;
        else if (playerNum == PlayerNumber.Player2)
            pos = _bossSpawnPosition2;

        GameObject boss = Instantiate(_bossPrefabs[bossIndex], pos, Quaternion.identity, _bossRoot);
        boss.GetComponent<BossMoving>().Initialize(playerNum, MonsterType.Boss);
        boss.GetComponent<BossHealth>().Initialize(wave);
        _spawnedBossList.Add(boss);
        _monsterCount++;
        _bossCount += 1;

        gameInfoUIManager.UpdateMonsterCount(_monsterCount, _monsterLimit);

    }
    IEnumerator WaveCountDown(int time)
    {
        int remainTime = time - 1;
        while (remainTime >= 0)
        {
            gameInfoUIManager.UpdateTime(remainTime);
            yield return new WaitForSeconds(1f);
            remainTime--;
        }
    }

    IEnumerator BossCountDown(int time)
    {
        int remainTime = time - 1;
        while (remainTime >= 0)
        {
            gameInfoUIManager.UpdateTime(remainTime);
            yield return new WaitForSeconds(1f);
            remainTime--;
        }

        if(_bossCount > 0)
            GameManager.Instance.GameOver();
    }

    public void OnMonsterDeath(GameObject monster, MonsterType type)
    {
        _monsterCount--;
        gameInfoUIManager.UpdateMonsterCount(_monsterCount, _monsterLimit);

        
        if (type == MonsterType.Boss)
        {
            _bossCount -= 1;
            _spawnedBossList.Remove(monster);
        }
        else if(type == MonsterType.General)
        {
            _spawnedMonsterList.Remove(monster);
        }

        int money = (type == MonsterType.Boss ? balanceData.bossKillGold : balanceData.monsterKillGold);
        onMonsterDeath?.Invoke(money);
    }

    public void ResetSetting()
    {
        // Manage Coroutine
        StopAllCoroutines();

        // Remove Monster
        foreach (var monster in _spawnedMonsterList)
        {
            if(monster != null)
                Destroy(monster);
        }
        _spawnedMonsterList.Clear();

        foreach(var monster in _spawnedBossList)
        {
            if (monster != null)
                Destroy(monster);
        }
        _spawnedBossList.Clear();

        // Wave Reset
        _currentWave = 1;
        _monsterCount = 0;
        gameInfoUIManager.UpdateMonsterCount(_monsterCount, _monsterLimit);
        _bossCount = 0;
        gameInfoUIManager.UpdateTime(0);

        StartCoroutine(SpawnLoop());
    }
}
