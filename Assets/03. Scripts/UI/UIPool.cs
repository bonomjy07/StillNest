using System.Collections.Generic;
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

    private List<T> _activeInstances = new();

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

        if (!_activeInstances.Contains(instance))
            _activeInstances.Add(instance);
    }

    private void OnRelease(T instance)
    {
        instance.gameObject.SetActive(false);
        _activeInstances.Remove(instance);
    }

    private void OnDestroy(T instance)
    {
        Object.Destroy(instance.gameObject);
    }

    public T Get() => _pool.Get();
    public void Release(T instance) => _pool.Release(instance);

    public void ReleaseAll()
    {
        List<T> tmp = new List<T>(_activeInstances);
        foreach(var instance in tmp)
            _pool.Release(instance);
        
        _activeInstances.Clear();
    }
}