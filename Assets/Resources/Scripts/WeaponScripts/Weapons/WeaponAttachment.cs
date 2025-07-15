using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponAttachment : MonoBehaviour
{
    [Range(0, 100)] public float SpreadIncreaseFactorReduce = 20;
    [Range(-100, 100)] public float DamageBuff = 20;
}
