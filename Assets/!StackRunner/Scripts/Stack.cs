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
    public int persistentId;
    private bool last = false;

    Stack lastStack;

    public static UnityAction<Stack, float, float> OnCutOff = null; //sender, currentWidth, cutWidth
    public static UnityAction<Stack, bool> OnPlace = null; //sender, successful
    public static UnityAction<Stack, bool> OnLastPlace = null; //sender, successful

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
    public void Init(int id, int persistentId, Stack lastStack, bool fromRight, bool last, bool autoMove = true)
    {
        this.id = id;
        this.persistentId = persistentId;

        this.last = last;
        this.lastStack = lastStack;

        SetWidth(lastStack.boxCollider.bounds.size.x);

        boxCollider = GetComponent<BoxCollider>();
        rigidBody = GetComponent<Rigidbody>();

        var fwd = Vector3.forward * persistentId * GameUtil.DefaultStackScale.z;

        var right = Vector3.right * (lastStack.boxCollider.bounds.center.x
                                    + GameUtil.DefaultStackScale.x
                                    * (fromRight ? 1 : -1));

        if (id == 0)
        {
            fwd.z += StackManager.Instance.persistentPlacedCount - 1 * GameUtil.DefaultStackScale.z;
            right = Vector3.zero;
        }

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
            if (id == 0)
            {
                transform.localPosition *= StackManager.Instance.persistentPlacedCount;
            }

            OnPlace?.Invoke(this, true);
            if (last)
                OnLastPlace?.Invoke(this, true);
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
            if (last)
                OnLastPlace?.Invoke(this, true);

            return true;
        }

        OnPlace?.Invoke(this, false);
        if (last)
            OnLastPlace?.Invoke(this, false);

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
