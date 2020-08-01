using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraControl : MonoBehaviour {

    // Use this for initialization
    public Transform player;
	
	// Update is called once per frame
	void Update () {
        transform.position = new Vector3(player.position.x, player.position.y, transform.position.z); //Continuously set the postion of the camera to the positon of the player
	}
}
