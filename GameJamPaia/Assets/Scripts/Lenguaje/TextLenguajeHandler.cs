
using TMPro;
using UnityEngine;

public class TextLenguajeHandler : MonoBehaviour
{
    [SerializeField] private TMP_Text textComponent;
    [SerializeField] private LenguajeTexts[] LenguajeTexts;

    void Start()
    {
        LenguajeId gameLenguajeId = LenguajeManager.Instance.GameLenguaje();

        for(int i=0; i<LenguajeTexts.Length; i++)
        {
            if (LenguajeTexts[i].LenguajeId == gameLenguajeId)
            {
                textComponent.text = LenguajeTexts[i].text;
                return;
            }
        }

    }

  

}
