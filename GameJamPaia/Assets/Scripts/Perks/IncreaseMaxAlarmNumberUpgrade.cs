
using UnityEngine;

public class IncreaseMaxAlarmNumberUpgrade : UpgradeBase
{
    [SerializeField] private GameManager gameManager;
    [SerializeField] private int addValue;

    private void Awake()
    {
        if(gameManager == null)
            gameManager= FindObjectOfType<GameManager>();
    }

    public override void ApplyUpgrade()
    {
        gameManager.AddMaxAlarmOnValue(addValue);
    }

 
}
