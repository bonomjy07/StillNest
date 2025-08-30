using System;
using System.Collections;
using UniRx;
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
            fireball.Initialize(monsterHealth, _damageAmount);
        }
    }

    private void Start()
    {
        Player currentPlayer = PlayerManager.Instance.CurrentPlayer;
        if (currentPlayer != null)
        {
            currentPlayer.DamageUp
                         .Subscribe(UpgradeDamage)
                         .AddTo(this);
        }

        if (currentPlayer != null)
        {
            currentPlayer.AttackSpeedUp
                         .Subscribe(UpgradeAttackSpeed)
                         .AddTo(this);
        }
    }

    private void UpgradeDamage(int level)
    {
        //_damageAmount += 10 * level;
        //_damageAmount += 20;
        _damageAmount = 50 + (20 * level);
    }

    private void UpgradeAttackSpeed(int level)
    {
        //_attackCooldown = Math.Max((float)0.25, _attackCooldown - (float)(0.1 * level));
        //_attackCooldown = Math.Max((float)0.25, _attackCooldown - (float)(0.1));
        _attackCooldown = Math.Max((float)0.3, (float)(0.8 - (0.05 * level)));
    }
}
