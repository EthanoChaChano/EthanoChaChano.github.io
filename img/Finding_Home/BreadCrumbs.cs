using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class BreadCrumbs : MonoBehaviour
{
    public GameObject breadCrumbs;
    public int breadAmount = 5;
    public Text breadCounter;
    public Collider lost;
    public PlayerController controller;

    public void Update()
    {
        breadCounter.text = "Bread Left: " + breadAmount + " crumbs";

        //breadAmount can only be greater than '999' when interacting with the parents who gives enough bread to reach this value
        if (breadAmount >= 999){ StartCoroutine(Win()); }
    }

    //The player has a sphere collider attached to the GameObject. If they exit a collider with the tag 'BreadCrumbs',
    //instantiate a new breadcrumb at the current location. This eventually makes a path of breadcrumbs where the player can freely navigate
    //the scene if there is any breadcrumbs in the surrounding area.
    private void OnTriggerExit(Collider collisionInfo)
    {
        if (breadAmount > 0)
        {
            RaycastHit hit;
            if (Physics.Raycast(transform.position, transform.TransformDirection(Vector3.down), out hit, Mathf.Infinity) && collisionInfo.tag == "BreadCrumbs")
            {
                collisionInfo.enabled = false;
                Debug.DrawRay(transform.position, transform.TransformDirection(Vector3.down) * hit.distance, Color.yellow);
                GameObject bread = Instantiate(breadCrumbs);
                bread.transform.position = hit.point;
                bread.transform.rotation = Quaternion.Euler(0, Random.Range(0, 359), 0);
                breadAmount--;
            }
        }

        //When the player has no bread left, and left any nearby breadcrumbs
        if (breadAmount <= 0)
        {
            RenderSettings.fogDensity = 0.1f;
            lost.enabled = true;
            controller.enabled = false;
            StartCoroutine(Lost());
        }
    }

    //Player has lost, reload the scene
    private IEnumerator Lost()
    {
        yield return new WaitForSeconds(2.5f);
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    //Player has won, transition to the win screen ('EndScreen')
    private IEnumerator Win()
    {
        yield return new WaitForSeconds(3f);
        SceneManager.LoadScene("EndScreen");
    }
}
