using UnityEngine;
using FishNet.Object;
using UnityEngine.InputSystem;

public class PlayerCubeCreator : NetworkBehaviour
{
	public NetworkObject cubePrefab;
	public Vector3 offset = Vector3.forward * 30f;

	public override void OnStartClient()
	{
		GetComponent<PlayerInput>().enabled = IsOwner;
	}

	public void OnFire()
	{
		SpawnCube();
	}

	[ServerRpc]
	private void SpawnCube()
	{
		NetworkObject obj = Instantiate(cubePrefab, transform.position + offset, Quaternion.identity);
		if (obj && obj.TryGetComponent(out SyncMaterialColor syncMaterial))
		{
			syncMaterial.color.Value = Random.ColorHSV();
		}
		
		Spawn(obj);
	}
}
