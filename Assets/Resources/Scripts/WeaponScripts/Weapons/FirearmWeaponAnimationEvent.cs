using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FirearmWeaponAnimationEvent : MonoBehaviour
{
    #region PROPERTIES
    public AudioSource ReloadAudioSource;
    public AudioClip MagOutClip;
    public AudioClip MagInClip;
    public AudioClip BoltBackClip;
    public AudioClip BoltForwardClip;
    #endregion

    #region MAIN

    #region _events
    public void ReloadDone() => PlayerControlEventsMananger.OnWeaponReloadDone?.Invoke();
    public void SwitchedIn() => PlayerControlEventsMananger.OnWeaponSwitchInDone?.Invoke();
    public void SwitchedOut() => PlayerControlEventsMananger.OnWeaponSwitchOutDone?.Invoke();
    #endregion

    public void MagOut() => ReloadAudioSource.PlayOneShot(MagOutClip);
    public void MagIn() => ReloadAudioSource.PlayOneShot(MagInClip);
    public void BoltBack() => ReloadAudioSource.PlayOneShot(BoltBackClip);
    public void BoltForward() => ReloadAudioSource.PlayOneShot(BoltForwardClip);
    #endregion
}
