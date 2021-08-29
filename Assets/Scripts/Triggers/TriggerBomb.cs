using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TriggerBomb : TriggerBase
{
    [SerializeField] private float m_damage;
    [SerializeField] private SpriteRenderer m_bombSprite;
    [SerializeField] private ParticleSystem m_explosionParticleSystem;
    [SerializeField] private Collider2D m_collider;

    public override void OnTriggered(RocketControl rocket)
    {
        rocket.ApplyDamage(m_damage);

        if (m_collider != null)
        {
            m_collider.enabled = false;
        }

        if (m_bombSprite != null)
        {
            m_bombSprite.enabled = false;
        }

        if (m_explosionParticleSystem != null)
        {
            m_explosionParticleSystem.Play();
        }
    }
}
