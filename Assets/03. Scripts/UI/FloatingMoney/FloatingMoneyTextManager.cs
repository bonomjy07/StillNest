using UnityEngine;

public class FloatingMoneyTextManager : Singleton<FloatingMoneyTextManager>
{
    [SerializeField] private FloatingMoneyText _prefab;
    [SerializeField] private Canvas _canvas;

    private UIPool<FloatingMoneyText> _pool = new();

    protected override void Awake()
    {
        base.Awake();

        _pool.Initialize(_prefab, _canvas.transform);
    }

    public FloatingMoneyText Spawn() => _pool.Get();
    
    public void Despawn(FloatingMoneyText moneyText) => _pool.Release(moneyText);
}
