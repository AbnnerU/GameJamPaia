
using UnityEngine;

public class SimpleAnimationManager : MonoBehaviour
{
    [SerializeField] private bool active = true;
    [SerializeField] private InputManager inputManager;
    [SerializeField] private Animator anim;
    [SerializeField] private string walkAnimation;
    [SerializeField] private string idleAnimation;

    private void Awake()
    {
        if(inputManager ==null)
            inputManager = FindObjectOfType<InputManager>();

        inputManager.OnMoveInput += OnMoveInput;

        anim.Play(idleAnimation, 0, 0);

    }

    private void OnMoveInput(Vector2 vector)
    {
        if (!active) return;

        if (vector == Vector2.zero)
            anim.Play(idleAnimation, 0, 0);
        else
            anim.Play(walkAnimation, 0, 0);
    }

    public void PlayAnimation(string animationName)
    {
        anim.Play(animationName, 0, 0);
    }

    public void SetAnimationManagerActive(bool enabled)
    {
        active = enabled;
    }

    private void OnDestroy()
    {
        inputManager.OnMoveInput -= OnMoveInput;
    }
}
