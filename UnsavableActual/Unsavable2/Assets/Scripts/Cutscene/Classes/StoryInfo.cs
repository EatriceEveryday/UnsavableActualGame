using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class StoryInfo
{
    public DialogueInfo[] dialogueInfo;
    public MoveInfo moveInfo;
    public AnimationInfo animationInfo;
    public ComponentInfo componentInfo;
    public AudioInfo audioInfo;
    public WaitInfo waitInfo;
    public CameraInfo cameraInfo;
    public SceneChangeInfo sceneChangeInfo;
    public bool dontWait;

}
