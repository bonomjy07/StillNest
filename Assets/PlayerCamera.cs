using System;
using UnityEngine;
using UnityEngine.Serialization;

public class PlayerCamera : MonoBehaviour
{
    [Header("[Speed]")]
    public float mainSpeed = 100.0f; //regular speed
    public float shiftAdd = 250.0f; //multiplied by how long shift is held.  Basically running
    public float maxShift = 1000.0f; //Maximum speed when holdin gshift
    
    public float camSensitivity = 0.2f;
    
    [Header("[Mouse]")]
    public float maxLookUpAngle = 40f;   // 위로 최대 40도
    public float maxLookDownAngle = -30f; // 아래로 최대 -30도 (음수)
    
    private Vector3 _lastMouse = new Vector3(255, 255, 255); //kind of in the middle of the screen, rather than at the top (play)
    private bool _wasRotating; 
    private float _shiftHeldTime = 1.0f;

    private void Update ()
    {
        // Mouse commands
        HandleMouseInput();
         
        // Keyboard commands
        HandleKeyboardInput();
    }

    private void HandleMouseInput()
    {
        bool isRotatingCamera = Input.GetKey(KeyCode.Mouse1);
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
        Vector3 eulerAngles = transform.eulerAngles;
        float currentPitch = eulerAngles.x;
        float currentYaw = eulerAngles.y;

        // pitch 계산 및 제한
        float pitchDelta = mouseDelta.x;
        float targetPitchRaw = currentPitch + pitchDelta;
        float targetPitch = Mathf.DeltaAngle(0f, targetPitchRaw); // -180 ~ 180도 해석
        float clampedPitch = Mathf.Clamp(targetPitch, maxLookDownAngle, maxLookUpAngle);
        float finalPitch = clampedPitch < 0f ? clampedPitch + 360f : clampedPitch;

        float newYaw = currentYaw + mouseDelta.y;

        transform.eulerAngles = new Vector3(finalPitch, newYaw, 0);

        _lastMouse = Input.mousePosition;
    }

    private void HandleKeyboardInput()
    {
        Vector3 inputDirection = GetMovementInput();
        if (inputDirection.sqrMagnitude == 0)
        {
            return;
        }

        if (Input.GetKey(KeyCode.LeftShift))
        {
            _shiftHeldTime += Time.deltaTime;

            Vector3 accelerated = inputDirection * _shiftHeldTime * shiftAdd;
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

        if (Input.GetKey(KeyCode.Space))
        {
            // XZ 평면에서만 이동 (Y는 고정)
            float originalY = transform.position.y;
            transform.Translate(inputDirection);
            Vector3 correctedPosition = transform.position;
            correctedPosition.y = originalY;
            transform.position = correctedPosition;
        }
        else
        {
            transform.Translate(inputDirection);
        }
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
