using System;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;
using UniRx;

public class MainHUD : MonoBehaviour
{
	[Header("[Player]")]
	public Button spawnHeroButton;
	public TMP_Text moneyText;

	private void Awake()
	{
		Init();
	}

	private void Init()
	{
		Player currentPlayer = PlayerManager.Instance.CurrentPlayer;
		if (!currentPlayer)
		{
			return;
		}
		
		if (moneyText)
		{
			currentPlayer.Money
			             .Subscribe(money => moneyText.SetText($"{money}"))
			             .AddTo(this);
		}

		if (spawnHeroButton)
		{
			currentPlayer.Money
			             .Subscribe(money => spawnHeroButton.interactable = money >= 20)
			             .AddTo(this);
			
			spawnHeroButton.onClick.AddListener(OnSpawnHeroButtonClicked);
		}
	}

	private void OnSpawnHeroButtonClicked()
	{
		Player currentPlayer = PlayerManager.Instance.CurrentPlayer;
		if (currentPlayer && currentPlayer.HasEnoughMoney)
		{
			PlayerManager.Instance.SpawnHero();
		}
	}
}
