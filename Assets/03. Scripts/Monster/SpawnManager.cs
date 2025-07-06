using System.Collections;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Tilemaps;

public class SpawnManager : Singleton<SpawnManager>
{
    [SerializeField] private GameObject[] _monsterPrefabs;
    [SerializeField] private Transform _monsterRoot;
    [SerializeField] private Tilemap _tilemap;
    
    private GameInfoUIManager gameInfoUIManager; // UI 처리스크립트
    private Vector2 _spawnPosition; 

    private int _monsterCount = 0;
    private int _monsterLimit = 100;
    private bool _activeGame = true; // 이건 임시로 여기서만 쓸건데 전체 게임오버와 관련된 변수로 나중에 해줘야할듯

    private int _currentWave = 1;
    [SerializeField] private int _endWave = 50; // 50웨이브 까지 존재
    [SerializeField] private int _monsterPerWave = 60; // 웨이브 당 소환되는 몬스터의 수
    [SerializeField] private int _timePerWave = 40; // 웨이브 당 주어지는 시간
    
    public UnityAction onMonsterDeath; // 몬스터가 죽었을 때 호출되는 이벤트 (TODO 어떤 몬스터가 죽었는지 넘겨줘야할듯)
    
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        // Get Spawn Position     
        _spawnPosition = GetSpawnPosition();
        Debug.Log($"Spawn Position : ({_spawnPosition.x}, {_spawnPosition.y})");

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

    private Vector2 GetSpawnPosition()
    {
        Vector3Int topLeftCell = new Vector3Int(_tilemap.cellBounds.xMin + 1, _tilemap.cellBounds.yMax - 1, 0);
        Vector3 spawnPos3D = _tilemap.CellToWorld(topLeftCell) + (_tilemap.cellSize / 2f);

        return new Vector2(spawnPos3D.x, spawnPos3D.y + 0.14f);
    }

    IEnumerator SpawnLoop()
    {
        while (_currentWave <= _endWave && _activeGame)
        {
            StartCoroutine(SpawnWave(_currentWave));

            yield return StartCoroutine(WaveCountDown());
            _currentWave++;
        }
    }

    IEnumerator SpawnWave(int wave)
    {
        Debug.Log(wave + " Wave Start!");
        gameInfoUIManager.UpdateWave(wave);
        gameInfoUIManager.AlertWave(wave);

        int monsterIndex = Random.Range(0, _monsterPrefabs.Length);
        for (int i = 0; i < _monsterPerWave; i++)
        {
            GameObject mob = Instantiate(_monsterPrefabs[monsterIndex], _spawnPosition, Quaternion.identity, _monsterRoot);
            mob.GetComponent<MonsterMoving>().SetTilemap(_tilemap);
            mob.GetComponent<MonsterHealth>().Initialize(wave);
            _monsterCount++;

            gameInfoUIManager.UpdateMonsterCount(_monsterCount, _monsterLimit);
            yield return new WaitForSeconds(0.5f);
        }
    }
    IEnumerator WaveCountDown()
    {
        int remainTime = _timePerWave - 1;
        while(remainTime >= 0)
        {
            gameInfoUIManager.UpdateTime(remainTime);
            yield return new WaitForSeconds(1f);
            remainTime--;

        }
        
    }

    public void OnMonsterDeath()
    {
        _monsterCount--;
        gameInfoUIManager.UpdateMonsterCount(_monsterCount, _monsterLimit);
        
        onMonsterDeath?.Invoke();
    }
}
