using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerControlBoss : MonoBehaviour {

    public float maxHealth; //Stats
    private float currentHealth;
    private float mana;

    public Slider healthBar; //UI
    public Slider manaBar;

    private float speed = 6;
    private bool moveable;

    private Animator animator;
    public CameraShake mainCam;
    private AudioManager audioManager;
    public string deathDestination;
    public Vector2 scenePosition;
    public int order;

    [HideInInspector]
    public Rigidbody2D rb2d;

    [HideInInspector]
    public float horizontal, vertical;
    private bool cdown;

    void Awake()
    {
        currentHealth = maxHealth;
        healthBar.value = percentHealth();
        mana = 1;
        manaBar.value = mana;
        animator = GetComponent<Animator>();
    }

    // Use this for initialization
    void Start()
    {
        rb2d = GetComponent<Rigidbody2D>();
        audioManager = FindObjectOfType<AudioManager>();
        moveable = true;
        cdown = false;
    }

    // Update is called once per frame
    void Update()
    {

        if (moveable)
        {
            horizontal = Input.GetAxisRaw("Horizontal");
            vertical = Input.GetAxisRaw("Vertical");
        }

        if (Input.GetKeyDown(KeyCode.C))
        {
            cdown = true;
        }

        if (Input.GetKeyUp(KeyCode.C))
        {
            StartCoroutine(gracePeriod());
        }

        if (mana < 1)
        {
            mana += 0.1f * Time.deltaTime;
            if (mana > 1)
            {
                mana = 1;
            }
        }

        if (cdown && mana > 0.35f && (horizontal != 0 || vertical != 0))
        {
            StartCoroutine(dash());
        }

        move(horizontal, vertical, speed);
        manaBar.value = mana;
    }

    IEnumerator gracePeriod()
    {
        yield return new WaitForSeconds(0.1f);
        cdown = false;
    }

    public void move(float horizontal, float vertical, float speed) 
    {
        Vector2 velocity = new Vector2(horizontal * speed * Time.deltaTime, vertical * speed * Time.deltaTime);
        rb2d.MovePosition(rb2d.position + velocity);
    }

    public void dealDamage(float damage, bool shake)
    {
        currentHealth -= damage;
        healthBar.value = percentHealth();
        audioManager.Play("Hit");

        if (currentHealth <= 0)
        {
            StartCoroutine(die());
        }

        if (shake)
        {
            mainCam.Shake((damage / maxHealth) * 0.3f + 0.1f, (damage / maxHealth) * 0.8f + 0.2f); //max 0.4, 1 / min 0.1, 0.2
        }
        StartCoroutine(invunerable(1f));
    }

    private IEnumerator invunerable (float time)
    {
        transform.tag = "Untagged";
        animator.SetBool("isDashing", true);

        yield return new WaitForSeconds(time);

        animator.SetBool("isDashing", false);
        transform.tag = "Player";
    }

    private IEnumerator dash() //This is the dashing ability
    {
        bool hurt = false;

        if (transform.tag == "Untagged")
        {
            hurt = true;
        } else
        {
            transform.tag = "Untagged"; //Makes the player invunerable
            animator.SetBool("isDashing", true);
        }

        moveable = false; //Stops the player from inputting more commands
        speed = 30; //Increases the speed 
        mana -= 0.35f;

        yield return new WaitForSeconds(0.1f); //Waits for 0.1 sec for the dash

        if (!hurt)
        {
            transform.tag = "Player";
            animator.SetBool("isDashing", false);
        }

        moveable = true;
        speed = 6;
    }

    public void resetMana()
    {
        mana = 1;
    }

    private float percentHealth()
    {
        return currentHealth / maxHealth;
    }

    private IEnumerator die ()
    {
        GameObject.Find("AudioManager").GetComponent<AudioSource>().Stop();
        FindObjectOfType<AudioManager>().Stop("TRM Reset");
        FindObjectOfType<BattleManager>().die();
        FindObjectOfType<BulletHellManager>().die();
        currentHealth = 0;
        
        yield return new WaitForSeconds(3);

        FindObjectOfType<SceneManagers>().assignEntrance(scenePosition);
        FindObjectOfType<ProgressionTracker>().adjustStoryOrder(order);
        FindObjectOfType<ProgressionTracker>().adjustSceneOrder(FindObjectOfType<ProgressionTracker>().getSceneOrder() - 2);
        FindObjectOfType<SceneManagers>().FadetoLevel(deathDestination);
    }

}
