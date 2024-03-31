
using UnityEngine;

public class NegativeEffect_Stun : MonoBehaviour
{
    [SerializeField] private string targetTag;
    [SerializeField] private float time;
    [SerializeField] private bool releaseOnTrigger;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag(targetTag))
        {
            NegativeEffects movement = collision.GetComponent<NegativeEffects>();

            if (movement != null)
            {
                movement.Stun(time);

                if (releaseOnTrigger)
                {
                    gameObject.SetActive(false);
                }
            }
        }
    }
}
