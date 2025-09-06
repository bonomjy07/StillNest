using System;
using System.Collections;
using System.Collections.Generic;
using FishNet.Object;
using FishNet.Object.Synchronizing;
using UnityEngine;

public class Fireball : NetworkBehaviour
{
	[Header("[Properties]")]
	public float speed = 4f;
	public float damage = 30f;
	
	[Header("[Animator]")]
	[SerializeField] private Animator _animator;
	[SerializeField] private float _destroyDelay = 0.06f;

	private Monster _target;

	private readonly SyncVar<bool> _serverHasExploded = new();
	private static readonly int ExplodeHash = Animator.StringToHash("Explode");
	
	private bool HasExploded
	{
		get => _serverHasExploded.Value;
		set => _serverHasExploded.Value = value;
	}

	private bool IsInvalidTarget => _target == null || _target.IsDead;

	public void Initialize(Monster target, int dmg)
	{
		_target = target;
		damage = dmg;
	}

	private void Update()
	{
		if (IsServerInitialized)
		{
			if (!HasExploded)
			{
				UpdatePosition();
				UpdateRotation();
			}

			// Self destruct if no target or target is invalid
			if (IsInvalidTarget)
			{
				Explode();
			}
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
		if (HasExploded)
		{
			// TODO disable further collisions instead of 'hasExploded' check
			return;
		}
        
		MonsterHealth monsterHealth = other.GetComponent<MonsterHealth>();
		if (!monsterHealth)
		{
			return;
		}
		
		monsterHealth.TakeDamage(damage);
		Explode();
	}

	private void Explode()
	{
		HasExploded = true;
		
		PlayExplodeAnimation();

		StartCoroutine(ExplodeDelay());
	}
	
	[ObserversRpc]
	private void PlayExplodeAnimation()
	{
		if (_animator)
		{
			_animator.SetTrigger(ExplodeHash);
		}
	}

	private IEnumerator ExplodeDelay()
	{
		yield return new WaitForSeconds(_destroyDelay);
		
		Despawn();
	}
}
