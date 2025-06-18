using System.Collections;
using UnityEngine;

public class SpawnManager : MonoBehaviour
{
    public GameObject[] monsterPrefabs;

    private int monsterCount = 0;
    private int monsterLimit = 100;
    private bool activeGame = true; // 이건 임시로 여기서만 쓸건데 전체 게임오버와 관련된 변수로 나중에 적용해줘야할듯

    private int currentWave = 1;
    private int endWave = 50; // 50웨이브 까지 존재
    private int monsterPerWave = 60; // 웨이브 당 소환되는 몬스터의 수
    private int timePerWave = 40; // 웨이브 당 주어지는 시간
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        //Instantiate(monsterPrefabs[0], new Vector3(0, 1.2f, 9), monsterPrefabs[0].transform.rotation);
        StartCoroutine(SpawnLoop());
        //SpawnWave());
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

        Debug.Log("Monster Count : " + monsterCount);
    }

    IEnumerator SpawnLoop()
    {
        while (currentWave <= endWave && activeGame)
        {
            StartCoroutine(SpawnWave(currentWave));

            yield return StartCoroutine(WaveCountDown());
            currentWave++;
        }
        // 웨이브 당 카운트 다운을 하나 호출해주고 
        // 그다음에 해당 웨이브의 몬스터를 소환하는 함수를 호출한다
    }

    IEnumerator SpawnWave(int wave)
    {
        int my_wave = wave > 1 ? 2 : 1;
        //int my_wave;
        //if (wave == 1)
        //    my_wave = wave; // 일단 몬스터 종류가 2개라 1,2 만 사용할듯
        //else if (wave > 1)
        //    my_wave = 2;

        Debug.Log(wave + " Wave Start!");

        for (int i = 0; i < monsterPerWave; i++)
        {
            Instantiate(monsterPrefabs[my_wave - 1], new Vector3(0, 1.4f, 9), monsterPrefabs[my_wave - 1].transform.rotation);
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
