using System;
using FishNet.Managing;
using FishNet.Managing.Client;
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
		GameEventHub.Instance
		            .OnLocalClient
		            .Subscribe(client => Init())
		            .AddTo(this);
	}

	private void Init()
	{
		CoopPlayer localClient = CoopPlayer.Local;
		if (!localClient)
		{
			return;
		}
		
		if (moneyText)
		{
			localClient.OnMoneyChanged
			           .Subscribe(money => moneyText.SetText($"{money}"))
			           .AddTo(this);
		}

		if (spawnHeroButton)
		{
			localClient.OnMoneyChanged
			           .Subscribe(money => spawnHeroButton.interactable = money >= 20)
			           .AddTo(this);

			spawnHeroButton.onClick.AddListener(OnSpawnHeroButtonClicked);
		}
	}

	private void OnSpawnHeroButtonClicked()
	{
		GameEventHub.Instance.RaiseHeroSpawnClick();
	}
}
