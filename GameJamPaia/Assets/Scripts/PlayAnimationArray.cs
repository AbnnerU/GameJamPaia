
using UnityEngine;

public class PlayAnimationArray : MonoBehaviour
{
    [SerializeField] private PlayAnimationConfig[] config;


    public void PlayAnimations(int id)
    {
        if (HaveId(id) == false) return;

        AnimationData[] data = config[id].data;

        if (data.Length == 0) return;

        for(int i =0; i < data.Length; i++)       
            data[i].animatorRef.Play(data[i].animationName);
        
    }

    private bool HaveId(int id)
    {
        for(int i =0; i < config.Length; i++)
        {
            if (config[i].id == id)
                return true;
        }

        return false;
    }

    [System.Serializable]
    private struct PlayAnimationConfig
    {
        public int id;
        public AnimationData[] data;
    }

    [System.Serializable]
    private struct AnimationData
    {
        public Animator animatorRef;
        public string animationName;
    }
}
