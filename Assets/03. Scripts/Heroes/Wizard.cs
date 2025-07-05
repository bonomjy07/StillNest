using System.Collections;
using UnityEngine;

public class Wizard : HeroUnit
{
    [Header("[Fireball]")]
    [SerializeField] private Fireball _fireballPrefab;
    [SerializeField] private Transform _fireballPointRight;
    [SerializeField] private Transform _fireballPointLeft;

    protected override void Attack()
    {
        // base.Attack();
        if (_currentTarget && _currentTarget.TryGetComponent(out MonsterHealth monsterHealth))
        {
            Transform fireballPoint = _spriteRenderer.flipX ? _fireballPointLeft : _fireballPointRight;
            Fireball fireball = Instantiate(_fireballPrefab, fireballPoint.position, Quaternion.identity);
            fireball.Initialize(monsterHealth);
        }
    }
}
