using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MenuEffect : MonoBehaviour
{
    [SerializeField]private Texture textureBase;
    [SerializeField] private Texture[] textureRandom;
    [SerializeField] private Image spriteFieds;
    [SerializeField] private Material material;

    private bool usingTextureBase = true;

    public void Start()
    {
        material = spriteFieds.material;

        material.SetTexture("_BaseMap", textureBase);

        usingTextureBase = true;

        //InvokeRepeating(nameof(SyncTimer), 1, Random.Range(1f, 4f));
    }

    public void SetRandomTexture()
    {
        material.SetTexture("_BaseMap", textureRandom[Random.Range(0, 5)]);

        usingTextureBase = false;
    }

    public void SetBaseTexture()
    {
        material.SetTexture("_BaseMap", textureBase);

        usingTextureBase = true;
    }

    //public void SyncTimer()
    //{
    //    if (usingTextureBase)
    //    {
    //        material.SetTexture("_BaseMap", textureRandom[Random.Range(0, 5)]);


    //        print("oi");

    //        usingTextureBase = false;
    //    }
    //    else
    //    {
    //        material.SetTexture("_BaseMap", textureBase);

    //        usingTextureBase = true;
    //    }
    //}
}
