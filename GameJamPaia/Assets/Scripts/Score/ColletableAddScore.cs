
using UnityEngine;
using static GameScore;

public class ColletableAddScore :MonoBehaviour, IHasActiveState
{
    [SerializeField] private string targetTag = "Player";
    [SerializeField] private bool disableOnTriggerEnter = true;
    [Header("Score")]
    [SerializeField] private int addScoreValue = 1;
    [SerializeField] private GameScore scoreReference;
    [SerializeField] private ScoreName searchScoreName;

    public void Disable()
    {
        PoolManager.ReleaseObject(gameObject);
    }

    public void Enable()
    {
        //
    }

    private void Awake()
    {
        if (scoreReference == null)
            SearchScore();
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag(targetTag))
        {
            if (scoreReference == null)
                SearchScore();

            scoreReference?.AddPoints(addScoreValue);

            if (disableOnTriggerEnter)
                Disable();
        }
    }

    private void SearchScore()
    {
        GameScore[] all = FindObjectsOfType<GameScore>();

        for(int i = 0; i < all.Length; i++)
        {
            if(all[i].GetScoreName() == searchScoreName)
            {
                scoreReference = all[i];
                break;
            }

        }
    }


}
