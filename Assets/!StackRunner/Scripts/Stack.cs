using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

[RequireComponent(typeof(BoxCollider))]
[RequireComponent(typeof(Rigidbody))]
public class Stack : MonoBehaviour
{
    public BoxCollider boxCollider;
    public Rigidbody rigidBody;

    Tween moveTween;

    public bool inPerfectPlace = false;
    public int id;

    Stack lastStack;

    public static UnityAction<Stack, float, float> OnCutOff = null; //sender, currentWidth, cutWidth
    public static UnityAction<Stack, bool> OnPlace = null; //sender, successful

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
    public void Init(int id, Stack lastStack, bool fromRight, bool autoMove = true)
    {
        SetWidth(lastStack.boxCollider.bounds.size.x);

        boxCollider = GetComponent<BoxCollider>();
        rigidBody = GetComponent<Rigidbody>();

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

    public bool Place()
    {
        moveTween?.Kill();

        if (inPerfectPlace) //ToDo : Perfect Placement
        {
            transform.localPosition = lastStack.boxCollider.bounds.center
                                    + Vector3.forward
                                    * GameUtil.DefaultStackScale.z;

            OnPlace?.Invoke(this, true);
            return true;
        }

        var (widthDiff, onLeft) = boxCollider.bounds.GetWidthDiff(lastStack.boxCollider.bounds);
        if (widthDiff.HasValue)
        {
            var newWidth = Width - widthDiff.Value;
            Width = newWidth; //Cut Off Self

            //Calculate Realigned Pos
            var pos = transform.localPosition;
            pos.x = lastStack.transform.localPosition.x;

            var lastStackWidth = lastStack.boxCollider.bounds.Width(false);
            if (onLeft.Value)
                pos.x -= lastStackWidth - newWidth * .5f;
            else
                pos.x += lastStackWidth - newWidth * .5f;

            transform.localPosition = pos; //Set Realigned Pos

            OnCutOff?.Invoke(this, newWidth, widthDiff.Value);
            OnPlace?.Invoke(this, true);

            return true;
        }

        OnPlace?.Invoke(this, false);

        return false;
    }

    private void TryCheckPerfectPlace()
    {
        inPerfectPlace = boxCollider.bounds.IsInPerfectPlace(lastStack.boxCollider.bounds);
    }

    public void SetWidth(float width)
    {
        var s = GameUtil.DefaultStackScale;
        s.x = width;

        transform.localScale = s;
    }

    private void Grow(float width)
    {
        Debug.Log("GROW");
    }

    public void Fall()
    {
        rigidBody.isKinematic = false;
        rigidBody.useGravity = true;
    }
}
