using UnityEngine;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.Globalization;

public class CSVUtilities : MonoBehaviour
{
    protected void SaveCSV(string directoryPath, string fileName, string[][] outputData)
    {
        if (!Directory.Exists(directoryPath))
            Directory.CreateDirectory(directoryPath);

        string filePath = Path.Combine(directoryPath, fileName);

        StringBuilder sb = new StringBuilder();
        string delimiter = ";";

        int length = outputData.GetLength(0);
        for (int index = 0; index < length; index++)
        {
            if (outputData[index] != null)
                sb.AppendLine(string.Join(delimiter, outputData[index]));
        }

        File.WriteAllText(filePath, sb.ToString());

        Debug.Log("DATA saved.");
    }

    protected string[][] ReadCSV(string directoryPath, string fileName)
    {
        string filePath = Path.Combine(directoryPath, fileName);

        StreamReader strReader = new StreamReader(filePath);

        int lineIndex = 0;
        List<string> dataLines = new List<string>();
        bool endOfFile = false;

        while (!endOfFile)
        {
            string dataLine = strReader.ReadLine();

            if (dataLine == null)
            {
                endOfFile = true;
                strReader.Close();

                break;
            }

            lineIndex++;
            dataLines.Add(dataLine);
        }

        string[][] CSVData = new string[lineIndex][];

        for (int i = 0; i < dataLines.Count; i++)
            CSVData[i] = dataLines[i].Split(';');

        Debug.Log("DATA loaded.");

        return CSVData;
    }

    protected Vector3 StringToVector3(string strVector)
    {
        if (strVector.StartsWith("(") && strVector.EndsWith(")"))
            strVector = strVector.Substring(1, strVector.Length - 2);

        string[] vectorValues = strVector.Split(',');

        for (int i = 0; i < vectorValues.Length; i++)
            vectorValues[i] = vectorValues[i].Trim();

        Vector3 result = new Vector3(
            float.Parse(vectorValues[0], CultureInfo.InvariantCulture),
            float.Parse(vectorValues[1], CultureInfo.InvariantCulture),
            float.Parse(vectorValues[2], CultureInfo.InvariantCulture));

        return result;
    }
}
