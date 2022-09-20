using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.SceneManagement;
#if UNITY_EDITOR
using UnityEditor;
#endif

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;

    public string PlayerName;
    public int Score;

    public string BestScoreName;
    public int BestScore;

    private string path;

    private void Awake()
    {
        if (Instance != null)
        {
            Destroy(gameObject);
            return;
        }

        path = Application.persistentDataPath + "/savefile.json";
        Instance = this;
        DontDestroyOnLoad(gameObject);
        LoadSavedData();
    }

    private void LoadSavedData()
    {
        if (File.Exists(path))
        {
            string json = File.ReadAllText(path);
            SaveData saveData = JsonUtility.FromJson<SaveData>(json);

            BestScoreName = saveData.BestScoreName;
            BestScore = saveData.Score;
        }
    }

    public void Save()
    {
        SaveData saveData = new SaveData();
        saveData.BestScoreName = BestScoreName;
        saveData.Score = BestScore;

        string json = JsonUtility.ToJson(saveData);

        File.WriteAllText(path, json);
    }

    public void Exit()
    {
        Save();

#if UNITY_EDITOR
        EditorApplication.ExitPlaymode();
#else
        Application.Quit(); // Quit method usable after the build
#endif
    }

    [Serializable]
    class SaveData
    {
        public string BestScoreName;
        public int Score;
    }
}
