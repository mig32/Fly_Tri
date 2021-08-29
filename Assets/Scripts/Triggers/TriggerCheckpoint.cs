using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TriggerCheckpoint : TriggerBase
{
    [SerializeField] private float m_allowedVelosity;

    private int m_idx = 0;
    private Rigidbody2D m_targetRigidbody;
    private bool m_isChecking = false;

    public void SetIdx(int idx)
    {
        m_idx = idx;
    }

    public override void OnTriggered(RocketControl rocket)
    {
        if (m_isChecking || m_idx == WorldControl.GetInstance().CurrentCheckpoint)
        {
            return;
        }

        m_targetRigidbody = rocket.GetComponent<Rigidbody2D>();
        m_isChecking = true;
    }
    public override void OnTrigerExit(RocketControl rocket)
    {
        m_targetRigidbody = null;
        m_isChecking = false;
    }

    private void Update()
    {
        if (!m_isChecking)
        {
            return;
        }

        if (m_allowedVelosity < m_targetRigidbody.velocity.sqrMagnitude)
        {
            return;
        }

        m_isChecking = false;
        m_targetRigidbody = null;
        WorldControl.GetInstance().OnCheckpointSaved(m_idx);
        WorldControl.GetInstance().ShowMessage("Checkpoint saved!");
    }
}
