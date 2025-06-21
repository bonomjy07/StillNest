using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class PlacementManager : MonoBehaviour
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

    [Header("유닛 설정")]
    [SerializeField] private GameObject _unitPrefab;
    [SerializeField] private Transform _unitRoot;

    private bool _isSummonMode;
    private Dictionary<Vector3Int, GameObject> _unitMap = new();

    private Vector3Int _previousMousePos = Vector3Int.zero;
    
    // 유닛선택
    private GameObject _draggingUnit; 
    private Vector3Int _draggingFromCell;
    
    // 유닛선택 타일
    private Vector3Int? _selectedCell;
    private Vector3Int? _targetCell;

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            _isSummonMode = !_isSummonMode;
            Debug.Log($"소환 모드: {_isSummonMode}");
            if (!_isSummonMode)
            {
                ClearHighlight();
            }
        }

        Vector3Int mouseCellPos = GetMouseCellPosition();

        if (_isSummonMode)
        {
            HandleSummonMode(mouseCellPos);
        }
        else
        {
            HandleNormalMode(mouseCellPos);
        }
    }

    private void HandleSummonMode(Vector3Int cellPos)
    {
        UpdatePlacementHighlight(cellPos);

        if (Input.GetMouseButtonDown(0))
        {
            if (_unitMap.TryGetValue(cellPos, out GameObject unit))
            {
                _draggingUnit = unit;
                _draggingFromCell = cellPos;
                SelectCell(cellPos);
            }
            else
            {
                TryPlaceUnit(cellPos);
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
        }

        if (_draggingUnit != null && cellPos != _draggingFromCell)
        {
            UpdateTargetCell(cellPos);
        }

        if (Input.GetMouseButtonDown(1))
        {
            TryRemoveUnit(cellPos);
        }
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
        }

        if (_draggingUnit && cellPos != _draggingFromCell)
        {
            UpdateTargetCell(cellPos);
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

    private void TryPlaceUnit(Vector3Int cellPos)
    {
        if (!IsPlaceable(cellPos))
        {
            Debug.Log("해당 타일에는 유닛을 배치할 수 없습니다.");
            return;
        }

        if (_unitMap.ContainsKey(cellPos))
        {
            Debug.Log("이미 유닛이 존재합니다.");
            return;
        }

        Vector3 worldPos = _tilemap.GetCellCenterWorld(cellPos);
        GameObject unit = Instantiate(_unitPrefab, worldPos, Quaternion.identity, _unitRoot);
        _unitMap[cellPos] = unit;
        Debug.Log($"유닛 배치 완료: {cellPos}");
    }

    private void TryRemoveUnit(Vector3Int cellPos)
    {
        if (_unitMap.TryGetValue(cellPos, out GameObject unit))
        {
            Destroy(unit);
            _unitMap.Remove(cellPos);
            Debug.Log($"유닛 제거 완료: {cellPos}");
        }
    }

    private void UpdatePlacementHighlight(Vector3Int cellPos)
    {
        if (cellPos == _previousMousePos)
        {
            return;
        }

        _highlightTilemap.SetTile(_previousMousePos, null);

        bool canPlace = IsPlaceable(cellPos) && !_unitMap.ContainsKey(cellPos);
        Color color = canPlace ? new Color(1f, 1f, 1f, 1f) : new Color(1f, 0f, 0f, 0.4f);

        Tile tileInstance = ScriptableObject.CreateInstance<Tile>();
        tileInstance.sprite = _highlightTile.sprite;
        tileInstance.color = color;

        _highlightTilemap.SetTile(cellPos, tileInstance);
        _previousMousePos = cellPos;
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

    private void ClearHighlight()
    {
        _highlightTilemap.SetTile(_previousMousePos, null);
        _previousMousePos = Vector3Int.zero;
        ClearSelectionHighlight();
    }
}
