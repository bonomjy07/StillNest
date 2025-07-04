using System;
using System.Collections.Generic;
using UniRx;
using UnityEngine;
using UnityEngine.Events;

public class Player : MonoBehaviour
{
	public ReactiveProperty<int> Money { get; } = new();

	public ReactiveCollection<HeroUnit> Heroes { get; } = new();

	public bool HasEnoughMoney => Money.Value >= 20;

	private void Awake()
	{
		Money.Value = 100; // 초기 돈 설정
	}

	public void SpendMoney(int amount)
	{
		Money.Value = Math.Max(0, Money.Value - amount);
	}
	
	public void AddHero(HeroUnit hero)
	{
		if (hero == null)
		{
			return;
		}
		
		Heroes.Add(hero);
	}
}
