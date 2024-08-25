
using UnityEngine;
using UnityEngine.UI;

public class MenuEffect : MonoBehaviour
{
    [SerializeField]private Sprite textureBase;
    [SerializeField] private Sprite[] textureRandom;
    [SerializeField] private Image spriteFieds;

    private bool usingTextureBase = true;

    public void Start()
    {
        spriteFieds.sprite = textureBase;

        usingTextureBase = true;
    }

    public void SetRandomTexture()
    {
        spriteFieds.sprite = textureRandom[Random.Range(0, 5)];

        usingTextureBase = false;
    }

    public void SetBaseTexture()
    {
        spriteFieds.sprite = textureBase;

        usingTextureBase = true;
    }

}
