using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DecisionMaker : MonoBehaviour {

    public DialogueInfo[] dialogue;
    public string question;
    public ComponentInfo result;
    public GameObject npc;
    public Text questionText;
    public bool person;

    private Animator playerAnimator;
    private Animator ruSureAnimator;
    private int decision;

    private bool isInRange;
    private DialogueInputer dialogueInputer;
    private int state;

    private void TriggerDialogue() //Function to run another function in the dialogue manager
    {
        FindObjectOfType<DialogueManager>().StartDialogue(dialogue, false);
        state = 1;
        if (person)
        {
            StartCoroutine(FacePlayer());
        }
    }

    private IEnumerator FacePlayer()
    {
        Animator npcAnimator = npc.GetComponent<Animator>();
        string playerState = (playerAnimator.GetCurrentAnimatorClipInfo(0))[0].clip.name;

        npcAnimator.SetFloat("x", 0);
        npcAnimator.SetFloat("y", 0);
        npcAnimator.SetLayerWeight(2, 0.9f);

        switch (playerState)
        {
            case "Player Idle Front": npcAnimator.SetFloat("y", 1); break;
            case "Player Idle Right": npcAnimator.SetFloat("x", -1); break;
            case "Player Idle Back": npcAnimator.SetFloat("y", -1); break;
            case "Player Idle Left": npcAnimator.SetFloat("x", 1); break;
        }

        yield return new WaitForSeconds(0.3f);

        npcAnimator.SetLayerWeight(2, 0);
    }

    private void askQuestion()
    {
        questionText.text = question;
        ruSureAnimator.SetInteger("state", 1); //Opens the RUsure box
        decision = 1;

    }

    void Start()
    {
        playerAnimator = GameObject.Find("Player").GetComponent<Animator>();
        isInRange = false;
        dialogueInputer = GameObject.Find("DialogueInputer").GetComponent<DialogueInputer>();
        ruSureAnimator = GameObject.Find("RUsureBox").GetComponent<Animator>();
        if (npc != null)
        {
            this.GetComponent<Transform>().position = npc.GetComponent<Transform>().position;
        }
        state = 0;

    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.CompareTag("Player")) //Checks if the player is in range to talk
        {
            isInRange = true;
        }
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Z) && dialogueInputer.enabled == false && isInRange && FindObjectOfType<MenuManager>().enabled == false && state == 0) //Determines of its the start of the dialogue or continuing a dialogue
        {
            TriggerDialogue();
        }
        else if (state == 1 && dialogueInputer.enabled == false)
        {
            FindObjectOfType<PlayerControl>().enabled = false;
            askQuestion();
            state = 2;
        }

        else if (state == 2)
        {
            if (Input.GetKeyDown(KeyCode.LeftArrow)) //Allows the user to highlight yes
            {
                FindObjectOfType<AudioManager>().Play("Menu_Navigation");
                decision = 1;
                ruSureAnimator.SetInteger("state", 1);
            }

            if (Input.GetKeyDown(KeyCode.RightArrow)) //Or no
            {
                FindObjectOfType<AudioManager>().Play("Menu_Navigation");
                decision = 2;
                ruSureAnimator.SetInteger("state", 2);
            }

            if (Input.GetKeyDown(KeyCode.Z)) //When something is selected
            {

                if (decision == 1)
                {
                    FindObjectOfType<AudioManager>().Play("Menu_Select");

                    GameObject componentBody = result.body;
                    switch (result.name)
                    {
                        case "DialogueTrigger": componentBody.GetComponent<DialogueTrigger>().enabled = result.on; break;
                        case "StoryTrigger": componentBody.GetComponent<StoryTrigger>().enabled = result.on; break;
                        case "Collider2D": componentBody.GetComponent<Collider2D>().enabled = result.on; break;
                        case "Image": componentBody.GetComponent<Image>().enabled = result.on; break;
                    }
                }

                ruSureAnimator.SetInteger("state", 0); //Opens the RUsure box
                state = 0;
                questionText.text = "This action will erase all your progress.\nAre you sure ? ";
                FindObjectOfType<PlayerControl>().enabled = true;
            }
        }
    }

    void OnTriggerExit2D(Collider2D other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            isInRange = false;
        }
    }
}
