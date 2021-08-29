using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TriggerCollectable : TriggerBase
{
    public override void OnTriggered(RocketControl rocket)
    {
        if (WorldControl.GetInstance().getMap().GetComponent<MapInfo>().m_collectSound)
        {
            WorldControl.GetInstance().soundController.PlayShortSound(WorldControl.GetInstance().getMap().GetComponent<MapInfo>().m_collectSound);
        }
        Destroy(gameObject);
    }
}
