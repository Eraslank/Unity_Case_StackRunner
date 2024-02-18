using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

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
            return new Vector3(3, 1, 3);
        }
    }

    public static float PosDiffTolerance
    {
        get
        {
            return .25f;
        }
    }
}

public static class BoundsExtensions
{

    public static float Width(this Bounds a, bool full = true) => full switch
    {
        true => a.size.x,
        _ => a.extents.x,
    };
    public static Vector2 WidthRange(this Bounds a)
    {
        return new Vector2(a.center.x - a.extents.x, a.center.x + a.extents.x);
    }

    public static bool IsInPerfectPlace(this Bounds a, Bounds b)
    {
        return Mathf.Abs(a.center.x - b.center.x) < GameUtil.PosDiffTolerance;
    }

    public static (float? widthDiff, bool? onLeft) GetWidthDiff(this Bounds a, Bounds b)
    {
        var aRange = a.WidthRange();
        var bRange = b.WidthRange();

        if (aRange.x <= bRange.x) // A is Left Most 
        {
            if (aRange.y <= bRange.x) // A is Outside The Reach Of B
            {
                return (null,null);
            }

            return (Mathf.Abs(bRange.x - aRange.x), true); // Return A's Left Part That Is Outside Of B
        }
        else // B Is Left Most
        {
            if (bRange.y <= aRange.x) // A is Outside The Reach Of B
            {
                return (null, null);
            }

            return (Mathf.Abs(aRange.y - bRange.y), false); // Return A's Right Part That Is Outside Of B
        }
    }
}