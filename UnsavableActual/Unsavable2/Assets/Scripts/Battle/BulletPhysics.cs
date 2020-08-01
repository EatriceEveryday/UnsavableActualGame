using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BulletPhysics : MonoBehaviour {

    public float damage;
    public Vector2 direction;
    public bool parent;
	
	// Update is called once per frame

    void Start()
    {
        if (parent)
        {
            for (int i = 0; i < transform.childCount; i++) //Goes through each bullet and turns them on
            {
                Transform bullet = transform.GetChild(i);
                if (bullet.GetComponent<CurvingBulletPhysics>() == null)
                {
                    bullet.GetComponent<LaserPhysics>().enabled = true;
                }
                else
                {
                    bullet.GetComponent<CurvingBulletPhysics>().enabled = true;
                }

            }
        }
    }

	void Update () {

        if (direction != new Vector2(0, 0))
        {
            transform.position += new Vector3(direction.x * Time.deltaTime, direction.y * Time.deltaTime, 0);
        }

    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            PlayerControlBoss player = other.GetComponent<PlayerControlBoss>();
            player.dealDamage(damage, true);
            Destroy(gameObject);
        }
    }
}
