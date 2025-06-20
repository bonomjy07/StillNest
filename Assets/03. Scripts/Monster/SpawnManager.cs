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
    private bool activeGame = true; // �̰� �ӽ÷� ���⼭�� ���ǵ� ��ü ���ӿ����� ���õ� ������ ���߿� ������ҵ�

    private int currentWave = 1;
    private int endWave = 50; // 50���̺� ���� ����
    private int monsterPerWave = 60; // ���̺� �� ��ȯ�Ǵ� ������ ��
    private int timePerWave = 40; // ���̺� �� �־����� �ð�
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
        // ���̺� ī��Ʈ �ٿ� �Լ� UIǥ�� ���� �۾��� ���⼭ ȣ��
        yield return new WaitForSeconds(timePerWave);
        // �ϴ� ������ ������ �س��µ� 1�ʸ��� UI�ٲ��ַ��� �ݺ������� 1�ʾ� �ϰ� �ϸ�ɵ�
        
    }

    public void RemoveMonster()
    {
        monsterCount--;
    }
}
