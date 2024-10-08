using System;
using System.Collections;
using UnityEngine;

public class HealArea : MonoBehaviour
{
    [SerializeField] private string targetTag;
    [SerializeField] private int healValue;
    [SerializeField] private float healDelay;
    private IHealable[] objectsInside;

    public Action OnHeal;
    public Action OnReadyToUse;

    private bool canHeal = true;

    private void Awake()
    {
        objectsInside = new IHealable[0];
    }

    private void OnTriggerStay2D(Collider2D collision)
    {
        if (!canHeal) return;

        if (collision.CompareTag(targetTag))
        {
            IHealable hittable = collision.GetComponent<IHealable>();

            if (hittable != null)
            {
               StartCoroutine(HealAction(hittable));
            }
        }
    }

    IEnumerator HealAction(IHealable healable)
    {
        canHeal = false;
        healable.OnHeal(healValue);

        OnHeal?.Invoke();

        yield return new WaitForSeconds(healDelay);

        canHeal = true;

        OnReadyToUse?.Invoke();

        yield break;

    }


}
