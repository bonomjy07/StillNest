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

    [Header("[Upgrade]")]
    public Button upgradeDamageButton;
    public TMP_Text upgradeDamageText;
    public Button upgradeAttackSpeedButton;
    public TMP_Text upgradeAttackSpeedText;

    public GameBalanceData balanceData;

    private void Awake()
    {
        balanceData = Resources.Load<GameBalanceData>("ScriptableObjects/GameBalanceData");
        
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


		if (hostMarker)
		{
			hostMarker.SetActive(localClient.IsHostInitialized);
		}
		
        if (spawnHeroButton)
        {
            localClient.OnMoneyChanged
                       .Subscribe(money => spawnHeroButton.interactable = money >= balanceData.heroCost)
                       .AddTo(this);

            spawnHeroButton.onClick.AddListener(OnSpawnHeroButtonClicked);
        }

		Player currentPlayer = PlayerManager.Instance.CurrentPlayer;
        if (upgradeDamageButton)
        {
            currentPlayer.Money
                .Subscribe(money => upgradeDamageButton.interactable = money >= balanceData.upgradeDamageCost)
                .AddTo(this);

            upgradeDamageButton.onClick.AddListener(OnUpgradeDamageButtonClicked);
        }

        if (upgradeAttackSpeedButton)
        {
            currentPlayer.Money
                .Subscribe(money =>
                {
                    upgradeAttackSpeedButton.interactable = money >= balanceData.upgradeAttackSpeedCost;
                }).AddTo(this);

            upgradeAttackSpeedButton.onClick.AddListener (OnUpgradeAttackSpeedButtonClicked);
        }

        if (upgradeDamageText)
        {
            currentPlayer.DamageUp
                            .Subscribe(level =>
                            {
                                if(level < balanceData.maximumUpgradeLevel)
                                    upgradeDamageText.SetText($"공격력 Up : {level}");
                                else
                                    upgradeDamageText.SetText($"공격력 Up : Max");
                            }).AddTo(this);
        }

        if (upgradeAttackSpeedText)
        {
            currentPlayer.AttackSpeedUp
                            .Subscribe(level =>
                            {
                                if (level < balanceData.maximumUpgradeLevel)
                                    upgradeAttackSpeedText.SetText($"공격속도 Up : {level}");
                                else
                                    upgradeAttackSpeedText.SetText($"공격속도 Up : Max");
                            })
                            .AddTo(this);
        }
	}

    private void OnSpawnHeroButtonClicked()
    {
        /*
        Player currentPlayer = PlayerManager.Instance.CurrentPlayer;
        if (currentPlayer && currentPlayer.HasEnoughMoney)
        {
            PlayerManager.Instance.SpawnHero();
        }
        */
        
        GameEventHub.Instance.RaiseHeroSpawnClick();
    }

    private void OnUpgradeDamageButtonClicked()
    {
        GameEventHub.Instance.RaiseDamageUpClick();
    }
    
    private void OnUpgradeAttackSpeedButtonClicked()
    {
        GameEventHub.Instance.RaiseAttackSpeedUpClick();
    }
}
