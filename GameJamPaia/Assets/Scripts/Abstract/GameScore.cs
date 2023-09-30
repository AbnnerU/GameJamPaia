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

    public abstract void RemoveAll();
    
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
        MAIN,
        COINS
    }
}

public abstract class ScoreVisual:MonoBehaviour
{
    public abstract void UpdateAll();
}
