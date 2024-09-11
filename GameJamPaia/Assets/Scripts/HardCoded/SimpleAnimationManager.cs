
using UnityEngine;

public class SimpleAnimationManager : MonoBehaviour
{
    [SerializeField] private bool active = true;
    [SerializeField] private InputManager inputManager;
    [SerializeField] private PlayerMovement playerMovement;
    [SerializeField] private Animator anim;
    [SerializeField] private string walkAnimation;
    [SerializeField] private string idleAnimation;

    private bool idle=true;

    private void Awake()
    {
        if(inputManager ==null)
            inputManager = FindObjectOfType<InputManager>();

        inputManager.OnMoveInput += OnMoveInput;

        anim.Play(idleAnimation, 0, 0);

        playerMovement.OnInputValueChange += OnMoveInput;

    }

    private void OnMoveInput(Vector2 vector)
    {
        if (!active) return;

        if (idle && vector == Vector2.zero) return;
        else if (!idle && vector != Vector2.zero) return;

        if (vector == Vector2.zero)
        {
            anim.Play(idleAnimation, 0, 0);
            idle = true;
        }
        else
        {
            anim.Play(walkAnimation, 0, 0);
            idle = false;
        }
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
