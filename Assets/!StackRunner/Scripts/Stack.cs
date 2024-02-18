using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(BoxCollider))]
public class Stack : MonoBehaviour
{

    public BoxCollider boxCollider;

    Tween moveTween;

    public bool inPerfectPlace = false;
    public int id;

    Stack lastStack;

    public float Width
    {
        get
        {
            return boxCollider.bounds.Width();
        }
        set
        {
            if (value <= lastStack.boxCollider.bounds.size.x)
                SetWidth(value);
            else
                Grow(value);
        }
    }

    public bool t = false;
    private void Update()
    {
        if (!t)
            return;
        t = false;
        Debug.Log(Width, gameObject);
    }
    public void Init(int id, Stack lastStack, bool fromRight, bool autoMove = true)
    {
        SetWidth(lastStack.boxCollider.bounds.size.x);

        boxCollider = GetComponent<BoxCollider>();

        this.id = id;
        this.lastStack = lastStack;

        var fwd = Vector3.forward * id * GameUtil.DefaultStackScale.z;

        var right = Vector3.right * (lastStack.boxCollider.bounds.center.x
                                    + GameUtil.DefaultStackScale.x
                                    * (fromRight ? 1 : -1));

        transform.localPosition = fwd + right;

        if (autoMove)
            Move(fromRight);
    }

    public void Move(bool fromRight)
    {
        var movePosX = GameUtil.DefaultStackScale.x * 2;

        if (fromRight)
            movePosX = -movePosX;

        moveTween = transform.DOLocalMoveX(movePosX, 5f)
                             .SetRelative()
                             .SetSpeedBased()
                             .OnUpdate(TryCheckPerfectPlace)
                             .SetLoops(-1, LoopType.Yoyo)
                             .SetEase(Ease.InOutSine);
    }

    public void Place()
    {
        moveTween?.Kill();

        if (inPerfectPlace) //ToDo : Perfect Placement
        {
            transform.localPosition = lastStack.boxCollider.bounds.center 
                                    + Vector3.forward 
                                    * GameUtil.DefaultStackScale.z;
            return;
        }

        var (widthDiff, onLeft) = boxCollider.bounds.GetWidthDiff(lastStack.boxCollider.bounds);
        if (widthDiff.HasValue)
        {
            Width = Width - widthDiff.Value;
            var posDiff = Vector3.right * widthDiff.Value * .5f;

            if (onLeft.Value)
                posDiff = -posDiff;

            transform.localPosition -= posDiff;
        }
    }

    private void TryCheckPerfectPlace()
    {
        inPerfectPlace = boxCollider.bounds.IsInPerfectPlace(lastStack.boxCollider.bounds);
    }

    private void SetWidth(float width)
    {
        var s = GameUtil.DefaultStackScale;
        s.x = width;

        transform.localScale = s;
    }

    private void Grow(float width)
    {
    }
}
