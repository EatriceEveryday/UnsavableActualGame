using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MenuManager : MonoBehaviour {

    public Text charactername, title;
    private string[] namelist, titlelist;
    public DialogueInfo[] hint;

    public Animator Profileanimator, Menuanimator;
    private bool isOpen;
    private int state;

    [HideInInspector]
    public DialogueInfo[] dialogueInfo;

    [HideInInspector]
    public int tempState;

    private DialogueInputer dialogueInputer;
    private GameObject player;

	// Use this for initialization
    void Awake()
    {
        namelist = new string[] { "Heshi", "Jeffrey", "Nathaniel", "Jimmy", "Brandon", "Azhar", "Benny", "Conan", "Alex", "Peter", "Zexi", "Jason", "Bing Yao", "Raymond", "Ameya", "Kevin", "Leo", "Aaron", "Angelina", "Ting", "Norman", "Ryan", "Kathy", "Norman", "Waiky", "Atharson", "Kelly", "Willson", "Jacob", "Alan"}; //Create a list of posible names and tiles to be displayed
        titlelist = new string[] { "Master of Deception", "The Awakened", "The Disappointment", "Voice of God", "Deciever of the Teachers", "The Incarnation of Trash", "The Two Feet Troll", "The God Hand", "Big Brain Main", "ACCI Scholar", "President of Computer Club", "The Best Oboe Player in ACCI", "The Alpha", "The Memelord"};
        player = GameObject.Find("Player");
    }
    
	void Start () {
        dialogueInputer = GameObject.Find("DialogueInputer").GetComponent<DialogueInputer>();
    }

    public void TriggerDialogue() //Function to run another function in the dialogue manager
    {
        FindObjectOfType<DialogueManager>().StartDialogue(dialogueInfo, false);
    }

    private void Randomize (string[] array, Text textUI) //function to show a random name + title combo
    {
        int number = Mathf.RoundToInt(Random.Range(0f, array.Length - 1));
        textUI.text = array[number];
    }

    void OnEnable()
    {
        player.GetComponent<PlayerControl>().rb2d.velocity = new Vector3(0, 0, 0); //Set the velocity to 0
        player.GetComponent<PlayerControl>().enabled = false; //Stops the user from moving when in dialogue
        player.GetComponent<Animator>().SetLayerWeight(1, 0); //Stops the walking animation

        if (tempState == 0)
        {
            tempState = 1;
            Randomize(namelist, charactername); //Show a random profile
            Randomize(titlelist, title);
        }

        state = tempState; //Set the state to 1(highlights save)
        isOpen = true; //Set the boolean to true
    }

    // Update is called once per frame
    void Update () {

        Profileanimator.SetBool("isOpen", isOpen);
        Menuanimator.SetInteger("state", state);

        if (Input.GetKeyDown(KeyCode.DownArrow) && isOpen && state < 4) //Increases the state
        {
            FindObjectOfType<AudioManager>().Play("Menu_Navigation");
            state = state + 1;
        }

        if (Input.GetKeyDown(KeyCode.UpArrow) && isOpen && state > 1) //Decreases the state
        {
            FindObjectOfType<AudioManager>().Play("Menu_Navigation");
            state = state - 1;
        }

        if (Input.GetKeyDown(KeyCode.Z) && dialogueInputer.enabled == false)
        {
            tempState = state; //Temporary state

            switch (tempState)
            {
                case 1:
                    FindObjectOfType<AudioManager>().Play("Menu_Select");
                    dialogueInfo = new DialogueInfo[4];

                    FindObjectOfType<ProgressionTracker>().SaveProgress();

                    for (int i = 0; i < 4; i++)
                    {
                        dialogueInfo[i] = new DialogueInfo();
                    }

                    dialogueInfo[0].sentence = "Saved."; //If the button is save
                    dialogueInfo[1].sentence = "Probably should warn you that because I'm a terrible programmer, ";
                    dialogueInfo[2].sentence = "{this only saves the scene you're in.";
                    dialogueInfo[3].sentence = "So if you're halfway through a cutscene or a puzzle, it's getting reset to the beginning of the scene.";
                    TriggerDialogue();
                    break;
                case 2:
                    FindObjectOfType<HintTimer>().stopTimer();
                    FindObjectOfType<AudioManager>().Play("Menu_Select");
                    dialogueInfo = hint; //If the button is cell
                    TriggerDialogue();
                    break;
                case 3:
                    FindObjectOfType<AudioManager>().Play("Menu_Select");
                    GameObject.Find("Manager").GetComponent<RUsureManager>().enabled = true; //If the button is reset, turn this off and turn the RUsure manager on
                    break;
                case 4:
                    FindObjectOfType<AudioManager>().Play("Menu_Select");
                    Application.Quit();
                    break;
            }

            this.enabled = false;
        }

        if (Input.GetKeyDown(KeyCode.C))
        {
            tempState = 0;
            this.enabled = false;
        }
    }

    void OnDisable()
    {
        state = 0; //Set the state to 0 (closed menu)
        isOpen = false; //set the boolean to false

        Profileanimator.SetBool("isOpen", isOpen);
        Menuanimator.SetInteger("state", state);

        if (tempState == 0)
        {
            player.GetComponent<PlayerControl>().enabled = true; //Allows the user to move again
        }
    }
}
