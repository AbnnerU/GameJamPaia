
public interface IAIState 
{
    public AIState GetCurrentAIState();

    public void SetAIState(AIState newState);
}

public enum AIState
{
    SPAWNING,
    FOLLOWTARGET,
    HITTEDTARGET,
    STOPPED, 
    STUNNED,
    SHOOTING
}