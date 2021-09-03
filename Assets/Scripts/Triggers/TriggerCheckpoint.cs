using UnityEngine;

public class TriggerCheckpoint : TriggerBase
{
    [SerializeField] private float m_allowedVelosity;

    private int m_idx = 0;

    public void SetIdx(int idx)
    {
        m_idx = idx;
    }

    public override void OnTrigerStay(RocketControl rocket)
    {
        if (m_idx == WorldControl.GetInstance().CurrentCheckpoint || m_allowedVelosity < rocket.GetComponent<Rigidbody2D>().velocity.magnitude)
        {
            return;
        }

        WorldControl.GetInstance().OnCheckpointSaved(m_idx);
        WorldControl.GetInstance().ShowMessage("Checkpoint saved!");
    }
}
