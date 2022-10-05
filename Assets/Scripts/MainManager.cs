using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MainManager : MonoBehaviour
{
    const int maxLineCount = 6;

    public Brick BrickPrefab;
    public int LineCount = maxLineCount;
    public Rigidbody Ball;

    public Text ScoreText;
    public Text BestScoreText;
    public GameObject GameOverText;
    public Text LevelUpText;

    private bool m_Started = false;
    private int m_Points;
    
    private bool m_GameOver = false;
    private int m_BricksLeft = 0;
    private int m_Level = 1;

    private IEnumerator m_Coroutine;

    private Paddle m_Paddle;
    private Vector3 m_BallStartPos;
    private Vector3 m_PaddleStartPos;
    private Vector3 m_BallVelocity = Vector3.zero;

    private void Awake()
    {
        m_BallStartPos = Ball.transform.position;
        m_Paddle = FindObjectOfType<Paddle>();
        m_PaddleStartPos = m_Paddle.transform.position;
    }

    // Start is called before the first frame update
    void Start()
    {
        BestScoreText.text = SetBestScore();

        const float step = 0.6f;
        int perLine = Mathf.FloorToInt(4.0f / step);
        
        int[] pointCountArray = new [] {1,1,2,2,5,5};
        for (int i = 0; i < LineCount; ++i)
        {
            for (int x = 0; x < perLine; ++x)
            {
                Vector3 position = new Vector3(-1.5f + step * x, 2.5f + i * 0.3f, 0);
                var brick = Instantiate(BrickPrefab, position, Quaternion.identity);
                brick.PointValue = pointCountArray[i];
                brick.onDestroyed.AddListener(AddPoint);
                m_BricksLeft++;
            }
        }

        Debug.Log("Bricks: " + m_BricksLeft);
    }

    private string SetBestScore()
    {
        if (GameManager.Instance.BestScore > 0)
            return $"Best Score : {GameManager.Instance.BestScoreName} : {GameManager.Instance.BestScore}";
        else
            return "";
    }

    private void Update()
    {
        if (!m_Started)
        {
            if (Input.GetKeyDown(KeyCode.Space))
            {
                Debug.Log("Space pressed");
                m_Started = true;
                float randomDirection = Random.Range(-1.0f, 1.0f);
                Vector3 forceDir = new Vector3(randomDirection, 1, 0);
                forceDir.Normalize();

                Ball.transform.SetParent(null);
                Ball.velocity = m_BallVelocity;
                Ball.AddForce(forceDir * (1 + m_Level), ForceMode.VelocityChange);
            }

            return;
        }
        
        if (m_GameOver)
        {
            if (Input.GetKeyDown(KeyCode.Space))
            {
                SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
            }
            else if (Input.GetKeyDown(KeyCode.Escape))
            {
                SceneManager.LoadScene(0);
            }

            return;
        }
        
        if (m_BricksLeft == 0)
        {
            LevelUp();
            return;
        }
    }

    void AddPoint(int point)
    {
        AddPoint(point, true);
    }

    void AddBonus(int point)
    {
        AddPoint(point, false);
    }

    void AddPoint(int point, bool decreaseBrick)
    {
        if (decreaseBrick)
            m_BricksLeft--;
        
        m_Points += point;
        ScoreText.text = $"Score : {m_Points}";
    }

    void LevelUp()
    {
        if (LineCount < maxLineCount)
            LineCount++;

        RestartBallPosition();
        Start();
        int bonusPoints = m_Level * 10;
        AddBonus(bonusPoints);
        LevelUpText.text = $"LEVEL UP!\n{bonusPoints} Bonus Points";
        LevelUpText.gameObject.SetActive(true);
        m_Coroutine = FadeLevelUpText();
        StartCoroutine(m_Coroutine);
        m_Level++;
    }

    private void RestartBallPosition()
    {
        Ball.position = m_BallStartPos;
        m_Paddle.transform.position = m_PaddleStartPos;
    }

    private IEnumerator FadeLevelUpText()
    {
        yield return new WaitForSeconds(2.0f);
        LevelUpText.gameObject.SetActive(false);
    }

    public void GameOver()
    {
        m_GameOver = true;
        SaveBestScore();
        GameOverText.SetActive(true);
    }

    private void SaveBestScore()
    {
        if (m_Points > GameManager.Instance.BestScore)
        {
            GameManager.Instance.BestScoreName = GameManager.Instance.PlayerName;
            GameManager.Instance.BestScore = m_Points;
            GameManager.Instance.Save();
        }
    }
}
