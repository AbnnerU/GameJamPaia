
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
       
        agent.enabled = false;
    }


    protected override void GetTargetAnimation()
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
            targetAnimator.SetAnimationManagerActive(false);
            targetAnimator.PlayAnimation(teleportPlayerAnimation);
        }
    }

    protected override void ReleaseTargetAnimations()
    {
        if (target && mapManager && gameManager)
        {
            gameManager.PauseAlarmsLoop(true);
            mapManager.SetRandomRoom(transformsArray, offSetArray);

            scoreRef.RemoveAll();
        }
    }
}
