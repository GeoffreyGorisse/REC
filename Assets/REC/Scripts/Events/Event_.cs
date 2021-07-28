using System.Collections;
using System.Collections.Generic;
using UnityEngine;

abstract public class Event_ : MonoBehaviour {

    protected bool p_eventConfigurated = false;
    protected float p_eventDuration;

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
        p_eventDuration += Time.deltaTime;
    }

    abstract protected void EventInitialisation();

    abstract protected void EventReinitialisation();

    virtual public void EventEnd()
    {
        EventManager.Instance.NextEvent();
    }
}
