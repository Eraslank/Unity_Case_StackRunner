using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class InputManager : MonoBehaviour
{
    public static UnityAction OnPress = null;

    private void Update()
    {
        if(Input.touchCount > 0)
        {
            foreach (var t in Input.touches)
            {
                if (t.phase == TouchPhase.Began)
                    OnPress?.Invoke();
            }
        }
        else if (Input.GetMouseButtonDown(0))
        {
            OnPress?.Invoke();
        }
    }
}
