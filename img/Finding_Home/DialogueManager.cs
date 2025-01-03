using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DialogueManager : MonoBehaviour
{

    [SerializeField] private Text nameText;
    [SerializeField] private Text dialogueText;
    [SerializeField] private Image npcImage;

    [SerializeField] private Animator animator;
    [SerializeField] private AudioSource speak;

    public BreadCrumbs breadCrumbs;
    private int givenBread;
    private Queue<string> sentences;

    void Start()
    {
        sentences = new Queue<string>();   
    }

    //Opens the dialogue box when called, updating the box to contain the correct NPC Name, NPC Image, and how much bread they should give
    //Dialogue is a custom scriptableo object.
    public void StartDialogue(Dialogue dialogue)
    {
        animator.SetBool("isOpen", true);

        nameText.text = dialogue.name;
        npcImage.sprite = dialogue.img;
        givenBread = dialogue.breadGiven;

        sentences.Clear();

        foreach (string sentence in dialogue.sentences)
        {
            sentences.Enqueue(sentence);
        }
        speak.Play();
        DisplayNextSentence(dialogue);
    }

    //If there are no sentences left, end the dialogue, and give bread if not already givem
    //Otherwise, If there is still sentences in the queue, call TypeSentence() function
    public void DisplayNextSentence(Dialogue dialogue)
    {
        if (sentences.Count == 0)
        {
            EndDialogue();
            if (dialogue.isGiven == false)
            {
                breadCrumbs.breadAmount += givenBread;
                dialogue.isGiven = true;                
            }
            return;
        }

        string sentence = sentences.Dequeue();
        StopAllCoroutines();
        StartCoroutine(TypeSentence(sentence));
    }

    //TypeSentence() increments each text one after another so that they do not appear in the dialogue box all at once
    private IEnumerator TypeSentence(string sentence)
    {
        dialogueText.text = "";
        foreach (char letter in sentence.ToCharArray())
        {
            dialogueText.text += letter;
            yield return null;
        }
    }

    //Ends Dialogue if player leaves the dialogue area early.
    public void EndDialogue()
    {
        animator.SetBool("isOpen", false);
        speak.Stop();
    }
}
