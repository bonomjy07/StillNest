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

    public GameBalanceData balanceData;

    private void Awake()
	{
		balanceData = Resources.Load<GameBalanceData>("ScriptableObjects/GameBalanceData");
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
			             .Subscribe(money => spawnHeroButton.interactable = money >= balanceData.heroCost)
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
