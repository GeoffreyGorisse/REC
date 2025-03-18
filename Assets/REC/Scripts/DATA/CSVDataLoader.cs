using UnityEngine;
using System.Collections.Generic;
using System.Globalization;

public class CSVDataLoader : CSVUtilities
{
    public static CSVDataLoader Instance { get; private set; }

    [SerializeField] private string _CSVFileNameToLoad = "MotionDATA";
    [SerializeField] private GameObject _character;
    [SerializeField] private Transform _rootBone;
    [SerializeField] private Event_Replay _event_Replay;

    private string _directoryPath;
    private string[][] _inputData;
    private bool _dataRead = false;
    private bool _characterDataInitialized = false;
    private bool _initialization = false;
    private Vector3 _characterInitialPosition;
    private float _lastReplayingTime;
    private int _replayingIndex = 0;
    private float _replayingDelay;

    private List<Transform> _bones = new List<Transform>();
    private List<string> _characterScale = new List<string>();
    private List<string> _recordingDelay = new List<string>();
    private List<string> _characterRootPositions = new List<string>();
    private List<string> _characterRootRotations = new List<string>();
    private List<string> _rootBoneLocalPositions = new List<string>();
    private List<List<string>> _bonesLocalRotations = new List<List<string>>();

    void Awake()
    {
        if (Instance == null)
            Instance = this;

        else
        {
            Debug.LogWarning("A singleton cannot be instanciated twice!");
            Destroy(this.gameObject);
        }
    }

    void Update()
    {
        if (_event_Replay.isActiveAndEnabled)
        {
            if(_character && _rootBone)
            {
                if (!_initialization)
                {
                    _lastReplayingTime = Time.time;
                    _characterInitialPosition = _character.transform.position;

                    ReadCSVData();
                    InitializeCharacterData();

                    if (_dataRead && _characterDataInitialized)
                        _initialization = true;

                    else
                    {
                        Debug.LogError("Initialization failed!");
                        return;
                    }
                }

                else if (_initialization && _replayingIndex < _bonesLocalRotations[0].Count - 1 && Time.time >= _lastReplayingTime + _replayingDelay)
                {
                    _replayingIndex++;
                    ReplayMotion(_replayingIndex);
                    _lastReplayingTime = Time.time;

                    if (_replayingIndex == _bonesLocalRotations[0].Count - 1)
                        Debug.Log("Motion replaying end.");
                }
            }

            else
                Debug.LogWarning("No character and/or root bone to apply the recorded motion!");
        }
    }

    private void ReadCSVData()
    {
        _directoryPath = Application.dataPath + @"/StreamingAssets/DATA/";

        try
        {
            _inputData = ReadCSV(_directoryPath, _CSVFileNameToLoad + ".csv");
            _dataRead = true;
        }

        catch (System.Exception)
        {
            Debug.LogWarning("Cannot access CSV file! Please check the file path.");
        }
    }

    private void InitializeCharacterData()
    {

        if(LoadData("Character_Scale", out _characterScale) &&
           LoadData("Recording_Delay", out _recordingDelay) &&
           LoadData("Character_Root_Position", out _characterRootPositions) &&
           LoadData("Character_Root_Rotation", out _characterRootRotations) &&
           LoadData("Root_Bone_Local_Position", out _rootBoneLocalPositions))
        {
            _character.transform.localScale = StringToVector3(_characterScale[1]);
            _replayingDelay = float.Parse(_recordingDelay[1], CultureInfo.InvariantCulture);

            for (int i = 0; i < _inputData.Length; i++)
                GetBonesRotationData(_character.transform, _inputData[i][0], i);

            _characterDataInitialized = true;
        }

        else
        {
            _characterDataInitialized = false;
            Debug.LogError("Character data initialization has failed!");
        }
    }

    private bool LoadData(string inputDataName, out List<string> dataListToBeInitialized)
    {
        dataListToBeInitialized = new List<string>();

        for (int i = 0; i < _inputData.Length; i++)
        {
            if (_inputData[i][0] == inputDataName)
            {
                for (int j = 0; j < _inputData[i].Length; j++)
                {
                    if (string.IsNullOrEmpty(_inputData[i][j]))
                    {
                        Debug.LogError(inputDataName + " incorrect or missing data!");
                        return false;
                    }
                }

                dataListToBeInitialized.AddRange(_inputData[i]);
                return true;
            }
        }

        Debug.LogError(inputDataName + " missing data!");
        return false;
    }

    private void GetBonesRotationData(Transform root, string boneName, int currentIterator)
    {
        if (root.name == boneName)
        {
            _bones.Add(root);

            List<string> boneRotations = new List<string>();

            for (int j = 0; j < _inputData[currentIterator].Length; j++)
            {
                if (string.IsNullOrEmpty(_inputData[currentIterator][j]))
                {
                    Debug.LogError(boneName + "local rotation incorrect or missing data!");
                    return;
                }
            }
            
            boneRotations.AddRange(_inputData[currentIterator]);
            _bonesLocalRotations.Add(boneRotations);
            return;
        }

        else if (root.childCount > 0)
            foreach (Transform child in root.transform)
                GetBonesRotationData(child, boneName, currentIterator);
    }

    private void ReplayMotion(int dataIndex)
    {
        string tmpStr;
        Vector3 tmpV;

        tmpStr = _characterRootPositions[dataIndex];
        tmpV = StringToVector3(tmpStr);
        _character.transform.position = _characterInitialPosition + tmpV;

        tmpStr = _characterRootRotations[dataIndex];
        tmpV = StringToVector3(tmpStr);
        _character.transform.rotation = Quaternion.Euler(tmpV);

        tmpStr = _rootBoneLocalPositions[dataIndex];
        tmpV = StringToVector3(tmpStr);
        _rootBone.transform.localPosition = tmpV;

        for (int i = 0; i < _bones.Count; i++)
        {
            if (_bones[i].name == _bonesLocalRotations[i][0])
            {
                tmpStr = _bonesLocalRotations[i][dataIndex];
                tmpV = StringToVector3(tmpStr);
                _bones[i].localRotation = Quaternion.Euler(tmpV);
            }
        }
    }
}
