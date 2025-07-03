using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class PlacementManager : Singleton<PlacementManager>
{
    [Header("그리드")]
    [SerializeField] private Grid _grid;

    [Header("타일맵 설정")]
    [SerializeField] private Tilemap _tilemap;                // 실제 배경 타일맵
    [SerializeField] private Tilemap _highlightTilemap;       // 하이라이트용 타일맵
    [SerializeField] private Tile _highlightTile;             // 배치 가능 여부 표시용
    [SerializeField] private Tile _currentTileHighlight;      // 유닛 선택 위치용
    [SerializeField] private Tile _targetTileHighlight;       // 이동 목표 위치용

    [Header("배치 조건")]
    [SerializeField] private List<TileBase> _placeableTiles;

    [Header("선 그리기")]
    [SerializeField] private DottedLineDrawer _lineDrawer;

    [Header("유닛 설정")]
    [SerializeField] private GameObject _unitPrefab;
    [SerializeField] private Transform _unitRoot;

    private Dictionary<Vector3Int, GameObject> _unitMap = new();

    // 유닛선택
    private GameObject _draggingUnit; 
    private Vector3Int _draggingFromCell;

    // 유닛선택 타일
    private Vector3Int? _selectedCell;
    private Vector3Int? _targetCell;

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
            if (!_unitMap.ContainsKey(cellPos) && IsPlaceable(cellPos))
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
    }

    private void MoveUnit(GameObject unitObject, Vector3Int from, Vector3Int to)
    {
        Vector3 worldPos = _tilemap.GetCellCenterWorld(to);

        Wizard wizard = unitObject.GetComponent<Wizard>();
        if (wizard) 
        {
            wizard.MoveTo(worldPos);
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

    private bool IsPlaceable(Vector3Int cellPos)
    {
        TileBase currentTile = _tilemap.GetTile(cellPos);
        return _placeableTiles.Contains(currentTile);
    }

    private void SelectCell(Vector3Int cellPos)
    {
        ClearSelectionHighlight();
        _selectedCell = cellPos;
        _highlightTilemap.SetTile(cellPos, _currentTileHighlight);
    }

    private void UpdateTargetCell(Vector3Int cellPos)
    {
        if (_targetCell.HasValue && _targetCell != cellPos)
        {
            _highlightTilemap.SetTile(_targetCell.Value, null);
        }

        _targetCell = cellPos;
        _highlightTilemap.SetTile(cellPos, _targetTileHighlight);
    }

    private void ClearSelectionHighlight()
    {
        if (_selectedCell.HasValue)
        {
            _highlightTilemap.SetTile(_selectedCell.Value, null);
        }

        if (_targetCell.HasValue)
        {
            _highlightTilemap.SetTile(_targetCell.Value, null);
        }

        _selectedCell = null;
        _targetCell = null;
    }

    private void DrawLine(Vector3Int fromCell, Vector3Int toCell)
    {
        Vector3 fromWorld = _tilemap.GetCellCenterWorld(fromCell);
        Vector3 toWorld = _tilemap.GetCellCenterWorld(toCell);
        _lineDrawer.Draw(fromWorld, toWorld);
    }

    public Wizard SpawnHero()
    {
        BoundsInt bounds = _tilemap.cellBounds;

        for (int y = bounds.yMax - 1; y >= bounds.yMin; y--) // 위에서 아래로
        {
            for (int x = bounds.xMin; x < bounds.xMax; x++) // 왼쪽에서 오른쪽으로
            {
                Vector3Int cellPos = new Vector3Int(x, y, 0);

                if (!_tilemap.HasTile(cellPos))
                {
                    continue;
                }

                if (!IsPlaceable(cellPos))
                {
                    continue;
                }

                if (_unitMap.ContainsKey(cellPos))
                {
                    continue;
                }

                Vector3 worldPos = _tilemap.GetCellCenterWorld(cellPos);
                GameObject unit = Instantiate(_unitPrefab, worldPos, Quaternion.identity, _unitRoot);
                _unitMap[cellPos] = unit;

                return unit.GetComponent<Wizard>();
            }
        }

        return null;
    }
}
