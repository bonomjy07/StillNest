using System;
using System.Collections.Generic;
using System.Linq;

using FishNet.Connection;
using FishNet.Object;
using FishNet.Object.Synchronizing;

using UniRx;
using UnityEngine;
using Random = UnityEngine.Random;

public class CoopPlayer : NetworkBehaviour
{
    [Header("[Spawn Hero]")]
    [SerializeField] private HeroPlacementController _placementController;
    [SerializeField] private NetworkObject _heroesRoot;
    [SerializeField] private List<GameObject> _heroPrefabList;

    private readonly SyncList<HeroUnit> _heroList = new();
    private GameBalanceData _balanceData; 
    
    public int Money
    {
        get => _serverMoney.Value;
        private set => _serverMoney.Value = value;
    }

    public int DamageUp
    {
        get => _serverDamageUp.Value;
        private set => _serverDamageUp.Value = value;
    }
    
    public int AttackSpeedUp
    {
        get => _serverAttackSpeedUp.Value;
        private set => _serverAttackSpeedUp.Value = value;
    }

    public IObservable<int> OnMoneyChanged => _clientMoney.AsObservable();
    
    private readonly SyncVar<int> _serverMoney = new();
    private readonly ReactiveProperty<int> _clientMoney = new();

    private readonly SyncVar<int> _serverDamageUp = new();
    private readonly ReactiveProperty<int> _clientDamageUp = new();
    
    private readonly SyncVar<int> _serverAttackSpeedUp = new();
    private readonly ReactiveProperty<int> _clientAttackSpeedUp = new();
    
    private void Awake()
    {
        RegisterCallbacks();
            
        _balanceData = Resources.Load<GameBalanceData>("ScriptableObjects/GameBalanceData");
        
        // setup initial money
        _serverMoney.SetInitialValues(_balanceData.startMoney);
        _clientMoney.Value = _balanceData.startMoney;
    }

    private void RegisterCallbacks()
    {
        _placementController.OnMoveTo = HandleOnMoveTo;

        _serverMoney.OnChange += (prev, next, asServer) =>
        {
            if (!asServer)
            {
                _clientMoney.Value = next;
            }
        };

        _serverDamageUp.OnChange += (prev, next, asServer) =>
        {
            if (asServer)
            {
                UpdateHeroesDamage(next);
            }
            else
            {
                _clientDamageUp.Value = next;
            }
        };

        _serverAttackSpeedUp.OnChange += (prev, next, asServer) =>
        {
            if (asServer)
            {
                UpdateHeroesAttackSpeed(next);
            }
            else
            {
                _clientAttackSpeedUp.Value = next;
            }
        };

        GameEventHub.Instance
                    .OnDamageUp
                    .Where(__ => IsOwner)
                    .Where(__ => Money >= _balanceData.upgradeDamageCost)
                    .Subscribe(_ =>
                    {
                        DamageUp += 1;
                        Money -= _balanceData.upgradeDamageCost;
                    })
                    .AddTo(this);
        
        GameEventHub.Instance
                    .OnAttackSpeedUp
                    .Where(__ => IsOwner)
                    .Where(__ => Money >= _balanceData.upgradeAttackSpeedCost)
                    .Subscribe(_ =>
                    {
                        AttackSpeedUp += 1;
                        Money -= _balanceData.upgradeAttackSpeedCost;
                    })
                    .AddTo(this);
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
        
        Clear();
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
        GameEventHub.Instance
                    .RaiseLocalClientOn(this);

        // Subscribe to the hero spawn button click event
        GameEventHub.Instance
                    .OnHeroSpawnButtonClick
                    .Subscribe(unit => SpawnHeroServerRpc(Owner))
                    .AddTo(this);
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
        int heroIndex = Random.Range(0, 2);
        GameObject heroPrefab = _heroPrefabList[heroIndex];
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
    
    [Server]
    private void UpdateHeroesDamage(int next)
    {
        foreach (HeroUnit hero in _heroList)
        {
            hero.UpgradeDamage(next);
        }
    }
        
    [Server]
    private void UpdateHeroesAttackSpeed(int next)
    {
        foreach (HeroUnit hero in _heroList)
        {
            hero.UpgradeAttackSpeed(next);
        }
    }

    public void Clear()
    {
        if (!IsServerInitialized)
        {
            return;
        }
        // Clear the hero list
        foreach (HeroUnit heroUnit in _heroList)
        {
            Despawn(heroUnit.gameObject);
        }
        _heroList.Clear();

        Money = _balanceData.startMoney;
        DamageUp = 0;
        AttackSpeedUp = 0;
    }
}
