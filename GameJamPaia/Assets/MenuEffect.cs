using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MenuEffect : MonoBehaviour
{
    public Sprite spriteBase;
    public Sprite[] spriteRandom;
    public Image spriteFieds;
    public void Start() => InvokeRepeating(nameof(SyncTimer), 1, Random.Range(1f, 4f));
    public void SyncTimer()
    {
        if (spriteFieds.sprite = spriteBase)
        {
            spriteFieds.sprite = spriteRandom[Random.Range(0,5)];
        }
        else
        {
            spriteFieds.sprite = spriteBase;
        }
    }
}
