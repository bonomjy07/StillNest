using System;
using UniRx;
using UnityEngine;

public class Warrior : HeroUnit
{
    private static readonly int Attack1ClipId = Animator.StringToHash("Attack1");
    private static readonly int Attack2ClipId = Animator.StringToHash("Attack2");

    protected override void StartAttack(Transform target)
    {
        _currentAttackClipId = UnityEngine.Random.Range(0, 2) == 0 ? Attack1ClipId : Attack2ClipId;
        
        base.StartAttack(target);
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

        if(currentPlayer != null)
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
        _attackCooldown = Math.Max((float)0.2, (float)(0.6 - (0.04 * level)));
    }


}
