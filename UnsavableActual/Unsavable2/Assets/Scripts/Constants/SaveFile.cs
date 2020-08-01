using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

[System.Serializable]
public class SaveFile {

    public int cutsceneOrder;
    public int hintOrder;
    public int actualSceneNumber;
    public bool reset;

    public SaveFile(int a, int b, bool c)
    {
        hintOrder = b;

        if (SceneManager.GetActiveScene().buildIndex == 16)
        {
            actualSceneNumber = 14;
        } else
        {
            actualSceneNumber = SceneManager.GetActiveScene().buildIndex;
        }

        reset = c;

        switch (a)
        {
            case 2: cutsceneOrder = 1; break;
            case 3: cutsceneOrder = 1; break;
            case 7: cutsceneOrder = 6; break;
            case 14: cutsceneOrder = 13; break;
            case 17: cutsceneOrder = 16; break;
            case 20: cutsceneOrder = 19; break;
            case 22: cutsceneOrder = 21; break;
            case 23: cutsceneOrder = 21; break;
            case 24: cutsceneOrder = 21; break;
            case 26: cutsceneOrder = 25; break;
            case 34: cutsceneOrder = 33; break;
            case 43: cutsceneOrder = 42; break;
            default: cutsceneOrder = a; break;

        }
        

    }

}
