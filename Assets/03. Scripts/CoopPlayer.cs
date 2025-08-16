using System;
using System.Collections.Generic;
using FishNet.Connection;
using FishNet.Object;
using FishNet.Object.Synchronizing;
using UniRx;
using UnityEngine;
using UnityEngine.Serialization;

public class CoopPlayer : NetworkBehaviour
{
    [Header("[Spawn Hero]")]
    [SerializeField] private HeroPlacementController _placementController;
    [SerializeField] private NetworkObject _heroesRoot;
    [SerializeField] private List<GameObject> _heroPrefabList;

    private readonly SyncList<HeroUnit> _heroList = new();

    private void Awake()
    {
        _placementController.OnMoveTo = HandleOnMoveTo;
    }

    private void HandleOnMoveTo(HeroUnit hero, Vector3Int from, Vector3Int to, Vector3 worldPos)
    {
        hero.MoveTo(worldPos);
        
        //MoveToServerRpc(hero, worldPos);
    }

    public override void OnStartClient()
    {
        base.OnStartClient();
        
        name += $" ID:{Owner.ClientId}";

        int playerIndex = Owner.IsHost ? 0 : 1;
        _placementController.SetPlayerIndex(playerIndex);
        
        if (!IsOwner)
        {
            _placementController.enabled = false;
            return;
        }

        // Subscribe to the hero spawn button click event
        GameEventHub.Instance
                    .OnHeroSpawnButtonClick
                    .Subscribe(unit => SpawnHeroServerRpc(Owner))
                    .AddTo(this);
    }

    [ServerRpc]
    private void SpawnHeroServerRpc(NetworkConnection conn)
    {
        Vector3 heroSpawnPosition = _placementController.GetFirstEmptyTilePosition();
        GameObject heroPrefab = _heroPrefabList[1]; // 일단 ㅜ어리어만
        GameObject heroInstance = Instantiate(heroPrefab, heroSpawnPosition, Quaternion.identity);
        if (!heroInstance)
        {
            Debug.LogError($"Failed to Spawn hero for player {conn.ClientId}");
            return;
        }

        HeroUnit heroUnit = heroInstance.GetComponent<HeroUnit>();
        if (!heroUnit)
        {
            return;
        }
        
        Spawn(heroUnit.gameObject, conn);
        heroUnit.NetworkObject.SetParent(_heroesRoot);
        
        // Cache the heroes
        _heroList.Add(heroUnit);
        // cache heroes location
        AddSpawnedUnit(heroUnit);
    }

    [ObserversRpc]
    private void AddSpawnedUnit(HeroUnit heroUnit)
    {
        _placementController.AddSpawnedUnit(heroUnit.gameObject);
    }
}
