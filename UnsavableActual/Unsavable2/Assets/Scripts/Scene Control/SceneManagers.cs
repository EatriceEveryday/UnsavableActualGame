using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneManagers : MonoBehaviour {

    private Animator fade;
    private string level;
    public Transform player;

    public bool fadeIn;

    public static Vector2 entrance;
    public AudioInfo backgroundMusic;

    // Use this for initialization
    void Start()
    {
        fade = this.GetComponent<Animator>();

        fade.SetBool("FadeIn", fadeIn);

        player.position = entrance;

        AudioSource speaker = GameObject.Find("AudioManager").GetComponent<AudioSource>();

        if (backgroundMusic.clip != null && speaker.clip != backgroundMusic.clip)
        {
            speaker.clip = backgroundMusic.clip;
            speaker.volume = backgroundMusic.volume;
            speaker.loop = backgroundMusic.loop;
            speaker.Play();
        }
    }

    public void FadetoLevel (string levelname)
    {
        level = levelname;
        fade.SetTrigger("FadeOut");
    }

    public IEnumerator SnapToBattle (string levelname)
    {
        level = levelname;
        fade.SetTrigger("SnapOut");
        FindObjectOfType<AudioManager>().Play("Hit");
        yield return new WaitForSeconds(1);
        OnFadeComplete();
    }

    public void assignEntrance(Vector2 num)
    {
        entrance = num;
    }

    public void OnFadeComplete()
    {
        SceneManager.LoadScene(level);
    }
}
