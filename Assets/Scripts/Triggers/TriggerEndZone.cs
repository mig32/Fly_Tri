using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Collider2D))]
public class TriggerEndZone : TriggerBase
{
    private enum State
    {
        Inactive,
        Active,
        Timer,
        Checked
    }

    [SerializeField] private float m_timer;
    [SerializeField] private ParticleSystem m_particleSystem;

    private Collider2D m_collider;
    private float m_timeLeft;
    private State m_state = State.Inactive;

    public bool IsChecked { get { return m_state == State.Checked; } }
    public Collider2D GetCollider() => m_collider;

    private void Awake()
    {
        m_collider = GetComponent<Collider2D>();
    }

    public override void OnTriggered(RocketControl rocket)
    {
        if (m_state != State.Active)
        {
            return;
        }

        m_state = State.Timer;
        m_timeLeft = m_timer;
    }

    public override void OnTrigerExit(RocketControl rocket)
    {
        if (m_state != State.Timer)
        {
            return;
        }

        m_state = State.Active;
    }

    private void Update()
    {
        if (m_state != State.Timer)
        {
            return;
        }

        m_timeLeft -= Time.deltaTime;
        if (m_timeLeft <= 0)
        {
            m_state = State.Checked;
            if (m_particleSystem)
            {
                m_particleSystem.Stop();
            }
            m_collider.enabled = false;
            WorldControl.GetInstance().ShowMessage("Checkpoint collected!");
            WorldControl.GetInstance().OnCheckpointCollected();
        }
        else
        {
            WorldControl.GetInstance().ShowMessage("Checkpoint will be collected in (" + string.Format("{0:0.00}", Mathf.Round(m_timeLeft * 100.0f) / 100.0f) + ")");
        }
    }

    public void Activate()
    {
        if (m_state != State.Inactive)
        {
            return;
        }

        m_state = State.Active;
        if (m_particleSystem)
        {
            m_particleSystem.Play();
        }

        WorldControl.GetInstance().ShowMessage("Checkpoint activated!");
    }
}
