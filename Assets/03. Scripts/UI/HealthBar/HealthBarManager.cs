using UnityEngine;
using UnityEngine.Pool;

public class HealthBarManager : Singleton<HealthBarManager>
{
    [SerializeField] private GameObject _healthBarPrefab;
    [SerializeField] private Canvas _parentCanvas;
        
    private IObjectPool<HealthBar> _healthBarPool;

    protected override void Awake()
    {
        base.Awake();
        _healthBarPool = new ObjectPool<HealthBar>(CreateHealthBar, OnGetHealthBar, OnReleaseHealthBar, OnDestroyHealthBar, false, 20, 100);
    }

    private HealthBar CreateHealthBar()
    {
        GameObject instance = Instantiate(_healthBarPrefab, _parentCanvas.transform);
        if (instance && instance.TryGetComponent(out HealthBar healthBar))
        {
            healthBar.SetReleaseCallback(ReleaseHealthBar);
            return healthBar;
        }

        return null;
    }

    private void OnGetHealthBar(HealthBar bar)
    {
        bar.gameObject.SetActive(true);
    }

    private void OnReleaseHealthBar(HealthBar bar)
    {
        bar.gameObject.SetActive(false);
    }

    private void OnDestroyHealthBar(HealthBar bar)
    {
        Destroy(bar.gameObject);
    }

    private void ReleaseHealthBar(HealthBar bar)
    {
        _healthBarPool.Release(bar);
    }

    public HealthBar GetHealthBar()
    {
        return _healthBarPool.Get();
    }
}
