using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StackManager : MonoBehaviour
{
    [SerializeField] Stack stackPrefab;
    [SerializeField] Stack fallingPiecePrefab;
    [SerializeField] Transform stackHolder;

    Stack currentStack;
    Stack lastPlacedStack;

    int placedStacks = 0;

    private bool placedFirstStack = false;

    private void OnEnable()
    {
        Stack.OnCutOff -= OnStackCutOff;
        Stack.OnCutOff += OnStackCutOff;
    }
    private void OnDisable()
    {
        Stack.OnCutOff -= OnStackCutOff;
    }

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
        currentStack.Place();
        lastPlacedStack = currentStack;
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
        currentStack.name = $"Stack {placedStacks}";
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

    private void OnStackCutOff(Stack sender, float currentWidth, float cutWidth)
    {
        var piece = Instantiate(stackPrefab, stackHolder);
        piece.SetWidth(cutWidth);
        piece.name = $"Stack {placedStacks} Fall";
        piece.transform.position = sender.transform.position;

        var posDiff = Vector3.right * cutWidth * .5f;
        var currentPosDiff = Vector3.right * currentWidth * .5f;
        if (lastPlacedStack.transform.position.x < sender.transform.position.x) //Drop To Right Side
            piece.transform.position += posDiff + currentPosDiff;
        else
            piece.transform.position -= posDiff + currentPosDiff;

        piece.Fall();
    }
}
