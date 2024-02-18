using Cinemachine;
using DG.Tweening;
using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;

[RequireComponent(typeof(Animator))]
public class Chibi : MonoBehaviourSingleton<Chibi>
{
    [SerializeField] Animator animator;
    [SerializeField] Transform chibiRoot;
    [SerializeField] Transform chibiT;

    [SerializeField] CinemachineVirtualCamera rotateCam;

    Queue<Stack> moveRequest = new Queue<Stack>();

    private bool moving = false;

    Tween dollyTween;

    private void OnEnable()
    {
        Stack.OnPlace -= OnStackPlace;
        Stack.OnPlace += OnStackPlace;

        Stack.OnLastPlace -= OnLastPlace;
        Stack.OnLastPlace += OnLastPlace;

        animator.SetBool("IsDancing", true);

        dollyTween = rotateCam.DODollyPath();
    }
    private void OnDisable()
    {
        Stack.OnPlace -= OnStackPlace;
        Stack.OnLastPlace -= OnLastPlace;

        chibiT.DOKill();
        chibiRoot.DOKill();

        dollyTween?.Kill();
    }
    private void OnStackPlace(Stack sender, bool successful)
    {
        if (sender.persistentId == 0) //Wait On Initial
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

    private void OnLastPlace(Stack sender, bool successful)
    {
        moveRequest.Enqueue(null);
        TryMove();
    }

    private void TryMove()
    {
        if (moving)
            return;

        if (moveRequest.TryDequeue(out var s))
            Move(s);
    }

    private void Move(Stack stack = null)
    {
        moving = true;

        animator.SetBool("IsDancing", !moving);

        StartCoroutine(C_MoveToStack());

        IEnumerator C_MoveToStack()
        {
            float lastDelay = .5f;
            float pos = 0;
            if (stack != null)
            {
                pos = stack.transform.position.x;
                lastDelay = 0;
            }
            rotateCam.Priority = (stack == null) ? 11 : 9;

            chibiT.transform.DOMoveX(pos, 3f)
                            .SetDelay(lastDelay)
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
                Move(s);
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
