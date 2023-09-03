using System;
using UnityEngine;

public abstract class GameScore : MonoBehaviour
{
    [SerializeField] protected bool active = true;

    [SerializeField] protected ScoreName scoreName;

    public Action<int> OnScoreChange;

    public Action<int> OnValueAdded;

    protected float score;

    public abstract void AddPoints(int value);

    public abstract void RemovePoints(int value);

    public abstract void StopThisGameScore();
    
    public ScoreName GetScoreName()
    {
        return scoreName;
    }

    public float GetScore()
    {
        return score;
    }

    public enum ScoreName
    {
        MAIN
    }
}

//public abstract class GameScoreAgent:MonoBehaviour
//{
//    [SerializeField] protected string scoreName;

//    [SerializeField] protected int points;

//    protected GameScore gameScore=null;

//    protected void FindGameScore()
//    {
//        GameScore[] allGameScore = FindObjectsOfType<GameScore>();

//        for(int i = 0; i < allGameScore.Length; i++)
//        {
//            if(allGameScore[i].GetScoreName() == scoreName)
//            {
//                gameScore = allGameScore[i];
//                break;
//            }

//        }

//        if (gameScore == null)
//            Debug.LogError("Game score of type " + scoreName.ToString() + " not exist");
//    }
//}

public abstract class ScoreVisual:MonoBehaviour
{
    public abstract void UpdateAll();
}
