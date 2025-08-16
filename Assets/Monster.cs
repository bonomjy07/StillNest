using System;
using FishNet.Object;
using UnityEngine;

public class Monster : NetworkBehaviour
{
    [SerializeField] private MonsterHealth _health;
    [SerializeField] private MonsterMoving _movement;
    
    public MonsterHealth Health => _health;
    public MonsterMoving Movement => _movement;
    public MonsterType MobType => Movement.MonsterType;
    
    private void Start()
    {
        _health.OnDeadChange += HealthOnOnDeadChange;
    }

    private void OnDestroy()
    {
        _health.OnDeadChange -= HealthOnOnDeadChange;
    }

    private void HealthOnOnDeadChange(bool prev, bool next, bool asServer)
    {
        // Disable movement when the monster is dead
        if (next)
        {
            _movement.StopMovement();
        }
    }
}
