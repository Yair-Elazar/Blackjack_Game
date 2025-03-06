using System.Collections.Generic;
using UnityEngine;

public class PlayerScript : MonoBehaviour
{
    // -- > This script is for both player(user) and dealer

    //Get other scripts
    public CardScript cardScript;
    public DeckScript deckScript;

    // Total value of player/dealer's hand
    public int handValue = 0;

    // Betting money
    private int money = 0;

    // Array of card object on table
    public GameObject[] hand;

    // Index of next card to be turnet over
    public int cardIndex = 0;

    //Tracking aces for 1 to 11 conversions
    List<CardScript> aceList = new List<CardScript>();


    public int[] playerHand = new int[2];
    public int playerCardIndex = 0;
    public int[] dealerHand = new int[12];
    public int dealerCardIndex = 0;

    public void StarHand(bool isPlayer)
    {
        GetCard(isPlayer);
        GetCard(isPlayer);
    }

    // Add a hand to player/dealer's hand
    public int GetCard(bool isPlayer)
    {
        //Get a card, use deal card to assign sprite and value to card on table
        int cardValue = deckScript.DealCard(hand[cardIndex].GetComponent<CardScript>());
        // Show card on game screen
        hand[cardIndex].GetComponent<Renderer>().enabled = true;

        if(isPlayer)
        {
            if(playerCardIndex <= 1)
            {
                playerHand[playerCardIndex] = cardValue;
                playerCardIndex++;
            }
            
        }
        else
        {
            dealerHand[dealerCardIndex] = cardValue;
            dealerCardIndex++;
        }

        // Add card value to runnig total of the hand
        handValue += cardValue;
        // If value is 1, it is an ace
        if (cardValue == 1)
        {
            aceList.Add(hand[cardIndex].GetComponent<CardScript>());
        }

        // Check if we should use an 11 instead of a 1
        AceCheck();
        cardIndex++;
        return handValue;
    }

    public void AceCheck()
    {
        foreach (CardScript ace in aceList)
        {
            if (handValue + 10 < 22 && ace.GetValueOfCard() == 1)
            {
                // if convertinng adjust card object value and hand
                ace.SetValue(11);
                handValue += 10;

            }
            else if (handValue > 21 && ace.GetValueOfCard() == 11)
            {
                // if converting adjust gameobject value and hand value
                ace.SetValue(1);
                handValue -= 10;

            }
        }
    }

    // Add or substarc from money, for bets
    public void AdjustMoney(int amount)
    {
        money += amount;
    }

    // Output players current money amount
    public int GetMoney()
    {
        return money;
    }

    public void SetMoney(int amount)
    {
        money = amount;
    }

    // Hides all cards, resets the neede varibales
    public void ResetHand()
    {
        for (int i = 0; i < hand.Length; i++)
        {
            hand[i].GetComponent<CardScript>().ReserCard();
            hand[i].GetComponent<Renderer>().enabled = false;
        }
        cardIndex = 0;
        handValue = 0;
        aceList = new List<CardScript>();
        playerCardIndex = 0;
        dealerCardIndex = 0;
        playerHand = new int[2];
        dealerHand = new int[12];
    }
}
