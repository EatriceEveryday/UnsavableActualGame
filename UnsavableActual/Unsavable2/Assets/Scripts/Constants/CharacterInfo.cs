using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterInfo : MonoBehaviour {

    public Vector2 offset;
    public string startingDirection;
    private Animator animator;

    void Start()
    {
        animator = GetComponent<Animator>();

        animator.SetFloat("x", 0);
        animator.SetFloat("y", 0);
        
        switch (startingDirection)
        {
            case "Front": animator.SetFloat("y", -1); break;
            case "Back": animator.SetFloat("y", 1); break;
            case "Left": animator.SetFloat("x", -1); break;
            case "Right": animator.SetFloat("x", 1); break;
        }
        
    }

    public void ShakeCamera()
    {
        FindObjectOfType<CameraShake>().Shake(0.3f, 1f);
    }
}
