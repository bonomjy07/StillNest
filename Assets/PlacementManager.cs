using System;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Serialization;
using UnityEngine.Tilemaps;

public class PlacementManager : MonoBehaviour
{
	[Header("타일맵 설정")]
	[SerializeField] private Tilemap _tilemap;                // 실제 타일맵
	[SerializeField] private Tilemap _highlightTilemap;       // 하이라이트 표시용 타일맵
	[SerializeField] private Tile _highlightTile;             // 하이라이트 타일 (반투명 등)

	private bool _isPlacementMode = false;                    // 배치 모드 여부
	private Vector3Int _prevCellPos = Vector3Int.zero;        // 이전 하이라이트 위치

	private void Update()
	{
		// 배치 모드 토글: Space 키
		if (Input.GetKeyDown(KeyCode.Space))
		{
			TogglePlacementMode();
		}

		// 배치 모드일 때만 하이라이트
		if (_isPlacementMode)
		{
			HandleMouseHighlight();
		}
		else
		{
			ClearHighlight();
		}
	}

	/// <summary>
	/// 배치 모드 on/off 전환
	/// </summary>
	public void TogglePlacementMode()
	{
		_isPlacementMode = !_isPlacementMode;
		Debug.Log($"배치 모드: {_isPlacementMode}");

		if (!_isPlacementMode)
		{
			ClearHighlight();
		}
	}

	/// <summary>
	/// 마우스 아래 타일을 하이라이트함
	/// </summary>
	private void HandleMouseHighlight()
	{
		Vector3 mouseWorldPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
		Vector3Int cellPos = _tilemap.WorldToCell(mouseWorldPos);

		if (cellPos != _prevCellPos)
		{
			_highlightTilemap.SetTile(_prevCellPos, null); // 이전 하이라이트 제거
			_highlightTilemap.SetTile(cellPos, _highlightTile); // 현재 위치에 표시
			
			_prevCellPos = cellPos;
		}
	}

	/// <summary>
	/// 하이라이트 제거
	/// </summary>
	private void ClearHighlight()
	{
		_highlightTilemap.SetTile(_prevCellPos, null);
		_prevCellPos = Vector3Int.zero;
	}
	
}
