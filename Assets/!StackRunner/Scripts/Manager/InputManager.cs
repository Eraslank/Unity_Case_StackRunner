using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class InputManager : MonoBehaviour
{
    public static UnityAction OnPress = null;

    private void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            OnPress?.Invoke();
        }
    }
}
