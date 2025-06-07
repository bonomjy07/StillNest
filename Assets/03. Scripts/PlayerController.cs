using System;
using System.Collections.Generic;
using System.Linq;
using Cinemachine;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    [Header("[Camera]")]
    public CinemachineVirtualCamera virtualCamera;
    public Transform cameraRig; // 리그를 따로 둔다
    public CharacterController controller; // cameraRig에 붙어 있어야 함

    [Header("[Character Movement]")]
    public List<AICharacter> characters;
    public float spacing = 5.0f;
    public LayerMask groundLayer;

    [Header("[Zoom]")]
    public float zoomSpeed = 5f;
    public float zoomMin = 3f;
    public float zoomMax = 20f;

    [Header("[Movement Speed]")]
    public float mainSpeed = 100.0f;
    public float shiftAdd = 250.0f;
    public float maxShift = 1000.0f;

    [Header("[Mouse]")]
    public float maxLookUpAngle = 40f;
    public float maxLookDownAngle = -30f;
    public float camSensitivity = 0.2f;

    private Vector3 _lastMouse;
    private float _shiftHeldTime = 1.0f;
    private bool _isRotating;
    private bool _wasRotating;
    
    // Camera
    private Camera _mainCamera;

    private void Start()
    {
        _mainCamera = Camera.main;
    }

    private void Update()
    {
        HandleCharacterMovement();
        
        HandleZoomInput();
        
        HandleRotationInput();
        
        HandleMovementInput();
    }

    private void HandleCharacterMovement()
    {
        if (!Input.GetMouseButtonDown(0))
        {
            return;
        }
        
        if (!_mainCamera)
        {
            return;
        }
        
        Ray ray = _mainCamera.ScreenPointToRay(Input.mousePosition);
        if (!Physics.Raycast(ray, out RaycastHit hit, Mathf.Infinity, groundLayer))
        {
            return;
        }

        int unitCount = characters.Count;
        int columns = Mathf.CeilToInt(Mathf.Sqrt(unitCount));
        int rows = Mathf.CeilToInt((float)unitCount / columns);

        Vector3 center = hit.point;
        int index = 0;

        for (int row = 0; row < rows; row++)
        {
            for (int col = 0; col < columns; col++)
            {
                if (index >= unitCount) break;

                float xOffset = (col - (columns - 1) / 2f) * spacing;
                float zOffset = (row - (rows - 1) / 2f) * spacing;
                Vector3 offset = new Vector3(xOffset, 0f, zOffset);
                Vector3 destination = center + offset;

                characters[index].MoveTo(destination);
                index++;
            }
        }
    }

    private void HandleZoomInput()
    {
        float scroll = Input.GetAxis("Mouse ScrollWheel");
        if ((Mathf.Abs(scroll) == 0.00f) || !virtualCamera)
        {
            return;
        }

        Cinemachine3rdPersonFollow follow = virtualCamera.GetCinemachineComponent<Cinemachine3rdPersonFollow>();
        if (!follow)
        {
            return;
        } 
        
        // 카메라와 대상 간 거리
        float currentZ = follow.CameraDistance;
        currentZ -= scroll * zoomSpeed;
        currentZ = Mathf.Clamp(currentZ, zoomMin, zoomMax);

        follow.CameraDistance = currentZ;
    }
    
   
    private void HandleRotationInput()
    {
        bool isRotatingCamera = Input.GetKey(KeyCode.Mouse2);
        if (!isRotatingCamera)
        {
            _wasRotating = false;
            return;
        }
        
        if (!_wasRotating)
        {
            _wasRotating = true;
            _lastMouse = Input.mousePosition;
            return;
        }

        Vector3 mouseDelta = Input.mousePosition - _lastMouse;
        mouseDelta = new Vector3(-mouseDelta.y * camSensitivity, mouseDelta.x * camSensitivity, 0);

        // 현재 회전 각도
        Vector3 eulerAngles = cameraRig.eulerAngles;
        float currentPitch = eulerAngles.x;
        float currentYaw = eulerAngles.y;

        // pitch 계산 및 제한
        float pitchDelta = mouseDelta.x;
        float targetPitchRaw = currentPitch + pitchDelta;
        float targetPitch = Mathf.DeltaAngle(0f, targetPitchRaw); // -180 ~ 180도 해석
        float clampedPitch = Mathf.Clamp(targetPitch, maxLookDownAngle, maxLookUpAngle);
        float finalPitch = clampedPitch < 0f ? clampedPitch + 360f : clampedPitch;

        float newYaw = currentYaw + mouseDelta.y;

        cameraRig.eulerAngles = new Vector3(finalPitch, newYaw, 0);

        _lastMouse = Input.mousePosition;
    }

    private void HandleMovementInput()
    {
        Vector3 inputDirection = GetMovementInput();
        if (inputDirection.sqrMagnitude == 0)
        {
            return;
        }

        inputDirection = cameraRig.TransformDirection(inputDirection);

        if (Input.GetKey(KeyCode.LeftShift))
        {
            _shiftHeldTime += Time.deltaTime;
            Vector3 accelerated = inputDirection * (_shiftHeldTime * shiftAdd);
            accelerated.x = Mathf.Clamp(accelerated.x, -maxShift, maxShift);
            accelerated.y = Mathf.Clamp(accelerated.y, -maxShift, maxShift);
            accelerated.z = Mathf.Clamp(accelerated.z, -maxShift, maxShift);
            inputDirection = accelerated;
        }
        else
        {
            _shiftHeldTime = Mathf.Clamp(_shiftHeldTime * 0.5f, 1f, 1000f);
            inputDirection *= mainSpeed;
        }

        inputDirection *= Time.deltaTime;

        controller.Move(inputDirection);
    }

    private Vector3 GetMovementInput()
    {
        Vector3 direction = Vector3.zero;
        if (Input.GetKey(KeyCode.W)) direction += Vector3.forward;
        if (Input.GetKey(KeyCode.S)) direction += Vector3.back;
        if (Input.GetKey(KeyCode.A)) direction += Vector3.left;
        if (Input.GetKey(KeyCode.D)) direction += Vector3.right;
        return direction;
    }
}
