using System.Collections;
using UnityEngine;
using UnityEngine.Tilemaps;

public class MonsterController : MonoBehaviour
{
    //private Rigidbody rb; // #2D ������
    private Rigidbody2D rb;
    private Tilemap tilemap;

    private float speed = 3f; // monster�� ���� �ٸ� speed �����Ϸ��� public �ϰ� prefab���� ���������Ұ�
    public float health;
    // ������ ����� �������� ���̺� ���� �ɷ�ġ �ٸ��� �Ҵ��ϴ� ������� ������

    public SpawnManager spawnManager;

    // ������ �ǰ� �� ���� ������ ���� ����
    private Renderer rend;
    private Color originColor;
    private float flashDuration = 0.2f;

    //public Transform[] turningPoints;
    private int turnIndex = 0;
    public Vector2[] turnPos;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        spawnManager = GameObject.Find("Spawn Manager").GetComponent<SpawnManager>(); // SpawnManager script
        rend = GetComponent<Renderer>();
        originColor = rend.material.color;

        // turning point
        SetTurnPos();

    }

    // Update is called once per frame
    void Update()
    {
        //if (health <= 0)
        //{
        //    Destroy(gameObject);
        //    spawnManager.RemoveMonster(); // SpawnManager�ʿ� ���� ������ �˸��°ǵ� �̿ϼ�
        //}
    }

    private void FixedUpdate()
    {
        Vector2 targetPos = turnPos[turnIndex];
        Vector2 moveDirection = (targetPos - rb.position).normalized;

        Vector3 originScale = transform.localScale;
        if (moveDirection.x > 0.01f) // right direction
            transform.localScale = new Vector3(Mathf.Abs(originScale.x), originScale.y, originScale.z);
        else if(moveDirection.x < -0.01f) // left direction
            transform.localScale = new Vector3(-Mathf.Abs(originScale.x), originScale.y, originScale.z);

        rb.MovePosition(rb.position + moveDirection * speed * Time.fixedDeltaTime);
        if (Vector2.Distance(rb.position, targetPos) < 0.1f)
        {
            turnIndex = (turnIndex + 1) % turnPos.Length;
        }
    }

    public void SetTilemap(Tilemap tilemapParameter)
    {
        tilemap = tilemapParameter;
    }

    public void SetTurnPos()
    {
        // monster ���� �ٸ� �������� �ʿ��ϸ� ���⼭ ����
        turnPos = new Vector2[4];

        BoundsInt bound = tilemap.cellBounds;
        Vector3Int topRightCell = new Vector3Int(bound.xMax - 2, bound.yMax - 1, 0);
        Vector3Int bottomRightCell = new Vector3Int(bound.xMax - 2, bound.yMin, 0);
        Vector3Int bottomLeftCell = new Vector3Int(bound.xMin + 1, bound.yMin, 0);
        Vector3Int topLeftCell = new Vector3Int(bound.xMin + 1, bound.yMax - 1, 0);

        Vector3 topRightPos = tilemap.GetCellCenterWorld(topRightCell);
        Vector3 bottomRightPos = tilemap.GetCellCenterWorld(bottomRightCell);
        Vector3 bottomLeftPos = tilemap.GetCellCenterWorld(bottomLeftCell);
        Vector3 topLeftPos = tilemap.GetCellCenterWorld(topLeftCell);

        turnPos[0] = new Vector2(topRightPos.x, topRightPos.y + 0.14f);
        turnPos[1] = new Vector2(bottomRightPos.x, bottomRightPos.y + 0.14f);
        turnPos[2] = new Vector2(bottomLeftPos.x, bottomLeftPos.y + 0.14f);
        turnPos[3] = new Vector2(topLeftPos.x, topLeftPos.y + 0.14f);
    }

    public void TakeDamage(float dmg)
    {
        health -= dmg;
        //StartCoroutine(HitEffect());
    }

    // ���� ���ݹ��� �� ������ ����Ʈ
    IEnumerator HitEffect()
    {
        rend.material.color = Color.red;
        yield return new WaitForSeconds(flashDuration);
        rend.material.color = originColor;
    }
}

