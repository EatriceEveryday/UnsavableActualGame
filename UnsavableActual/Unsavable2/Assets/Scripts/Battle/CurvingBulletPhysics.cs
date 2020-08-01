using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CurvingBulletPhysics : MonoBehaviour
{

    public float damage;
    public float period = 1f;
    public float radius;
    public bool clockwise;
    public bool parent;

    private float angle;
    private Vector3 oriPosition;
    private Vector3 nextPosition;
    private float speed;

    // Update is called once per frame

    void Start()
    {
        if (!clockwise)
        {
            angle = transform.eulerAngles.z / 180 * Mathf.PI;
        } else
        {
            angle = (transform.eulerAngles.z + 180)/ 180 * Mathf.PI;
        }

        if(parent)
        {
            for (int i = 0; i < transform.childCount; i++) //Goes through each bullet and turns them on
            {
                Transform bullet = transform.GetChild(i);
                bullet.GetComponent<CurvingBulletPhysics>().enabled = true;

            }
        }

        oriPosition = transform.localPosition;
        speed = 2 * Mathf.PI / period;

        nextPosition = new Vector3(Mathf.Cos(angle) * radius, Mathf.Sin(angle) * radius, 0);
        transform.localPosition = oriPosition + nextPosition;
    }


    void Update()
    {
        if (!clockwise)
        {
            angle += speed * Time.deltaTime;

            nextPosition = new Vector3(Mathf.Cos(angle) * radius, Mathf.Sin(angle) * radius, 0);
            transform.rotation = Quaternion.Euler(0, 0, angle * 180 / Mathf.PI);

            transform.localPosition = oriPosition + nextPosition;
        }

        else
        {
            angle -= speed * Time.deltaTime;
            nextPosition = new Vector3(Mathf.Cos(angle) * radius, Mathf.Sin(angle) * radius, 0);
            transform.rotation = Quaternion.Euler(0, 0, angle * 180 / Mathf.PI + 180);

            transform.localPosition = oriPosition + nextPosition;
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
