using System;
using System.Timers;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.UI;

//TODO spilt, double, cancel bet buttons, timer, exit button, database, AI,

public class GameManager : MonoBehaviour
{
    //Game buttons
    public Button dealBtn;
    public Button hitBtn;
    public Button standBtn;
    public Button bet10Btn;
    public Button bet50Btn;
    public Button bet100Btn;
    public Button bet200Btn;

    //private bool timerRunning;
    private float timeLeft = 30f;
    public Text timerText;

    //public Button spilt;
    //public Button Double;

    //private int standClicks = 0;

    // Access to the Player and dealer's script
    public PlayerScript playerScript;
    public PlayerScript dealerScript;

    
    // Public Text to access and update - hub
    public Text scoreText;
    public Text dealerScoreText;
    public Text betsText;
    public Text cashText;
    //public Text standBtnText;
    
    public Text mainText;

    // Card hiding dealer's script
    public GameObject hideCard;

    // How much is bet
    int pot = 0;


    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        PlayerTurn();
    }

    private void PlayerTurn()
    {
        // Add on click listeners to the buttons
        dealBtn.onClick.AddListener(() => DealClicked());
        hitBtn.onClick.AddListener(() => HitClicked());
        standBtn.onClick.AddListener(() => StandClicked());
        bet10Btn.onClick.AddListener(() => bet10Clicked());
        bet50Btn.onClick.AddListener(() => bet50Clicked());
        bet100Btn.onClick.AddListener(() => bet100Clicked());
        bet200Btn.onClick.AddListener(() => bet200Clicked());
        // Add a clear button - if the user wont to reset the bet
        hitBtn.gameObject.SetActive(false);
        standBtn.gameObject.SetActive(false);
    }
    private void DealClicked()
    {
        // Start player timer
        //StarTimer();

        playerScript.ResetHand();
        dealerScript.ResetHand();

     
        mainText.gameObject.SetActive(false);

        // Hide the dealer score at the start of the deal
        dealerScoreText.gameObject.SetActive(false);


        GameObject.Find("Deck").GetComponent<DeckScript>().Shuffle();
        playerScript.StarHand();
        dealerScript.StarHand();


        // Update the score display
        scoreText.text = "HAND: " + playerScript.handValue.ToString();
        dealerScoreText.text = "HAND: " + dealerScript.handValue.ToString();

        // Enable to hide one of the dealer's cards
        hideCard.GetComponent<Renderer>().enabled = true;

        // Adjust buttons visibility
        dealBtn.gameObject.SetActive(false);
        hitBtn.gameObject.SetActive(true);
        standBtn.gameObject.SetActive(true);
        bet10Btn.enabled = false;
        bet50Btn.enabled = false;
        bet100Btn.enabled = false;
        bet200Btn.enabled = false;
        //TODO Add Double and spilt button

        // Set standard pot size
        pot *= 2;
        betsText.text = "BET: $" + pot.ToString();

        RoundOver("player");

    }

    private void HitClicked()
    {
        //StandClicked();
        playerScript.GetCard();
        RoundOver("player");
        scoreText.text = "HAND: " + playerScript.handValue.ToString();
        // TODO Afetr the player get a card - > Check if Player as blackjack or bust
    }
    
    private void StandClicked()
    {
        DealerTurn();
    }

    private void DealerTurn()
    {
        // Showing the dealer hide card
        hideCard.GetComponent<Renderer>().enabled = false;

        //check if dealer win after show hide card
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
            dealerScript.GetCard();
            dealerScoreText.text = "HAND: " + dealerScript.handValue.ToString();
            RoundOver("dealerHit");

        }
        RoundOver("dealerHit");
    }

    void RoundOver(String name)
    {
        bool roundOver = true;

        // if the player bust/win after deal/hit button
        if (name == "player")
        {
            if (playerScript.handValue > 21)
            {
                mainText.text = "Bust: Dealer Win";
               // ResetTimer();
            }
            else if (playerScript.handValue == 21)
            {
                mainText.text = "Blackjack!! You Win";
                playerScript.AdjustMoney(pot);
                //ResetTimer();
            }
            else
            {
                return;
            }
        }

        /*
        if (name == "dealer")
        {
            //check if the dealer have 21 - after player press stand button
            if (dealerScript.handValue == 21)
            {
                mainText.text = "Dealer Win!! Blackjack";
                //playerScript.AdjustMoney(pot / 2);
                //ResetTimer();
            }
            else if (dealerScript.handValue > playerScript.handValue)
            {
                mainText.text = "Dealer Wins - Dealer Hand: " + dealerScript.handValue.ToString();
                //ResetTimer();

            }
            else
            {
                return;
            }
        }*/

        if (name == "dealerHit")
        {
            if (dealerScript.handValue == 21)
            {
                mainText.text = "Dealer Win!! Blackjack";
                //playerScript.AdjustMoney(pot / 2);
                //ResetTimer();
            }
            //Check dealer bust after hit card
            if (dealerScript.handValue > 21)
            {
                mainText.text = "Dealer Bust: You Win - Dealer Hand: " + dealerScript.handValue.ToString();
                playerScript.AdjustMoney(pot);
                //ResetTimer();
                //playerScript.AdjustMoney(pot / 2);
            }

            // If dealer has more points, dealer wins
            else if (dealerScript.handValue > playerScript.handValue)
            {
                mainText.text = "Dealer Wins - Dealer Hand: " + dealerScript.handValue.ToString();
                //ResetTimer();

            }
            else if (playerScript.handValue > dealerScript.handValue && dealerScript.handValue >= 17)
            {
                mainText.text = "You Win!! - Dealer Hand: " + dealerScript.handValue.ToString(); 
                playerScript.AdjustMoney(pot);
                //ResetTimer();

            }

            else if (playerScript.handValue == dealerScript.handValue && dealerScript.handValue >= 17)
            {
                mainText.text = "Push: Bets returned";
                //playerScript.AdjustMoney(pot / 2);
                //ResetTimer();
            }
            else
            {
                roundOver = false;
            }
        }

        // Set ui for next move / hand / turn
        if (roundOver)
        {
            dealBtn.gameObject.SetActive(true);
            hitBtn.gameObject.SetActive(false);
            standBtn.gameObject.SetActive(false);
            mainText.gameObject.SetActive(true);
            bet10Btn.enabled = true;
            bet50Btn.enabled = true;
            bet100Btn.enabled = true;
            bet200Btn.enabled = true;
            dealerScoreText.gameObject.SetActive(true);
            hideCard.GetComponent<Renderer>().enabled = false;
            cashText.text = "BANK: $" + playerScript.GetMoney().ToString();
            betsText.text = "BET: $0";
            pot = 0;
            //standClicks = 0;

        }
    }


    private void bet10Clicked()
    {
        //Text newBet = bet10Btn.GetComponentInChildren(typeof(Text)) as Text;
        //int intBet = int.Parse(newBet.text.ToString().Remove(0, 1));
        playerScript.AdjustMoney(-10);
        cashText.text = "BANK: $" + playerScript.GetMoney().ToString();
        pot += 10;
        betsText.text = "BET: $" + pot.ToString();
    }


    private void bet50Clicked()
    {
        playerScript.AdjustMoney(-50);
        cashText.text = "BANK: $" + playerScript.GetMoney().ToString();
        pot += 50;
        betsText.text = "BET: $" + pot.ToString();
    }

    private void bet100Clicked()
    {
        playerScript.AdjustMoney(-100);
        cashText.text = "BANK: $" + playerScript.GetMoney().ToString();
        pot += 100;
        betsText.text = "BET: $" + pot.ToString();
    }

    private void bet200Clicked()
    {
        playerScript.AdjustMoney(-200);
        cashText.text = "BANK: $" + playerScript.GetMoney().ToString();
        pot += 200;
        betsText.text = "BET: $" + pot.ToString();
    }

    /*
    private void StarTimer()
    {
        timeLeft = 30f;
        timerRunning = true;
    }*/

    /*
    void Update()
    {
        if (timerRunning)
        {
            if (timeLeft > 0)
            {
                timeLeft -= Time.deltaTime; // Decrease time
                UpdateTimerUI();
            }
            else
            {
                //ResetTimer();
                StandClicked();
            }
        }

        /*
        if(pot <= 0)
        {
            dealBtn.gameObject.SetActive(false);
        }

        else
        {
            dealBtn.gameObject.SetActive(true);
        }
        
    }
    */

    /*
    private void ResetTimer()
    {
        timeLeft = 30f; // Ensure it doesn’t go negative
        timerRunning = false;
    }
    */

    //TODO Timer
    private void UpdateTimerUI()
    {
        int seconds = Mathf.FloorToInt(timeLeft % 60); // Get seconds
        timerText.text = $"Time: {seconds}s";
    }


    /*
private void betClicked()
{
   throw new NotImplementedException();
}
*/
}
