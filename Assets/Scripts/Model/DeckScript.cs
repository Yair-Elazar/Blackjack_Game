using UnityEngine;

public class DeckScript : MonoBehaviour
{
    public Sprite[] cardSprites;
    int[] cardValues = new int[53];
    int currentIndex = 0;

    void Start()
    {
        GetCardValues();
    }


    void GetCardValues()
    {
        // Loop to assign values to the cards
        for (int i = 0; i < cardSprites.Length; i++)
        {
            int num = i;
            //Count up to the amount of cards, 52
            // num %= 13 -> return the card value(num=27, 28%=13= 1 => 2 -> card value)
            num %= 13;
            //if the num is over 10 -> means that is K/Q/P -> them value is 10
            if (num > 10 || num == 0)
            {
                num = 10;
            }
            cardValues[i] = num++;
        }
    }

    public void Shuffle()
    {
        //Standard array data swapping technique
        for (int i = cardSprites.Length - 1; i > 0; --i)
        {
            int j = Mathf.FloorToInt(Random.Range(0.0f, 1.0f) * cardSprites.Length - 1) + 1;
            Sprite face = cardSprites[i];
            cardSprites[i] = cardSprites[j];
            cardSprites[j] = face;

            int value = cardValues[i];
            cardValues[i] = cardValues[j];
            cardValues[j] = value;
        }
        currentIndex = 1;
    }

    public int DealCard(CardScript cardScrite)
    {
        cardScrite.SetSprite(cardSprites[currentIndex]);
        cardScrite.SetValue(cardValues[currentIndex]);
        currentIndex++;
        return cardScrite.GetValueOfCard();
    }

    public Sprite GetCardBack()
    {
        return cardSprites[0];
    }
}
