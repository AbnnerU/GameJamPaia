using UnityEngine;

public class AITurnOnAlarm : AIBasicBehaviour
{
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

            negativeEffects.CancelAll();
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
        

            mapManager.EneableAlarmOnRoom(id);

        }
    }

   
}
