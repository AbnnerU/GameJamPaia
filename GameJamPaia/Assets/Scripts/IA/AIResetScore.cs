
using UnityEngine;


public class AIResetScore : AIBasicBehaviour
{
    [Header("Score")]
    [SerializeField] protected GameScore scoreRef;
    [SerializeField] protected string scoreTag = "Coin";

    protected override void Start()
    {
        base.Start();
       

        if (scoreRef == null)
            scoreRef = GameObject.FindGameObjectWithTag(scoreTag).GetComponent<GameScore>();

    }

    public override void Setup()
    {
        base.Setup();

        if (scoreRef == null)
            scoreRef = GameObject.FindGameObjectWithTag(scoreTag).GetComponent<GameScore>();


        agent.enabled = false;
    }


    protected override void TargetWasCaughtAnimation()
    {
        if (targetAnimator)
        {
            if (shield.IsShieldActive())
            {
                currentState = AIState.STUNNED;
                shield.HitShield();
                return;
            }

            targetMovement.Disable();
            targetCollider.enabled = false;
            targetAnimator.SetAnimationManagerActive(false);
            targetAnimator.PlayAnimation(teleportPlayerAnimation);

            gameManager.PauseAlarmsLoop(true);
        }
    }

    protected override void ReleaseTargetAnimations()
    {
        if (target && mapManager && gameManager)
        {
            int id = 0;
            targetAnimator.PlayAnimation(releasePlayerAnimation);

            mapManager.RandomRoomNoRepeatAndAlarmOff(target, Vector3.zero, out id);
            mapManager.SetObjectInRoom(cameraTransform, new Vector3(0, 0, -10), id);


            scoreRef.RemoveAll();
        }
    }
}
