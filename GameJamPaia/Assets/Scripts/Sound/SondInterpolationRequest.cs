
using UnityEngine;

public class SondInterpolationRequest : MonoBehaviour
{
    [SerializeField] private SoundInterpolation soundInterpolation;
    [SerializeField] private bool requestOnStart;

    [Header("Sound")]
    [SerializeField] private AudioConfig audioConfig;

    private void Awake()
    {
        if(soundInterpolation==null)
            soundInterpolation = FindFirstObjectByType<SoundInterpolation>();
    }

    private void Start()
    {
        if (requestOnStart)
        {
            soundInterpolation?.PlaySound(audioConfig);
        }
    }

}
