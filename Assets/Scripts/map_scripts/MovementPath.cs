using SuperTiled2Unity;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class MovementPath : MonoBehaviour
{
    [SerializeField] private List<Transform> m_waypoints;
    [SerializeField] private bool m_isLooped;

    public bool IsLooped { get { return m_isLooped; } }
    public List<Transform> Waypoints { get { return m_waypoints; } }

    private void OnDrawGizmos()
    {
        if (m_waypoints == null)
        {
            return;
        }

        var checkedWaypoints = m_waypoints.FindAll(point => point != null).ToList();
        if (checkedWaypoints.IsEmpty())
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
}
