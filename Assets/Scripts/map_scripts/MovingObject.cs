using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class MovingObject: MonoBehaviour
{
    [SerializeField, Range(0.1f, 10.0f)] private float m_speed = 1.0f;
    [SerializeField] private List<Transform> m_waypoints;
    [SerializeField] private bool m_isLooped;

    private int m_currentPoint;
    private bool m_isReverse = false;

    private void OnDrawGizmos()
    {
        if (m_waypoints == null)
        {
            return;
        }

        var checkedWaypoints = m_waypoints.FindAll(point => point != null).ToList();
        if (!checkedWaypoints.Any())
        {
            return;
        }

        Gizmos.color = Color.magenta;
        Gizmos.DrawSphere(checkedWaypoints.First().position, 0.05f);
        if (checkedWaypoints.Count == 1)
        {
            return;
        }

        for (var i = 1; i < checkedWaypoints.Count; ++i)
        {
            var point1 = checkedWaypoints[i - 1];
            var point2 = checkedWaypoints[i];
            Gizmos.color = Color.magenta;
            Gizmos.DrawSphere(point2.position, 0.05f);
            Gizmos.color = Color.magenta;
            Gizmos.DrawLine(point1.position, point2.position);
        }

        if (m_isLooped && checkedWaypoints.Count > 2)
        {
            Gizmos.color = Color.magenta;
            Gizmos.DrawLine(checkedWaypoints.First().position, checkedWaypoints.Last().position);
        }
    }

    private void Start()
    {
        if (!m_waypoints.Any())
        {
            return;
        }

        transform.position = m_waypoints[0].position;
        transform.rotation = m_waypoints[0].rotation;
        m_currentPoint = 0;
        NextPoint();
    }

    private void FixedUpdate()
    {
        MoveDistance(m_speed * Time.fixedDeltaTime);
    }

    private void MoveDistance(float distance)
    {
        if (m_currentPoint >= m_waypoints.Count)
        {
            return;
        }

        var targetTransform = m_waypoints[m_currentPoint];
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
                if (m_isLooped)
                {
                    m_currentPoint = m_waypoints.Count - 1;
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
            if (m_currentPoint < m_waypoints.Count - 1)
            {
                ++m_currentPoint;
            }
            else
            {
                if (m_isLooped)
                {
                    m_currentPoint = 0;
                }
                else
                {
                    m_currentPoint = m_waypoints.Count - 1;
                    m_isReverse = true;
                }
            }
        }

    }
}
