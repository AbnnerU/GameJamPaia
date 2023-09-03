using System.Collections;
using UnityEngine;


namespace Assets.Scripts.BT
{
    public class BehaviorTree : MonoBehaviour
    {
        [SerializeField] private bool active = true;
        [SerializeField] private bool autoResetOnEnable;
        [SerializeField] private bool stopOnDisable = true;

        private BTnode root;

        private float executioninterval;
   
        public IEnumerator Begin()
        {
            while (active)
            {
                yield return StartCoroutine(root.Run(this));
                yield return new WaitForSeconds(executioninterval);
            }

            yield break;
        }

        public void Stop()
        {
            active = false;
            StopCoroutine(Begin());
        }

        public void SetBehaviorRoot(BTnode rootnode)
        {
            root = rootnode;
        }

        public void SetExecutionInterval(float value)
        {
            executioninterval = value;
        }

        public void SetActive(bool value)
        {
            active = value;
        }
    }
}