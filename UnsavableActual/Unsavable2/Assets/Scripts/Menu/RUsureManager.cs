using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class RUsureManager : MonoBehaviour {

    public Animator animator;

    private int state;
    private MenuManager menu;
    private bool selected;

	// Use this for initialization
	void Start () {
        menu = GameObject.Find("Manager").GetComponent<MenuManager>();
	}

    void OnEnable()
    {
        animator.SetInteger("state", 1); //Open the RUsure box
        state = 1;
        selected = false;
    }

	// Update is called once per frame
	void Update () { 

        if (Input.GetKeyDown(KeyCode.LeftArrow) && state == 2) //Allows the user to highlight yes
        {
            FindObjectOfType<AudioManager>().Play("Menu_Navigation");
            state = 1;
            animator.SetInteger("state", 1);
        }

        if (Input.GetKeyDown(KeyCode.RightArrow) && state == 1) //Or no
        {
            FindObjectOfType<AudioManager>().Play("Menu_Navigation");
            state = 2;
            animator.SetInteger("state", 2);
        }

        if (!selected)
        {
            if (Input.GetKeyDown(KeyCode.Z)) //When something is selected
            {
                switch (state)
                {
                    case 1:
                        selected = true;
                        FindObjectOfType<ProgressionTracker>().Reset();

                        break; //Reload the scene
                    case 2:
                        this.enabled = false;//turn the script off
                        break;
                }
            }
        }

    }

    void OnDisable()
    {
        if (menu.tempState != 0) //turn the menu back on if its off
        {
            menu.enabled = true;
        }
        if (animator != null)
        {
            animator.SetInteger("state", 0);//close the box
        }
    }

}
