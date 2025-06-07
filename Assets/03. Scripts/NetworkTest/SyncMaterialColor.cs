using System;
using FishNet.Object;
using FishNet.Object.Synchronizing;
using UnityEngine;

public class SyncMaterialColor : NetworkBehaviour
{
	public readonly SyncVar<Color> color = new();

	private void Awake()
	{
		color.OnChange += OnColorChanged;
	}

	private void OnDestroy()
	{
		color.OnChange -= OnColorChanged;
	}

	private void OnColorChanged(Color prev, Color next, bool asServer)
	{
		GetComponent<MeshRenderer>().material.color = color.Value;
	}
}
