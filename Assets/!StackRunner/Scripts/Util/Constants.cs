using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class GameUtil
{
    public static bool InEditor //True if in editor
    {
        get { return !Application.isPlaying; }
    }

    public static Vector3 DefaultStackScale
    { 
        get 
        { 
            return new Vector3(3,1,3); 
        } 
    }
}