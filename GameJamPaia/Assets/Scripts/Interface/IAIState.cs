
public interface IAIState 
{
    public AIState GetCurrentAIState();

    public void SetAIState(AIState newState);
}

public enum AIState
{
    FOLLOWTARGET,
    HITTEDTARGET,
    STOPPED
}