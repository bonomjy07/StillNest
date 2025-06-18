using System.Collections;
using UnityEngine;

public class MonsterController : MonoBehaviour
{
    private Rigidbody rb;
    private float speed = 3f; // monster�� ���� �ٸ� speed �����Ϸ��� public �ϰ� prefab���� ���������Ұ�
    public float health; // ��༮�� ���̺� wave�� �̿��� ������ ���� ���̵��� �ø����� 
    // �ƴϸ� �������� ���̺긶�� �ϳ��� �����ؼ� �ű⿡�ٰ� ���� �Է��� ���� 2������ �����غ���

    public SpawnManager spawnManager;

    // ������ �ǰ� �� ���� ������ ���� ����
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
    // ��ǥ�� �����Ϸ��ٰ� �� Ÿ�� position�̿������� �õ�

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
        // Tag�� �ߴ��� �̰� ���� ���ϴ� ������� �ȴ�ܿ�

    }

    // Update is called once per frame
    void Update()
    {
        if(health <= 0)
        {
            Destroy(gameObject);
            spawnManager.RemoveMonster(); // SpawnManager�ʿ� ���� ������ �˸��°ǵ� �̿ϼ�
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

    // ���� ���ݹ��� �� ������ ����Ʈ
    IEnumerator HitEffect()
    {
        rend.material.color = Color.red;
        yield return new WaitForSeconds(flashDuration);
        rend.material.color = originColor;
    }
}

