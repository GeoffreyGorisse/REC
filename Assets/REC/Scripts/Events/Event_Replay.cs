using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Event_Replay : Event_
{
    private bool m_eventConfigurated = false;

    override protected void Update()
    {
        base.Update();

        if (m_eventConfigurated)
            EventUpdate();
    }

    override protected void EventInitialisation()
    {
        m_eventConfigurated = true;
    }

    override protected void EventReinitialisation()
    {

    }

    private void EventUpdate()
    {

    }

    override public void EventEnd()
    {
        base.EventEnd();
    }
}
