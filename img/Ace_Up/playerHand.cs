using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class playerHand : MonoBehaviour
{
    public playerInfo playerInfo;
    public int handValue;

    [SerializeField] private List<GameObject> hand = new();
    [SerializeField] private List<cardInfo> cheatCards = new();
    [HideInInspector] public bool isBust;

    //Adds up all the cards currently in your hand and returns the total value,
    //Alongside whether it is a bust (over 21)
    public (int, bool) totalCardValue() 
    {
        handValue = 0;
        foreach (GameObject card in hand)
        {
            handValue = handValue + card.GetComponent<cardInfo>().card.cardValue;
        }
        if (checkForAces() > 21) { isBust = true; }
        else { isBust = false; }

        return (handValue, isBust);
    }

    //Reset player hand (called after every round)
    //Deactivates all card game objects in inventory, and clears the list of cards in the hand.
    public void resetCards() {
        foreach (GameObject card in hand) { card.SetActive(false); }
        isBust = false;

        hand = new();
    }

    //For every card in the players hand, check if it is an ace. If so:
    //Subtract 10 from the hand value (changing the Aces value from 11 to 1).
    //If the handValue is no longer greater than 21, break out of the 'For loop' early to avoid deducting more than neccesary.
    //This function is called every 'totalCardValue()' call so that we get an accurate result.
    private int checkForAces()
    {
        foreach (GameObject card in hand)
        {
            if (handValue > 21)
            {
                cardCreator currentCard = card.GetComponent<cardInfo>().card;
                if (currentCard.isAce) { handValue -= 10; }
            }
            else { break; }
        }
        return handValue;
    }


    //Loads the cheat cards based on PlayerInfo instance
    //(called when loading into the scene).
    public void loadCheatCards()
    {
        playerInfo = FindObjectOfType<playerInfo>();

        for (int i = 0; i < playerInfo.cheatHand.Count; i++)
        {
            cheatCards[i].card = playerInfo.cheatHand[i].card;
            if (cheatCards[i].gameObject.activeSelf)
            {
                cheatCards[i].updateInfo();
            }
        }
    }

    //Sets the PlayerInfo cheat cards based on current cheat cards 
    //(called when about to load into another scene).
    public void setCheatCards()
    {
        playerInfo = FindObjectOfType<playerInfo>();

        for (int i = 0; i < playerInfo.cheatHand.Count; i++)
        {
            playerInfo.cheatHand[i].card = cheatCards[i].card;
            playerInfo.cheatHand[i].updateInfo();
        }
    }

    //Set current cards in hand into PlayerInfo
    //(called when about to load into fight scene so that the cards can be used as ammunition (throwing cards)).
    public void setCards()
    {
        playerInfo = FindObjectOfType<playerInfo>();
        playerInfo.hand.Clear();
        foreach (GameObject card in hand)
        {
            cardCreator cardCreator = card.GetComponent<cardInfo>().card;
            playerInfo.hand.Add(cardCreator);
        }
    }
}
