using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FinalBossCutscene : MonoBehaviour {

    public StoryInfo[] storyInfo;
    public string[] storyOrder;

    public FullDialogueInfo[] dialogue;
    public string sceneName;

    private GameObject player;
    private int state;
    
    void Start () {
        player = GameObject.Find("Player");
        state = 0;
    }

    void Update()
    {
        if (state == 2 && !FindObjectOfType<DialogueInputer>().enabled)
        {
            state = 1;
        }
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.CompareTag("Player") && state == 0)
        {
            StartCoroutine(startStory());
        }
    }

    private IEnumerator startStory()
    {
        player.GetComponent<PlayerControl>().rb2d.velocity = new Vector3(0, 0, 0); //Set the velocity to 0
        player.GetComponent<PlayerControl>().enabled = false; //Stops the user from moving when in dialogue
        player.GetComponent<Animator>().SetLayerWeight(1, 0); //Stops the walking animation
        FindObjectOfType<CameraControl>().enabled = false;
        state = 1;

        StartCoroutine(FindObjectOfType<StoryManager>().FadeBGM(new AudioInfo()));

        CameraInfo cameraInfo = new CameraInfo();

        cameraInfo.speed = 2.1f;
        cameraInfo.finalPosition.y = 5;

        StartCoroutine(FindObjectOfType<StoryManager>().Camera(cameraInfo));

        MoveInfo moveInfo = new MoveInfo();
        moveInfo.npc = player;
        moveInfo.speed = 2f;
        moveInfo.finalPosition.y = 3;
        moveInfo.playerX = true;

        StartCoroutine(FindObjectOfType<StoryManager>().MoveNPC(moveInfo, player, true));

        yield return new WaitForSeconds(3);

        AnimationInfo animationInfo = new AnimationInfo();
        animationInfo.npc = GameObject.Find("The Roman Messenger");
        animationInfo.animationState = "Front";

        StartCoroutine(FindObjectOfType<StoryManager>().RunAnimation(animationInfo));

        yield return new WaitForSeconds(1);

        if (!FindObjectOfType<ProgressionTracker>().getReset())
        {
            FindObjectOfType<StoryManager>().StartCutScene(storyOrder, storyInfo); //Runs the control function
        }
        else
        {
            StartCoroutine(randomizeDialogue());
        }


    }

    IEnumerator randomizeDialogue()
    {
        int number = Mathf.RoundToInt(Random.Range(0f, dialogue.Length - 1));

        FindObjectOfType<DialogueManager>().StartDialogue(dialogue[number].fullDialogue, true);

        state = 2;

        while (state == 2)
        {
            yield return null;
        }

        StartCoroutine(FindObjectOfType<SceneManagers>().SnapToBattle(sceneName));
    }
}
