using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerControl : MonoBehaviour {

    public float speed = 5f;
    private Animator animator;

    private float hp;

    [HideInInspector]
    public Rigidbody2D rb2d;

    [HideInInspector]
    public float horizontal, vertical;

    void Awake()
    {
        hp = 100;
    }

    // Use this for initialization
    void Start () {
        animator = GetComponent<Animator>();
        rb2d = GetComponent<Rigidbody2D>();
    }

    void OnEnable()
    {
        rb2d.constraints = RigidbodyConstraints2D.FreezeRotation;
    }
	
	// Update is called once per frame
	void Update () {
        horizontal = Input.GetAxisRaw("Horizontal");
        vertical = Input.GetAxisRaw("Vertical");

        move(horizontal, vertical, speed);

        if (horizontal != 0 || vertical != 0)
        {
            AnimateMovement(horizontal, vertical);
        }
        else
        {
            animator.SetLayerWeight(1, 0);
        }
        
        if (Input.GetKeyDown(KeyCode.C))
        {
            if (GameObject.Find("Manager").GetComponent<MenuManager>().enabled == false)
            {
                FindObjectOfType<AudioManager>().Play("Menu_Navigation");
                GameObject.Find("Manager").GetComponent<MenuManager>().enabled = true;
            }
        }
	}

    public void move(float horizontal, float vertical, float speed) {
        // transform.Translate(Vector2.right * speed * horizontal * Time.deltaTime); Other ways to move the character
        // transform.Translate(Vector2.up * speed * vertical * Time.deltaTime);
        // rb2d.velocity = new Vector2 (horizontal * speed, vertical * speed);

        Vector2 velocity = new Vector2(horizontal * speed * Time.deltaTime, vertical * speed * Time.deltaTime);
        rb2d.MovePosition(rb2d.position + velocity);

    }

    public void AnimateMovement(float horizontal, float vertical)
    {
        animator.SetLayerWeight(1, 1);
        animator.SetFloat("x", horizontal);
        animator.SetFloat("y", vertical);
    }

    public void dealDamage(float damage)
    {
        this.hp -= damage;
    }
}

