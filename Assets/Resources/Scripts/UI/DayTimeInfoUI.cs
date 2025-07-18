using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using static SceneSharedAttributes;

public class DayTimeInfoUI : MonoBehaviour
{
    public TMP_Text TimeDisplayText;

    private void Awake()
    {
        GameplayEventManager.OnAnHourPassed.AddListener(OnHourPassed);
    }

    private void OnDestroy()
    {
        GameplayEventManager.OnAnHourPassed.RemoveListener(OnHourPassed);
    }

    private void OnHourPassed(int daycount, DayPart dayPart)
    {
        TimeDisplayText.text = "Day " + daycount + " - " + dayPart.ToString();
    }
}
