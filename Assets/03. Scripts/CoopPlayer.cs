using System;
using System.Collections.Generic;

using FishNet.Connection;
using FishNet.Object;
using FishNet.Object.Synchronizing;

using UniRx;
using UnityEngine;

public class CoopPlayer : NetworkBehaviour
{
    public static CoopPlayer Local { get; private set; }
    
    [Header("[Spawn Hero]")]
    [SerializeField] private HeroPlacementController _placementController;
    [SerializeField] private NetworkObject _heroesRoot;
    [SerializeField] private List<GameObject> _heroPrefabList;

    private readonly SyncList<HeroUnit> _heroList = new();
    
    public int Money
    {
        get => _serverMoney.Value;
        private set => _serverMoney.Value = value;
    }

    private readonly SyncVar<int> _serverMoney = new();
    private readonly ReactiveProperty<int> _clientMoney = new();

    public IObservable<int> OnMoneyChanged => _clientMoney.AsObservable();

    private void Awake()
    {
        _placementController.OnMoveTo = HandleOnMoveTo;
        
        _serverMoney.OnChange += (prev, next, asServer) =>
        {
            if (!asServer)
            {
                _clientMoney.Value = next; 
            }
        };

        // setup initial money
        _serverMoney.SetInitialValues(100);
        _clientMoney.Value = _serverMoney.Value;
    }
    
    public override void OnStartServer()
    {
        SpawnManager.Instance.onMonsterDeath += OnMonsterDeath;
    }

    public override void OnStopServer()
    {
        if (!SpawnManager.IsQuitting)
        {
            SpawnManager.Instance.onMonsterDeath -= OnMonsterDeath;
        }
        
        // Clear the hero list
        foreach (HeroUnit heroUnit in _heroList)
        {
            Despawn(heroUnit.gameObject);
        }
        _heroList.Clear();
    }

    public override void OnStartClient()
    {
        name += $" ID:{Owner.ClientId}";

        int playerIndex = Owner.IsHost ? 0 : 1;
        _placementController.SetPlayerIndex(playerIndex);
        
        if (!IsOwner)
        {
            _placementController.enabled = false;
            return;
        }

        // Notify
        Local = this;
        GameEventHub.Instance
                    .RaiseLocalClientOn(this);

        // Subscribe to the hero spawn button click event
        GameEventHub.Instance
                    .OnHeroSpawnButtonClick
                    .Subscribe(unit => SpawnHeroServerRpc(Owner))
                    .AddTo(this);
    }

    public override void OnStopClient()
    {
        if (IsOwner)
        {
            Local = this;
        }
    }

    private void OnMonsterDeath(int money)
    {
        if (IsServerInitialized)
        {
            Money += money;
        }
    }

    private void HandleOnMoveTo(HeroUnit hero, Vector3Int from, Vector3Int to, Vector3 worldPos)
    {
        hero.MoveTo(worldPos);
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

        Money -= 20; // Deduct money for spawning the hero
    }

    [ObserversRpc]
    private void AddSpawnedUnit(HeroUnit heroUnit)
    {
        _placementController.AddSpawnedUnit(heroUnit.gameObject);
    }
}
