using Assets.Scripts.GameAction;
using UnityEngine;

namespace Assets.Scripts.GameAction
{
    public class OnTriggerEnterAction2D : MonoBehaviour
    {
        [SerializeField] private string targetTag = "Player";
        [SerializeField] private GameAction[] actionsToDo;

        private int lenght;

        private void Awake()
        {
            lenght = actionsToDo.Length;
        }

        private void OnTriggerEnter2D(Collider2D collision)
        {
            if (collision.CompareTag(targetTag))
            {
                for (int i = 0; i < lenght; i++)
                {
                    actionsToDo[i].DoAction();
                }
            }
        }
    }
}