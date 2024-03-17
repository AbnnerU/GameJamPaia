using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VisionClamp : MonoBehaviour, IHasActiveState
{
    [SerializeField] private bool active;
    [SerializeField] private Transform targetRef;
    [SerializeField] private Camera camRef;
    [SerializeField] private GameObject panel;
    [SerializeField] private RectTransform maskRect;
   
    // Start is called before the first frame update
    void Start()
    {
        if (active)
            Enable();
        else 
            Disable();

        camRef = Camera.main;
    }

    // Update is called once per frame
    void Update()
    {
        if (active)
        {
            Vector3 screenPos = Camera.main.WorldToScreenPoint(targetRef.position);

            // Atualiza a posição do elemento da UI para seguir o objeto
            maskRect.position = screenPos;
        }
    }

    public bool IsActive()
    {
        return active;
    }

    public void Disable()
    {
        active = false;
        panel.SetActive(false);
    }

    public void Enable()
    {
        active = true;
        panel.SetActive(true);
    }

  
}
