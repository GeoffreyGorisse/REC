using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;

public class CSVDataLoader : CSVUtilities
{
    public static CSVDataLoader Instance { get; private set; }

    public string CSVFileNameToLoad = "MotionDATA";
    public GameObject character;
    public Transform rootBone;
    public Event_Replay event_Replay;

    private string m_directoryPath;
    private string m_fileName;
    private string[][] m_inputData;
    private bool m_dataRead = false;
    private bool m_characterDataInitialized = false;
    private bool m_initialization = false;
    private Vector3 m_characterInitialPosition;
    private float m_lastReplayingTime;
    private int m_replayingIndex = 0;
    private float m_replayingDelay;

    private List<Transform> m_bones = new List<Transform>();
    private List<string> m_characterScale = new List<string>();
    private List<string> m_recordingDelay = new List<string>();
    private List<string> m_characterRootPositions = new List<string>();
    private List<string> m_characterRootRotations = new List<string>();
    private List<string> m_rootBoneLocalPositions = new List<string>();
    private List<List<string>> m_bonesLocalRotations = new List<List<string>>();



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
        if (event_Replay.isActiveAndEnabled)
        {
            if(character && rootBone)
            {
                if (!m_initialization)
                {
                    m_lastReplayingTime = Time.time;
                    m_characterInitialPosition = character.transform.position;

                    ReadCSVData();
                    InitializeCharacterData();

                    if (m_dataRead && m_characterDataInitialized)
                        m_initialization = true;

                    else
                    {
                        Debug.LogError("Initialization failed!");
                        return;
                    }
                }

                else if (m_initialization && m_replayingIndex < m_bonesLocalRotations[0].Count - 1 && Time.time >= m_lastReplayingTime + m_replayingDelay)
                {
                    m_replayingIndex++;
                    ReplayMotion(m_replayingIndex);
                    m_lastReplayingTime = Time.time;

                    if (m_replayingIndex == m_bonesLocalRotations[0].Count - 1)
                        Debug.Log("Motion replaying end.");
                }
            }

            else
                Debug.LogWarning("No character and/or root bone to apply the recorded motion!");
        }
    }

    private void ReadCSVData()
    {
        m_directoryPath = Application.dataPath + @"/StreamingAssets/DATA/";
        m_fileName = CSVFileNameToLoad + ".csv";

        try
        {
            m_inputData = ReadCSV(m_directoryPath, m_fileName);
            m_dataRead = true;
        }

        catch (System.Exception)
        {
            Debug.LogWarning("Cannot access CSV file! Please check the file path.");
        }
    }

    private void InitializeCharacterData()
    {

        if(DataInitializer("Character_Scale", out m_characterScale) &&
           DataInitializer("Recording_Delay", out m_recordingDelay) &&
           DataInitializer("Character_Root_Position", out m_characterRootPositions) &&
           DataInitializer("Character_Root_Rotation", out m_characterRootRotations) &&
           DataInitializer("Root_Bone_Local_Position", out m_rootBoneLocalPositions))
        {
            character.transform.localScale = StringToVector3(m_characterScale[1]);
            m_replayingDelay = float.Parse(m_recordingDelay[1], CultureInfo.InvariantCulture);

            for (int i = 0; i < m_inputData.Length; i++)
                GetRotationDataForMatchingBoneInChildren(character.transform, m_inputData[i][0], i);

            m_characterDataInitialized = true;
        }

        else
        {
            m_characterDataInitialized = false;
            Debug.LogError("Character data initialization has failed!");
        }
    }

      private bool DataInitializer(string inputDataName, out List<string> dataListToBeInitialized)
    {
        dataListToBeInitialized = new List<string>();

        for (int i = 0; i < m_inputData.Length; i++)
        {
            if (m_inputData[i][0] == inputDataName)
            {
                for (int j = 0; j < m_inputData[i].Length; j++)
                {
                    if (string.IsNullOrEmpty(m_inputData[i][j]))
                    {
                        Debug.LogError(inputDataName + " incorrect or missing data!");
                        return false;
                    }
                }

                dataListToBeInitialized.AddRange(m_inputData[i]);
                return true;
            }
        }

        Debug.LogError(inputDataName + " missing data!");
        return false;
    }

    private void GetRotationDataForMatchingBoneInChildren(Transform root, string boneName, int currentIterator)
    {
        if (root.name == boneName)
        {
            m_bones.Add(root);

            List<string> boneRotations = new List<string>();

            for (int j = 0; j < m_inputData[currentIterator].Length; j++)
            {
                if (string.IsNullOrEmpty(m_inputData[currentIterator][j]))
                {
                    Debug.LogError(boneName + "local rotation incorrect or missing data!");
                    return;
                }
            }
            
            boneRotations.AddRange(m_inputData[currentIterator]);
            m_bonesLocalRotations.Add(boneRotations);
            return;
        }

        else if (root.childCount > 0)
            foreach (Transform child in root.transform)
                GetRotationDataForMatchingBoneInChildren(child, boneName, currentIterator);
    }

    private void ReplayMotion(int dataIndex)
    {
        string tmpStr;
        Vector3 tmpV;

        tmpStr = m_characterRootPositions[dataIndex];
        tmpV = StringToVector3(tmpStr);
        character.transform.position = m_characterInitialPosition + tmpV;

        tmpStr = m_characterRootRotations[dataIndex];
        tmpV = StringToVector3(tmpStr);
        character.transform.rotation = Quaternion.Euler(tmpV);

        tmpStr = m_rootBoneLocalPositions[dataIndex];
        tmpV = StringToVector3(tmpStr);
        rootBone.transform.localPosition = tmpV;

        for (int i = 0; i < m_bones.Count; i++)
        {
            if (m_bones[i].name == m_bonesLocalRotations[i][0])
            {
                tmpStr = m_bonesLocalRotations[i][dataIndex];
                tmpV = StringToVector3(tmpStr);
                m_bones[i].localRotation = Quaternion.Euler(tmpV);
            }
        }
    }
}
