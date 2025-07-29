using System;
using System.Collections.Generic;
using FishNet.Connection;
using FishNet.Object;
using UniRx;
using UnityEngine;

public class CoopPlayer : NetworkBehaviour
{
    public static CoopPlayer Instance { get; private set; }
    
    public int Money { get; }
    public int Name { get; }

    private List<HeroUnit> _heroUnits = new();
    
    [SerializeField] private HeroPlacementController _placementController;

    public override void OnStartClient()
    {
        base.OnStartClient();
        Debug.Log($"[meng] Coop Player is started. id is {Owner.ClientId}. IsOwner: {IsOwner}");

        if (IsOwner)
        {
            Instance = this;
            
            // Subscribe to the hero spawn button click event
            GameEventHub.Instance
                        .OnHeroSpawnButtonClick
                        .Subscribe(unit => SpawnHero(Owner))
                        .AddTo(this);

            // 왼쪽? 오른쪽?
            int playerIndex = IsServerInitialized ? 0 : 1;
            _placementController.InitClientInfo(playerIndex);
        }
    }

    [ServerRpc]
    private void SpawnHero(NetworkConnection sender)
    {
        if (!IsServerInitialized)
        {
            return;
        }
        
        int playerIndex = sender != null && sender.IsHost ? 0 : 1;
        _placementController.InitClientInfo(playerIndex);

        HeroUnit heroUnit = _placementController.SpawnHero();
        if (heroUnit)
        {
            Spawn(heroUnit.gameObject, Owner);
        }
    }
}
