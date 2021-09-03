using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RotatingObject : MonoBehaviour
{
    [SerializeField, Range(-100.0f, 100.0f)] private float m_speed = 0.1f;

    private void FixedUpdate()
    {
        transform.Rotate(0.0f, 0.0f, m_speed * Time.fixedDeltaTime, Space.Self);
    }
}
