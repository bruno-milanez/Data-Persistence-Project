using System;
using System.IO;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
#if UNITY_EDITOR
using UnityEditor;
#endif

// Sets the script to be executed later than all default scripts
// This is helpful for UI, since other things may need to be initialized before setting the UI
[DefaultExecutionOrder(1000)]
public class MenuUIHandler : MonoBehaviour
{
    public Text BestScoreText;

    private void Awake()
    {
        if (GameManager.Instance.BestScore > 0)
            BestScoreText.text = $"Best Score : {GameManager.Instance.BestScoreName} : {GameManager.Instance.BestScore}";
        else
            BestScoreText.text = "";
    }

    public void SetName(string name)
    {
        GameManager.Instance.PlayerName = name;
    }

    public void StartGame()
    {
        SceneManager.LoadScene(0);
    }

    public void ExitApplication()
    {
        GameManager.Instance.Exit();
    }
}
