using System.Collections;
using UnityEngine;
using UnityEngine.Tilemaps;

public class SpawnManager : MonoBehaviour
{
    public GameObject[] monsterPrefabs;
    public Tilemap tilemap;

    private Vector2 spawnPosition; 

    private int monsterCount = 0;
    private int monsterLimit = 100;
    private bool activeGame = true; // 이건 임시로 여기서만 쓸건데 전체 게임오버와 관련된 변수로 나중에 해줘야할듯

    private int currentWave = 1;
    private int endWave = 50; // 50웨이브 까지 존재
    private int monsterPerWave = 60; // 웨이브 당 소환되는 몬스터의 수
    private int timePerWave = 40; // 웨이브 당 주어지는 시간
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        // Get Spawn Position
        Vector3Int topLeftCell = new Vector3Int(tilemap.cellBounds.xMin + 1, tilemap.cellBounds.yMax - 1, 0);
        Vector3 spawnPos3D = tilemap.CellToWorld(topLeftCell) + (tilemap.cellSize / 2f);
        spawnPosition = new Vector2(spawnPos3D.x, spawnPos3D.y + 0.14f);
        
        Debug.Log("Spawn Position : (" + spawnPosition.x + ", " + spawnPosition.y + ")");
        

        StartCoroutine(SpawnLoop());


    }

    // Update is called once per frame
    void Update()
    {
        if (monsterCount == monsterLimit)
        {
            Debug.Log("Game Over");
            activeGame = false;
            // Game Over Logic Start!
        }

        //Debug.Log("Monster Count : " + monsterCount);
    }

    IEnumerator SpawnLoop()
    {
        while (currentWave <= endWave && activeGame)
        {
            StartCoroutine(SpawnWave(currentWave));

            yield return StartCoroutine(WaveCountDown());
            currentWave++;
        }
    }

    IEnumerator SpawnWave(int wave)
    {
        Debug.Log(wave + " Wave Start!");

        int monsterIndex = Random.Range(0, monsterPrefabs.Length);
        for (int i = 0; i < monsterPerWave; i++)
        {
            GameObject mob = Instantiate(monsterPrefabs[monsterIndex], spawnPosition, Quaternion.identity);
            mob.GetComponent<MonsterController>().SetTilemap(tilemap);
            monsterCount++;
            yield return new WaitForSeconds(0.5f);
        }
    }
    IEnumerator WaveCountDown()
    {
        // 웨이브 카운트 다운 함수 UI표기 관련 작업도 여기서 호출
        yield return new WaitForSeconds(timePerWave);
        // 일단 지금은 통으로 해놨는데 1초마다 UI바꿔주려면 반복문으로 1초씩 하게 하면될듯
        
    }

    public void RemoveMonster()
    {
        monsterCount--;
    }
}
