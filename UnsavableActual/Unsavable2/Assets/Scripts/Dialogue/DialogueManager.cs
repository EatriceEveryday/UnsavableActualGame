using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DialogueManager : MonoBehaviour {

    public Image imageFace;
    public Text dialogueText;

    public Animator animator;

    private Queue<string> sentences;
    private Queue<Sprite> images;
    private Queue<string> directions;
    private Queue<Animator> animators;
    private Queue<bool> fastClear;
    private DialogueInputer dialogueInputer;
    private string sentence, pastSentence;

    [HideInInspector]
    public bool isStory;

	// Use this for initialization
	void Start () {
        sentences = new Queue<string>(); //Queues up the sentences inputed
        images = new Queue<Sprite>();
        directions = new Queue<string>();
        animators = new Queue<Animator>();
        fastClear = new Queue<bool>();
        dialogueInputer = GameObject.Find("DialogueInputer").GetComponent<DialogueInputer>();
	}
	
    public void StartDialogue (DialogueInfo[] dialogueInfo, bool type) //Function called when the dialogue starts
    {
        sentences.Clear(); //Clears existing sentences
        images.Clear();
        isStory = type;

        foreach(DialogueInfo dialogue in dialogueInfo)
        {
            sentences.Enqueue(dialogue.sentence); //Queues up the sentences
            images.Enqueue(dialogue.face);
            directions.Enqueue(dialogue.direction);
            animators.Enqueue(dialogue.animator);
            fastClear.Enqueue(dialogue.dontWait);
        }

        dialogueInputer.enabled = true; //Turns on the Dialogue Inputer
        DisplayNextSentence();
    }

    public bool DisplayNextSentence()
    {
        if (sentences.Count == 0) //Stops displaying sentences if there are no more sentences
        {
            EndDialogue();
            return(true); //Returns true to indicate there are no more sentences.
        }

        if (sentence != null)
        {
            if (sentence.ToCharArray()[0] == '{')
            {
                pastSentence = pastSentence + sentence;
            }
            else
            {
                pastSentence = sentence;
            }
        }

        sentence = sentences.Dequeue();
        Sprite image = images.Dequeue();
        string direction = directions.Dequeue();
        Animator npcAnimator = animators.Dequeue();

        if (pastSentence != null) //Only checks if the current sentence is NOT the first sentence
        {
            if (sentence.ToCharArray()[0] == '{') //If the current sentence contains '{', keep the previous sentence to continue adding on to it
            {
                dialogueText.text = "";
                foreach (char letter in pastSentence.ToCharArray())
                {
                    if (letter != '{') //Types out the sentence while ignoring '{'
                    {
                        dialogueText.text += letter;
                    }
                }
            }
        }

        if (image == null)
        {
            animator.SetInteger("state", 1); //Opens the dialogue box according to whether or not an image was placed.
        }
        else
        {
            animator.SetInteger("state", 2);
        }

        StopAllCoroutines(); //If this function was called while the previous sentence was still being typed, this will stop it.

        if (npcAnimator != null)
        {
            StartCoroutine(RunAnimation(direction, npcAnimator));
        }

        StartCoroutine(TypeSentence(sentence)); //Start typing the next sentence

        if (image != null) //Display the image if any
        {
            imageFace.sprite = image;
        }

        return (false); //Return false to indicate that there are more sentences.
    }

    IEnumerator TypeSentence (string sentence) //Function to display the sentences letter by letter
    {
        char[] letters = sentence.ToCharArray();
        bool dontWait = fastClear.Dequeue();

        if (letters[0] != '{') //Only deletes the previous sentence if the current one has no '{'
        {
            dialogueText.text = "";
        }

        for (int i = 0; i < letters.Length; i++)
        {
            if (letters[i] != '{') //Types out the sentence while ignoring '{'
            {
                dialogueText.text += letters[i];
                yield return null;
            }
            else
            {
                yield return null;
            }
        }

        if (dontWait)
        {
            DisplayNextSentence();
        }
    }

    public IEnumerator RunAnimation(string direction, Animator animator) //Function to make the npc face the right way
    {
        animator.SetFloat("x", 0);
        animator.SetFloat("y", 0);

        switch (direction)
        {
            case "Back": animator.SetLayerWeight(2, 0.9f); ; animator.SetFloat("y", 1); break;
            case "Right": animator.SetLayerWeight(2, 0.9f); animator.SetFloat("x", 1); break;
            case "Front": animator.SetLayerWeight(2, 0.9f); animator.SetFloat("y", -1); break;
            case "Left": animator.SetLayerWeight(2, 0.9f); animator.SetFloat("x", -1); break;
            case "Reset": animator.SetTrigger("Reset"); break;
            default: animator.Play(direction, -1, 0); break;
        }

        yield return new WaitForSeconds(0.1f);

        animator.SetLayerWeight(2, 0);
    }

    void EndDialogue()
    {
        animator.SetInteger("state", 0); //Closes the dialogue box
    }
}
