using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OnTriggerSendToRandomLocation : MonoBehaviour
{
    [SerializeField] private string targetTag = "Player";
    [SerializeField] private Transform playerTransform;
    [SerializeField] private Transform cameraTransform;

    [SerializeField] private SoundInterpolation soundInterpolation;

    [SerializeField] private PositionAndTrack[] positionAndTrack;

    [Header("Sound")]
    [SerializeField] private AudioChannel channel;
    [SerializeField] private AudioConfig audioConfig;

    private void Awake()
    {
        soundInterpolation = FindFirstObjectByType<SoundInterpolation>();
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag(targetTag))
        {
            int id = Random.Range(0, positionAndTrack.Length);

            playerTransform.position = positionAndTrack[id].transformRef.position;
            cameraTransform.position = positionAndTrack[id].transformRef.position + new Vector3(0,0,-10);

            soundInterpolation.PlaySound(positionAndTrack[id].audioConfig);

            channel.AudioRequest(audioConfig, Vector3.zero);
        }
    }


    [System.Serializable]
    private struct PositionAndTrack
    {
        public Transform transformRef;
        public AudioConfig audioConfig;
    }
}
