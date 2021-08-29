using UnityEngine;

public abstract class TriggerBase: MonoBehaviour
{
    public abstract void OnTriggered(RocketControl rocket);
    public virtual void OnTrigerExit(RocketControl rocket) 
    { 
    }
}
