using System;
using UnityEngine;

public abstract class Singleton<T> : MonoBehaviour where T : MonoBehaviour
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
			
			DontDestroyOnLoad(_instance.gameObject);

			return _instance;
		}
	}

	protected virtual void Awake()
	{
		if (_instance == null)
		{
			_instance = this as T;
			DontDestroyOnLoad(gameObject);
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