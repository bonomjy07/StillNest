using UnityEngine;
using UnityEngine.Pool;

/// <summary>
/// UI 오브젝트 전용 풀
/// </summary>
public class UIPool<T> where T : Component
{
    private IObjectPool<T> _pool;
    private T _prefab;
    private Transform _parent;

    public void Initialize(T prefab, Transform parent, int defaultCapacity = 20, int maxSize = 100)
    {
        _prefab = prefab;
        _parent = parent;

        _pool = new ObjectPool<T>(
            CreateInstance,
            OnGet,
            OnRelease,
            OnDestroy,
            collectionCheck: false,
            defaultCapacity,
            maxSize);
    }

    private T CreateInstance()
    {
        return Object.Instantiate(_prefab, _parent);
    }

    private void OnGet(T instance)
    {
        instance.gameObject.SetActive(true);
    }

    private void OnRelease(T instance)
    {
        instance.gameObject.SetActive(false);
    }

    private void OnDestroy(T instance)
    {
        Object.Destroy(instance.gameObject);
    }

    public T Get() => _pool.Get();
    public void Release(T instance) => _pool.Release(instance);
}