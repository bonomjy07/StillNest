using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class PlacementManager : MonoBehaviour
{
    [Header("그리드")]
    [SerializeField] private Grid _grid;

    [Header("타일맵 설정")]
    [SerializeField] private Tilemap _tilemap;                // 실제 타일맵 (배경)
    [SerializeField] private Tilemap _highlightTilemap;       // 하이라이트용 타일맵
    [SerializeField] private Tile _highlightTile;             // 하이라이트 기본 타일

    [Header("배치 조건")]
    [SerializeField] private List<TileBase> _placeableTiles;  // 배치 가능 타일들

    [Header("유닛 설정")]
    [SerializeField] private GameObject _unitPrefab;          // 배치할 유닛 프리팹

    private Vector3Int _previousMousePos = Vector3Int.zero;
    private bool _isPlacementMode;
    private Dictionary<Vector3Int, GameObject> _unitMap = new(); // 유닛 위치 기록

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            _isPlacementMode = !_isPlacementMode;
            Debug.Log($"배치 모드: {_isPlacementMode}");

            if (!_isPlacementMode)
            {
                ClearHighlight();
            }
        }

        if (_isPlacementMode)
        {
            Vector3Int mouseCellPos = GetMouseCellPosition();
            UpdateHighlight(mouseCellPos);

            if (Input.GetMouseButtonDown(0))
            {
                TryPlaceUnit(mouseCellPos);
            }
        }
    }

    private Vector3Int GetMouseCellPosition()
    {
        Vector3 worldPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        return _grid.WorldToCell(worldPos);
    }

    private void UpdateHighlight(Vector3Int cellPos)
    {
        if (cellPos == _previousMousePos)
            return;

        _highlightTilemap.SetTile(_previousMousePos, null);

        bool canPlace = IsPlaceable(cellPos) && !_unitMap.ContainsKey(cellPos);
        Color color = canPlace ? new Color(1f, 1f, 1f, 1f) : new Color(1f, 0f, 0f, 0.4f);

        Tile tileInstance = ScriptableObject.CreateInstance<Tile>();
        tileInstance.sprite = _highlightTile.sprite;
        tileInstance.color = color;

        _highlightTilemap.SetTile(cellPos, tileInstance);
        _previousMousePos = cellPos;
    }

    private void ClearHighlight()
    {
        _highlightTilemap.SetTile(_previousMousePos, null);
        _previousMousePos = Vector3Int.zero;
    }

    private bool IsPlaceable(Vector3Int cellPos)
    {
        TileBase currentTile = _tilemap.GetTile(cellPos);
        return _placeableTiles.Contains(currentTile);
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
        GameObject unit = Instantiate(_unitPrefab, worldPos, Quaternion.identity);
        _unitMap[cellPos] = unit;

        Debug.Log($"유닛 배치 완료: {cellPos}");
    }
}
