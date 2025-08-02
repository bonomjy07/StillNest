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
        }
        else
        { 
            // Update()문 안돌게 (이런 방법은 딱히)
            // 그냥 애도 네트워크로 뺴는게;
            _placementController.enabled = false;
        }

        name = name + $", Id:{OwnerId}";
    }

    [ServerRpc]
    private void SpawnHero(NetworkConnection sender)
    {
        if (!IsServerInitialized)
        {
            return;
        }
        
        int playerIndex = sender != null && sender.IsHost ? 0 : 1;
        _placementController.SetPlayerIndex(playerIndex);

        HeroUnit heroUnit = _placementController.SpawnHero();
        if (heroUnit)
        {
            Spawn(heroUnit.gameObject, sender);
        }
    }
}
