using UnityEngine;
using System.Collections.Generic;
using System.Globalization;

public class CSVDataRecorder : CSVUtilities
{
    public static CSVDataRecorder Instance { get; private set; }

    [SerializeField] private string _CSVFileNameToExport = "MotionDATA";
    [SerializeField] private GameObject _character;
    [SerializeField] private Transform _rootBone;
    [SerializeField] private float _recordingFrequency = 0.0166f;   // 60 Hz
    [SerializeField] private Event_Record _event_Record;

    private string _directoryPath;
    private string[][] _outputData;
    private bool _initialization = false;
    private Vector3 _characterInitialPosition;
    private float _lastRecordingTime;

    private List<Transform> _bones = new List<Transform>();
    private List<string> _characterScale = new List<string>();
    private List<string> _recordingDelay = new List<string>();
    private List<List<string>> _constDataLists = new List<List<string>>();
    private List<string> _time = new List<string>();
    private List<string> _characterRootPositions = new List<string>();
    private List<string> _characterRootRotations = new List<string>();
    private List<string> _rootBoneLocalPositions = new List<string>();
    private List<List<string>> _bonesLocalRotations = new List<List<string>>();
    private List<List<string>> _continuousDataLists = new List<List<string>>();

    void Awake()
    {
        if (Instance == null)
            Instance = this;

        else
        {
            Debug.LogWarning("A singleton cannot be instanciated twice");
            Destroy(this.gameObject);
        }
    }

    void Update()
    {
        if(_event_Record.isActiveAndEnabled)
        {
            if (_character && _rootBone)
            {
                if(!_initialization)
                {
                    _lastRecordingTime = Time.time;
                    _characterInitialPosition = _character.transform.position;

                    GetBonesInChildren(_character.transform);
                    DataListsInitialization();

                    _initialization = true;
                }

                if (_event_Record.isActiveAndEnabled && _initialization && Time.time >= _lastRecordingTime + _recordingFrequency)
                {
                    ContinuousDataRecorder();
                    _lastRecordingTime = Time.time;
                }

                if (Input.GetKeyDown(KeyCode.S))
                    DATACollection();
            }

            else
                Debug.LogWarning("No character and/or root bone to record the motion from!");
        }
    }

    private void GetBonesInChildren(Transform root)
    {
        if (root.tag == "Bone")
            _bones.Add(root);

        if (root.childCount > 0)
            foreach (Transform child in root.transform)
                GetBonesInChildren(child);
    }

    private void DataListsInitialization()
    {
        _characterScale.Add("Character_Scale");
        _characterScale.Add(_character.transform.localScale.ToString());
        _constDataLists.Add(_characterScale);

        _recordingDelay.Add("Recording_Delay");
        _recordingDelay.Add(_recordingFrequency.ToString(CultureInfo.InvariantCulture));
        _constDataLists.Add(_recordingDelay);

        _time.Add("Time");
        _characterRootPositions.Add("Character_Root_Position");
        _characterRootRotations.Add("Character_Root_Rotation");
        _rootBoneLocalPositions.Add("Root_Bone_Local_Position");

        for (int i = 0; i < _bones.Count; i++)
        {
            List<string> boneRotations = new List<string>();
            boneRotations.Add(_bones[i].name);
            _bonesLocalRotations.Add(boneRotations);
        }
    }

    private void ContinuousDataRecorder()
    {
        float time = Time.realtimeSinceStartup;
        time = Mathf.Round(time * 100f) / 100f;
        _time.Add(time.ToString(CultureInfo.InvariantCulture));

        _characterRootPositions.Add((_character.transform.position - _characterInitialPosition).ToString("N3"));
        _characterRootRotations.Add(_character.transform.eulerAngles.ToString());

        _rootBoneLocalPositions.Add(_rootBone.localPosition.ToString("N3"));

        for (int i = 0; i < _bonesLocalRotations.Count; i++)
            _bonesLocalRotations[i].Add(_bones[i].localEulerAngles.ToString());
    }

    public void DATACollection()
    {
        ContinuousDataCollection();

        _outputData = new string[_constDataLists.Count + _continuousDataLists.Count][];

        int outputDataIndex = 0;

        for (int i = 0; i < _constDataLists.Count; i++)
        {
            _outputData[outputDataIndex] = _constDataLists[i].ToArray();
            outputDataIndex++;
        }

        for (int i = 0; i < _continuousDataLists.Count; i++)
        {
            _outputData[outputDataIndex] = _continuousDataLists[i].ToArray();
            outputDataIndex++;
        }

        _directoryPath = Application.dataPath + @"/StreamingAssets/DATA/";
        SaveCSV(_directoryPath, _CSVFileNameToExport /*+ System.DateTime.Now.ToString("yyyy-MM-dd_HH-mm")*/ + ".csv", _outputData);
    }

    private void ContinuousDataCollection()
    {
        _continuousDataLists.Add(_time);
        _continuousDataLists.Add(_characterRootPositions);
        _continuousDataLists.Add(_characterRootRotations);
        _continuousDataLists.Add(_rootBoneLocalPositions);

        for (int i = 0; i < _bonesLocalRotations.Count; i++)
            _continuousDataLists.Add(_bonesLocalRotations[i]);
    }
}