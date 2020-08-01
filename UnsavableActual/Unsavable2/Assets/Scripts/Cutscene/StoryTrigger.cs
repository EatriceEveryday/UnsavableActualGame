using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class StoryTrigger : MonoBehaviour {

    public bool triggerOnSceneLoad;
    public StoryInfo[] storyInfo;
    public string[] storyOrder;
    public int cutsceneInt;

    private GameObject player;
    private int state;

    void Start ()
    {
        player = GameObject.Find("Player");
        state = 0;
        StartCoroutine(checkLate());
    }

    private IEnumerator checkLate() //This is done to give every other gameobject initialize
    {
        yield return new WaitForSeconds(0.2f);

        if (triggerOnSceneLoad)
        {
            if (state == 0 && FindObjectOfType<ProgressionTracker>().isInStoryOrder(cutsceneInt))
            {
                startStory();
            }
        }

    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (!triggerOnSceneLoad)
        {
            if (other.gameObject.CompareTag("Player") && state == 0 && FindObjectOfType<ProgressionTracker>().isInStoryOrder(cutsceneInt)) //Checks if the player is in range to talk
            {
                startStory();
            }
        }
    }

    private void startStory()
    {
        player.GetComponent<PlayerControl>().rb2d.velocity = new Vector3(0, 0, 0); //Set the velocity to 0
        player.GetComponent<PlayerControl>().enabled = false; //Stops the user from moving when in dialogue
        player.GetComponent<Animator>().SetLayerWeight(1, 0); //Stops the walking animation
        FindObjectOfType<CameraControl>().enabled = false;
        state = 1;

        FindObjectOfType<StoryManager>().StartCutScene(storyOrder, storyInfo); //Runs the control function
    }

}

