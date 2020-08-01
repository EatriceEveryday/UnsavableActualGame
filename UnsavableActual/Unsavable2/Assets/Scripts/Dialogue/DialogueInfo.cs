using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[System.Serializable]
public class DialogueInfo {

    public Sprite face;
    public Animator animator;
    [TextArea(3, 10)]
    public string sentence;
    public string direction;
    public bool dontWait;
}
