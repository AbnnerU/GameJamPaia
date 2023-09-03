using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace Assets.Scripts.BT
{
    public class BTConditionalSequence : BTnode
    {
        private List<BTnode> conditionsNodes;

        private BTnode mainNode;

        private Dictionary<BTnode, Coroutine> runningNodes;

        public BTConditionalSequence(List<BTnode> conditionsNodes, BTnode mainNode)
        {
            this.conditionsNodes = conditionsNodes;
            this.mainNode = mainNode;

            runningNodes = new Dictionary<BTnode, Coroutine>();
        }

        public override IEnumerator Run(BehaviorTree behaviorTree)
        {

            status = BTstatus.RUNNING;

            Coroutine mainCoroutine = behaviorTree.StartCoroutine(mainNode.Run(behaviorTree));

            if (mainNode.GetStatus() != BTstatus.RUNNING)
            {
                status = mainNode.GetStatus();

                yield break;
            }

            do
            {
                for (int i = 0; i < conditionsNodes.Count; i++)
                {
                    BTnode node = conditionsNodes[i];

                    if (!runningNodes.ContainsKey(node))
                        runningNodes.Add(node, behaviorTree.StartCoroutine(node.Run(behaviorTree)));

                    if (node.GetStatus() != BTstatus.RUNNING)
                        runningNodes.Remove(node);

                    if (node.GetStatus() == BTstatus.FAILURE)
                    {
                        behaviorTree.StopCoroutine(mainCoroutine);

                        status = BTstatus.FAILURE;

                        yield break;
                    }
                }

                if (mainNode.GetStatus() != BTstatus.RUNNING)
                {
                    status = mainNode.GetStatus();
                    behaviorTree.StopCoroutine(mainCoroutine);
                    yield break;
                }

                yield return null;
            } while (status == BTstatus.RUNNING);


            behaviorTree.StopCoroutine(mainCoroutine);

            if (runningNodes.Count > 0)
            {
                foreach (Coroutine coroutine in runningNodes.Values)
                {
                    behaviorTree.StopCoroutine(coroutine);
                }
            }

            yield break;
        }


    }
}