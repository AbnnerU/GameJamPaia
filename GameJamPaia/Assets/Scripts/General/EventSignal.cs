using System;
using UnityEngine;

public class EventSignal : MonoBehaviour
{


    public Action OnSendSignal;

    public void SendSignal()
    {
        OnSendSignal?.Invoke();
    }
}
