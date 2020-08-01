using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BulletHellManager : MonoBehaviour {

    public Animator animator;
    public Slider timer;
    private AudioManager audioManager;

    private GameObject bulletPattern;
    private Text combinationUI;
    public Canvas canvas;
    private PlayerControlBoss player;
    private float time;

    private string buttonToCheck;
    private bool check;
    private bool correctButton;

    Coroutine countDown, runningAttack;

    [HideInInspector]
    public bool isRunning = false;

    void Start()
    {
        player = GameObject.Find("PlayerBattle").GetComponent<PlayerControlBoss>();
        audioManager = GameObject.Find("AudioManager").GetComponent<AudioManager>();
    }

    void Update()
    {
        /** This check is called by runAttack()
         * 
         *  returns true if the correct button is pressed
         *  return false if its anything but certain exceptions
         */

        if (check)
        {
            if (Input.GetButtonDown(buttonToCheck))
            {
                correctButton = true;
                check = false;
            }

            else if (Input.anyKeyDown && !Input.GetButtonDown("Space") && !Input.GetButtonDown("Shift") && !Input.GetButtonDown("Caps Lock") && !Input.GetButtonDown("Backspace") && !Input.GetButton("MouseClick"))  
            {
                correctButton = false;
                check = false;
            }
        }

    }

    /** Name: spawnBullets (called by BattleManager)
     *  
     *  Function:   1. Copy and paste an existing bullet pattern
     *              2. Turn on all the bullet's physics to let then move
     *              3. Wait for a certain duration
     *              4. Delete all of the bullets afterwards
     */

    public IEnumerator spawnBullets(GameObject bulletPattern, float duration)
    {
        animator.SetBool("bulletOn", true); //smoothly transitiions into the battle UI

        GameObject.Find("PlayerBattle").GetComponent<Transform>().position = new Vector3(0, -2, 0); //resets the player's position
        FindObjectOfType<PlayerControlBoss>().resetMana();

        this.bulletPattern = Instantiate(bulletPattern, new Vector3(0, 0, 0), new Quaternion(0, 0, 0, 0)); //copies and pastes the bullet pattern
        
        for (int i = 0; i < this.bulletPattern.transform.childCount; i++) //Goes through each bullet and turns them on
        {
            Transform bullet = this.bulletPattern.transform.GetChild(i);
            if (bullet.GetComponent<BulletPhysics>() == null)
            {
                bullet.GetComponent<LaserPhysics>().enabled = true;
            } else
            {
                bullet.GetComponent<BulletPhysics>().enabled = true;
            }

        }

        yield return new WaitForSeconds(duration); //Wait for a set duration

        animator.SetBool("bulletOn", false); //smoothly transistions out of the battle UI

        Destroy(this.bulletPattern); //destroy the pattern
    }

    /** Name: runAttack (called by startAttack())
     *  
     *  Function:   1. Creates a string array containing each index in the combination (spaces included)
     *              2. Originally sets the turquiose colour tags at the front of the combination
     *              3. Iterates through each index to:
     *                  - Save the index
     *                  - Skip all spaces
     *                  - Turn on check in update
     *                  - If the correct button was pressed, advance the greed colour tags
     *                  - If the wrong button was pressed, cover the entire text with the tags and change it to red, deal damage to the player and end the coroutine 
     *              4. If the player goes through the loop without hitting a single wrong button, change the text to green
     */

    public IEnumerator runAttack(float damage, bool fast)
    {
        string[] combination = new string[combinationUI.text.ToString().Length]; //Create an string array based on the length of the combination (.Split() removes the spaces)

        for (int i = 0; i < combinationUI.text.ToString().Length; i++)
        {
            combination[i] = combinationUI.text.ToString()[i].ToString().ToUpper(); //iterates through the indexes to add it to the string array (buttons only recognize uppercases)
        }

        combinationUI.text = "<color=#00fffa></color>" + combinationUI.text; //Initializes the color change at the beginning, to be moved around later

        for (int i = 0; i < combination.Length; i++) //iterates through each button to check
        {
            if (!(combination[i] == " ")) //Skips the spaces
            {
                buttonToCheck = combination[i]; //Sets the button to check equal to the current index
                check = true; //Allows the manager to check

                while (check) //Waits till it's over
                {
                    yield return null;
                }

                if (correctButton) // if the correct button was pressed
                {
                    combinationUI.text = combinationUI.text.Replace("</color>", ""); //Deletes the current end of the color tag
                    combinationUI.text = combinationUI.text.ToString().Substring(0, 16 + i) + "</color>" + combinationUI.text.ToString().Substring(16 + i); //Extends the color tag to where the current button is

                    if (i != combination.Length - 1)
                    {
                        audioManager.Play("Menu_Navigation");
                    }
                    
                    /**16 is the length of the first half of the colour tag + 1
                     * replaces the entire text by readding the text from the beginning up to the index inclusive (thats why there's + 1)
                     * adds the end of the colour tag
                     * adds the rest of the text behind it, unaffected by the colour tags
                     */
                }

                else
                {
                    StopCoroutine(countDown); //Stops the timer from finishing (or else it's going to do damage regardless)
                    CancelInvoke("showTime"); //Stop the timer from updating and stop runAttack() from continuing 

                    combinationUI.text = combinationUI.text.Replace("<color=#00fffa>", "<color=#ff0000>"); //Changes the color to red
                    combinationUI.text = combinationUI.text.Replace("</color>", ""); //Deletes the current end of the color tag
                    combinationUI.text += "</color>"; //Extends the color change point to the end of the combination

                    animator.SetBool("damaged", true); //Quickly show the health bar
                    player.dealDamage(damage, true); //Deal damage to the player for failing

                    if (!fast)
                    {
                        yield return new WaitForSeconds(1); //Wait for one second for the player to realize his/her mistake
                    } else
                    {
                        yield return new WaitForSeconds(0.5f);
                    }

                    Destroy(combinationUI); //Destroy the combination clone
                    animator.SetBool("damaged", false); //Smoothly hides the health bar
                    isRunning = false; //Signal to the battle manager that the coroutine is no longer running
                    yield break; //Ends the coroutine
                }
            }
        }

        //If the player gets to this point, that means that the player has succesffuly pressed all the correct buttons

        StopCoroutine(countDown); //Stops the timer from finishing (or else it's going to do damage regardless)
        CancelInvoke("showTime"); //Stop the timer from updating and stop runAttack() from continuing 
        audioManager.Play("Menu_Select");

        combinationUI.text = combinationUI.text.Replace("<color=#00fffa>", "<color=#2afc38>"); //Change the colour from turquiose to green
        animator.SetTrigger("timerOn"); //Trigger it again to hide the timer

        if (!fast)
        {
            yield return new WaitForSeconds(1); //Wait for one second for the player to bask in his/her success
        } else
        {
            yield return new WaitForSeconds(0.2f);
        }

        Destroy(combinationUI); //Destroy the combination clone
        isRunning = false; //Signal to the battle manager that the coroutine is finished

    }

    /** Name: startAttack (called by BattleManager)
     *  
     *  Function:   1. Resets all variables to their initial values
     *              2. Copies and pastes the combination and shows it
     *              3. Runs both the timerCountdown and runAttack at the same time
     *              P.S. the timerCountdown and runAttack both have functions to stop the other from finishing if they finish first
     */

    public IEnumerator startAttack (Text combinationUIOriginal, float damage, float time, bool fast, bool endFast)
    {
        isRunning = true; //Signal the BattleManager that the function is starting and will continue to run

        this.time = time; 
        timer.value = time; //Resets the time to full
        animator.SetTrigger("timerOn"); //Shows the timer

        if (!fast)
        {
            yield return new WaitForSeconds(1); //Wait one second for the player to prepare (player may still have the directional keys down)
        } else
        {
            yield return new WaitForSeconds(0.2f);
        }

        combinationUI = Instantiate(combinationUIOriginal, new Vector3(0, 0, 0), new Quaternion(0, 0, 0, 0)) as Text; //Creates a copy of the combination text object
        combinationUI.transform.SetParent(canvas.transform.GetChild(0).transform, false); //Sets it as a child of the UI 

        countDown = StartCoroutine(timerCountdown(damage, endFast)); //run both timerCountdwon and runAttack simutanously and saves their respective coroutines to be stopped later
        runningAttack = StartCoroutine(runAttack(damage, endFast));
    }

    /** Name: timerCountdown (called by startAttack)
     *  
     *  Function:   1. Start updating the timer to represent how much time is left
     *              2. Wait until the timer hits 0
     *              3. Stop the timer from updating and stop runAttack() from continuing
     *              4. Cover the entire text with the tags and change it to red and deal damage to the player
     */

    public IEnumerator timerCountdown(float damage, bool fast)
    {
        InvokeRepeating("showTime", 0, 0.01f); //Repeatedly run the showTime function every 0.01 seconds to update time

        yield return new WaitForSeconds(time); //Wait until the timer hits 0

        CancelInvoke("showTime"); //Stop the timer from updating and stop runAttack() from continuing 
        StopCoroutine(runningAttack);

        combinationUI.text = combinationUI.text.Replace("<color=#00fffa>", "<color=#ff0000>"); //Changes the color to red
        combinationUI.text = combinationUI.text.Replace("</color>", "");
        combinationUI.text += "</color>"; //Extends the color change point to the end of the combination

        animator.SetBool("damaged", true); //Quickly show the health bar and deal damage to the player
        player.dealDamage(damage, true);

        if (!fast)
        {
            yield return new WaitForSeconds(1); //Wait for one second for the player to realize his/her mistake
        }
        else
        {
            yield return new WaitForSeconds(0.5f);
        }

        Destroy(combinationUI); //Destroy the text object and hide the UI to prepare for the next event
        animator.SetBool("damaged", false);
        isRunning = false; //Signal to BattleManager that the coroutine is finished
    }

    public void showTime()
    {
        timer.value -= 0.01f/time;
    }

    public void die()
    {
        if (this.bulletPattern != null)
        {
            Destroy(this.bulletPattern); //destroy the pattern
        }

        if (this.combinationUI != null)
        {
            Destroy(this.combinationUI);
        }

        StopAllCoroutines();

        animator.SetLayerWeight(1, 1);
    }

}
