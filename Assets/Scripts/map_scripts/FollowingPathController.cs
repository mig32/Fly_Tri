using SuperTiled2Unity;
using System.Linq;
using UnityEngine;

public class FollowingPathController : MonoBehaviour
{
    [SerializeField, Range(0.1f, 10.0f)] private float m_speed = 1.0f;
    [SerializeField] private MovementPath m_path;

    private int m_currentPoint;
    private bool m_isReverse = false;

    private void Start()
    {
        if (m_path.Waypoints.IsEmpty())
        {
            return;
        }

        transform.position = m_path.Waypoints[0].position;
        transform.rotation = m_path.Waypoints[0].rotation;
        m_currentPoint = 0;
        NextPoint();
    }

    private void FixedUpdate()
    {
        MoveDistance(m_speed * Time.fixedDeltaTime);
    }

    private void MoveDistance(float distance)
    {
        if (m_currentPoint >= m_path.Waypoints.Count)
        {
            return;
        }

        var targetTransform = m_path.Waypoints[m_currentPoint];
        var dPos = targetTransform.position - transform.position;
        var distanceToTarget = dPos.magnitude;
        if (distance > distanceToTarget)
        {
            transform.position = targetTransform.position;
            transform.rotation = targetTransform.rotation;
            distance -= distanceToTarget;
            NextPoint();
            MoveDistance(distance);
            return;
        }

        transform.position = Vector3.Lerp(transform.position, targetTransform.position, distance / distanceToTarget);
    }

    private void NextPoint()
    {
        if (m_isReverse)
        {
            if (m_currentPoint > 0)
            {
                --m_currentPoint;
            }
            else
            {
                if (m_path.IsLooped)
                {
                    m_currentPoint = m_path.Waypoints.Count - 1;
                }
                else
                {
                    m_currentPoint = 1;
                    m_isReverse = false;
                }
            }
        }
        else
        {
            if (m_currentPoint < m_path.Waypoints.Count - 1)
            {
                ++m_currentPoint;
            }
            else
            {
                if (m_path.IsLooped)
                {
                    m_currentPoint = 0;
                }
                else
                {
                    m_currentPoint = m_path.Waypoints.Count - 1;
                    m_isReverse = true;
                }
            }
        }

    }
}
