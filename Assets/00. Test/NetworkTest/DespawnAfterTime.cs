using System.Collections;
using FishNet.Object;
using UnityEngine;

public class DespawnAfterTime : NetworkBehaviour
{
	public float secondsBeforeDespawn = 2f;

	public override void OnStartServer()
	{
		base.OnStartServer();

		StartCoroutine(DespawnAfterSeconds());
	}

	private IEnumerator DespawnAfterSeconds()
	{
		yield return new WaitForSeconds(secondsBeforeDespawn);

		Despawn();
	}
}
