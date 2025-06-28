using System.Collections;
using UnityEngine;
using UnityEngine.Tilemaps;

public class SpawnManager : MonoBehaviour
{
    [SerializeField] private GameObject[] _monsterPrefabs;
    [SerializeField] private Tilemap _tilemap;
    
    private GameInfoUIManager gameInfoUIManager; // UI 처리스크립트
    private Vector2 _spawnPosition; 

    private int _monsterCount = 0;
    private int _monsterLimit = 100;
    private bool _activeGame = true; // 이건 임시로 여기서만 쓸건데 전체 게임오버와 관련된 변수로 나중에 해줘야할듯

    private int _currentWave = 1;
    private int _endWave = 50; // 50웨이브 까지 존재
    private int _monsterPerWave = 60; // 웨이브 당 소환되는 몬스터의 수
    private int _timePerWave = 40; // 웨이브 당 주어지는 시간
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        // Get Spawn Position
        Vector3Int topLeftCell = new Vector3Int(_tilemap.cellBounds.xMin + 1, _tilemap.cellBounds.yMax - 1, 0);
        Vector3 spawnPos3D = _tilemap.CellToWorld(topLeftCell) + (_tilemap.cellSize / 2f);
        _spawnPosition = new Vector2(spawnPos3D.x, spawnPos3D.y + 0.14f);
        
        Debug.Log("Spawn Position : (" + _spawnPosition.x + ", " + _spawnPosition.y + ")");

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
            GameObject mob = Instantiate(_monsterPrefabs[monsterIndex], _spawnPosition, Quaternion.identity);
            mob.GetComponent<MonsterController>().SetTilemap(_tilemap);
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
        
        //yield return new WaitForSeconds(_timePerWave);
        // 일단 지금은 통으로 해놨는데 1초마다 UI바꿔주려면 반복문으로 1초씩 하게 하면될듯
        
    }

    public void RemoveMonster()
    {
        _monsterCount--;
    }
}
