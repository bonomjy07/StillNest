using System;
using System.Collections.Generic;
using FishNet.Object;
using UniRx;
using UnityEngine;
using UnityEngine.Events;

public class Player : NetworkBehaviour
{
	public ReactiveProperty<int> Money { get; } = new();
	public ReactiveCollection<HeroUnit> Heroes { get; } = new();
	public ReactiveProperty<int> DamageUp { get; } = new();
	public ReactiveProperty<int> AttackSpeedUp { get; } = new();
	public GameBalanceData balanceData;

	public bool HasEnoughMoney => Money.Value >= balanceData.heroCost;

	private void Awake()
	{
        balanceData = Resources.Load<GameBalanceData>("ScriptableObjects/GameBalanceData");
		Money.Value = balanceData.startMoney;
		DamageUp.Value = 0;
		AttackSpeedUp.Value = 0;
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

	public void UpgradeDamage()
	{
		if (DamageUp.Value < balanceData.maximumUpgradeLevel)
		{
			DamageUp.Value++;
			SpendMoney(balanceData.upgradeDamageCost);
		}
	}
	public void UpgradeAttackSpeed()
	{
		if (AttackSpeedUp.Value < balanceData.maximumUpgradeLevel)
		{
			AttackSpeedUp.Value++;
			SpendMoney(balanceData.upgradeAttackSpeedCost);
		}
	}

	public void ResetSetting()
	{
		Heroes.Clear();
		Money.Value = balanceData.startMoney;
        
        DamageUp.Value = 0;
        AttackSpeedUp.Value = 0;
    }
}
