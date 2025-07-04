using UnityEngine;

public class Warrior : HeroUnit
{
    private static readonly int Attack1ClipId = Animator.StringToHash("Attack1");
    private static readonly int Attack2ClipId = Animator.StringToHash("Attack2");

    protected override void StartAttack(Transform target)
    {
        _currentAttackClipId = Random.Range(0, 2) == 0 ? Attack1ClipId : Attack2ClipId;
        
        base.StartAttack(target);
    }
}
