using System;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Serialization;

public class PlacementManager : MonoBehaviour
{
	public Camera sceneCamera;
	public LayerMask placementLayerMask;
	public GameObject mouseIndicator;

	private Vector3 _lastPosition;

	public event Action OnClicked, OnExit;

	private void Update()
	{
		if (Input.GetMouseButtonDown(0))
			OnClicked?.Invoke();
		if (Input.GetKeyDown(KeyCode.Escape))
			OnExit?.Invoke();

		Vector3 mousePosition = GetSelectedMapPosition();
		if (mouseIndicator)
		{
			mouseIndicator.transform.position = mousePosition;
		}
	}

	public bool IsPointerOverUI() => EventSystem.current.IsPointerOverGameObject();

	private Vector3 GetSelectedMapPosition()
	{
		Vector3 mousePos = Input.mousePosition;
		//mousePos.z = sceneCamera.nearClipPlane;
		
		Ray ray = sceneCamera.ScreenPointToRay(mousePos);
		if (Physics.Raycast(ray, out RaycastHit hit))
		{
			_lastPosition = hit.point;
		}
		
		Debug.LogError($"last position is : {_lastPosition}");
		
		return _lastPosition;
	}
}
