using Assets.Scripts.GameAction;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundInterpolationAction : GameAction
{
    [SerializeField] private SoundInterpolation soundInterpolation;

    [Header("Sound")]
    [SerializeField] private AudioConfig audioConfig;

    private void Awake()
    {
        soundInterpolation = FindFirstObjectByType<SoundInterpolation>();   
    }

    public override void DoAction()
    {
        soundInterpolation.PlaySound(audioConfig);
    }


}
