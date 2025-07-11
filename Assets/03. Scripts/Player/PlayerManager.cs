using System;
using System.Collections.Generic;
using UniRx;
using UnityEngine;

public class PlayerManager : Singleton<PlayerManager>
{
	public Player CurrentPlayer { get; private set; }

	protected override void Awake()
	{
		base.Awake();

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
		CurrentPlayer.SpendMoney(20);
	}
}
