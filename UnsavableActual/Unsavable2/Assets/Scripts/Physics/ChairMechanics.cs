using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChairMechanics : MonoBehaviour {

    private int directionOfMovement;

	// Use this for initialization
	void Start () {
        directionOfMovement = 0;
	}

    public bool compareDirection(int direction)
    {
        if (direction == directionOfMovement)
        {
            return true;
        }

        return false;
    }

    public int getDirection()
    {
        return directionOfMovement;
    }

    public void setDirection (int direction)
    {
        directionOfMovement = direction;
    }

}
