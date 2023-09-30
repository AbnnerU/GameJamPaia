using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GenericScore : GameScore
{
    public override void AddPoints(int value)
    {
        if (active == false)
            return;

        score += value;

        OnValueAdded?.Invoke(value);

        OnScoreChange?.Invoke((int)score);
    }

    public override void RemoveAll()
    {
        if (active == false)
            return;

        score = 0;

        OnScoreChange?.Invoke(0);
    }

    public override void RemovePoints(int value)
    {
        if (active == false)
            return;

        score -= value;

        OnScoreChange?.Invoke((int)score);
    }

    public override void StopThisGameScore()
    {
        active = false;
    }

    private void OnDisable()
    {
        active = false;
    }

    private void OnEnable()
    {
        active = true;
    }
}
