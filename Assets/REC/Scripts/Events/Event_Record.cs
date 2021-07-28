using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Event_Record : Event_
{
    private bool m_eventConfigurated = false;

    public Animator agentAnimator;
    public bool recordMotion = true;

    private AnimatorClipInfo[] m_clipInfos;
    private bool m_animationEnd = false;

    override protected void Update()
    {
        base.Update();

        if (m_eventConfigurated)
            EventUpdate();
    }

    override protected void EventInitialisation()
    {
        if(agentAnimator)
        {
            agentAnimator.enabled = true;
            StartCoroutine(WaitForAnimationEnd());
        }

        m_eventConfigurated = true;
    }

    override protected void EventReinitialisation()
    {
        if (agentAnimator)
            agentAnimator.enabled = false;
    }

    private void EventUpdate()
    {
        if (Input.GetKeyDown(KeyCode.Space) || m_animationEnd)
            EventEnd();
    }

    override public void EventEnd()
    {
        if(recordMotion)
            CSVDataRecorder.Instance.DATACollection();

        base.EventEnd();
    }

    private IEnumerator WaitForAnimationEnd()
    {
        m_clipInfos = agentAnimator.GetCurrentAnimatorClipInfo(0);
        Debug.Log("Animation name: " + m_clipInfos[0].clip.name + "; Duration: " + m_clipInfos[0].clip.length.ToString() + "; Speed: " + agentAnimator.GetFloat("Speed"));

        yield return new WaitForSeconds(m_clipInfos[0].clip.length / agentAnimator.GetFloat("Speed"));

        m_animationEnd = true;
    }
}
