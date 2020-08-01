using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class HintTimer : MonoBehaviour {

    public static HintTimer instance;

    private ProgressionTracker pt;
    public bool[] times;
    Coroutine timer;
    int sceneInt;

    private bool wrongScene;

    void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        else
        {
            Destroy(gameObject);
            return;
        }

        DontDestroyOnLoad(gameObject);
        SceneManager.sceneLoaded += OnSceneLoaded;

        pt = FindObjectOfType<ProgressionTracker>();
        sceneInt = 0;
        wrongScene = false;
    }

    void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        StartCoroutine(lateCheck());
    }
    
    IEnumerator lateCheck()
    {
        yield return new WaitForSeconds(0.5f);

        if (wrongScene && SceneManager.GetActiveScene().buildIndex == sceneInt)
        {
            DialogueInfo[] dialogue = pt.hints[sceneInt].hint;
            FindObjectOfType<DialogueManager>().StartDialogue(dialogue, false);
        }

        if (pt.getSceneOrder() > sceneInt && timer != null)
        {
            StopCoroutine(timer);
        }

        sceneInt = pt.getSceneOrder();

        if (times[sceneInt])
        {
            times[sceneInt] = false;
            timer = StartCoroutine(startCountDown());
        }
    }

    IEnumerator startCountDown()
    {
        yield return new WaitForSeconds(300f);

        if (FindObjectOfType<MenuManager>().enabled)
        {
            FindObjectOfType<MenuManager>().enabled = false;
        }

        if (FindObjectOfType<RUsureManager>().enabled)
        {
            FindObjectOfType<RUsureManager>().enabled = false;
            FindObjectOfType<MenuManager>().enabled = false;
        }

        if (SceneManager.GetActiveScene().buildIndex == sceneInt)
        {
            DialogueInfo[] dialogue = pt.hints[sceneInt].hint;
            FindObjectOfType<DialogueManager>().StartDialogue(dialogue, false);
        } else
        {
            wrongScene = true;
        }

    }

    public void stopTimer()
    {
        if (timer != null)
        {
            StopCoroutine(timer);
        }
    }

}
