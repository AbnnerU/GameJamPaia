using System.Collections;
using UnityEngine;

public class ParticleHandler : MonoBehaviour
{
    [SerializeField] private ParticleSystem particleRef;
    [SerializeField] private bool playOnEnable;
    [SerializeField] private bool releaseOnStop;

    private void OnEnable()
    {
        if (playOnEnable)
        {
            particleRef.Play();

            if(releaseOnStop)
                StartCoroutine(DisableOnEnd());
        }
    }

    IEnumerator DisableOnEnd()
    {
        do
        {
            yield return null;
        } while (particleRef.isPlaying);

        PoolManager.ReleaseObject(particleRef.gameObject);
    }
}
