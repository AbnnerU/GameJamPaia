
using UnityEngine;

public class MovementSprintInputBlock : MonoBehaviour
{
    [SerializeField] private MovementSprint movementSprint;
    [SerializeField] private string targetTag;

    private void OnTriggerStay2D(Collider2D collision)
    {
        if (collision.CompareTag(targetTag))
        {
            movementSprint.Disable();
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag(targetTag))
        {
            movementSprint.Enable();
        }
    }

}
