using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.Tilemaps;

public class MonsterMoving : MonoBehaviour
{
    protected Rigidbody2D _rb;
    protected Tilemap _pathTilemap;
    protected TileBase _pathTile;
    protected SpriteRenderer _sr;

    protected PlayerNumber _playerNumber; // 1p(= 0), 2p(= 1)
    protected MonsterType _monsterType; // general(= 0), boss(= 1)

    protected float _speed = 3f; // monster에 따라 다른 speed 적용하려면 추후에 prefab별로 차등적용할것
    protected bool _isDead;

    private SpawnManager _spawnManager;

    //public Vector3Int bb;

    protected int _turnIndex = 0;
    protected Vector2[] _turnPos;
    protected float _generalMonsterOffsetY = 0.14f;
    protected float _bossMonsterOffsetY = 0.64f;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        //Initialize();
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
        _sr = GetComponent<SpriteRenderer>();
        _isDead = false;
    }

    public void Initialize(PlayerNumber playerNum, MonsterType type)
    {
        _playerNumber = playerNum;
        _monsterType = type;
        _pathTilemap = DuoMap.Inst.GetPathTileMap((int)_playerNumber);
        _pathTile = DuoMap.Inst.GetPathTileBase();
        SetTurnPos(_playerNumber, _monsterType);
    }

    //public void SetTilemap(Tilemap tilemapParameter)
    //{
    //    _tilemap = tilemapParameter;
    //}

    protected virtual void SetTurnPos(PlayerNumber player, MonsterType type)
    {
        // monster 마다 다른 기준점이 필요하면 여기서 수정
        _turnPos = new Vector2[4];
        BoundsInt bound = _pathTilemap.cellBounds;

        Vector3Int topRightCell = new Vector3Int(bound.xMax - 1, bound.yMax - 1, 0);
        Vector3Int bottomRightCell = new Vector3Int(bound.xMax - 1, bound.yMin, 0);
        Vector3Int bottomLeftCell = new Vector3Int(bound.xMin, bound.yMin, 0);
        Vector3Int topLeftCell = new Vector3Int(bound.xMin, bound.yMax - 1, 0);

        Vector3 topRightPos = _pathTilemap.GetCellCenterWorld(topRightCell);
        Vector3 bottomRightPos = _pathTilemap.GetCellCenterWorld(bottomRightCell);
        Vector3 bottomLeftPos = _pathTilemap.GetCellCenterWorld(bottomLeftCell);
        Vector3 topLeftPos = _pathTilemap.GetCellCenterWorld(topLeftCell);

        if (player == PlayerNumber.Player1)
        {
            _turnPos[0] = new Vector2(topRightPos.x, topRightPos.y);
            _turnPos[1] = new Vector2(bottomRightPos.x, bottomRightPos.y);
            _turnPos[2] = new Vector2(bottomLeftPos.x, bottomLeftPos.y);
            _turnPos[3] = new Vector2(topLeftPos.x, topLeftPos.y);
        }
        else if (player == PlayerNumber.Player2)
        {
            _turnPos[0] = new Vector2(topLeftPos.x, topLeftPos.y);
            _turnPos[1] = new Vector2(bottomLeftPos.x, bottomLeftPos.y);
            _turnPos[2] = new Vector2(bottomRightPos.x, bottomRightPos.y);
            _turnPos[3] = new Vector2(topRightPos.x, topRightPos.y);
        }

        for (int i=0; i<4; i++)
        {
            if (type == MonsterType.General)
                _turnPos[i].y += _generalMonsterOffsetY;
            else if (type == MonsterType.Boss)
                _turnPos[i].y += _bossMonsterOffsetY;
        }

    }

    protected void MoveAround()
    {
        if (!_isDead)
        {
            Vector2 targetPos = _turnPos[_turnIndex];
            Vector2 moveDirection = (targetPos - _rb.position).normalized;

            Vector3 originScale = transform.localScale;
            if (moveDirection.x > 0.01f)
                _sr.flipX = false;
            else if (moveDirection.x < -0.01f)
                _sr.flipX = true;
            else if (moveDirection.y > 0.01f)
            {
                if (_playerNumber == PlayerNumber.Player1)
                    _sr.flipX = false;
                else if (_playerNumber == PlayerNumber.Player2)
                    _sr.flipX = true;
            }
            else if (moveDirection.y < -0.01f)
            {
                if (_playerNumber == PlayerNumber.Player1)
                    _sr.flipX = true;
                else if (_playerNumber == PlayerNumber.Player2)
                    _sr.flipX = false;
            }

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

