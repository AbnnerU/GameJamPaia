using Assets.Scripts.GameAction;

using UnityEngine;

public class ChangeSpriteActiveStateAction : GameAction
{
    [SerializeField] private SpriteRenderer spriteRender;
    [SerializeField] private bool setActive;

    public override void DoAction()
    {
        spriteRender.enabled = setActive;
    }

   
}
