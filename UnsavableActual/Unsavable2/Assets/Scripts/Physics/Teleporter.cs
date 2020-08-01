using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Teleporter : MonoBehaviour {

    public Vector2 destination;
    private Vector3 destination3D;
    private Transform player;
    private Animator playerAnimator;
    private Transform camerA;
    private float timer;
    private bool move;

    void Start()
    {
        player = GameObject.Find("Player").GetComponent<Transform>();
        camerA = GameObject.Find("Camera").GetComponent<Transform>();
        destination3D = new Vector3(destination.x, destination.y, -10);
        move = false;
    }

    void FixedUpdate()
    {
        if (move)
        {
            Vector3 smoothedPosition = Vector3.Lerp(camerA.position, destination3D, 0.1f); //Continuously updates a new position to smooth it out
            camerA.position = smoothedPosition;
            player.position = destination;
            timer += Time.deltaTime; //Adds to the timer

            if (timer >= 1.25) //Allows the user to move again after 1.25 seconds
            {
                move = false;
                FindObjectOfType<PlayerControl>().enabled = true;
                FindObjectOfType<CameraControl>().enabled = true;
            }
        }
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.CompareTag("Player") && FindObjectOfType<PlayerControl>().enabled == true) //When the player comes into contact
        {
            FindObjectOfType<PlayerControl>().enabled = false; //Disable camerA and player controls
            FindObjectOfType<CameraControl>().enabled = false;
            player.GetComponent<Animator>().SetLayerWeight(1, 0); //Stops the walking animation
            timer = 0; //Reset timer to start counting
            move = true; //Run the "function"
        }

    }
}
