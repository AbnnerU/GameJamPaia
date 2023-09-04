using UnityEngine;
using System.Collections;
using UnityEngine.Events;

[System.Serializable]
public class HighScoreEvent: UnityEvent { }

public class HighScore : GameScore
{
    [SerializeField] private GameScore gameScoreReference;

    [SerializeField] private bool playEventOnAwake;

    public HighScoreEvent OnNewHighScore;

    private int currentHighScore;
   
    private void Awake()
    {
        currentHighScore = Save.LoadHighScore();
      
    }


    private void Start()
    {
        if (playEventOnAwake)
            ScoreEvent();
    }


    public void TrySetNewHighScore()
    { 

        int currentScore = (int)gameScoreReference.GetScore();

        if (currentHighScore != -1)
        {           
            if (currentScore > currentHighScore)
            {
                SetNewHighScore(currentScore);
            }
        }
        else
        {
            SetNewHighScore(currentScore);
        }
    }

    private void SetNewHighScore(int value)
    {
        OnScoreChange?.Invoke(value);

        HighScoreValue score = new HighScoreValue()
        {
            highScore = value
        };

        Save.SaveHighScore(score);

        currentHighScore = value;

        OnNewHighScore?.Invoke();
    }

    public void ScoreEvent()
    {
        OnScoreChange?.Invoke(currentHighScore);
    }

    public int GetHighScore()
    {
        return currentHighScore;
    }

    public override void AddPoints(int value)
    {
        Debug.LogWarning("cant add points to high score");
    }

    public override void RemovePoints(int value)
    {
        Debug.LogWarning("cant remove points of high score");
    }

    public override void StopThisGameScore()
    {
        active = false;
    }
}
