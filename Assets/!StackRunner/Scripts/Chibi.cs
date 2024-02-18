using DG.Tweening;
using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;

[RequireComponent(typeof(Animator))]
public class Chibi : MonoBehaviour
{
    [SerializeField] Animator animator;
    [SerializeField] Transform chibiRoot;
    [SerializeField] Transform chibiT;

    Queue<Stack> moveRequest = new Queue<Stack>();

    private bool moving = false;
    private void OnEnable()
    {
        Stack.OnPlace -= OnStackPlace;
        Stack.OnPlace += OnStackPlace;

        animator.SetBool("IsDancing", true);
    }
    private void OnDisable()
    {
        Stack.OnPlace -= OnStackPlace;
    }

    private void OnStackPlace(Stack sender, bool successful)
    {
        if (sender.id == 0)
            return;

        if (!successful)
        {
            moveRequest.Clear();
            Fail();
            return;
        }

        moveRequest.Enqueue(sender);
        TryMove();
    }

    private void TryMove()
    {
        if (moving)
            return;

        if (moveRequest.TryDequeue(out var s))
            MoveToStack(s);
    }

    private void MoveToStack(Stack stack)
    {
        moving = true;

        animator.SetBool("IsDancing", !moving);

        StartCoroutine(C_MoveToStack());

        IEnumerator C_MoveToStack()
        {
            chibiT.transform.DOMoveX(stack.transform.position.x, 3f)
                            .SetSpeedBased();

            yield return chibiRoot.transform.DOMoveZ(GameUtil.DefaultStackScale.z, 4f)
                           .SetRelative()
                           .SetSpeedBased()
                           .SetEase(Ease.Linear)
                           .WaitForCompletion();

            moving = false;

            //Set IsDancing Back To True While Waiting Because It's Funnier
            animator.SetBool("IsDancing", !moving);

            if (moveRequest.TryDequeue(out var s))
                MoveToStack(s);
        }
    }

    private void Fail()
    {
        chibiT.DOKill();
        chibiRoot.DOKill();

        OnDisable();
        animator.DOSpeed(1, 0);

        StartCoroutine(C_Fail());

        IEnumerator C_Fail()
        {
            yield return chibiT.transform.DORotate(Vector3.up * 180f, 1f, RotateMode.FastBeyond360)
                                  .SetRelative()
                                  .WaitForCompletion();

            animator.SetBool("IsDancing", false);

            yield return animator.DOSpeed(0, 1, .2f).WaitForCompletion();

            chibiT.transform.DOMoveZ(-10, 2f)
                     .SetEase(Ease.Linear)
                     .SetSpeedBased()
                     .SetRelative();
        }
    }
}
