using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;

public class CSVDataRecorder : CSVUtilities
{
    public static CSVDataRecorder Instance { get; private set; }

    public string CSVFileNameToExport = "MotionDATA";
    public GameObject character;
    public Transform rootBone;
    public float recordingDelay = 0.0166f; //60 Hz
    public Event_Record event_Record;

    private string m_directoryPath;
    private string m_fileName;
    private string[][] m_outputData;
    private bool m_initialization = false;
    private Vector3 m_characterInitialPosition;
    private float m_lastRecordingTime;

    private List<Transform> m_bones = new List<Transform>();
    private List<string> m_characterScale = new List<string>();
    private List<string> m_recordingDelay = new List<string>();
    private List<List<string>> m_constDataList = new List<List<string>>();
    private List<string> m_time = new List<string>();
    private List<string> m_characterRootPositions = new List<string>();
    private List<string> m_characterRootRotations = new List<string>();
    private List<string> m_rootBoneLocalPositions = new List<string>();
    private List<List<string>> m_bonesLocalRotations = new List<List<string>>();
    private List<List<string>> m_continuousDataList = new List<List<string>>();

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
        if(event_Record.isActiveAndEnabled)
        {
            if (character && rootBone)
            {
                if(!m_initialization)
                {
                    m_lastRecordingTime = Time.time;
                    m_characterInitialPosition = character.transform.position;

                    GetBonesInChildren(character.transform);
                    DataListsInitialization();

                    m_initialization = true;
                }

                if (event_Record.isActiveAndEnabled && m_initialization && Time.time >= m_lastRecordingTime + recordingDelay)
                {
                    ContinuousDataRecorder();
                    m_lastRecordingTime = Time.time;
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
            m_bones.Add(root);

        if (root.childCount > 0)
            foreach (Transform child in root.transform)
                GetBonesInChildren(child);
    }

    private void DataListsInitialization()
    {
        m_characterScale.Add("Character_Scale");
        m_characterScale.Add(character.transform.localScale.ToString());
        m_constDataList.Add(m_characterScale);

        m_recordingDelay.Add("Recording_Delay");
        m_recordingDelay.Add(recordingDelay.ToString(CultureInfo.InvariantCulture));
        m_constDataList.Add(m_recordingDelay);

        m_time.Add("Time");
        m_characterRootPositions.Add("Character_Root_Position");
        m_characterRootRotations.Add("Character_Root_Rotation");
        m_rootBoneLocalPositions.Add("Root_Bone_Local_Position");

        for (int i = 0; i < m_bones.Count; i++)
        {
            List<string> boneRotations = new List<string>();
            boneRotations.Add(m_bones[i].name);
            m_bonesLocalRotations.Add(boneRotations);
        }
    }

    private void ContinuousDataRecorder()
    {
        float time = Time.realtimeSinceStartup;
        time = Mathf.Round(time * 100f) / 100f;
        m_time.Add(time.ToString(CultureInfo.InvariantCulture));

        m_characterRootPositions.Add((character.transform.position - m_characterInitialPosition).ToString("N3"));
        m_characterRootRotations.Add(character.transform.eulerAngles.ToString());

        m_rootBoneLocalPositions.Add(rootBone.localPosition.ToString("N3"));

        for (int i = 0; i < m_bonesLocalRotations.Count; i++)
            m_bonesLocalRotations[i].Add(m_bones[i].localEulerAngles.ToString());
    }

    public void DATACollection()
    {
        ContinuousDataCollection();

        m_outputData = new string[m_constDataList.Count + m_continuousDataList.Count][];

        int outputDataIndex = 0;

        for (int i = 0; i < m_constDataList.Count; i++)
        {
            m_outputData[outputDataIndex] = m_constDataList[i].ToArray();
            outputDataIndex++;
        }

        for (int i = 0; i < m_continuousDataList.Count; i++)
        {
            m_outputData[outputDataIndex] = m_continuousDataList[i].ToArray();
            outputDataIndex++;
        }

        m_directoryPath = Application.dataPath + @"/StreamingAssets/DATA/";
        m_fileName = CSVFileNameToExport /*+ System.DateTime.Now.ToString("yyyy-MM-dd_HH-mm")*/ + ".csv";
        SaveCSV(m_directoryPath, m_fileName, m_outputData);
    }

    private void ContinuousDataCollection()
    {
        m_continuousDataList.Add(m_time);
        m_continuousDataList.Add(m_characterRootPositions);
        m_continuousDataList.Add(m_characterRootRotations);
        m_continuousDataList.Add(m_rootBoneLocalPositions);

        for (int i = 0; i < m_bonesLocalRotations.Count; i++)
            m_continuousDataList.Add(m_bonesLocalRotations[i]);
    }
}