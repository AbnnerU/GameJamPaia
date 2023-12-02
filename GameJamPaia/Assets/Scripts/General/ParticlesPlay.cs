
using UnityEngine;

public class ParticlesPlay : MonoBehaviour
{
    [SerializeField]private ParticleSystem[] particleSystems;

    public void PlayParticle(int id)
    {
        particleSystems[id].Play();
    }
}
