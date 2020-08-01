using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpriteOrder : MonoBehaviour {

    private Transform playerPosition;
    private Transform spritePosition;
    private Renderer spriteLayer;
    private float playerY, spriteY;

    public int highLayer, lowLayer;

	// Use this for initialization
	void Start () {
        spriteLayer = GetComponent<Renderer>();
        playerPosition = GameObject.Find("Player").GetComponent<Transform>();
        spritePosition = GetComponent<Transform>();
    }
	
	// Update is called once per frame
	void Update () {

        playerY = playerPosition.position.y;
        spriteY = spritePosition.position.y;

        if (playerY > spriteY)
        {
            spriteLayer.sortingOrder = highLayer;
        }
        else
        {
            spriteLayer.sortingOrder = lowLayer;
        }

	}
}
