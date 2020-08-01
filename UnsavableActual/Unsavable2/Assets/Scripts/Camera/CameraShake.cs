using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraShake : MonoBehaviour {

    public Camera mainCam;
    public GameObject UI;

    float shakeAmount = 0;
    float length;
    Vector3 mainCamOriPos, UIPos;

    // Use this for initialization
    void Awake () {
		
        if (mainCam == null){
            mainCam = Camera.main;
        }

	}
	
    public void Shake (float amt, float length)
    {
        CancelInvoke("DoShake");
        CancelInvoke("StopShake");
        shakeAmount = amt;
        this.length = length;

        mainCamOriPos = mainCam.transform.position;

        if (UI != null)
        {
            UIPos = UI.transform.position;
            InvokeRepeating("DoShakeUI", 0, 0.01f);
        }
        else
        {
            InvokeRepeating("DoShake", 0, 0.01f);
        }

        Invoke("StopShake", length);

    }

    void DoShakeUI()
    {
        if (shakeAmount > 0)
        {
            Vector3 camPos = this.mainCamOriPos;
            Vector3 UIPos = this.UIPos;

            float offsetX = Random.Range(-1f, 1f) * shakeAmount * 2;
            float offsetY = Random.Range(-1f, 1f) * shakeAmount * 2;

            camPos.x += offsetX;
            camPos.y += offsetY;
            UIPos.x += offsetX*50;
            UIPos.y += offsetY*50;

            UI.transform.position = UIPos;
            mainCam.transform.position = camPos;
            shakeAmount -= (shakeAmount/length)/50;
        }
    }

    void DoShake()
    {
        if (shakeAmount > 0)
        {
            Vector3 camPos = this.mainCamOriPos;

            float offsetX = Random.Range(-1f, 1f) * shakeAmount * 2;
            float offsetY = Random.Range(-1f, 1f) * shakeAmount * 2;

            camPos.x += offsetX;
            camPos.y += offsetY;

            mainCam.transform.position = camPos;
            shakeAmount -= (shakeAmount / length) / 50;
        }
    }

    void StopShake()
    {
        CancelInvoke("DoShake");
        CancelInvoke("DoShakeUI");
        mainCam.transform.localPosition = Vector3.zero;

        if (UI != null)
        {
            UI.transform.localPosition = Vector3.zero;
        }
    }

}
