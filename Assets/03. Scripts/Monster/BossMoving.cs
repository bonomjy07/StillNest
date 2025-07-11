using UnityEngine;

public class BossMoving : MonsterMoving
{
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

    protected override void SetTurnPos()
    {
        //base.SetTurnPos();
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

        _turnPos[0] = new Vector2(topRightPos.x, topRightPos.y + _bossMonsterOffsetY);
        _turnPos[1] = new Vector2(bottomRightPos.x, bottomRightPos.y + _bossMonsterOffsetY);
        _turnPos[2] = new Vector2(bottomLeftPos.x, bottomLeftPos.y + _bossMonsterOffsetY);
        _turnPos[3] = new Vector2(topLeftPos.x, topLeftPos.y + _bossMonsterOffsetY);
    }
}
