using UnityEngine;

public class TestCharacter : MonoBehaviour
{
	[SerializeField]
	private float _moveSpeed = 5f;
	[SerializeField]
	private float _stopDistance = 0.1f;

	private CharacterController _controller;
	private Vector3 _targetPosition;
	private bool _isMoving = false;

	private void Awake()
	{
		_controller = GetComponent<CharacterController>();
	}

	public void MoveTo(Vector3 destination)
	{
		_targetPosition = destination;
		_isMoving = true;
	}

	private void Update()
	{
		if (!_isMoving)
		{
			return;
		}
		
		Vector3 direction = (_targetPosition - transform.position);
		direction.y = 0f; // 수직 이동 제거 (필요 시 제거 안해도 됨)
		float distance = direction.magnitude;

		if (distance <= _stopDistance)
		{
			_isMoving = false;
			return;
		}

		direction.Normalize();
		_controller.Move(direction * (_moveSpeed * Time.deltaTime));
	}
}
