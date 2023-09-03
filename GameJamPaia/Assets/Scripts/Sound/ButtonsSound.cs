using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ButtonsSound : MonoBehaviour
{
    [SerializeField] private Button[] buttons;
    [SerializeField] private bool getButtonsInChildren=true;

    [Header("Sound")]
    [SerializeField] private AudioChannel channel;
    [SerializeField] private AudioConfig audioConfig;

    private void Awake()
    {
        if (getButtonsInChildren)
        {
            buttons = GetComponentsInChildren<Button>(true);
        }

        foreach(Button b in buttons)
        {
            b.onClick.AddListener(OnClickSound);
        }
    }

    private void OnClickSound()
    {
        channel.AudioRequest(audioConfig, Vector3.zero);
    }
}
