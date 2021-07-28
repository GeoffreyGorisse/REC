using UnityEngine;
using System.Collections.Generic;

public class EventManager : MonoBehaviour
{
    public static EventManager Instance { get; private set; }
    public int CurrentEventRef { get; private set; }

    [SerializeField]
    private Event_[] m_events;

    void Awake ()
    {
        if (Instance == null)
            Instance = this;

        else
        {
            Debug.LogWarning("A singleton cannot be instanciated twice");
            Destroy(this.gameObject);
        }
    }

    void Start ()
    {
        CurrentEventRef = -1;

        m_events = GetComponentsInChildren<Event_>(true);

        if (m_events.Length > 0)
        {
            foreach (Event_ event_ in m_events)
                event_.gameObject.SetActive(false);

            NextEvent();
        }

        else
            Debug.LogWarning("No event assigned to the event manager!");

    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.RightArrow))
        {
            m_events[CurrentEventRef].EventEnd();
        }
    }

    public void NextEvent()
    {
        CurrentEventRef++;

        if (CurrentEventRef == m_events.Length)
        {
            m_events[CurrentEventRef - 1].gameObject.SetActive(false);
            
            Debug.Log("End");
            return;
        }

        else
        {
            for (int i = 0; i < m_events.Length; i++)
            {
                bool eventActivation = i == CurrentEventRef ? true : false;
                m_events[i].gameObject.SetActive(eventActivation);
            }
        }
    }
}