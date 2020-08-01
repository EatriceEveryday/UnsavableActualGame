using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BoulderPhysics : MonoBehaviour {

    private Rigidbody2D parentRigidBody;
    private Animator playerAnimator;
    private static bool isMoving; //This bool is shared among all the sides. This means that only one boulder can be moved at a time.
    private ChairMechanics parentBody; //0 = not moving, 1 = N, 2 = E, 3 = S, 4 = W
    private bool isInRange;
    public string side;
    public bool slide;

	// Use this for initialization
	void Start () {

        parentRigidBody = this.transform.parent.GetComponent<Rigidbody2D>();
        isInRange = false;
        playerAnimator = GameObject.Find("Player").GetComponent<Animator>();
        isMoving = false;
        parentBody = this.transform.parent.GetComponent<ChairMechanics>();

    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            isInRange = true;

            if (isMoving && slide) //Checks if the player is trying to stop the chair using their body. This is done because of a particular bug the chair could push the player into the wall. (0.7 is the width of the chair and 0.25 is half the width of the player)
            {
                switch (side) //Depending on which side and whether the chair is pushing into them, teleport them behind the chair.
                {
                    case "Top": //If the player interacts from the top

                        if (parentBody.compareDirection(1))
                        {
                            other.gameObject.GetComponent<Rigidbody2D>().position -= new Vector2(0, 0.95f);
                        }

                        break;

                    case "Bottom": //If the player interacts from the bottom

                        if (parentBody.compareDirection(3))
                        {
                            other.gameObject.GetComponent<Rigidbody2D>().position += new Vector2(0, 0.95f); ;
                        }

                        break;

                    case "Left": //If the player interacts from the left side

                        if (parentBody.compareDirection(4))
                        {
                            other.gameObject.GetComponent<Rigidbody2D>().position += new Vector2(0.95f, 0);
                        }

                        break;

                    case "Right": //If the player interacts from the right side

                        if (parentBody.compareDirection(2))
                        {
                            other.gameObject.GetComponent<Rigidbody2D>().position -= new Vector2(0.95f, 0);
                        }

                        break;
                }

            }

        } else if (isMoving && slide)
        {
            switch (side) //Depending on which side and whether the chair is pushing into them, teleport them behind the chair.
            {
                case "Top": //If the player interacts from the top

                    if (parentBody.compareDirection(1))
                    {
                        StopSliding(other);
                    }

                    break;

                case "Bottom": //If the player interacts from the bottom

                    if (parentBody.compareDirection(3))
                    {
                        StopSliding(other);
                    }

                    break;

                case "Left": //If the player interacts from the left side

                    if (parentBody.compareDirection(4))
                    {
                        StopSliding(other);
                    }

                    break;

                case "Right": //If the player interacts from the right side

                    if (parentBody.compareDirection(2))
                    {
                        StopSliding(other);
                    }

                    break;
            }
        }
    }

    private void StopSliding(Collider2D other)
    {
        if (other.gameObject.CompareTag("Breakable"))
        {
            other.GetComponent<Animator>().SetTrigger("Break");
            other.enabled = false;
        }

        if (other.gameObject.CompareTag("Wall"))
        {
            switch (parentBody.getDirection())
            {
                case 1: parentRigidBody.position += new Vector2(0, -0.05f); break;
                case 2: parentRigidBody.position += new Vector2(-0.1f, 0); break;
                case 3: parentRigidBody.position += new Vector2(0, 0.05f); break;
                case 4: parentRigidBody.position += new Vector2(0.1f, 0); break;
            }
        }

        parentRigidBody.position = new Vector2(Mathf.Round(parentRigidBody.position.x * 10) / 10, Mathf.Round(parentRigidBody.position.y * 10) / 10); //Rounds the position to the nearest tenth
        parentRigidBody.constraints = RigidbodyConstraints2D.FreezeAll; //Stops the NPC
        parentBody.setDirection(0);
        isMoving = false;
    }

    void OnTriggerExit2D(Collider2D other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            isInRange = false;
        }
    }

    // Update is called once per frame
    void Update () {

        if (Input.GetKeyDown(KeyCode.Z) && isInRange && !isMoving) //Moves the bolder as long as the player is inRange and a boulder is not already moving
        {
            MoveBoulder();
        }

    }

    private void MoveBoulder()
    {
        int horizontal = 0;
        int vertical = 0;

        switch (side)
        {
            case "Top": //If the player interacts from the top

                if ((playerAnimator.GetCurrentAnimatorClipInfo(0))[0].clip.name == "Player Idle Front") //And the player is facing the boulder
                {
                    vertical = -1; //Set vertical so that it moves down
                    parentBody.setDirection(3);
                }

                break;

            case "Bottom": //If the player interacts from the bottom

                if ((playerAnimator.GetCurrentAnimatorClipInfo(0))[0].clip.name == "Player Idle Back") //And the player is facing the boulder
                {
                    vertical = 1; //Set vertical so that it moves up
                    parentBody.setDirection(1);
                }

                break;

            case "Left": //If the player interacts from the left side

                if ((playerAnimator.GetCurrentAnimatorClipInfo(0))[0].clip.name == "Player Idle Right") //And the player is facing the boulder
                {
                    horizontal = 1; //Set horizontal so that it moves to the right
                    parentBody.setDirection(2);
                }

                break;

            case "Right": //If the player interacts from the right side

                if ((playerAnimator.GetCurrentAnimatorClipInfo(0))[0].clip.name == "Player Idle Left") //And the player is facing the boulder
                {
                    horizontal = -1; //Set horizontal so that it moves to the left
                    parentBody.setDirection(4);
                }

                break;
        }

        if (horizontal != 0 || vertical != 0) //If the player wasn't facing the right way, nothing will happen
        {
            if (slide) //Check the boolean to see if the boulder should act like an ice cube instead
            {
                Slide(horizontal, vertical);
            }
            else
            {
                StartCoroutine(Move(horizontal, vertical));
            }
        }
    }

    IEnumerator Move(int horizontal, int vertical)
    {
        isMoving = true;
        Vector2 deltaDistance = new Vector2(parentRigidBody.position.x + horizontal*0.7f, parentRigidBody.position.y + vertical * 0.7f); //Sets the new position based on the horizontal and verticals. If its 0, the block won't move
        parentRigidBody.constraints = RigidbodyConstraints2D.FreezeRotation; //Allows the NPC to be moved
        float timer = 0;

        if (horizontal + vertical > 0)
        {
            if (horizontal > 0)
            {
                while (parentRigidBody.position.x < deltaDistance.x && timer < 1)
                {
                    parentRigidBody.MovePosition(parentRigidBody.position + new Vector2(1f * Time.deltaTime, 0)); //Moves the boulder a bit each frame
                    timer += Time.deltaTime;
                    yield return null;
                }

            }else
            {
                while (parentRigidBody.position.y < deltaDistance.y && timer < 1)
                {
                    parentRigidBody.MovePosition(parentRigidBody.position + new Vector2(0, 1f * Time.deltaTime)); //Moves the boulder a bit each frame
                    timer += Time.deltaTime;
                    yield return null;
                }
            }
        } else
        {
            if (horizontal < 0)
            {
                while (parentRigidBody.position.x > deltaDistance.x && timer < 1)
                {
                    parentRigidBody.MovePosition(parentRigidBody.position - new Vector2(1f * Time.deltaTime, 0)); //Moves the boulder a bit each frame
                    timer += Time.deltaTime;
                    yield return null;
                }

            }
            else
            {
                while (parentRigidBody.position.y > deltaDistance.y && timer < 1)
                {
                    parentRigidBody.MovePosition(parentRigidBody.position - new Vector2(0, 1f * Time.deltaTime)); //Moves the boulder a bit each frame
                    timer += Time.deltaTime;
                    yield return null;
                }
            }
        }

        if (timer < 1)
        {
            parentRigidBody.position = deltaDistance;
        } else
        {
            parentRigidBody.position = new Vector2(Mathf.Round(parentRigidBody.position.x * 10) / 10, Mathf.Round(parentRigidBody.position.y * 10) / 10); //Rounds the position to the nearest tenth
        }

        parentRigidBody.constraints = RigidbodyConstraints2D.FreezeAll; //Stops the block from moving
        parentBody.setDirection(0);
        isMoving = false;
    }

    public void Slide(int horizontal, int vertical)
    {
        parentRigidBody.constraints = RigidbodyConstraints2D.FreezeRotation; //Allows the NPC to be moved
        parentRigidBody.velocity = new Vector2(horizontal * 1.5f, vertical * 1.5f); //Sets the velocity to move in a certain direction based on the horizontal and vertical
        isMoving = true;
    }

}
