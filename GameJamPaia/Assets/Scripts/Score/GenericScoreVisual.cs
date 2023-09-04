
using UnityEngine;

using TMPro;

public class GenericScoreVisual : ScoreVisual
{
    [SerializeField] private GameScore gameScoreRef;

    [SerializeField] private ScoreTextConfig[] scoreTexts;

    private void Awake()
    {
        gameScoreRef.OnScoreChange += GameScore_OnPointsChange;
    }

    private void GameScore_OnPointsChange(int value)
    {
        for (int i = 0; i < scoreTexts.Length; i++)
        {
            if(scoreTexts[i].alwaysUpdate)
                scoreTexts[i].textComponent.text = value.ToString();
        }
    }

    public override void UpdateAll()
    {
        int currentScore = (int)gameScoreRef.GetScore();

        for (int i = 0; i < scoreTexts.Length; i++)
        {
            scoreTexts[i].textComponent.text = currentScore.ToString();
        }
    }
}


[System.Serializable]
public struct ScoreTextConfig
{
    public TMP_Text textComponent;
    public bool alwaysUpdate;
}

