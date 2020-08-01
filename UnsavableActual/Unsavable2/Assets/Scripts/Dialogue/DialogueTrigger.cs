using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DialogueTrigger : MonoBehaviour {

    public DialogueInfo[] dialogue;
    public GameObject npc;
    public bool person;
    private Animator npcAnimator;
    private Animator playerAnimator;

    private bool isInRange;
    private DialogueInputer dialogueInputer;

    private void TriggerDialogue() //Function to run another function in the dialogue manager
    {
        FindObjectOfType<DialogueManager>().StartDialogue(dialogue, false);
        if (person)
        {
            StartCoroutine(FacePlayer(npcAnimator));
        }
    }

    private IEnumerator FacePlayer(Animator npcAnimator)
    {
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

    void Start ()
    {
        playerAnimator = GameObject.Find("Player").GetComponent<Animator>();
        npcAnimator = npc.GetComponent<Animator>();
        isInRange = false;
        dialogueInputer = GameObject.Find("DialogueInputer").GetComponent<DialogueInputer>();
        this.GetComponent<Transform>().position = npc.GetComponent<Transform>().position;

    }

    void OnTriggerEnter2D (Collider2D other)
    {
        if (other.gameObject.CompareTag("Player")) //Checks if the player is in range to talk
        {
            isInRange = true;
        }
    }

    void Update (){

        if (Input.GetKeyDown(KeyCode.Z) && dialogueInputer.enabled == false && isInRange && FindObjectOfType<MenuManager>().enabled == false) //Determines of its the start of the dialogue or continuing a dialogue
        {
            TriggerDialogue();
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
