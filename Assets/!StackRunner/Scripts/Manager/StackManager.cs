using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StackManager : MonoBehaviourSingleton<StackManager>
{
    [SerializeField] Stack stackPrefab;
    [SerializeField] Transform stackHolder;

    Stack currentStack;
    Stack lastPlacedStack;

    int placedStackCount = 0;

    public int persistentPlacedCount = 0;

    private bool placedFirstStack = false;

    Color baseColor;

    public bool autoPlay = false;
    private bool autoPlayBuffer = false;

    public int allowedStackCount = 0;

    public static bool CAN_PLACE = true;
    public void StartSpawning(int allowedStackCount)
    {
        this.allowedStackCount = allowedStackCount;

        placedFirstStack = false;
        placedStackCount = 0;

        GetNextStack();
    }

    private void OnEnable()
    {
        Stack.OnCutOff -= OnStackCutOff;
        Stack.OnCutOff += OnStackCutOff;

        InputManager.OnPress -= OnPress;
        InputManager.OnPress += OnPress;
    }
    private void OnDisable()
    {
        Stack.OnCutOff -= OnStackCutOff;

        InputManager.OnPress -= OnPress;
    }

    private void Awake()
    {
        persistentPlacedCount = placedStackCount = 0;

        baseColor = Color.red; //Initial Is From Red To Ensure Vibrant Colors
        baseColor = baseColor.GetNextHue(Random.Range(0, 100));
    }

    private void Update()
    {
        if (!CAN_PLACE)
            return;

        if (currentStack == null)
            return;

        if (autoPlay && !autoPlayBuffer && currentStack.inPerfectPlace)
        {
            autoPlayBuffer = true;
            DOVirtual.DelayedCall(.1f, () => autoPlayBuffer = false); //To Disallow Rapid Placing
            PlaceStack();
        }
    }

    private void OnPress()
    {
        if (!autoPlay && !GameManager.Instance.finished)
            PlaceStack();
    }

    private void PlaceStack()
    {
        if (!CAN_PLACE)
            return;

        if (currentStack == null)
            return;

        if (!currentStack.Place())
        {
            GameManager.Instance.FinishLevel(false);
            return;
        }

        lastPlacedStack = currentStack;
        ++placedStackCount;
        ++persistentPlacedCount;

        StartCoroutine(C_PlaceStack());
        IEnumerator C_PlaceStack()
        {
            yield return new WaitForFixedUpdate();
            GetNextStack();
        }

    }

    private void GetNextStack()
    {
        if (placedStackCount == allowedStackCount)
            return;

        bool isLast = placedStackCount + 1 == allowedStackCount;

        Debug.Log("Get");

        currentStack = Instantiate(stackPrefab, stackHolder);
        currentStack.GetComponent<Renderer>().material.color = baseColor.GetNextHue(persistentPlacedCount);
        currentStack.name = $"Stack {placedStackCount} ({persistentPlacedCount})";
        if (!placedFirstStack)
        {
            placedFirstStack = true;
            lastPlacedStack = currentStack;
            currentStack.Init(placedStackCount, persistentPlacedCount, lastPlacedStack, false, isLast, autoMove: false);
            currentStack.inPerfectPlace = true;
            PlaceStack();

            return;
        }
        currentStack.Init(placedStackCount, persistentPlacedCount, lastPlacedStack, placedStackCount % 2 == 0, isLast);
    }

    private void OnStackCutOff(Stack sender, float currentWidth, float cutWidth)
    {
        var piece = Instantiate(stackPrefab, stackHolder);
        piece.GetComponent<Renderer>().material.color = baseColor.GetNextHue(persistentPlacedCount);
        piece.SetWidth(cutWidth);
        piece.name = $"Stack {placedStackCount} Fall";
        piece.transform.position = sender.transform.position;

        var posDiff = Vector3.right * cutWidth * .5f;
        var currentPosDiff = Vector3.right * currentWidth * .5f;
        if (lastPlacedStack.transform.position.x < sender.transform.position.x) //Drop To Right Side
            piece.transform.position += posDiff + currentPosDiff;
        else
            piece.transform.position -= posDiff + currentPosDiff;

        piece.Fall();
    }

    public Stack GetLastPlaced() => lastPlacedStack;
}
