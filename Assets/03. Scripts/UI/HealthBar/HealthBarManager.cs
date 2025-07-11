using System;
using UnityEngine;
using UnityEngine.Pool;

public class HealthBarManager : Singleton<HealthBarManager>
{
    [SerializeField] private HealthBar _prefab;
    [SerializeField] private Canvas _canvas;

    private UIPool<HealthBar> _pool = new();

    protected override void Awake()
    {
        base.Awake();

        _pool.Initialize(_prefab, _canvas.transform);
    }

    public HealthBar Spawn() => _pool.Get();
    
    public void Despawn(HealthBar bar) => _pool.Release(bar);
}
