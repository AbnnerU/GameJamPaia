
using UnityEngine;

public class OnTriggerAnimation : MonoBehaviour
{
    [SerializeField] private string targetTag;
    [SerializeField] private bool getAnimatorByTag;
    [SerializeField] private Animator anim;
    [SerializeField] private string animationName;
    [SerializeField] private string animatorTag;

    private void Awake()
    {
        if(getAnimatorByTag)
            anim = GameObject.FindGameObjectWithTag(animatorTag).GetComponent<Animator>();
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag(targetTag))
        {
            anim.Play(animationName, 0, 0);
        }
    }
}
