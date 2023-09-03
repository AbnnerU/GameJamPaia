using System.Collections;
using UnityEngine;


namespace Assets.Scripts.BT
{
    public class BTWaitForSeconds : BTnode
    {
        private float time;

        private float minTime;
        private float maxTime;

        private bool useRandom;

        private float value;
        public BTWaitForSeconds(float time)
        {
            this.time = time;
            useRandom = false;
        }

        public BTWaitForSeconds(float minTime, float maxTime)
        {
            this.minTime = minTime;
            this.maxTime = maxTime;
            useRandom = true;
        }

        public override IEnumerator Run(BehaviorTree behaviorTree)
        {
            status = BTstatus.RUNNING;

            if (useRandom)
                value = Random.Range(minTime, maxTime);
            else
                value = time;


            yield return new WaitForSeconds(value);

            status = BTstatus.SUCCESS;

            yield break;
        }
    }
}