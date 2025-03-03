using System;
using UnityEngine;
using UnityEngine.UI;

public class BetManager : MonoBehaviour
{
    public PlayerScript playerScript;
    public Text betsText, cashText;
    public int Pot { get; private set; } = 0;

    public BetManager()
    {
        
        Pot = 0;
    }

    public void PlaceBet(int amount)
    {
        playerScript.AdjustMoney(-amount);
        Pot += amount;
        UpdateUI();
    }

    public void DoublePot()
    {
        Pot *= 2;
        betsText.text = "BET: $" + Pot;
    }

    public void ResetBet()
    {
        Pot = 0;
        betsText.text = "BET: $0";
        UpdateUI();
    }

    public void UpdateUI()
    {
        betsText.text = "BET: $" + Pot;
        cashText.text = "BANK: $" + playerScript.GetMoney().ToString();
    }
}

