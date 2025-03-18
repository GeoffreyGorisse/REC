using UnityEngine;

public class CameraTravelling : MonoBehaviour
{
    [SerializeField] private float _rotationAngle = 20f;
    [SerializeField] private float _speed = 0.2f;

    void Update()
    {
        transform.rotation = Quaternion.Euler(0.0f, Mathf.Sin(Time.time * _speed) * _rotationAngle, 0.0f);
    }
}
