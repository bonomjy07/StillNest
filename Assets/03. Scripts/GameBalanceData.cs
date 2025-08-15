using UnityEngine;

[CreateAssetMenu(fileName = "GameBalanceData", menuName = "Scriptable Objects/GameBalanceData")]
public class GameBalanceData : ScriptableObject
{
    [Header("Player")]
    public int startMoney = 200;

    [Header("Hero")]
    public int heroCost = 100;

    [Header("Monster")]
    public int monsterKillGold = 5;
    public int bossKillGold = 500;
}
