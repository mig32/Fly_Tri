using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TriggerTeleport : TriggerBase
{
    [SerializeField] private Transform m_targetPosition;
    [SerializeField] private float m_teleportTime;

    private RocketControl m_savedRocket;

    public override void OnTriggered(RocketControl rocket)
    {
        if (m_targetPosition == null)
        {
            return;
        }

        m_savedRocket = rocket;
        m_savedRocket.StartTeleport();
        StartCoroutine(DoTeleport());
    }

    private IEnumerator DoTeleport()
    {
        yield return new WaitForSeconds(m_teleportTime);
        if (m_savedRocket != null)
        {
            m_savedRocket.transform.position = m_targetPosition.transform.position;
            m_savedRocket.transform.rotation = m_targetPosition.transform.rotation;
            m_savedRocket.FinishTeleport();
        }
    }

    public void OnDrawGizmos()
    {
        var point1 = transform.position;
        var collider = GetComponent<Collider2D>();
        if (collider)
        {
            Gizmos.color = Color.cyan;
            Gizmos.DrawWireCube(collider.bounds.center, collider.bounds.size);
            point1 = collider.bounds.center;
        }

        if (m_targetPosition)
        {
            Gizmos.color = Color.cyan;
            Gizmos.DrawLine(point1, m_targetPosition.position);
            Gizmos.color = Color.cyan;
            Gizmos.DrawSphere(point1, 0.05f);
            Gizmos.color = Color.cyan;
            Gizmos.DrawSphere(m_targetPosition.position, 0.05f);
        }
    }
}
