using UnityEngine;
using UnityEngine.Events;

public class AnimationEventHandler : MonoBehaviour
{
    public UnityAction onAttackAnimationEnd;
    
    public void OnAttackAnimationEnd()
    {
        onAttackAnimationEnd?.Invoke();
    }
}
