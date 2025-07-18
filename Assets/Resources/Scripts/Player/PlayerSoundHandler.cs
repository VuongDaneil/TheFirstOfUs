using System.Collections.Generic;
using UnityEngine;

public class PlayerSoundHandler : MonoBehaviour
{
    #region PROPERTIES
    [Header("AUDIO")]
    public AudioSource PlayerAudioSource;
    public List<AudioClip> HurtAudioClip = new List<AudioClip>();

    public AudioClip CallRadioClip;
    public AudioClip BeepSoundClip;
    public AudioClip ImOnPositionClip;
    #endregion

    #region UNITY CORE
    private void Awake()
    {
        PlayerControlEventMananger.OnPlayerGetHurt.AddListener(OnPlayerGetHurt);
    }

    private void OnDestroy()
    {
        PlayerControlEventMananger.OnPlayerGetHurt.RemoveListener(OnPlayerGetHurt);
    }
    #endregion

    #region MAIN

    private void OnPlayerGetHurt()
    {
        if (!PlayerAudioSource.isPlaying && HurtAudioClip.Count > 0) PlayerAudioSource.PlayOneShot(HurtAudioClip.GetRandom()); 
    }

    public void PlayCallRadio()
    {
        PlayerAudioSource.PlayOneShot(CallRadioClip);
    }
    public void PlayBeepSound()
    {
        PlayerAudioSource.PlayOneShot(BeepSoundClip);
    }
    public void PlayImOnPosition()
    {
        PlayerAudioSource.PlayOneShot(ImOnPositionClip);
    }
    #endregion
}