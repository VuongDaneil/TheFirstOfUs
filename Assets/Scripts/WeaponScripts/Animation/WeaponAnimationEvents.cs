using UnityEngine;
using WeaponSystem;

[RequireComponent(typeof(AudioSource))]
public class WeaponAnimationEvents : MonoBehaviour
{
    private AudioSource audioSource;
    private WeaponBase weaponBase;

    private void Awake()
    {
        audioSource = GetComponent<AudioSource>();
        weaponBase = GetComponent<WeaponBase>();
        
        if (weaponBase == null)
        {
            Debug.LogError($"[{GetType().Name}] WeaponBase component not found!");
        }
    }

    // Magazine handling events
    public void OnMagazineOut()
    {
        PlaySound(WeaponSoundType.MagazineOut);
    }

    public void OnMagazineIn()
    {
        PlaySound(WeaponSoundType.MagazineIn);
    }

    // Bolt handling events
    public void OnBoltPull()
    {
        PlaySound(WeaponSoundType.BoltPull);
    }

    public void OnBoltRelease()
    {
        PlaySound(WeaponSoundType.BoltRelease);
    }

    // Animation completion events
    public void OnReloadComplete()
    {
        if (weaponBase != null)
        {
            weaponBase.OnReloadAnimationComplete();
        }
    }

    public void OnInspectComplete()
    {
        if (weaponBase != null)
        {
            weaponBase.OnInspectAnimationComplete();
        }
    }

    private void PlaySound(WeaponSoundType soundType)
    {
        if (audioSource != null && weaponBase != null)
        {
            AudioClip sound = weaponBase.GetWeaponSound(soundType);
            if (sound != null)
            {
                audioSource.PlayOneShot(sound);
            }
        }
    }
}