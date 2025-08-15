using UnityEngine;
using FishNet.Object;

public class NetworkSingleton<T> : NetworkBehaviour where T : NetworkBehaviour
{
	private static T _instance;
	private static bool _isQuitting;

	public static T Instance
	{
		get
		{
			if (_isQuitting)
			{
				return null;
			}

			if (_instance)
			{
				return _instance;
			}

			_instance = FindFirstObjectByType<T>();
			if (_instance)
			{
				return _instance;
			}

			GameObject prefab = Resources.Load<GameObject>($"Managers/{typeof(T).Name}");
			if (prefab)
			{
				GameObject go = Instantiate(prefab);
				_instance = go.GetComponent<T>();
			}
			else
			{
				GameObject go = new GameObject(typeof(T).Name);
				_instance = go.AddComponent<T>();
			}

			return _instance;
		}
	}

	protected virtual void Awake()
	{
		if (_instance == null)
		{
			_instance = this as T;
		}
		else if (_instance != this)
		{
			Destroy(gameObject);
		}
	}

	protected virtual void OnApplicationQuit()
	{
		_isQuitting = true;
	}
}
