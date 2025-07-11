using System.Collections;
using UnityEngine;
using UnityEngine.Tilemaps;

public class SpawnManager : MonoBehaviour
{
    [SerializeField] private GameObject[] _monsterPrefabs;
    [SerializeField] private GameObject[] _bossPrefabs;
    [SerializeField] private Transform _monsterRoot;
    [SerializeField] private Transform _bossRoot;
    [SerializeField] private Tilemap _tilemap;
    
    private GameInfoUIManager gameInfoUIManager; // UI 처리스크립트
    private Vector2 _generalSpawnPosition;
    private Vector2 _bossSpawnPosition;
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
    
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        // Get Spawn Position     
        GetSpawnPosition();
        Debug.Log($"Spawn General Position : ({_generalSpawnPosition.x}, {_generalSpawnPosition.y})");
        Debug.Log($"Spawn Boss Position : ({_bossSpawnPosition.x}, {_bossSpawnPosition.y})");

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

    private void GetSpawnPosition()
    {
        Vector3Int topLeftCell = new Vector3Int(_tilemap.cellBounds.xMin + 1, _tilemap.cellBounds.yMax - 1, 0);
        Vector3 spawnPos3D = _tilemap.CellToWorld(topLeftCell) + (_tilemap.cellSize / 2f);
        Vector2 spawnPos2D= new Vector2(spawnPos3D.x, spawnPos3D.y);

        _generalSpawnPosition = new Vector2(spawnPos2D.x, spawnPos2D.y + _generalMonsterOffsetY);
        _bossSpawnPosition = new Vector2(spawnPos2D.x, spawnPos2D.y + _bossMonsterOffsetY);
    }

    IEnumerator SpawnLoop()
    {
        while (_currentWave <= _endWave && _activeGame)
        {
            if(_currentWave % 5 == 0) // Boss Wave
            {
                SpawnBossWave(_currentWave);
                yield return StartCoroutine(BossCountDown(_timePerBoss));
            }
            else
            {
                StartCoroutine(SpawnWave(_currentWave));
                yield return StartCoroutine(WaveCountDown(_timePerWave));
            }
                
            _currentWave++;
        }
    }

    IEnumerator SpawnWave(int wave)
    {
        Debug.Log($"{wave} Wave Start!");
        gameInfoUIManager.UpdateWave(wave);
        gameInfoUIManager.AlertWave(wave);

        int monsterIndex = Random.Range(0, _monsterPrefabs.Length);
        for (int i = 0; i < _monsterPerWave; i++)
        {
            GameObject mob = Instantiate(_monsterPrefabs[monsterIndex], _generalSpawnPosition, Quaternion.identity, _monsterRoot);
            mob.GetComponent<MonsterMoving>().SetTilemap(_tilemap);
            mob.GetComponent<MonsterHealth>().Initialize(wave);
            _monsterCount++;

            gameInfoUIManager.UpdateMonsterCount(_monsterCount, _monsterLimit);
            yield return new WaitForSeconds(0.5f);
        }
    }
    void SpawnBossWave(int wave)
    {
        Debug.Log($"Boss Wave({wave}) Start!");
        gameInfoUIManager.UpdateWave(wave);
        gameInfoUIManager.AlertWave(wave);

        int bossIndex = Random.Range(0, _bossPrefabs.Length);
        GameObject boss = Instantiate(_bossPrefabs[bossIndex], _bossSpawnPosition, Quaternion.identity, _bossRoot);
        _bossAlive = true;
        boss.GetComponent<BossMoving>().SetTilemap(_tilemap);
        boss.GetComponent<BossHealth>().Initialize(wave);
        _monsterCount++;
        gameInfoUIManager.UpdateMonsterCount(_monsterCount, _monsterLimit);

    }
    IEnumerator WaveCountDown(int time)
    {
        int remainTime = time - 1;
        while(remainTime >= 0)
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

        if(_bossAlive)
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
    }
}
