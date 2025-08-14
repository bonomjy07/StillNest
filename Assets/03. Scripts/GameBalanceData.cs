using UnityEngine;

[CreateAssetMenu(fileName = "GameBalanceData", menuName = "Scriptable Objects/GameBalanceData")]
public class GameBalanceData : ScriptableObject
{
    [Header("Hero")]
    public int heroCost = 100;

    [Header("Monster")]
    public int monsterKillGold = 10;
    public int bossKillGold = 500;
}
