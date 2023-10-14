
public class AITurnOnAlarm : AIBasicBehaviour
{
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
            int id;
            gameManager.PauseAlarmsLoop(true);
            mapManager.SetRandomRoom(transformsArray, offSetArray, out id);

            mapManager.EneableAlarmOnRoom(id);

        }
    }

   
}
