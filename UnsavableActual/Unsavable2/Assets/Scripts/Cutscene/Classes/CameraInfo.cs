using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class CameraInfo
{
    public float speed = 5f;
    public float zoom = 1f;
    public float duration;
    public Vector2 finalPosition;
    public bool playerXY;
    public Vector3 rotation;
}
