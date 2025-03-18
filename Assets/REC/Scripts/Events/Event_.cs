using UnityEngine;

abstract public class Event_ : MonoBehaviour {

    protected bool EventConfigurated = false;
    protected float EventDuration = 0f;

    void OnEnable()
    {
        EventInitialisation();
    }

    void OnDisable()
    {
        EventReinitialisation();
    }

    virtual protected void Update()
    {
        EventDuration += Time.deltaTime;
    }

    abstract protected void EventInitialisation();

    abstract protected void EventReinitialisation();

    virtual public void EventEnd()
    {
        EventManager.Instance.NextEvent();
    }
}
