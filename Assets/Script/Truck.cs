using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Truck : MonoBehaviour
{
    public float speed = 1f;
    public float stopPositionX = 10f;

    private bool hasStopped = false;

    void Update()
    {
        if (!hasStopped && transform.position.x < stopPositionX)
        {
            transform.Translate(Vector2.right * speed * Time.deltaTime);

            if (transform.position.x >= stopPositionX)
            {
                Vector3 stopPosition = transform.position;
                stopPosition.x = stopPositionX;
                transform.position = stopPosition;

                hasStopped = true;
            }
        }
    }
}