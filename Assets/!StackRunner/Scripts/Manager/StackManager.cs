using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StackManager : MonoBehaviour
{
    [SerializeField] Stack stackPrefab;
    [SerializeField] Transform stackHolder;

    Stack currentStack;
    Stack lastPlacedStack;

    int placedStacks = 0;

    private bool placedFirstStack = false;

    private void Awake()
    {
        GetNextStack();
    }

    private void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            PlaceStack();
        }
    }

    private void PlaceStack()
    {
        lastPlacedStack = currentStack;
        currentStack.Place();
        ++placedStacks;

        StartCoroutine(C_PlaceStack());
        IEnumerator C_PlaceStack()
        {
            yield return new WaitForFixedUpdate();
            GetNextStack();
        }
    }

    private void GetNextStack()
    {
        currentStack = Instantiate(stackPrefab, stackHolder);
        if (!placedFirstStack)
        {
            placedFirstStack = true;
            lastPlacedStack = currentStack;

            currentStack.Init(placedStacks, lastPlacedStack, false, autoMove: false);

            currentStack.inPerfectPlace = true;
            PlaceStack();

            lastPlacedStack.transform.localPosition = Vector3.zero;
            return;
        }

        currentStack.Init(placedStacks, lastPlacedStack, placedStacks % 2 == 0);
    }
}
