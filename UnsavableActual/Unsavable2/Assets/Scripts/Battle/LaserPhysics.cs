using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LaserPhysics : MonoBehaviour {

    public float delay, duration, damage;
    public bool followPlayer;
    private float width;

    private Renderer rend;
    private Collider2D collider2d;

	// Use this for initialization
	void Start () {

        width = transform.localScale.y;
        rend = GetComponent<Renderer>();
        collider2d = GetComponent<Collider2D>();

        transform.localScale = new Vector3(transform.localScale.x, 0f, transform.localScale.z);
        collider2d.enabled = false;

        StartCoroutine(runBeam());
	}

    IEnumerator runBeam()
    {
        yield return new WaitForSeconds(delay - 1); //Wait until 1 second b4 the delay

        if (followPlayer)
        {
            transform.position = GameObject.Find("PlayerBattle").GetComponent<Transform>().position;
        }

        showLaser(); //show a laser

        yield return new WaitForSeconds(1); //after one second fire!!

        transform.localScale = new Vector3(transform.localScale.x, 0, transform.localScale.z); //Reset the beam to 0 width and white colour
        rend.material.SetColor("_Color", new Color(1, 1, 1, 1));

        float scaleIncreasePerFrame = width*15; //Set the change in width

        GetComponent<AudioSource>().Play(); //Play the WOOOSSHH sound
        FindObjectOfType<CameraShake>().Shake(0.15f + width * 3, duration); //Shake the camera
        collider2d.enabled = true; //Allow the player to get hit

        while (transform.localScale.y < width) //Increase the width from 0 to the original width
        {
            transform.localScale += new Vector3(0, scaleIncreasePerFrame*Time.deltaTime, 0);
            yield return null;
        }

        transform.localScale = new Vector3(transform.localScale.x, width, transform.localScale.z); //Just in case it goes over, set it back to the width

        float timer = 0;

        while (timer < duration) //Pulsate for a duration by adding sin(duration) to the width
        {
            transform.localScale = new Vector3(transform.localScale.x, width + width*0.1f*Mathf.Sin(timer*25), transform.localScale.z);
            timer += Time.deltaTime;
            yield return null;
        }

        collider2d.enabled = false; //Stop the player from getting hit

        float transparency = 1;

        while (transform.localScale.y > 0) //decrease the beam width at a slower pace
        {
            transform.localScale -= new Vector3(0, scaleIncreasePerFrame*0.15f * Time.deltaTime, 0);

            transparency -= Time.deltaTime*2; //Decrease the transparency over time

            rend.material.SetColor("_Color", new Color(1, 1, 1, transparency));
            yield return null;
        }

        transform.localScale = new Vector3(transform.localScale.x, 0, transform.localScale.z); //Just in case it goes over, reset it to 0;
    }

    private void showLaser()
    {
        transform.localScale = new Vector3(transform.localScale.x, 0.15f*width, transform.localScale.z); //Set the width to 15% of the width
        rend.material.SetColor("_Color", new Color(1, 0 , 0, 0.3f)); //set the colour to translucent red
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.CompareTag("Player")) //If the player hits the laser, deal damage but dont shake (camera already shaking from the laser)
        {
            PlayerControlBoss player = other.GetComponent<PlayerControlBoss>();
            player.dealDamage(damage, false);
        }
    }

}
