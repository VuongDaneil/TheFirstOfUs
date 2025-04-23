using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public static class VExtension
{
    public static T GetRandom<T>(this IEnumerable<T> input)
    {
        if (input == null || !input.Any()) return default(T);
        return input.ElementAt(UnityEngine.Random.Range(0, input.Count()));
    }
}
