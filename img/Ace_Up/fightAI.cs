using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.UI;

public class fightAI : MonoBehaviour
{
    [Header("AI Movement")]
    public float rotationSpeed = 5;
    public float distanceThreshold = 5;

    [Header("AI FPS")]
    public int shotCount;
    public int maxShotInterval = 10;
    public Transform cardsUI;

    public GameObject throwingCard;
    public Transform throwPosition;

    public List<GameObject> players = new();
    public List<cardCreator> deck = new();
    public Sprite blankCard;

    [Header("Additional")]
    private Animator anim;
    private NavMeshAgent agent;
    [SerializeField] private Transform selectedPlayer;
    [SerializeField] private Transform mainPlayer;
    [SerializeField] private Vector3 targetPos;

    private void Start() 
    {
        anim = GetComponent<Animator>();
        agent = GetComponent<NavMeshAgent>();
        agent.updateRotation = false;

        shotCount = Random.Range(2, cardsUI.childCount + 1);
        loadShots();
        StartCoroutine("shootCard");

        selectPlayer();
    }

    private void Update()
    {
        //If they have reached their destination, start to shoot and find a new player to move to
        if (Vector3.Distance(transform.position, targetPos) <= 2) { selectPlayer(); }

        if (selectedPlayer != transform && selectedPlayer != null)
        { transform.rotation = Quaternion.Lerp(transform.rotation, Quaternion.LookRotation(selectedPlayer.position - transform.position), Time.deltaTime * rotationSpeed); }

        RaycastHit hitInfo;
        if (mainPlayer != null && Physics.Raycast(mainPlayer.position, mainPlayer.TransformDirection(Vector3.forward), out hitInfo, Mathf.Infinity) && hitInfo.transform.CompareTag("Player"))
        { hitInfo.transform.GetComponent<fightAI>().cardsUI.gameObject.SetActive(true); }
        else { cardsUI.gameObject.SetActive(false); }
    }

    #region EnemyMovement
    //Selects a player (whether the closest one or the main player)
    private void selectPlayer()
    {
        for (int i = 0; i < players.Count; i++)
        {
            if (players[i] == (null))
            { players.RemoveAt(i); }
        }

        int randOutcome = Random.Range(0, 3);
        if (randOutcome == 0 && players.Count >= 1) { selectedPlayer = getClosestPlayer(); }
        else if (randOutcome == 1 && players.Count >= 1)
        {
            int randInt = Random.Range(0, players.Count);
            while (players[randInt] == null) { randInt = Random.Range(0, players.Count); }
            selectedPlayer = players[randInt].transform; 
        }
        else{ selectedPlayer = transform; }

        moveToPlayer();
    }

    //Gets the closest player, as long as there are any
    private Transform getClosestPlayer()
    {
        float closestDistance = Mathf.Infinity;
        Transform closestPlayer = null; ;

        foreach (GameObject p in players)
        {
            if (p == null){ break; }
            Transform player = p.transform;

            float distance = Vector3.Distance(player.position, transform.position);
            if (distance <= closestDistance)
            {
                closestDistance = distance;
                closestPlayer = player;
            }
        }
        return closestPlayer;
    }

    //Move directly towards the plater if they are too far away
    private void moveToPlayer()
    {
        if (selectedPlayer == null) { return; }

        Vector3 pos = Vector3.zero;
        float distance = Vector3.Distance(selectedPlayer.position, transform.position);

        //Selected Player further than Threshold
        if (distance >= distanceThreshold) { pos = (selectedPlayer.position + transform.position) / 2; }

        //Selected Player is within Threshold
        else { pos = transform.position - (selectedPlayer.position - transform.position); }

        if (selectPoint(pos, out targetPos)) { agent.SetDestination(targetPos); }
    }

    //Selects a random position available in the navmesh closest to the selected position
    private bool selectPoint(Vector3 position, out Vector3 result)
    {
        Vector3 randomPoint = position + Random.insideUnitSphere * 2;
        NavMeshHit hit;
        if (NavMesh.SamplePosition(randomPoint, out hit, 5.0f, NavMesh.AllAreas))
        {
            result = hit.position;
            return true;
        }
        result = targetPos;
        return false;
    }
    #endregion

    #region EnemyShooter

    //Sets a random card texture for each card in inventory
    private void loadShots()
    {
        int counter = 1;
        foreach (Transform card in cardsUI)
        {
            if (counter <= shotCount)
            {
                //Sets a random card UI
                card.gameObject.SetActive(true);

                Image cardIMG = card.GetComponent<Image>();
                if (cardIMG.sprite == blankCard) { cardIMG.sprite = deck[Random.Range(0, deck.Count)].cardImg; }
            }
            else { card.gameObject.SetActive(false); }
            counter++;
        }
    }

    private IEnumerator shootCard()
    {
        yield return new WaitForSeconds(shotPeriods());
        if (shotCount >= 1) //If the enemy has cards to shoot
        {
            anim.SetTrigger("startShoot");  //charge up the shot
            yield return new WaitForSeconds(2.2f);

            //instantiate the throwing cards that shoots forwards
            GameObject cardObject = Instantiate(throwingCard, throwPosition.position, throwPosition.rotation);
            cardObject.GetComponent<throwingCard>().initialOwner = gameObject;

            cardCreator randomCard = deck[Random.Range(0, deck.Count)];
            cardObject.GetComponent<SpriteRenderer>().sprite = randomCard.cardImg;

            Destroy(cardObject, 5f);

            shotCount--;
            loadShots();
        }
        StartCoroutine("shootCard");
    }

    //Increments the period the enemy shoots based on how many cards they have left
    private float shotPeriods()
    {
        float min;      float max;

        if (shotCount == 6) { min = 2f; max = 3f; }
        else if (shotCount == 5) { min = 2f; max = 4f; }
        else if (shotCount == 4) { min = 3f; max = 5f; }
        else if (shotCount == 3) { min = 4f; max = 6f; }
        else { min = 5; max = 7; }

        return Random.Range(min, max);
    }

    #endregion
}
