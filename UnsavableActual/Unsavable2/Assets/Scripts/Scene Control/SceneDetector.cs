using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SceneDetector : MonoBehaviour {

    public string scene;
    public Vector2 scenePosition;
    public int[] cutscenes;
    public bool multipleCutscenes;

    private PlayerControl player;
    private bool yes;
    

	// Use this for initialization
	void Start () {
        player = GameObject.Find("Player").GetComponent<PlayerControl>();
        yes = false;

    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.CompareTag("Player")) //Checks if the player is in range to talk
        {
            player.enabled = false; //Stops the user from moving when in dialogue
            yes = true;
            FindObjectOfType<SceneManagers>().assignEntrance(scenePosition);
            FindObjectOfType<SceneManagers>().FadetoLevel(scene);

            if (multipleCutscenes)
            {
                for (int i = 0; i < cutscenes.Length; i++)
                {
                    if (cutscenes[i] == FindObjectOfType<ProgressionTracker>().getCutsceneOrder())
                    {
                        FindObjectOfType<ProgressionTracker>().adjustStoryOrder(FindObjectOfType<ProgressionTracker>().getCutsceneOrder() - i - 1);
                    }
                }
            }

        }
    }

    void Update()
    {
        if (yes)
        {
            player.move(player.horizontal, player.vertical, player.speed);
        }
    }
}
