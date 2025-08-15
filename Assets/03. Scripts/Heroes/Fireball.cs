using System;
using UnityEngine;

public class Fireball : MonoBehaviour
{
	[Header("[Properties]")]
	public float speed = 4f;
	public float damage = 30f;
	
	[Header("[Animator]")]
	[SerializeField] private Animator _animator;
	[SerializeField] private float _destroyDelay = 0.06f;

	private MonsterHealth _target;

	private bool _hasExploded;
	private static readonly int ExplodeHash = Animator.StringToHash("Explode");
	
	private bool IsInvalidTarget => _target == null || _target.IsDead;

	public void Initialize(MonsterHealth target)
	{
		_target = target;
	}

	private void Update()
	{
		if (!_hasExploded)
		{
			UpdatePosition();
			UpdateRotation();
		}

		if (IsInvalidTarget)
		{
			Explode();
		}
	}

	private void UpdatePosition()
	{
		if (!_target)
		{
			return;
		}

		Vector3 dir = (_target.transform.position - transform.position).normalized;
		transform.position += dir * (speed * Time.deltaTime);
	}

	private void UpdateRotation()
	{
		if (!_target)
		{
			return;
		}
		
		Vector3 direction = (_target.transform.position - transform.position).normalized;
		float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
		transform.rotation = Quaternion.Euler(new Vector3(0, 0, angle));
	}

	private void OnTriggerEnter2D(Collider2D other)
	{
		MonsterHealth monster = other.GetComponent<MonsterHealth>();
		if (!monster)
		{
			return;
		}
		
		monster.TakeDamage(damage);
		Explode();
	}

	private void Explode()
	{
		_hasExploded = true;
		
		_animator.SetTrigger(ExplodeHash);
		
		Destroy(gameObject, _destroyDelay);
	}
}
