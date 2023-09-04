
using UnityEngine;

public class SimpleAnimationManager : MonoBehaviour
{
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
        if (vector == Vector2.zero)
            anim.Play(idleAnimation, 0, 0);
        else
            anim.Play(walkAnimation, 0, 0);
    }


    private void OnDestroy()
    {
        inputManager.OnMoveInput -= OnMoveInput;
    }
}
