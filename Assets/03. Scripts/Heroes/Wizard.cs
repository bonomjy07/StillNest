using System;
using UnityEngine;
using UnityEngine.Serialization;

public class Wizard : MonoBehaviour
{
	[Header("[Movement]")]
	[SerializeField] private float _moveSpeed = 2f;

	[Header("[Attack]")]
	[SerializeField] private FireBall _fireballPrefab;
	[SerializeField] private Transform _fireballPoint;
	[SerializeField] private float _attackCooldown = 0.3f;
	[SerializeField] private float _attackRange = 10f;
	[SerializeField] private LayerMask _monsterLayer;

	// Components
	private Animator _animator;
	private SpriteRenderer _spriteRenderer; 
	
	private Vector3 _destination;
	private bool _isMoving;
	private const float STOP_THRESHOLD = 0.05f;

	private float _lastAttackTime;

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

		if (Time.time >= _lastAttackTime + _attackCooldown)
		{
			Transform target = FindTargetInRange();
			if (target) 
			{
				Attack(target);
			}
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

	public void MoveTo(Vector3 worldPosition)
	{
		_destination = worldPosition;
		_isMoving = true;
	}
	
	private Transform FindTargetInRange()
	{
		// 비싼연산...
		Collider2D[] hits = Physics2D.OverlapCircleAll(transform.position, _attackRange, _monsterLayer);

		if (hits.Length == 0)
		{
			return null;
		}

		// 가장 가까운 적 찾기 (선택사항)
		float minDist = float.MaxValue;
		Transform closest = null;

		foreach (var hit in hits)
		{
			float dist = Vector2.Distance(transform.position, hit.transform.position);
			if (dist < minDist)
			{
				minDist = dist;
				closest = hit.transform;
			}
		}

		return closest;
	}

	private void Attack(Transform target)
	{
		_lastAttackTime = Time.time;

		SpawnFireball(target);
		
		UpdateFacing(target.position - transform.position);
		
		_animator.SetTrigger(AttackHash);
	}

	private FireBall SpawnFireball(Transform target)
	{
		FireBall fireball = Instantiate(_fireballPrefab, _fireballPoint.position, Quaternion.identity, transform);
		fireball.Initialize(target.GetComponent<MonsterController>());
		return fireball;
	}
	
	private void OnDrawGizmosSelected()
	{
		Gizmos.color = Color.red;
		Gizmos.DrawWireSphere(transform.position, _attackRange);
	}
}
