using System.Collections;
using UnityEngine;

public class JoyStick : MonoBehaviour, IHasActiveState
{
    [SerializeField] private bool active = true;

    [SerializeField] private GameObject joystickBorder;

    [SerializeField] private GameObject joystickCenter;

    [SerializeField] private TouchPositionTrigger positionTrigger;

    private RectTransform _borderTransform;

    private RectTransform _centerTransform;

    private Coroutine joystickCoroutine;

    private Vector2 direction=Vector2.zero;

    private float limitValue;

    private void Awake()
    {
        _borderTransform = joystickBorder.GetComponent<RectTransform>();
        _centerTransform = joystickCenter.GetComponent<RectTransform>();

        limitValue = (_borderTransform.rect.width / 2) - (_centerTransform.rect.width / 2);

        limitValue -= 5;

        positionTrigger.OnTouchScreen += PositionTrigger_OnTouchScreen;

        joystickCoroutine = StartCoroutine(ShowJoyStick());
    }

    IEnumerator ShowJoyStick()
    {
        while (active)
        {
            direction = positionTrigger.GetInputDirection().normalized;
            //print(direction);
            _centerTransform.localPosition = limitValue * direction;

            yield return new WaitForSeconds(0.01f);     
        }


        yield break;
    }

    private void PositionTrigger_OnTouchScreen(bool touching)
    {
        if (active == false)
            return;

        if (touching)
        {
            joystickBorder.SetActive(true);
            joystickCenter.SetActive(true);

            _borderTransform.position = positionTrigger.GetStartPoint();
            _centerTransform.localPosition = Vector3.zero;

            //if(joystickCoroutine == null)
            //  
        }
        //else
        //{
        //    joystickBorder.SetActive(false);
        //    joystickCenter.SetActive(false);

        //    if (joystickCoroutine != null)
        //        StopCoroutine(joystickCoroutine);

        //    joystickCoroutine = null;
        //}
    }

    public void Disable()
    {
        active = false;

        joystickBorder.SetActive(false);
        joystickCenter.SetActive(false);

        if (joystickCoroutine != null)
        {
            StopCoroutine(joystickCoroutine);

            joystickCoroutine = null;
        }

    }

    public void Enable()
    {
        active = true;
    }
}
