using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UniRx;

public class MainHUD : MonoBehaviour
{
	[Header("[Player]")]
	public Button spawnHeroButton;
	public TMP_Text moneyText;
	public GameObject hostMarker;

	private void Awake()
	{
		GameEventHub.Instance
		            .OnLocalClient
		            .Subscribe(BindForLocalClient)
		            .AddTo(this);
	}

	private void BindForLocalClient(CoopPlayer localClient)
	{
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

		if (hostMarker)
		{
			hostMarker.SetActive(localClient.IsHostInitialized);
		}
	}

	private void OnSpawnHeroButtonClicked()
	{
		GameEventHub.Instance.RaiseHeroSpawnClick();
	}
}
