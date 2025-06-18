using System.Collections;
using UnityEngine;

public class SpawnManager : MonoBehaviour
{
    public GameObject[] monsterPrefabs;

    private int monsterCount = 0;
    private int monsterLimit = 100;
    private bool activeGame = true; // �̰� �ӽ÷� ���⼭�� ���ǵ� ��ü ���ӿ����� ���õ� ������ ���߿� ����������ҵ�

    private int currentWave = 1;
    private int endWave = 50; // 50���̺� ���� ����
    private int monsterPerWave = 60; // ���̺� �� ��ȯ�Ǵ� ������ ��
    private int timePerWave = 40; // ���̺� �� �־����� �ð�
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
        // ���̺� �� ī��Ʈ �ٿ��� �ϳ� ȣ�����ְ� 
        // �״����� �ش� ���̺��� ���͸� ��ȯ�ϴ� �Լ��� ȣ���Ѵ�
    }

    IEnumerator SpawnWave(int wave)
    {
        int my_wave = wave > 1 ? 2 : 1;
        //int my_wave;
        //if (wave == 1)
        //    my_wave = wave; // �ϴ� ���� ������ 2���� 1,2 �� ����ҵ�
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
        // ���̺� ī��Ʈ �ٿ� �Լ� UIǥ�� ���� �۾��� ���⼭ ȣ��
        yield return new WaitForSeconds(timePerWave);
        // �ϴ� ������ ������ �س��µ� 1�ʸ��� UI�ٲ��ַ��� �ݺ������� 1�ʾ� �ϰ� �ϸ�ɵ�
        
    }

    public void RemoveMonster()
    {
        monsterCount--;
    }
}
