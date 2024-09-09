
using System;
using TMPro;
using UnityEngine;

public class LenguajeManager : ManagerBase<LenguajeManager>
{
    [SerializeField] private LenguajeId gameLenguaje;

    private void Awake()
    {
        Setup(this);
    }

    public LenguajeId GameLenguaje()
    {
        return gameLenguaje;
    }
}

public enum LenguajeId
{
    PT,
    EN
}

[Serializable]
public struct LenguajeTexts
{
    public LenguajeId LenguajeId;
    [TextArea]
    public string text;
}