using System.Collections;
using UnityEngine;

public class MonsterController : MonoBehaviour
{
    private Rigidbody rb;
    private float speed = 3f; // monster에 따라 다른 speed 적용하려면 public 하고 prefab별로 차등적용할것
    public float health; // 요녀석을 웨이브 wave를 이용한 수식을 만들어서 난이도를 올릴건지 
    // 아니면 프리팹을 웨이브마다 하나씩 설정해서 거기에다가 직접 입력할 건지 2가지를 생각해봤음

    public SpawnManager spawnManager;

    // 몬스터의 피격 시 색상 변경을 위한 변수
    private Renderer rend;
    private Color originColor;
    private float flashDuration = 0.2f;

    //public Transform[] turningPoints;
    private int turnIndex = 0;

    public Vector3[] turnPos;

    //private Vector3 rightUpPos = new Vector3(-22, 0, 9);
    //private Vector3 rightDownPos = new Vector3(-22, 0, 9);

    private int leftPosX = 0;
    private int rightPosX = -22;
    private int upPosZ = 9;
    private int downPosZ = 31;
    // 좌표로 설정하려다가 맵 타일 position이용쪽으로 시도

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        rb = GetComponent<Rigidbody>();
        spawnManager = GameObject.Find("Spawn Manager").GetComponent<SpawnManager>(); // SpawnManager script
        rend = GetComponent<Renderer>();
        originColor = rend.material.color;

        turnPos = new Vector3[4];
        turnPos[0] = new Vector3(rightPosX, 0, upPosZ);
        turnPos[1] = new Vector3(rightPosX, 0, downPosZ);
        turnPos[2] = new Vector3(leftPosX, 0, downPosZ);
        turnPos[3] = new Vector3(leftPosX, 0, upPosZ);

        //GameObject[] points = GameObject.FindGameObjectsWithTag("TurningPoint");
        //turningPoints = new Transform[points.Length];
        //for (int i = 0; i < points.Length; i++)
        //    turningPoints[i] = points[i].transform;
        // Tag로 했더니 이게 내가 원하는 순서대로 안담겨옴

    }

    // Update is called once per frame
    void Update()
    {
        if(health <= 0)
        {
            Destroy(gameObject);
            spawnManager.RemoveMonster(); // SpawnManager쪽에 몬스터 죽을때 알리는건데 미완성
        }
    }

    private void FixedUpdate()
    {
        //Vector3 targetPos = turningPoints[turnIndex].position;
        Vector3 targetPos = turnPos[turnIndex];
        targetPos.y = transform.position.y;
        Vector3 moveDirection = (targetPos - transform.position).normalized;

        rb.MovePosition(rb.position + moveDirection * speed * Time.fixedDeltaTime);

        if (Vector3.Distance(transform.position, targetPos) < 0.1f)
        {
            turnIndex = (turnIndex + 1) % turnPos.Length;
            //turnIndex = (turnIndex + 1) % turningPoints.Length;
            //turnIndex = (turnIndex + 1) % 4;
            //currentIndex = (currentIndex + 1) % waypoints.Length;
            Debug.Log("TurnIndex Changed : " + turnIndex);
        }
    }

    public void TakeDamage(float dmg)
    {
        health -= dmg;
        StartCoroutine(HitEffect());
    }

    // 몬스터 공격받을 시 빨간색 이펙트
    IEnumerator HitEffect()
    {
        rend.material.color = Color.red;
        yield return new WaitForSeconds(flashDuration);
        rend.material.color = originColor;
    }
}

