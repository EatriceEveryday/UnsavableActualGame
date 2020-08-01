using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BattleManager : MonoBehaviour {

    private BulletHellManager bulletHellManager;
    private PlayerControlBoss player;
    private int state;
    public int startingIndex = 0;

    public BattleInfo[] battleInfo;
    public string[] eventOrder;

	// Use this for initialization
	void Start () {

        bulletHellManager = FindObjectOfType<BulletHellManager>();
        player = GameObject.Find("PlayerBattle").GetComponent<PlayerControlBoss>();

        StartCoroutine(manageBattle());

	}

    void Update()
    {
        if (state == 1 && FindObjectOfType<DialogueInputer>().enabled == false)
        {
            state = 0;
        }

        if (state == 2 && !bulletHellManager.isRunning)
        {
            state = 0;
        }
    }

    ///////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
    //BGM

    IEnumerator PlayBGM(AudioInfo audioInfo) //Function to stop the current background music, replace it and replay it
    {
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

    IEnumerator FadeBGM(AudioInfo audioInfo)
    {
        AudioSource speaker = GameObject.Find("AudioManager").GetComponent<AudioSource>();

        if (speaker.volume == 0)
        {
            StartCoroutine(FindObjectOfType<AudioManager>().Fade("TRM Reset"));
        }
        else
        {
            float volumeChange = (audioInfo.volume - speaker.volume) / 60;

            for (int i = 0; i < 60; i++)
            {
                speaker.volume += volumeChange;
                yield return null;
            }
        }
    }

    IEnumerator manageBattle ()
    {
        yield return new WaitForSeconds(1);

        for (int i = startingIndex; i < battleInfo.Length; i++)
        {
            switch (eventOrder[i])
            {
                case "BulletHell":

                    player.enabled = true;
                    StartCoroutine(bulletHellManager.spawnBullets(battleInfo[i].bulletPatternInfo.bulletPattern, battleInfo[i].bulletPatternInfo.duration));
                    yield return new WaitForSeconds(battleInfo[i].bulletPatternInfo.duration);
                    player.enabled = false;
                    break;

                case "Dialogue":
                    FindObjectOfType<DialogueManager>().StartDialogue(battleInfo[i].dialogueInfo, true);
                    state = 1;

                    while (state == 1)
                    {
                        yield return null;
                    }
                    break;

                case "Attack":
                    StartCoroutine(bulletHellManager.startAttack(battleInfo[i].attackInfo.attackCombination, battleInfo[i].attackInfo.damage, battleInfo[i].attackInfo.time, false, false));
                    state = 2;

                    while (state == 2)
                    {
                        yield return null;
                    }
                    break;

                case "AttackFast":
                    StartCoroutine(bulletHellManager.startAttack(battleInfo[i].attackInfo.attackCombination, battleInfo[i].attackInfo.damage, battleInfo[i].attackInfo.time, true, true));
                    state = 2;

                    while (state == 2)
                    {
                        yield return null;
                    }
                    break;

                case "AttackFastFinal":
                    StartCoroutine(bulletHellManager.startAttack(battleInfo[i].attackInfo.attackCombination, battleInfo[i].attackInfo.damage, battleInfo[i].attackInfo.time, true, false));
                    state = 2;

                    while (state == 2)
                    {
                        yield return null;
                    }
                    break;

                case "BGM":
                    StartCoroutine(PlayBGM(battleInfo[i].audioInfo));
                    break;

                case "BGMStop":
                    StopBGM();
                    break;

                case "BGMFade":
                    StartCoroutine(FadeBGM(battleInfo[i].audioInfo));
                    break;

                case "Wait":
                    yield return new WaitForSeconds(battleInfo[i].duration);
                    break;

                case "SceneChange":

                    FindObjectOfType<SceneManagers>().FadetoLevel(battleInfo[i].sceneChangeInfo.levelName);
                    FindObjectOfType<SceneManagers>().assignEntrance(battleInfo[i].sceneChangeInfo.scenePosition);
                    break;
            }
        }
    }

    public void die()
    {
        StopAllCoroutines();
    }
	
}
