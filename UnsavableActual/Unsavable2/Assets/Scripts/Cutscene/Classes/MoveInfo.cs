using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[System.Serializable]
public class MoveInfo {

    public GameObject npc;
    public float speed = 5f;
    public Vector2 finalPosition;
    public bool xThenY, playerX, playerY;
}
