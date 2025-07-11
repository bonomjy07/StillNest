using System.Collections;
using UnityEngine;
using UnityEngine.Tilemaps;

public class MonsterMoving : MonoBehaviour
{
    protected Rigidbody2D _rb;
    protected Tilemap _tilemap;

    protected float _speed = 3f; // monster에 따라 다른 speed 적용하려면 추후에 prefab별로 차등적용할것
    protected bool _isDead;

    private SpawnManager _spawnManager;

    protected int _turnIndex = 0;
    protected Vector2[] _turnPos;
    protected float _generalMonsterOffsetY = 0.14f;
    protected float _bossMonsterOffsetY = 0.64f;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        Initialize();
    }

    // Update is called once per frame
    void Update()
    {

    }

    private void FixedUpdate()
    {
        MoveAround();
    }

    protected void Awake()
    {
        _rb = GetComponent<Rigidbody2D>();
        _isDead = false;
    }

    public void Initialize()
    {
        SetTurnPos();
    }

    public void SetTilemap(Tilemap tilemapParameter)
    {
        _tilemap = tilemapParameter;
    }

    protected virtual void SetTurnPos()
    {
        // monster 마다 다른 기준점이 필요하면 여기서 수정
        _turnPos = new Vector2[4];

        BoundsInt bound = _tilemap.cellBounds;
        Vector3Int topRightCell = new Vector3Int(bound.xMax - 2, bound.yMax - 1, 0);
        Vector3Int bottomRightCell = new Vector3Int(bound.xMax - 2, bound.yMin, 0);
        Vector3Int bottomLeftCell = new Vector3Int(bound.xMin + 1, bound.yMin, 0);
        Vector3Int topLeftCell = new Vector3Int(bound.xMin + 1, bound.yMax - 1, 0);

        Vector3 topRightPos = _tilemap.GetCellCenterWorld(topRightCell);
        Vector3 bottomRightPos = _tilemap.GetCellCenterWorld(bottomRightCell);
        Vector3 bottomLeftPos = _tilemap.GetCellCenterWorld(bottomLeftCell);
        Vector3 topLeftPos = _tilemap.GetCellCenterWorld(topLeftCell);

        _turnPos[0] = new Vector2(topRightPos.x, topRightPos.y + _generalMonsterOffsetY);
        _turnPos[1] = new Vector2(bottomRightPos.x, bottomRightPos.y + _generalMonsterOffsetY);
        _turnPos[2] = new Vector2(bottomLeftPos.x, bottomLeftPos.y + _generalMonsterOffsetY);
        _turnPos[3] = new Vector2(topLeftPos.x, topLeftPos.y + _generalMonsterOffsetY);
    }

    protected void MoveAround()
    {
        if (!_isDead)
        {
            Vector2 targetPos = _turnPos[_turnIndex];
            Vector2 moveDirection = (targetPos - _rb.position).normalized;

            Vector3 originScale = transform.localScale;
            if (moveDirection.x > 0.01f || moveDirection.y > 0.01f) // right direction
                transform.localScale = new Vector3(Mathf.Abs(originScale.x), originScale.y, originScale.z);
            else if (moveDirection.x < -0.01f || moveDirection.y < -0.01f) // left direction
                transform.localScale = new Vector3(-Mathf.Abs(originScale.x), originScale.y, originScale.z);

            _rb.MovePosition(_rb.position + moveDirection * _speed * Time.fixedDeltaTime);
            if (Vector2.Distance(_rb.position, targetPos) < 0.1f)
            {
                _turnIndex = (_turnIndex + 1) % _turnPos.Length;
            }
        }
    }

    public void NoticeMonsterDeath()
    {
        _isDead = true;
    }
}

