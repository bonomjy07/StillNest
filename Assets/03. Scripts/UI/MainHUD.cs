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

    [Header("[Upgrade]")]
    public Button upgradeDamageButton;
    public TMP_Text upgradeDamageText;
    public Button upgradeAttackSpeedButton;
    public TMP_Text upgradeAttackSpeedText;

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
        Player currentPlayer = PlayerManager.Instance.CurrentPlayer;
        if (currentPlayer && currentPlayer.HasEnoughMoney)
        {
            PlayerManager.Instance.SpawnHero();
        }
    }

    private void OnUpgradeDamageButtonClicked()
    {
        Player currentPlayer = PlayerManager.Instance.CurrentPlayer;
        if (currentPlayer && currentPlayer.HasEnoughMoney)
            PlayerManager.Instance.CurrentPlayer.UpgradeDamage();
    }
    private void OnUpgradeAttackSpeedButtonClicked()
    {
        Player currentPlayer = PlayerManager.Instance.CurrentPlayer;
        if (currentPlayer && currentPlayer.HasEnoughMoney)
            PlayerManager.Instance.CurrentPlayer.UpgradeAttackSpeed();
    }


}
