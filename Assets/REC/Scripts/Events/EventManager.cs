using UnityEngine;

public class EventManager : MonoBehaviour
{
    public static EventManager Instance { get; private set; }
    public int CurrentEventRef { get; private set; }

    [SerializeField] private Event_[] _events;

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

        _events = GetComponentsInChildren<Event_>(true);

        if (_events.Length > 0)
        {
            foreach (Event_ event_ in _events)
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
            _events[CurrentEventRef].EventEnd();
        }
    }

    public void NextEvent()
    {
        CurrentEventRef++;

        if (CurrentEventRef == _events.Length)
        {
            _events[CurrentEventRef - 1].gameObject.SetActive(false);
            
            Debug.Log("End");
            return;
        }

        else
        {
            for (int i = 0; i < _events.Length; i++)
            {
                bool eventActivation = i == CurrentEventRef ? true : false;
                _events[i].gameObject.SetActive(eventActivation);
            }
        }
    }
}