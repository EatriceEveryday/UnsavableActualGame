using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;

public class ProgressionTracker : MonoBehaviour {

    public Hints[] hints;

    public static ProgressionTracker instance;

    private int cutsceneOrder;
    private int sceneOrder;
    private bool reset;

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

        if (LoadProgress() == null){

            cutsceneOrder = 0;
            sceneOrder = 0;
            reset = false;

        } else
        {
            SaveFile data = LoadProgress();

            cutsceneOrder = data.cutsceneOrder;
            sceneOrder = data.hintOrder;
            reset = data.reset;
            SceneManager.LoadScene(data.actualSceneNumber);
        }
        
    }

    void OnSceneLoaded (Scene scene, LoadSceneMode mode)
    {
        MenuManager menuBox = FindObjectOfType<MenuManager>();
        int index = SceneManager.GetActiveScene().buildIndex;

        if (menuBox != null)
        {
            if (hints[index].hint.Length > 0 && index >= sceneOrder)
            {
                menuBox.hint = hints[index].hint;
            }

        }

        if (index > sceneOrder)
        {
            sceneOrder++;
        }

    }

    public bool isInStoryOrder (int order)
    {
        if (order >= cutsceneOrder)
        {
            cutsceneOrder++;
            return true;
        }

        return false;
    }

    public int getCutsceneOrder()
    {
        return cutsceneOrder;
    }

    public int getSceneOrder()
    {
        return sceneOrder;
    }

    public void adjustStoryOrder (int order)
    {
        cutsceneOrder = order;
    }

    public void adjustSceneOrder (int order)
    {
        sceneOrder = order;
    }

    public bool getReset()
    {
        bool previous = reset;
        reset = true;
        return previous;
    }

    public void SaveProgress()
    {
        BinaryFormatter formatter = new BinaryFormatter();
        string path = Application.persistentDataPath + "/player.yes";
        FileStream stream = new FileStream(path, FileMode.Create);

        SaveFile data = new SaveFile(cutsceneOrder, sceneOrder, reset);

        formatter.Serialize(stream, data);
        stream.Close();
    }

    public SaveFile LoadProgress()
    {
        string path = Application.persistentDataPath + "/player.yes";
        if (File.Exists(path))
        {
            BinaryFormatter formatter = new BinaryFormatter();
            FileStream stream = new FileStream(path, FileMode.Open);

            SaveFile data = formatter.Deserialize(stream) as SaveFile;
            stream.Close();

            return data;
        }
        else
        {
            Debug.LogError("No save file?");
            return null;
        }
    }

    public void Reset()
    {
        BinaryFormatter formatter = new BinaryFormatter();
        string path = Application.persistentDataPath + "/player.yes";
        FileStream stream = new FileStream(path, FileMode.Create);

        SaveFile data = new SaveFile(1, 1, false);
        data.actualSceneNumber = 62;

        formatter.Serialize(stream, data);
        stream.Close();

        cutsceneOrder = 1;
        sceneOrder = 1;
        reset = false;

        FindObjectOfType<AudioManager>().Play("Menu_Select");
        FindObjectOfType<SceneManagers>().assignEntrance(new Vector2(0, 0));
        FindObjectOfType<SceneManagers>().FadetoLevel("PurpleReset");

        StartCoroutine(FindObjectOfType<StoryManager>().FadeBGM(new AudioInfo()));
        StartCoroutine(replay());
    }

    IEnumerator replay()
    {
        yield return new WaitForSeconds(1);
        GameObject.Find("AudioManager").GetComponent<AudioSource>().clip = null;
    }
}
