using System.Collections;
using UnityEngine;

public class Event_Record : Event_
{
    [SerializeField] private Animator _agentAnimator;
    [SerializeField] private bool _recordMotion = true;

    private bool _eventConfigurated = false;
    private AnimatorClipInfo[] _clipInfos;
    private bool _animationEnd = false;

    override protected void Update()
    {
        base.Update();

        if (_eventConfigurated)
            EventUpdate();
    }

    override protected void EventInitialisation()
    {
        if(_agentAnimator)
        {
            _agentAnimator.enabled = true;
            StartCoroutine(WaitForAnimationEnd());
        }

        _eventConfigurated = true;
    }

    override protected void EventReinitialisation()
    {
        if (_agentAnimator)
            _agentAnimator.enabled = false;
    }

    private void EventUpdate()
    {
        if (Input.GetKeyDown(KeyCode.Space) || _animationEnd)
            EventEnd();
    }

    override public void EventEnd()
    {
        if(_recordMotion)
            CSVDataRecorder.Instance.DATACollection();

        base.EventEnd();
    }

    private IEnumerator WaitForAnimationEnd()
    {
        _clipInfos = _agentAnimator.GetCurrentAnimatorClipInfo(0);
        Debug.Log("Animation name: " + _clipInfos[0].clip.name + "; Duration: " + _clipInfos[0].clip.length.ToString() + "; Speed: " + _agentAnimator.GetFloat("Speed"));

        yield return new WaitForSeconds(_clipInfos[0].clip.length / _agentAnimator.GetFloat("Speed"));

        _animationEnd = true;
    }
}
