using System;
using System.Collections;
using UnityEngine;

public class DisableAfterTime : MonoBehaviour
{
    [SerializeField] private float timeToDisble;
    [SerializeField] private bool getIHasActiveState=true;
    [SerializeField] private bool startTimerOnEnable=true;

    public Action<Vector3> OnTimeOut;

    private IHasActiveState activeState;

    private float currentTime = 0;

    private void Awake()
    {
        if (getIHasActiveState)
            activeState = GetComponent<IHasActiveState>();
    }

    private void OnEnable()
    {
        if (startTimerOnEnable)
        {
            if (activeState != null)
                activeState.Enable();

            StartCoroutine(Timer());
        }
    }

    private void OnDisable()
    {
        StopAllCoroutines();
    }


    IEnumerator Timer()
    {
        currentTime = 0;

        do
        {       
            currentTime += Time.deltaTime;
            
            yield return null;

        }while(currentTime < timeToDisble);

        if (activeState!=null)
            activeState.Disable();

        OnTimeOut?.Invoke(transform.position);

        PoolManager.ReleaseObject(gameObject);
    }



}
