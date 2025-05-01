using KinematicCharacterController;
using KinematicCharacterController.Examples;
using UnityEngine;
using UnityEngine.Serialization;

public class TestManager : MonoBehaviour
{
    public ExampleCharacterController aiCharacter;
    public float stopDistance = 0.1f;

    private Vector3 _targetPosition;
    private bool _isMoving;

    private void Update()
    {
        if (Input.GetMouseButtonDown(0)) // 마우스 왼쪽 클릭
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;
            if (Physics.Raycast(ray, out hit))
            {
                _targetPosition = hit.point;
                _isMoving = true;
            }
        }

        if (_isMoving)
        {
            Vector3 direction = (_targetPosition - transform.position);
            direction.y = 0f;
            float distance = direction.magnitude;
            if (distance <= stopDistance)
            {
                _isMoving = false;
                return;
            }

            direction.Normalize();

            AICharacterInputs inputs = new AICharacterInputs()
            {
                LookVector = direction,
                MoveVector = direction,
            };

            if (aiCharacter)
            {
                aiCharacter.SetInputs(ref inputs);
            }
        }
    }
}
