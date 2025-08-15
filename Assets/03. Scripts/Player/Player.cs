using System;
using System.Collections.Generic;
using UniRx;
using UnityEngine;
using UnityEngine.Events;

public class Player : MonoBehaviour
{
	public ReactiveProperty<int> Money { get; } = new();
	public ReactiveCollection<HeroUnit> Heroes { get; } = new();
	public GameBalanceData balanceData;

	public bool HasEnoughMoney => Money.Value >= balanceData.heroCost;

	private void Awake()
	{
        balanceData = Resources.Load<GameBalanceData>("ScriptableObjects/GameBalanceData");
		Money.Value = balanceData.startMoney;
	}

	public void SpendMoney(int amount)
	{
		Money.Value = Math.Max(0, Money.Value - amount);
	}
	
	public void AddMoney(int amount)
	{
		Money.Value += amount;
	}
	
	public void AddHero(HeroUnit hero)
	{
		if (hero == null)
		{
			return;
		}
		
		Heroes.Add(hero);
	}

	public void ResetSetting()
	{
		Heroes.Clear();
		Money.Value = balanceData.startMoney;
	}
}
