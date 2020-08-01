using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class StoryManager : MonoBehaviour
{
    private DialogueInfo[] dialogueInfo;
    private int state;

    private DialogueInputer dialogueInputer;
    private GameObject player;
    private Coroutine bgmFade;
    private Coroutine cameraZoom;
    private Coroutine cameraMove;

    // Use this for initialization
    void Start()
    {
        state = 0;
        dialogueInputer = GameObject.Find("DialogueInputer").GetComponent<DialogueInputer>();
        player = GameObject.Find("Player");
    }

    // Update is called once per frame
    void Update()
    {
        if (state == 1 && dialogueInputer.enabled == false) //if the dialogue need to be triggered
        {
            TriggerDialogue();
            state = 2;
        }
        if (state == 2 && dialogueInputer.enabled == false)
        {
            state = 0;
        }
    }

    // Start

    public void StartCutScene (string[] storyOrder, StoryInfo[] storyInfo)
    {
        StartCoroutine(Control(storyOrder, storyInfo));
    }

    //////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
    //Dialogue

    public void TriggerDialogue() //Function to run another function in the dialogue manager
    {
        FindObjectOfType<DialogueManager>().StartDialogue(dialogueInfo, true);
    }

    //////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
    //Moving

    public IEnumerator MoveNPC(MoveInfo moveInfo, GameObject player, bool dontWait)
    {
        Animator npcAnimator = moveInfo.npc.GetComponent<Animator>();
        Transform npcTransform = moveInfo.npc.GetComponent<Transform>();
        Rigidbody2D npcRigidBody = moveInfo.npc.GetComponent<Rigidbody2D>();

        Vector2 npcOffset = moveInfo.npc.GetComponent<CharacterInfo>().offset;
        Vector2 finalPosition = moveInfo.finalPosition;

        if (moveInfo.playerX)
        {
            finalPosition.x = player.GetComponent<Transform>().position.x;
        }

        if (moveInfo.playerY)
        {
            finalPosition.y = player.GetComponent<Transform>().position.y;
        }

        float horizontal = 0;
        float vertical = 0;

        if (finalPosition.x != npcTransform.position.x)
        {
            horizontal = (finalPosition.x - npcTransform.position.x) / (Mathf.Abs(finalPosition.x - npcTransform.position.x)); //Determines the direction the NPC needs to go
        }
        if (finalPosition.y != npcTransform.position.y)
        {
            vertical = (finalPosition.y - npcTransform.position.y) / (Mathf.Abs(finalPosition.y - npcTransform.position.y));
        }

        bool xThenY = moveInfo.xThenY;

        finalPosition += npcOffset;

        npcRigidBody.constraints = RigidbodyConstraints2D.FreezeRotation; //Allows the NPC to be moved

        npcAnimator.SetFloat("x", 0);
        npcAnimator.SetFloat("y", 0);

        npcAnimator.SetLayerWeight(1, 1); //Animates the npc

        for (int i = 0; i < 2; i++) //Runs once for the x-axis and once for the y-axis
        {

            if (xThenY) //If I specified that I want the x-axis to be done first
            {
                npcAnimator.SetFloat("x", horizontal); //Animate moving

                if (horizontal > 0)
                {
                    while (npcTransform.position.x < finalPosition.x)
                    {
                        Vector3 newPosition = new Vector3(Time.deltaTime * moveInfo.speed, 0, 0);
                        npcRigidBody.MovePosition(npcTransform.position + newPosition);
                        yield return null;
                    }
                }

                else if (horizontal < 0)
                {
                    while (npcTransform.position.x > finalPosition.x)
                    {
                        Vector3 newPosition = new Vector3(Time.deltaTime * moveInfo.speed, 0, 0);
                        npcRigidBody.MovePosition(npcTransform.position - newPosition);
                        yield return null;
                    }
                }

                npcRigidBody.position = new Vector2(finalPosition.x, npcTransform.position.y);
                npcAnimator.SetFloat("x", 0);
            }
            else
            {
                npcAnimator.SetFloat("y", vertical); //Animate moving

                if (vertical > 0)
                {
                    while (npcTransform.position.y < finalPosition.y)
                    {
                        Vector3 newPosition = new Vector3(0, Time.deltaTime * moveInfo.speed, 0);
                        npcRigidBody.MovePosition(npcTransform.position + newPosition);
                        yield return null;
                    }
                }

                else if (vertical < 0)
                {
                    while (npcTransform.position.y > finalPosition.y)
                    {
                        Vector3 newPosition = new Vector3(0, Time.deltaTime * moveInfo.speed, 0);
                        npcRigidBody.MovePosition(npcTransform.position - newPosition);
                        yield return null;
                    }
                }

                npcRigidBody.position = new Vector2(npcTransform.position.x, finalPosition.y);
                npcAnimator.SetFloat("y", 0);
            }

            xThenY = !xThenY; //Flips the boolean

        }
        npcAnimator.SetLayerWeight(1, 0);
        npcRigidBody.constraints = RigidbodyConstraints2D.FreezeAll; //Make the npc unmovable again

        if (state == 3 && !dontWait)
        {
            state = 0;
        }
    }

    /////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
    //Animation

    public IEnumerator RunAnimation(AnimationInfo animationInfo) //Function to make the npc face the right way
    {
        Animator animator = animationInfo.npc.GetComponent<Animator>();

        animator.SetFloat("x", 0);
        animator.SetFloat("y", 0);

        switch (animationInfo.animationState)
        {
            case "Back": animator.SetLayerWeight(2, 0.9f); ; animator.SetFloat("y", 1); break;
            case "Right": animator.SetLayerWeight(2, 0.9f); animator.SetFloat("x", 1); break;
            case "Front": animator.SetLayerWeight(2, 0.9f); animator.SetFloat("y", -1); break;
            case "Left": animator.SetLayerWeight(2, 0.9f); animator.SetFloat("x", -1); break;
            case "Reset": animator.SetTrigger("Reset"); break;
            default: animator.Play(animationInfo.animationState, -1, 0); break;
        }

        yield return new WaitForSeconds(0.1f);

        animator.SetLayerWeight(2, 0);
    }

    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
    //Component

    public void ComponentActivate(ComponentInfo componentInfo)// function to turn on or off components
    {
        GameObject componentBody = componentInfo.body;
        switch (componentInfo.name)
        {
            case "DialogueTrigger": componentBody.GetComponent<DialogueTrigger>().enabled = componentInfo.on; break;
            case "StoryTrigger": componentBody.GetComponent<StoryTrigger>().enabled = componentInfo.on; break;
            case "Collider2D": componentBody.GetComponent<Collider2D>().enabled = componentInfo.on; break;
            case "Image": componentBody.GetComponent<Image>().enabled = componentInfo.on; break;
            case "DecisionMaker": componentBody.GetComponent<DecisionMaker>().enabled = componentInfo.on; break;
        }
    }

    ///////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
    //BGM

    IEnumerator PlayBGM(AudioInfo audioInfo) //Function to stop the current background music, replace it and replay it
    {

        if (bgmFade != null)
        {
            StopCoroutine(bgmFade);
        }

        AudioSource speaker = GameObject.Find("AudioManager").GetComponent<AudioSource>();
        speaker.clip = audioInfo.clip; //sets the new clip
        speaker.volume = audioInfo.volume; //sets the new volume
        speaker.loop = audioInfo.loop;
        yield return new WaitForSeconds(0.3f);

        speaker.Play(); //plays
    }

    public void StopBGM()
    {
        AudioSource speaker = GameObject.Find("AudioManager").GetComponent<AudioSource>();
        speaker.Stop();
    }

    public IEnumerator FadeBGM(AudioInfo audioInfo)
    {
        AudioSource speaker = GameObject.Find("AudioManager").GetComponent<AudioSource>();
        float volumeChange = (audioInfo.volume - speaker.volume) / 60;

        for (int i = 0; i < 60; i++)
        {
            speaker.volume += volumeChange;
            yield return null;
        }
    }

    public void SoundEffect(AudioInfo audioInfo)
    {
        FindObjectOfType<AudioManager>().Play(audioInfo.name);
    }

    ///////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
    //Wait

    IEnumerator Wait(WaitInfo waitInfo) //Function to wait
    {
        yield return new WaitForSeconds(waitInfo.duration);
        state = 0;
    }

    ///////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
    //Teleport

    public void Teleport(MoveInfo moveInfo)
    {
        Vector2 position = moveInfo.finalPosition;

        if (moveInfo.npc.GetComponent<CharacterInfo>() != null)
        {
           position += moveInfo.npc.GetComponent<CharacterInfo>().offset;
        }

        moveInfo.npc.GetComponent<Transform>().position = position;
    }

    ///////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
    //Camera

    public IEnumerator Camera(CameraInfo cameraInfo)
    {
        Transform cameraTransform = GameObject.Find("Camera").GetComponent<Transform>();
        Transform playerTransform = player.GetComponent<Transform>();
        Vector3 finalTarget;

        if (cameraInfo.playerXY)
        {
            finalTarget = new Vector3(playerTransform.position.x, playerTransform.position.y, -10);
        }
        else
        {
            finalTarget = new Vector3(cameraInfo.finalPosition.x, cameraInfo.finalPosition.y, -10);
        }

        while (cameraTransform.position != finalTarget)
        {
            Vector3 target = Vector3.MoveTowards(cameraTransform.position, finalTarget, Time.deltaTime * cameraInfo.speed);
            cameraTransform.position = new Vector3(target.x, target.y, -10);
            yield return null;
        }

        if (state == 5)
        {
            state = 0;
        }
    }

    IEnumerator CameraZoom(CameraInfo cameraInfo)
    {

        Camera camera = GameObject.Find("Main Camera").GetComponent<Camera>();

        float zoomPerSecond = (cameraInfo.zoom * 3f - camera.orthographicSize) / cameraInfo.duration;

        float timer = cameraInfo.duration;

        while (timer > 0)
        {
            camera.orthographicSize += zoomPerSecond * Time.deltaTime;
            timer -= Time.deltaTime;

            if ((zoomPerSecond < 0 && camera.orthographicSize < cameraInfo.zoom*3) || (zoomPerSecond > 0 && camera.orthographicSize > cameraInfo.zoom * 3))
            {
                camera.orthographicSize = cameraInfo.zoom * 3f;
                timer = 0;
            }

            yield return null;
        }

        if (state == 6)
        {
            state = 0;
        }
    }

    public void setCamera(CameraInfo cameraInfo)
    {
        Transform camera = GameObject.Find("Camera").GetComponent<Transform>();

        camera.GetChild(0).GetComponent<Camera>().orthographicSize = cameraInfo.zoom * 3f;
        camera.position = new Vector3(cameraInfo.finalPosition.x, cameraInfo.finalPosition.y, -10);
        camera.rotation = Quaternion.Euler(cameraInfo.rotation);

    }

    ///////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
    //Main Control

    IEnumerator Control(string[] storyOrder, StoryInfo[] storyInfo) //depending on the input, will run diffrent functions in the same order
    {
        for (int i = 0; i < storyOrder.Length; i++)
        {
            switch (storyOrder[i])
            {
                case "Dialogue":

                    dialogueInfo = storyInfo[i].dialogueInfo;

                    if (!storyInfo[i].dontWait)
                    {
                        state = 1;

                        while (state == 1 || state == 2)
                        {
                            yield return null;
                        }
                    } else
                    {
                        TriggerDialogue();
                    }

                    player.GetComponent<PlayerControl>().enabled = false;
                    break;

                case "Move":
            
                    StartCoroutine(MoveNPC(storyInfo[i].moveInfo, player, storyInfo[i].dontWait));

                    if (!storyInfo[i].dontWait)
                    {
                        state = 3;
                        while (state == 3)
                        {
                            yield return null;
                        }
                    }

                    break;
                case "Animation":
                    StartCoroutine(RunAnimation(storyInfo[i].animationInfo));
                    break;
                case "Component":
                    ComponentActivate(storyInfo[i].componentInfo);
                    break;
                case "BGM":
                    StartCoroutine(PlayBGM(storyInfo[i].audioInfo));
                    break;
                case "SoundEffect":
                    SoundEffect(storyInfo[i].audioInfo);
                    break;

                case "BGMStop":
                    StopBGM();
                    break;
                case "BGMFade":
                    bgmFade = StartCoroutine(FadeBGM(storyInfo[i].audioInfo));
                    break;
                case "Wait":
                    state = 4;
                    StartCoroutine(Wait(storyInfo[i].waitInfo));

                    while (state == 4)
                    {
                        yield return null;
                    }

                    break;
                case "Teleport":
                    Teleport(storyInfo[i].moveInfo);
                    break;
                case "Camera":

                    if (cameraMove != null)
                    {
                        StopCoroutine(cameraMove);
                    }

                    cameraMove = StartCoroutine(Camera(storyInfo[i].cameraInfo));

                    if (!storyInfo[i].dontWait)
                    {
                        state = 5;
                        while (state == 5)
                        {
                            yield return null;
                        }
                    }
                    break;
                case "CameraZoom":

                    if (cameraZoom != null)
                    {
                        StopCoroutine(cameraZoom);
                    }

                    cameraZoom = StartCoroutine(CameraZoom(storyInfo[i].cameraInfo));

                    if (!storyInfo[i].dontWait)
                    {
                        state = 6;
                        while (state == 6)
                        {
                            yield return null;
                        }
                    }
                    break;

                case "SetCamera":

                    setCamera(storyInfo[i].cameraInfo);
                    break;

                case "DialogueMove":

                    dialogueInfo = storyInfo[i].dialogueInfo;

                    state = 1;

                    StartCoroutine(MoveNPC(storyInfo[i].moveInfo, player, storyInfo[i].dontWait));

                    yield return new WaitForSeconds(storyInfo[i].waitInfo.duration);

                    while (state == 2)
                    {
                        yield return null;
                    }
                    break;
                case "BattleSceneChange":

                    StartCoroutine(FindObjectOfType<SceneManagers>().SnapToBattle(storyInfo[i].sceneChangeInfo.levelName));
                    break;

                case "ResetSceneChange":

                    FindObjectOfType<ProgressionTracker>().adjustStoryOrder(storyInfo[i].sceneChangeInfo.cutsceneOrder);
                    FindObjectOfType<ProgressionTracker>().adjustSceneOrder(storyInfo[i].sceneChangeInfo.hintOrder);
                    FindObjectOfType<SceneManagers>().assignEntrance(storyInfo[i].sceneChangeInfo.scenePosition);
                    StartCoroutine(FindObjectOfType<SceneManagers>().SnapToBattle(storyInfo[i].sceneChangeInfo.levelName));
                    break;

                case "ExitSceneChange":

                    FindObjectOfType<ProgressionTracker>().adjustStoryOrder(storyInfo[i].sceneChangeInfo.cutsceneOrder);
                    FindObjectOfType<SceneManagers>().assignEntrance(storyInfo[i].sceneChangeInfo.scenePosition);
                    FindObjectOfType<SceneManagers>().FadetoLevel(storyInfo[i].sceneChangeInfo.levelName);
                    break;
            }
        }
        player.GetComponent<PlayerControl>().enabled = true; //Allows the user to move again
        FindObjectOfType<CameraControl>().enabled = true;
    }
}
    
