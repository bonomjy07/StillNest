using System.Collections;
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

    private GameInfoUIManager gameInfoUIManager; // UI 처리스크립트
    private Vector2 _generalSpawnPosition1;
    private Vector2 _generalSpawnPosition2;
    private Vector2 _bossSpawnPosition1;
    private Vector2 _bossSpawnPosition2;
    private float _generalMonsterOffsetY = 0.14f;
    private float _bossMonsterOffsetY = 0.64f;

    private int _monsterCount = 0;
    private int _monsterLimit = 100;
    private bool _activeGame = true; // 이건 임시로 여기서만 쓸건데 전체 게임오버와 관련된 변수로 나중에 해줘야할듯
    private bool _bossAlive;

    private int _currentWave = 1;
    [SerializeField] private int _endWave = 50; // 50웨이브 까지 존재
    [SerializeField] private int _monsterPerWave = 60; // 웨이브 당 소환되는 몬스터의 수
    [SerializeField] private int _timePerWave = 40; // 웨이브 당 주어지는 시간
    [SerializeField] private int _timePerBoss = 90; // 웨이브 당 주어지는 시간
    [SerializeField] private float _spawnTerms = 0.5f; // 몬스터 스폰텀
    private float _playerTerms = 0.25f; // 플레이어간의 스폰텀

    public UnityAction<int /*Money*/> onMonsterDeath; // 몬스터가 죽었을 때 호출되는 이벤트 (TODO 어떤 몬스터가 죽었는지 넘겨줘야할듯)

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        _spawnTilemap1 = DuoMap.Inst.GetSpawnTileMap();
        _spawnTilemap2 = DuoMap.Inst.GetSpawnTileMap(1);
        // Get Spawn Position     
        _generalSpawnPosition1 = GetSpawnPosition(_spawnTilemap1, MonsterType.General);
        _generalSpawnPosition2 = GetSpawnPosition(_spawnTilemap2, MonsterType.General);
        _bossSpawnPosition1 = GetSpawnPosition(_spawnTilemap1, MonsterType.Boss);
        _bossSpawnPosition2 = GetSpawnPosition(_spawnTilemap2, MonsterType.Boss);
        Debug.Log($"General Spawn Position 1 : ({_generalSpawnPosition1.x}, {_generalSpawnPosition1.y})");
        Debug.Log($"General Spawn Position 2 : ({_generalSpawnPosition2.x}, {_generalSpawnPosition2.y})");
        Debug.Log($"Boss Spawn Position 1 : ({_bossSpawnPosition1.x}, {_bossSpawnPosition1.y})");
        Debug.Log($"Boss Spawn Position 2 : ({_bossSpawnPosition2.x}, {_bossSpawnPosition2.y})");

        gameInfoUIManager = GameObject.Find("Game Manager").GetComponent<GameInfoUIManager>();

        StartCoroutine(SpawnLoop());
    }

    // Update is called once per frame
    void Update()
    {
        if (_monsterCount == _monsterLimit)
        {
            Debug.Log("Game Over");
            _activeGame = false;
            // Game Over Logic Start!
        }

        //Debug.Log("Monster Count : " + monsterCount);
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


        //Vector3Int topLeftCell = new Vector3Int(_tilemap.cellBounds.xMin + 1, _tilemap.cellBounds.yMax - 1, 0);
        //Vector3 spawnPos3D = _tilemap.CellToWorld(topLeftCell) + (_tilemap.cellSize / 2f);
        //Vector2 spawnPos2D= new Vector2(spawnPos3D.x, spawnPos3D.y);

        //_generalSpawnPosition = new Vector2(spawnPos2D.x, spawnPos2D.y + _generalMonsterOffsetY);
        //_bossSpawnPosition = new Vector2(spawnPos2D.x, spawnPos2D.y + _bossMonsterOffsetY);
    }

    IEnumerator SpawnLoop()
    {
        while (_currentWave <= _endWave && _activeGame)
        {
            gameInfoUIManager.UpdateWave(_currentWave);
            gameInfoUIManager.AlertWave(_currentWave);

            if (_currentWave % 5 == 0) // Boss Wave
            {
                Debug.Log($"Boss Wave({_currentWave}) Start!");
                int bossIndex = Random.Range(0, _bossPrefabs.Length);
                SpawnBossWave(_currentWave, PlayerNumber.Player1, bossIndex);
                yield return new WaitForSeconds(_playerTerms);
                SpawnBossWave(_currentWave, PlayerNumber.Player2, bossIndex);
                yield return StartCoroutine(BossCountDown(_timePerBoss));
            }
            else
            {
                Debug.Log($"{_currentWave} Wave Start!");
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
        //Debug.Log($"{wave} Wave Start!");
        //gameInfoUIManager.UpdateWave(wave);
        //gameInfoUIManager.AlertWave(wave);
        Vector2 pos = new Vector2();
        if (playerNum == PlayerNumber.Player1)
            pos = _generalSpawnPosition1;
        else if (playerNum == PlayerNumber.Player2)
            pos = _generalSpawnPosition2;

        for (int i = 0; i < _monsterPerWave; i++)
        {
            GameObject mob = Instantiate(_monsterPrefabs[monsterIndex], pos, Quaternion.identity, _monsterRoot);
            mob.GetComponent<MonsterMoving>().Initialize(playerNum, MonsterType.General);
            //mob.GetComponent<MonsterMoving>().SetTilemap(_tilemap);
            mob.GetComponent<MonsterHealth>().Initialize(wave);
            _monsterCount++;

            gameInfoUIManager.UpdateMonsterCount(_monsterCount, _monsterLimit);
            yield return new WaitForSeconds(_spawnTerms);
        }
    }
    void SpawnBossWave(int wave, PlayerNumber playerNum, int bossIndex)
    {
        //Debug.Log($"Boss Wave({wave}) Start!");
        //gameInfoUIManager.UpdateWave(wave);
        //gameInfoUIManager.AlertWave(wave);
        Vector2 pos = new Vector2();
        if (playerNum == PlayerNumber.Player1)
            pos = _bossSpawnPosition1;
        else if (playerNum == PlayerNumber.Player2)
            pos = _bossSpawnPosition2;

        GameObject boss = Instantiate(_bossPrefabs[bossIndex], pos, Quaternion.identity, _bossRoot);
        _bossAlive = true;
        boss.GetComponent<BossMoving>().Initialize(playerNum, MonsterType.Boss);
        //boss.GetComponent<BossMoving>().SetTilemap(_tilemap);
        boss.GetComponent<BossHealth>().Initialize(wave);
        _monsterCount++;
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

        if (_bossAlive)
        {
            _activeGame = false;
        }
    }

    public void OnMonsterDeath(int type)
    {
        _monsterCount--;
        gameInfoUIManager.UpdateMonsterCount(_monsterCount, _monsterLimit);

        if (type == 1) // boss
            _bossAlive = false;

        // Notify monster's money
        bool isBoss = type == 1;
        int money = _currentWave * (isBoss ? 40 : 20); // 보스는 40, 일반 몬스터는 20
        onMonsterDeath?.Invoke(money);
    }
}
