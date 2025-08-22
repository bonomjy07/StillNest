using System;
using System.Collections.Generic;
using UniRx;
using UnityEngine;

public class PlayerManager : Singleton<PlayerManager>
{
	public Player CurrentPlayer { get; private set; }
    public GameBalanceData balanceData;

    protected override void Awake()
	{
		base.Awake();

        balanceData = Resources.Load<GameBalanceData>("ScriptableObjects/GameBalanceData");
        CurrentPlayer = GetComponent<Player>();
		if (!CurrentPlayer)
		{
			CurrentPlayer = gameObject.AddComponent<Player>();
		}
	}

	private void Start()
	{
		SpawnManager.Instance.onMonsterDeath += OnMonsterDeath;
	}

	private void OnMonsterDeath(int monsterMoney)
	{
		if (CurrentPlayer)
		{
			CurrentPlayer.AddMoney(monsterMoney);
		}
	}

	public void SpawnHero()
	{
		if (!CurrentPlayer)
		{
			return;
		}
		
		HeroUnit hero = PlacementManager.Instance.SpawnHero();
		if (!hero)
		{
			return;
		}
		
		CurrentPlayer.AddHero(hero);
		CurrentPlayer.SpendMoney(balanceData.heroCost);
	}

	//public void UpgradeDamage()
	//{
	//	if (!CurrentPlayer) return;

	//	//CurrentPlayer.Upgrade
 //   }
 //   public void UpgradeAttackSpeed()
 //   {
 //       if (!CurrentPlayer) return;
 //   }
}
