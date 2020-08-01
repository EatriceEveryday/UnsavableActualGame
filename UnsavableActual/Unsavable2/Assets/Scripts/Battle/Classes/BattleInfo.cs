using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class BattleInfo{

    public BulletPatternInfo bulletPatternInfo;
    public DialogueInfo[] dialogueInfo;
    public AttackInfo attackInfo;
    public AudioInfo audioInfo;
    public float duration;
    public SceneChangeInfo sceneChangeInfo;
}
