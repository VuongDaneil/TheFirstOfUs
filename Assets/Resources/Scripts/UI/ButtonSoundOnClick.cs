using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ButtonSoundOnClick : MonoBehaviour
{
    public Button button;
    public AudioClip clickSound;
    public AudioSource audioSource;

    private void Awake()
    {
        button.onClick.AddListener(OnButtonClick);
    }

    private void OnValidate()
    {
        if (button == null)
        {
            button = GetComponent<Button>();
        }
        if (audioSource == null)
        {
            audioSource = GetComponent<AudioSource>();
        }
        if (audioSource != null && clickSound != null)
        {
            audioSource.clip = clickSound;
        }
    }

    private void OnDestroy()
    {
        button.onClick.RemoveListener(OnButtonClick);
    }

    private void OnButtonClick()
    {
        if (audioSource != null && clickSound != null)
        {
            audioSource.PlayOneShot(clickSound);
        }
    }
}
