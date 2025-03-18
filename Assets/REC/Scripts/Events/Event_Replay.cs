using UnityEngine;

public class Event_Replay : Event_
{
    private bool _eventConfigurated = false;

    override protected void Update()
    {
        base.Update();

        if (_eventConfigurated)
            EventUpdate();
    }

    override protected void EventInitialisation()
    {
        _eventConfigurated = true;
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
