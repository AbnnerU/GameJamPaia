
using UnityEngine;
using Assets.Scripts.GameAction;


public class ChangeTransformPositionAction : GameAction
{

    [SerializeField] private Transform _transform;
    [SerializeField] private Transform target;
    [SerializeField] private Vector3 offSet;
    [SerializeField] private bool useLocalPosition;

    public override void DoAction()
    {
        if (target == null) return;

        if (useLocalPosition)
            _transform.localPosition = target.localPosition + offSet;
        else
            _transform.position = target.position + offSet;


    }


}

