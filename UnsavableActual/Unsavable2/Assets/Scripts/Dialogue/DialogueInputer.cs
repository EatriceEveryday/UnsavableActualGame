using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DialogueInputer : MonoBehaviour {

    private GameObject player;

    public bool ContinueDialogue() //Ditto
    {
        return (FindObjectOfType<DialogueManager>().DisplayNextSentence());
    }

    // Use this for initialization
    void Awake () {
        player = GameObject.Find("Player");
    }
	
    void OnEnable()
    {
        if (player != null) //To stop it from running if the scene is a boss battle
        {
            player.GetComponent<PlayerControl>().rb2d.velocity = new Vector3(0, 0, 0); //Set the velocity to 0
            player.GetComponent<PlayerControl>().enabled = false; //Stops the user from moving when in dialogue
            player.GetComponent<Animator>().SetLayerWeight(1, 0); //Stops the walking animation
        }
    }

	// Update is called once per frame
	void Update () {
		if (Input.GetKeyDown(KeyCode.Z))
        {
            if (ContinueDialogue())
            {
                this.enabled = false;
            }
        }
	}

    void OnDisable()
    {
        if (FindObjectOfType<DialogueManager>() != null & player != null)
        {
            if (!FindObjectOfType<DialogueManager>().isStory)
            {
                player.GetComponent<PlayerControl>().enabled = true; //Allows the user to move again
            }
        }

        if (FindObjectOfType<MenuManager>() != null) //Only run if it exists
        {
            if (FindObjectOfType<MenuManager>().tempState != 0)
            {
                FindObjectOfType<MenuManager>().enabled = true;
            }
        }
        
    }
}
