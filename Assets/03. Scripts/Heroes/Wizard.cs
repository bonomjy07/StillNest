using System;
using System.Collections;
using UniRx;
using FishNet.Object;
using UnityEngine;

public class Wizard : HeroUnit
{
    [Header("[Fireball]")]
    [SerializeField] private Fireball _fireballPrefab;
    [SerializeField] private Transform _fireballPointRight;
    [SerializeField] private Transform _fireballPointLeft;

    protected override void ApplyDamage()
    {
        if (_currentTarget && _currentTarget.TryGetComponent(out Monster monster))
        {
            SpawnFireball(monster);
        }
    }
    
    [Server]
    private void SpawnFireball(Monster target)
    {
        Transform fireballPoint = _spriteRenderer.flipX ? _fireballPointLeft : _fireballPointRight;
        Fireball fireball = Instantiate(_fireballPrefab, fireballPoint.position , Quaternion.identity);
        if (!fireball)
        {
            return;
        }
        
        fireball.Initialize(target, _damageAmount);
        Spawn(fireball.gameObject);
    }
}
