using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class HeroPlacementController : MonoBehaviour
{
    [Header("[Highlight]")]
    [SerializeField] private Tile _currentTileHighlight; // 유닛 선택 위치용
    [SerializeField] private Tile _targetTileHighlight; // 이동 목표 위치용

    [Header("배치 조건")]
    [SerializeField] private List<TileBase> _placeableTiles;

    [Header("선 그리기")]
    [SerializeField] private DottedLineDrawer _lineDrawer;

    [Header("유닛 설정")]
    [SerializeField] private Transform _unitRoot;
    [SerializeField] private List<GameObject> _heroPrefabList;

    // 영웅 배치
    private Grid _grid;
    private Tilemap _heroTileMap;
    
    // 하이라이트
    private TilemapRenderer _heroTileMapRenderer;
    private Tilemap _selectTileMap;
    
    [SerializeField]// 디버깅용으로 추가
    private Dictionary<Vector3Int, GameObject> _unitMap = new();

    // 유닛선택
    private GameObject _draggingUnit; 
    private Vector3Int _draggingFromCell;

    // 유닛선택 타일
    private Vector3Int? _selectedCell;
    private Vector3Int? _targetCell;

    private void Start()
    {
        // Map Setting
        _grid = DuoMap.Inst.grid;
        _selectTileMap = DuoMap.Inst.selectHighlightTileMap;
        
        // Highlight Color
        _targetTileHighlight.color = Color.white;
    }

    private void Update()
    {
        Vector3Int mouseCellPos = GetMouseCellPosition();

        HandleNormalMode(mouseCellPos);
    }

    private void HandleNormalMode(Vector3Int cellPos)
    {
        if (Input.GetMouseButtonDown(0))
        {
            if (_unitMap.TryGetValue(cellPos, out GameObject unit))
            {
                _draggingUnit = unit;
                _draggingFromCell = cellPos;
                SelectCell(cellPos);
            }
        }

        if (Input.GetMouseButtonUp(0) && _draggingUnit)
        {
            if (IsEmptyTile(cellPos))
            {
                MoveUnit(_draggingUnit, _draggingFromCell, cellPos);
            }

            _draggingUnit = null;
            ClearSelectionHighlight();
            _lineDrawer.Clear();
        }

        if (_draggingUnit && cellPos != _draggingFromCell)
        {
            UpdateTargetCell(cellPos);
            DrawLine(_draggingFromCell, cellPos);
        }

        UpdateGroundLine();
    }

    private void MoveUnit(GameObject unitObject, Vector3Int from, Vector3Int to)
    {
        Vector3 worldPos = _heroTileMap.GetCellCenterWorld(to);

        HeroUnit unit = unitObject.GetComponent<HeroUnit>();
        if (unit) 
        {
            unit.MoveTo(worldPos);
        }

        _unitMap.Remove(from);
        _unitMap[to] = unitObject;
        Debug.Log($"유닛 이동 완료: {from} → {to}");
    }

    private Vector3Int GetMouseCellPosition()
    {
        Vector3 worldPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        worldPos.z = 0;
        return _grid.WorldToCell(worldPos);
    }

    private bool IsEmptyTile(Vector3Int cellPos)
    {
        TileBase currentTile = _heroTileMap.GetTile(cellPos);
        return  currentTile && !_unitMap.ContainsKey(cellPos);
        //return _placeableTiles.Contains(currentTile) && !_unitMap.ContainsKey(cellPos);
    }

    private void SelectCell(Vector3Int cellPos)
    {
        ClearSelectionHighlight();
        _selectedCell = cellPos;
        _selectTileMap.SetTile(cellPos, _currentTileHighlight);
    }

    private void UpdateTargetCell(Vector3Int cellPos)
    {
        if (_targetCell.HasValue && _targetCell != cellPos)
        {
            _selectTileMap.SetTile(_targetCell.Value, null);
        }

        _targetCell = cellPos;
        _targetTileHighlight.color = IsEmptyTile(cellPos) ? Color.white : Color.red;
        _selectTileMap.SetTile(cellPos, _targetTileHighlight);
    }

    private void ClearSelectionHighlight()
    {
        if (_selectedCell.HasValue)
        {
            _selectTileMap.SetTile(_selectedCell.Value, null);
        }

        if (_targetCell.HasValue)
        {
            _selectTileMap.SetTile(_targetCell.Value, null);
        }

        _selectedCell = null;
        _targetCell = null;
    }

    private void UpdateGroundLine()
    {
        if (_draggingUnit)
        {
            ShowGroundLine();
        }
        else
        {
            HideGroundLine();
        }
    }

    private void ShowGroundLine()
    {
        if (_heroTileMapRenderer)
        {
            _heroTileMapRenderer.enabled = true;
        }
    }

    private void HideGroundLine()
    {
        if (_heroTileMapRenderer && _heroTileMapRenderer.enabled)
        {
            _heroTileMapRenderer.enabled = false;
        }
    }
    
    private void DrawLine(Vector3Int fromCell, Vector3Int toCell)
    {
        Vector3 fromWorld = _heroTileMap.GetCellCenterWorld(fromCell);
        Vector3 toWorld = _heroTileMap.GetCellCenterWorld(toCell);
        _lineDrawer.Draw(fromWorld, toWorld);
    }

    public void SetPlayerIndex(int playerIndex)
    {
        _heroTileMap = DuoMap.Inst.GetMyHeroTileMap(playerIndex);
        _heroTileMapRenderer = DuoMap.Inst.GetMyHeroTileMapRenderer(playerIndex);
    }

    public HeroUnit SpawnHero()
    {
        BoundsInt bounds = _heroTileMap.cellBounds;

        for (int y = bounds.yMax - 1; y >= bounds.yMin; y--) // 위에서 아래로
        {
            for (int x = bounds.xMin; x < bounds.xMax; x++) // 왼쪽에서 오른쪽으로
            {
                Vector3Int cellPos = new Vector3Int(x, y, 0);

                if (!_heroTileMap.HasTile(cellPos))
                {
                    continue;
                }

                if (!IsEmptyTile(cellPos))
                {
                    continue;
                }

                Vector3 worldPos = _heroTileMap.GetCellCenterWorld(cellPos);
                GameObject unitPrefab = _heroPrefabList[Random.Range(0, _heroPrefabList.Count)];
                GameObject unitInstance = Instantiate(unitPrefab, worldPos, Quaternion.identity, _unitRoot);
                _unitMap[cellPos] = unitInstance;

                return unitInstance.GetComponent<HeroUnit>();
            }
        }

        return null;
    }
}
