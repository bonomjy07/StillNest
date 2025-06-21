using System;
using UnityEngine;
using UnityEngine.Serialization;

public class Wizard : MonoBehaviour
{
	[Header("[Movement]")]
	[SerializeField] private float _moveSpeed = 2f;

	// Components
	private Animator _animator;
	private SpriteRenderer _spriteRenderer; 
	
	private Vector3 _destination;
	private bool _isMoving;
	
	private const float STOP_THRESHOLD = 0.05f;

	private static readonly int SpeedHash = Animator.StringToHash("Speed");
	private static readonly int AttackHash = Animator.StringToHash("Attack");

	private void Start()
	{
		_animator = GetComponent<Animator>();
		_spriteRenderer = GetComponent<SpriteRenderer>();
		_destination = transform.position;
	}

	private void Update()
	{
		if (_isMoving)
		{
			UpdateMovement();
		}
	}

	private void UpdateMovement()
	{
		Vector3 direction = transform.position - _destination;
		float distance = direction.magnitude;

		if (distance < STOP_THRESHOLD)
		{
			transform.position = _destination;
			_isMoving = false;
			_animator.SetFloat(SpeedHash, 0f);
			return;
		}

		Vector3 moveDir = -direction.normalized;
		UpdateFacing(moveDir);
		transform.position += moveDir * (_moveSpeed * Time.deltaTime);
		_animator.SetFloat(SpeedHash, _moveSpeed);
	}
	
	private void UpdateFacing(Vector3 moveDir)
	{
		if (moveDir.x != 0)
		{
			_spriteRenderer.flipX = moveDir.x < 0;
		}
	}

	/// <summary>
	/// 이동 명령 (외부에서 월드 좌표 전달)
	/// </summary>
	public void MoveTo(Vector3 worldPosition)
	{
		_destination = worldPosition;
		_isMoving = true;
	}

	/// <summary>
	/// 공격 명령
	/// </summary>
	public void Attack()
	{
		_animator.SetTrigger(AttackHash);
	}
}
