using UnityEngine;
using System.IO;
//using System.Runtime.Serialization.Formatters.Binary;
//using Newtonsoft.Json;

public static class Save 
{
    public static void SaveHighScore(HighScoreValue value)
    {
        string path = Application.persistentDataPath + "/highSocre.score";

        var content = JsonUtility.ToJson(value);

        File.WriteAllText(path, content);
    }

    public static int LoadHighScore()
    {
        string path = Application.persistentDataPath + "/highSocre.score";

        //Debug.Log(path);

        if (File.Exists(path))
        {
            string content = File.ReadAllText(path);

            HighScoreValue data = JsonUtility.FromJson<HighScoreValue>(content);

            return data.highScore;
        }
        else
        {
            return -1;
        }
    }

}

[System.Serializable]
public struct HighScoreValue
{
    public int highScore;
}

