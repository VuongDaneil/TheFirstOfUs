using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class VExtensions
{
    public static bool InRange(this float input, float min, float Max)
    {
        return input >= min && input <= Max;
    }

    public static float RandomBetweenXandY(this Vector2 randomRangeAsVector2) => UnityEngine.Random.Range(randomRangeAsVector2.x, randomRangeAsVector2.y);
    public static bool EqualsWithOffset(this Vector3 main, float offset)
    {
        return main.x.InRange(main.x - offset, main.x + offset)
            && main.y.InRange(main.y - offset, main.y + offset)
            && main.z.InRange(main.z - offset, main.z + offset);
    }
    public static bool EqualsWithOffset(this Vector3 main, Vector3 target, float offset)
    {
        return main.x.InRange(target.x - offset, target.x + offset)
            && main.y.InRange(target.y - offset, target.y + offset)
            && main.z.InRange(target.z - offset, target.z + offset);
    }
}