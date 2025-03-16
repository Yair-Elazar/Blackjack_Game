using System;
using System.Collections;
using System.Timers;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.UI;

//TODO spilt, double, cancel bet buttons, timer, exit button, database, AI,

public class GameManager : MonoBehaviour
{
    //Game buttons
    public Button dealBtn, hitBtn, standBtn, doubleBtn; 
    public Button bet10Btn, bet50Btn, bet100Btn, bet200Btn;
    public Text scoreText, dealerScoreText,mainText;
    public GameObject hideCard;

    public PlayerScript playerScript;
    public PlayerScript dealerScript;
    public BetManager betManager;
    private bool firstAction = true;
    private bool roundSaved = false;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        loadUSerData();
        SetupGame();
    }

    private void loadUSerData()
    {
        FirestoreManager.Instance.LoadUserData();
        playerScript.SetMoney(UserData.amount);
        betManager.UpdateUI();
    }

    private void SetupGame()
    {
        // Add on click listeners to the buttons
        dealBtn.onClick.AddListener(DealClicked);
        hitBtn.onClick.AddListener(HitClicked);
        standBtn.onClick.AddListener(StandClicked);
        doubleBtn.onClick.AddListener(Doublelicked);

        bet10Btn.onClick.AddListener(() => betManager.PlaceBet(10));
        bet50Btn.onClick.AddListener(() => betManager.PlaceBet(50));
        bet100Btn.onClick.AddListener(() => betManager.PlaceBet(100));
        bet200Btn.onClick.AddListener(() => betManager.PlaceBet(200));

        // Add a clear button - if the user wont to reset the bet
        hitBtn.gameObject.SetActive(false);
        standBtn.gameObject.SetActive(false);
    }

    private void DealClicked()
    {
        // TODO check if the player put mouny before start the game
        
        playerScript.ResetHand();
        dealerScript.ResetHand();
        mainText.gameObject.SetActive(false);

        // Hide the dealer score at the start of the deal
        dealerScoreText.gameObject.SetActive(false);

        GameObject.Find("Deck").GetComponent<DeckScript>().Shuffle();

        playerScript.StarHand(true);
        dealerScript.StarHand(false);

        UserData.userHand = playerScript.playerHand;
        UserData.dealerCard = dealerScript.dealerHand;
        UserData.games += 1;

        // Update the score display
        UpdateScoreUI();
        betManager.DoublePot();

        // Enable to hide one of the dealer's cards
        hideCard.GetComponent<Renderer>().enabled = true;
        ToggleGameButtons(true);

        RoundOver("player");
    }

    private void HitClicked()
    {
        if (firstAction)
        {
            UserData.userAction = "Hit";
            firstAction = false;
        }

        playerScript.GetCard(true);
        UpdateScoreUI();
        RoundOver("player");
    }
    
    private void StandClicked()
    {
        if (firstAction)
        {
            UserData.userAction = "Stand";
            firstAction = false;
        }
        DealerTurn();
    }

    private void Doublelicked()
    {
        playerScript.AdjustMoney(-(betManager.Pot / 2));
        betManager.DoublePot();
        if (firstAction)
        {
            UserData.userAction = "Stand";
            firstAction = false;
        }
        DealerTurn();
    }

    private void SplitClicked()
    {

    }

    private void DealerTurn()
    {
        hideCard.GetComponent<Renderer>().enabled = false;

        if(dealerScript.handValue > playerScript.handValue)
        {
            RoundOver("dealerHit");
        }
        else
        {
            HitDealer();
        }
    }

    private void HitDealer()
    {
        // TODO show the cards one by one 
        while (dealerScript.handValue < 17)
        {
            dealerScript.GetCard(false);
            UpdateScoreUI();
            RoundOver("dealerHit");
            StartCoroutine(wait());

        }
        RoundOver("dealerHit");
    }

    private IEnumerator wait()
    {
        yield return new WaitForSeconds(2);
    }

    private void RoundOver(String name)
    {
        if (roundSaved)
        {
            return; // Prevent multiple saves
        }

        bool roundOver = true;

        // if the player bust/win after deal/hit button
        if (name == "player")
        {
            if (playerScript.handValue > 21)
            {
                mainText.text = "Bust: Dealer Win";
                UserData.outcome = "Loss";
                UserData.amount = playerScript.GetMoney();
                UserData.loses += 1;
            }
            else if (playerScript.handValue == 21)
            {
                mainText.text = "Blackjack!! You Win";
                playerScript.AdjustMoney(betManager.Pot);

                UserData.userAction ??= "User Blackjack";
                UserData.outcome = "Win";
                UserData.wins += 1;
                UserData.amount = playerScript.GetMoney();
            }
            else
            {
                roundOver = false;
            }
        } 

        else if (name == "dealerHit")
        {
            if (dealerScript.handValue == 21)
            {
                mainText.text = "Dealer Win!! Blackjack";
                UserData.outcome = "Loss";
                if (UserData.userAction == null)
                {
                    UserData.userAction = "Dealer Blackjack";
                }
                UserData.loses += 1;
                UserData.amount = playerScript.GetMoney();
            }

            else if (dealerScript.handValue > 21)
            {
                mainText.text = "Dealer Bust: You Win - Dealer Hand: " + dealerScript.handValue.ToString();
                UserData.outcome = "Win";
                playerScript.AdjustMoney(betManager.Pot);
                UserData.wins += 1;
                UserData.amount = playerScript.GetMoney();
            }

            else if (dealerScript.handValue > playerScript.handValue)
            {
                mainText.text = "Dealer Wins - Dealer Hand: " + dealerScript.handValue.ToString();
                UserData.outcome = "Loss";
                UserData.loses += 1;
                UserData.amount = playerScript.GetMoney();
            }

            else if (playerScript.handValue > dealerScript.handValue && dealerScript.handValue >= 17)
            {
                mainText.text = "You Win!! - Dealer Hand: " + dealerScript.handValue.ToString();
                UserData.outcome = "Win";
                playerScript.AdjustMoney(betManager.Pot);
                UserData.wins += 1;
                UserData.amount = playerScript.GetMoney();
            }

            else if (playerScript.handValue == dealerScript.handValue && dealerScript.handValue >= 17)
            {
                mainText.text = "Push: Bets returned";
                UserData.outcome = "Draw";
                playerScript.AdjustMoney(betManager.Pot / 2);
                UserData.amount = playerScript.GetMoney();
            }
            else
            {
                roundOver = false;
            }
        }

        if (roundOver)
        {
            roundSaved = true;
            FirestoreManager.Instance.SaveGameData(UserData.userHand, UserData.dealerCard[0], UserData.userAction, UserData.outcome);
            FirestoreManager.Instance.SaveUserData(UserData.amount,UserData.games, UserData.wins, UserData.loses);
            //FirebaseManager.Instance.SaveGameState(UserData.userId, UserData.amount, UserData.games, UserData.wins, UserData.loses);
            ResetRound();
        }
    }

    private void ResetRound()
    {
        firstAction = true;
        roundSaved = false;
        ToggleGameButtons(false);
        mainText.gameObject.SetActive(true);
        dealerScoreText.gameObject.SetActive(true);
        hideCard.GetComponent<Renderer>().enabled = false;
        betManager.ResetBet();
    }


    private void UpdateScoreUI()
    {
        scoreText.text = "HAND: " + playerScript.handValue;
        dealerScoreText.text = "HAND: " + dealerScript.handValue;
    }

    private void ToggleGameButtons(bool enabled)
    {
        dealBtn.gameObject.SetActive(!enabled);
        hitBtn.gameObject.SetActive(enabled);
        standBtn.gameObject.SetActive(enabled);
        doubleBtn.gameObject.SetActive(enabled);

        bet10Btn.enabled = !enabled;
        bet50Btn.enabled = !enabled;
        bet100Btn.enabled = !enabled;
        bet200Btn.enabled = !enabled;
    }

    
}
