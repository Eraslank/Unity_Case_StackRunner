using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StackManager : MonoBehaviour
{
    [SerializeField] Stack stackPrefab;
    [SerializeField] Transform stackHolder;

    Stack currentStack;

    int placedStacks = 0;
    private void Awake()
    {
        GetNextStack();
    }

    private void Update()
    {
        if(Input.GetMouseButtonDown(0))
        {
            PlaceStack();
        }
    }

    private void PlaceStack()
    {
        ++placedStacks;
        currentStack.Place();
        GetNextStack();
    }

    private void GetNextStack()
    {
        currentStack = Instantiate(stackPrefab, stackHolder);
        currentStack.transform.localPosition = Vector3.forward * placedStacks * GameUtil.DefaultStackScale.z;
    }
}
