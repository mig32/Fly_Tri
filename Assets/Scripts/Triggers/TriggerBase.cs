using UnityEngine;

public abstract class TriggerBase: MonoBehaviour
{
    public virtual void OnTriggered(RocketControl rocket)
    {
    }

    public virtual void OnTrigerStay(RocketControl rocket)
    {
    }

    public virtual void OnTrigerExit(RocketControl rocket) 
    { 
    }
}
