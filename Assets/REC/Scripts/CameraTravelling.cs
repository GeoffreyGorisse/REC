using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraTravelling : MonoBehaviour
{
    public float rotationAngle = 5.0f;
    public float speed = 1.0f;

    void Update()
    {
        transform.rotation = Quaternion.Euler(0.0f, Mathf.Sin(Time.time * speed) * rotationAngle, 0.0f);
    }
}
