using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class cardManager : MonoBehaviour
{
    [Header("Linked Scripts")]
    public playerHand hand;
    public newPlayerInfo playerInfo;
    public sceneManager sceneManager;
    public crownManager crownManager;

    [Header("Betting Settings")]
    [SerializeField] private TMP_InputField betAmount;
    [SerializeField] private TMP_Text currencyText;
    [SerializeField] private TMP_Text betPot;
    [SerializeField] private TMP_Text cardValueText;
    [SerializeField] private GameObject pokerChip;

    [Header("Card Settings")]
    public int handValue;
    public GameObject selectedCard;
    public GameObject[] playerCards;
    public GameObject cheatDeck;
    public List<cardCreator> deck = new List<cardCreator>();
    private List<cardCreator> newDeck = new List<cardCreator>();

    [Header("Gameplay Settings")]
    public List<string> nameList = new();
    public Camera cam;
    public LayerMask targetLayer;
    public GameObject endScene;
    private Coroutine cheatDetector;
    [SerializeField] private newCardAI[] cardAI;
    
    [Header("User Interfaces")]
    public GameObject playerOptionsUI;
    [SerializeField] private TMP_Text winUI;
    [SerializeField] private GameObject betUI;
    [SerializeField] private GameObject callOutUI;
    [SerializeField] private GameObject LookDirection;

    [Header("StatInfo UI")]
    [SerializeField] private GameObject statInfo;
    [SerializeField] private GameObject statDropDown;
    [SerializeField] private GameObject quotaIcon;
    [SerializeField] private Slider roundsSlider;
    [SerializeField] private TMP_Text roundsPlayed;
    [SerializeField] private TMP_Text betTotal;
    [SerializeField] private TMP_Text debtAmount;
    

    [Header("Animators")]
    [SerializeField] private Animator playerAnim;
    [SerializeField] private Animator cardsAnim;
    [SerializeField] private Animator betAnim;
    [SerializeField] private Animator cheatWarningAnim;

    [Header("Audio")]
    [SerializeField] private AudioSource cardShuffle;
    [SerializeField] private AudioSource cheatWarningAudio;
    [SerializeField] private AudioSource endRoundAudio;
    [SerializeField] private AudioSource LoseAudio;
    [SerializeField] private AudioSource WinAudio;

    public bool canCheat;
    private bool lookingUp;
    private bool placedBet;
    private bool roundStarted;
    private bool AIStarted;
    private bool endSceneActive;

    private void Start()
    {
        playerInfo = FindObjectOfType<newPlayerInfo>();
        sceneManager = FindObjectOfType<sceneManager>();

        //Checks to see if the players currency count is 0 or less
        if (playerInfo.currency <= 0) { StartCoroutine("startEndScene"); return; }

        currencyText.text = playerInfo.currency.ToString();

        //Sets a random name for each AI
        foreach (newCardAI AI in cardAI)
        {
            AI.AIName = nameList[Random.Range(0, nameList.Count)];
            AI.updateName();
        }
    }


    void Update()
    {
        //If player has played more than 5 rounds, start the end scene
        if (playerInfo.roundsPlayed == 5 && roundStarted == false)
        {
            if (!endSceneActive) { StartCoroutine("startEndScene"); }
            return;
        }

        //If a bet is placed, allow player to look around & play Blackjack
        if (placedBet && roundStarted)
        {
            //If player is currently looking at cards and chooses to look up,
            //Hide the cards and player options
            if (lookingUp)
            {
                if (cardsAnim.GetBool("isShowing"))
                {
                    cardsAnim.SetTrigger("hideCards");
                    cardsAnim.SetBool("isShowing", false);

                    playerOptionsUI.SetActive(false);
                }
                detectLookAt();
            }
            //If player is not currently looking at cards and chooses to look down
            //Show the cards and player options
            else if (!lookingUp && !cardsAnim.GetBool("isShowing"))
            {
                cardsAnim.SetTrigger("showCards");
                cardsAnim.SetBool("isShowing", true);

                playerOptionsUI.SetActive(true);
                callOutUI.SetActive(false);
                cheatWarningAudio.Stop();
            }

            //If round has started, allow player to play Blackjack
            //Update every frane the handValue in case the player has swapped their cards.
            handValue = hand.totalCardValue().Item1;
            cardValueText.text = handValue.ToString();
        }

        //Otherwise, if round has not started and they have currency, make the player place a bet
        else if (!placedBet && playerInfo.currency >= 1)
        {
            if (betUI.activeSelf == false)
            {
                betUI.SetActive(true);
                betAnim.SetTrigger("showBet");
                playerOptionsUI.SetActive(false);
            }

            if (Input.GetKeyDown(KeyCode.Return))
            {
                placeBetBTN();
            }
        }

        StartCoroutine("resetTriggers");
    }

    #region cameraRotation

    //Make the player look up
    public void lookUp()
    { 
            playerAnim.SetTrigger("LookUp");
            selectedCard = null;
            updateSelectedCard();
            lookingUp = true; 
    }

    public void lookLeft() { playerAnim.SetTrigger("LookLeft"); } //Make the player look left

    public void lookRight() { playerAnim.SetTrigger("LookRight"); } //Make the player look right

    public void lookDown() { playerAnim.SetTrigger("LookDown"); lookingUp = false; } //Make the player look down

    #endregion

    //Resets all triggers to prevent spamming of camera animations
    IEnumerator resetTriggers()
    {
        yield return new WaitForEndOfFrame();
        playerAnim.ResetTrigger("LookUp");
        playerAnim.ResetTrigger("LookLeft");
        playerAnim.ResetTrigger("LookRight");
        playerAnim.ResetTrigger("LookDown");

        cardsAnim.ResetTrigger("hideCards");
        cardsAnim.ResetTrigger("showCards");
    }



    #region BlackJack Game

    //Resets the players cards and gives 2 new cards when the round starts
    private void newRound()
    {
        roundStarted = true;
        canCheat = true;

        newDeck = new List<cardCreator>(deck); //copies all cards from the list in 'deck' into 'newDeck'

        hand.resetCards();
        for (int i = 0; i < 2; i++) { createCard(); } //loops twice for the player to get 2 cards
    }


    //Creates a new card from available cards in deck
    public void createCard()
    {
        if (newDeck.Count > 0) //If there are cards in the deck
        {
            foreach (GameObject card in playerCards)
            {
                GameObject cardParent = card.transform.parent.gameObject;
                if (cardParent.activeSelf == false)
                {
                    cardParent.SetActive(true);

                    int randomIndex = Random.Range(0, newDeck.Count);

                    cardInfo cardInfo = card.GetComponent<cardInfo>();
                    cardInfo.card = newDeck[randomIndex];    //Give a random card value
                    cardInfo.updateInfo();

                    newDeck.RemoveAt(randomIndex);    //remove card from deck so no other player can recieve duplicate cards

                    hand.hand.Add(card);

                    break;
                }
            }
        }
    }
    #endregion

    #region Button Inputs

    //Places a bet as long as a value has been input
    public void placeBetBTN()
    {
        if (betAmount.text == "") { return; }

        int amountToBet = int.Parse(betAmount.text);
        betAmount.text = "";

        //Checks if the player has enough currency to bet that amount and if the amount bet is more than 0.
        if (amountToBet <= playerInfo.currency && amountToBet >= 1)
        {
            placeBet(amountToBet);
            return;
        }
        else if (amountToBet > playerInfo.currency)
        {
            betAnim.SetTrigger("BetError");
        }
    }

    //Bets ALL of the players currency
    public void AllInBTN() { placeBet(playerInfo.currency); }

    //Bets HALF of the players currency
    public void HalfInBTN() { placeBet(Mathf.Max(1, Mathf.RoundToInt(playerInfo.currency * 0.5f))); }

    //Places the bet and then starts the game
    private void placeBet(long amountToBet)
    {
        playerInfo.playerBetAmount = amountToBet;

        //Total bet amount is a random value somewhere above 4x the players bet (accounting for some random variables to simulate other AIs bets)
        playerInfo.totalBetAmount = Mathf.RoundToInt(playerInfo.playerBetAmount * Random.Range(2, 4) + Random.Range(0, playerInfo.playerBetAmount * 0.5f));
        betPot.text = playerInfo.totalBetAmount.ToString();

        //Removes the players betted amount from their currenct
        playerInfo.currency -= amountToBet;
        currencyText.text = playerInfo.currency.ToString();

        betAnim.SetTrigger("FoldPaper");
        playerAnim.SetTrigger("LookAtPot");

        cardShuffle.Play();

        placedBet = true;
        betUI.SetActive(false);

        StartCoroutine("StartGame");
    }

    private void endTurn() { StartCoroutine("endRound"); }

    private IEnumerator endRound()
    {
        playerAnim.SetTrigger("LookUp");
        LookDirection.SetActive(false);
        canCheat = false;

        //Stops AI from cheating
        foreach (newCardAI AI in cardAI)
        {
            AI.StopAllCoroutines();
            AI.pauseAI();
        }

        //Shows players current stats (e.g. currency, bet amount, quota, etc...)
        displayCurrencyStats();
        statInfo.SetActive(true);
        statDropDown.SetActive(false);

        endRoundAudio.Play();
        crownManager.startWinnerAnim();

        //Gets Winner
        StartCoroutine("getWinner");
        yield return new WaitForSeconds(2);

        //Resets playerInfo so a new round could be started next
        playerInfo.totalBetAmount = 0;
        playerInfo.totalRoundsPlayed++;
        playerInfo.roundsPlayed++;
        displayCurrencyStats();

        yield return new WaitForSeconds(3f);
        playerAnim.SetTrigger("LookDown");

        statInfo.SetActive(false);
        statDropDown.SetActive(true);

        //Resets AI cards
        AIStarted = false;
        foreach (newCardAI AI in cardAI) { AI.resetAI(); }

        placedBet = false;
        roundStarted = false;

        //Reset players cards
        selectedCard = null;
        updateSelectedCard();

        cardsAnim.SetTrigger("hideCards");
        cardsAnim.SetBool("isShowing", false);

        foreach (GameObject card in playerCards)
        {
            GameObject cardParent = card.transform.parent.gameObject;
            cardParent.SetActive(false);
            hand.hand.Remove(card);
        }

        //Checks if player has less than 0 currency
        if (playerInfo.currency <= 0) { StartCoroutine("startEndScene"); }
    }
    #endregion

    //Gets the winner of the round
    private IEnumerator getWinner()
    {
        newCardAI winnerAI = null;
        string winner = "";
        int highestHand = 0;

        foreach (newCardAI AI in cardAI)
        {
            //If AI has not busted (gone over 21), whilst having a higher hand than the current highest hand value
            if (!AI.isBust && AI.handValue >= highestHand)
            {
                highestHand = AI.handValue;
                winner = AI.AIName;
                winnerAI = AI;
            }
        }

        //after finding the winner of all AI's, test if the player has a higher hand than all of them
        //If not bust (.Item2 != true)
        //Player has priority over all enemies to reward them for aiming for their highest hand possible
        if (!hand.totalCardValue().Item2 && handValue >= highestHand)
        {
            highestHand = handValue;
            winner = "You";
            WinAudio.Play();
            yield return new WaitForSeconds(1.9f);
            playerInfo.currency += playerInfo.totalBetAmount;
            currencyText.text = playerInfo.currency.ToString();
            yield return new WaitForSeconds(0.35f);
            crownManager.crownToWinner(1);
        }
        //If the player does not beat all 3 AIs
        else 
        {
            LoseAudio.Play();
            yield return new WaitForSeconds(2.25f);
            if (winnerAI != null)
            {
                //Look towards the winner and moves the crown towards them (controlled by crownManager script)
                if (winnerAI == cardAI[0]) { playerAnim.SetTrigger("LookRight"); crownManager.crownToWinner(2); }
                else if (winnerAI == cardAI[2]) { playerAnim.SetTrigger("LookLeft"); crownManager.crownToWinner(4); }
                else { crownManager.crownToWinner(3); }
            }
        }
        winUI.text = getWinText();
        yield return null;
    }

    //Updates winner text so that player can see who won the previous rounds
    private string getWinText()
    {
        string winner = "";
        winner = "Your Hand : " + handValue + "\n";
        foreach (newCardAI AI in cardAI)
        {
            winner = winner + AI.AIName + " : " + AI.handValue + "\n";
        }
        return winner;
    }

    //Called when looking up, detects who the player looks at (which AI)
    //If that AI is cheating at the time, alert the player
    void detectLookAt()
    {
        //Raycast
        Ray ray = new Ray(cam.transform.position, cam.transform.forward);
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit, Mathf.Infinity, targetLayer))
        {
            Vector2 mousePosition = new Vector2(Input.mousePosition.x, Input.mousePosition.y);
            Vector2 mousePos = mousePosition;

            // Check if the hit object's layer matches the target layer (cheating player)
            if (((1 << hit.collider.gameObject.layer) & targetLayer) != 0)
            {
                callOutUI.SetActive(true);
                if (!cheatWarningAudio.isPlaying) { cheatWarningAudio.Play(); }

                //if player left clicks when the enemy is cheating and the fight scene is currently not loading
                if (Input.GetMouseButtonDown(0) &&
                    !sceneManager.sceneLoading &&
                    mousePos.x >= Screen.width * 0.2f && mousePos.x <= Screen.width * 0.8f && mousePos.y >= Screen.height * 0.3f)
                {
                    //Saves the players current hand and load fight scene
                    hand.setCards();
                    playerInfo.inFight = true;
                    sceneManager.loadScene(2);
                }
            }
            else { callOutUI.SetActive(false); cheatWarningAudio.Stop(); }
        }
        else { callOutUI.SetActive(false); cheatWarningAudio.Stop(); }
    }

    public void updateSelectedCard()
    {
        //Lowers every other card
        foreach (GameObject card in playerCards)
        {
            if (card.transform.parent.gameObject.activeSelf == true)
            {
                card.GetComponent<newPlayerManager>().defaultPosition();                
            }
        }

        //If the player has click on a card, show cheat hand
        if (selectedCard != null)
        {
            cheatDeck.SetActive(true);
            cheatWarningAnim.Play("WarningDefault");

            if (cheatDetector != null)
            {
                StopCoroutine(cheatDetector);
                cheatDetector = null;
            }
            cheatDetector = StartCoroutine("startCheat");
        }
        else 
        {
            cheatDeck.SetActive(false);
            cheatWarningAudio.Stop();
        }
    }

    //When the player starts cheating, given a random time delay, then start the warning for cheating
    //After a 4.5 seconds (duration of cheatWarningAudio), load the fight scene
    private IEnumerator startCheat()
    {
        yield return new WaitForSeconds(Random.Range(3f, 5f));

        cheatWarningAnim.SetTrigger("startShake");

        if (selectedCard != null)
        {
            cheatWarningAudio.loop = true;
            cheatWarningAudio.Play();
        }

        yield return new WaitForSeconds(4.5f);
        cheatWarningAudio.loop = false;

        if (selectedCard != null)
        {
            hand.setCards();
            playerInfo.inFight = true;
            sceneManager.loadScene(2);
        }
    }

    //Runs after the player places a bet
    private IEnumerator StartGame()
    {
        //Instantiates 25 chips above the betting pot (chips get destroyed after 0.5 seconds)
        for (int i = 0; i < 25; i++)
        {
            GameObject chip = Instantiate(pokerChip);

            float randomX = Random.Range(0f, 360f);
            float randomY = Random.Range(0f, 360f);
            float randomZ = Random.Range(0f, 360f);

            chip.transform.rotation = Quaternion.Euler(randomX, randomY, randomZ);
            yield return new WaitForSeconds(0.1f);

            Destroy(chip, 0.5f);
        }

        //If the round has not started prior, distribute 2 cards to the player
        newRound();
        cardsAnim.SetTrigger("showCards");
        cardsAnim.SetBool("isShowing", true);

        yield return new WaitForSeconds(1.5f);
        LookDirection.SetActive(true);
        playerOptionsUI.SetActive(true);

        //Starts the AI's
        if (!AIStarted)
        {
            AIStarted = true;
            foreach (newCardAI AI in cardAI) { AI.runAI(); }
        }
    }

    //Display UI so the player can see the previous winners
    public void displayWinnerButton()
    {
        GameObject winGO = winUI.transform.parent.gameObject;
        winGO.SetActive(!winGO.activeSelf);
    }

    //Displays UI of player stats, and updates their values
    public void displayCurrencyStats()
    {
        betTotal.text = playerInfo.totalBetAmount.ToString();
        roundsPlayed.text = playerInfo.roundsPlayed.ToString();
        roundsSlider.value = playerInfo.roundsPlayed;
        debtAmount.text = playerInfo.quotaAmount.ToString();

        if (playerInfo.quotaReached) { quotaIcon.SetActive(true); }
        else { quotaIcon.SetActive(false); }
    }

    //Starts the end scene, with a transition so that change between cameras is not noticeable
    private IEnumerator startEndScene()
    {
        endSceneActive = true;

        sceneManager.fadeDark();
        yield return new WaitForSeconds(1);
        sceneManager.fadeScene();

        cardsAnim.gameObject.SetActive(false);
        playerOptionsUI.SetActive(false);
        betUI.SetActive(false);
        winUI.transform.parent.parent.parent.gameObject.SetActive(false);
        statInfo.transform.parent.gameObject.SetActive(false);
        LookDirection.SetActive(false);

        endScene.SetActive(true);
    }

    //Loads the shop scene
    public void loadShop()
    {
        sceneManager.loadScene(3);
    }

    //Loads the main menu (resetting the players currency)
    public void loadMenu()
    {
        playerInfo.currency = 100;
        sceneManager.loadScene(0);
    }
}