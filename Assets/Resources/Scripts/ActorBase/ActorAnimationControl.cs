using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ActorAnimationControl : MonoBehaviour
{
    #region PROPERTIES
    public EnemyConfig AttributesConfig;
    public Animator ActorAnimator;
    public AudioSource ActorAudioSource;
    #endregion

    #region UNITY CORE
    private void OnValidate()
    {
        if (ActorAnimator == null)
        {
            ActorAnimator = GetComponent<Animator>();
        }
        if (ActorAudioSource == null)
        {
            ActorAudioSource = GetComponent<AudioSource>();
        }
    }
    #endregion

    #region MAIN

    public void Initialize(ActorBase actor)
    {
        AttributesConfig = actor.AttributesConfig;
    }

    #region _animations
    public void PlayIdleAnimation()
    {
        if (AttributesConfig.IdleAnim.Count > 0)
        {
            var randomAnimState = AttributesConfig.IdleAnim[Random.Range(0, AttributesConfig.IdleAnim.Count)];
            ActorAnimator.CrossFade(randomAnimState, 0.5f);
        }
    }

    public void PlayFastChaseAnimation()
    {
        if (AttributesConfig.ChaseAnim.Count > 0)
        {
            var randomAnimState = AttributesConfig.ChaseAnim[Random.Range(0, AttributesConfig.ChaseAnim.Count)];
            ActorAnimator.CrossFade(randomAnimState, 0.25f);
        }
    }

    public void PlayChaseAnimation()
    {
        bool aggressive = AttributesConfig.ChooseAggression();
        if (aggressive)
        {
            if (AttributesConfig.ChaseAnim.Count > 0)
            {
                var randomAnimState = AttributesConfig.ChaseAnim[Random.Range(0, AttributesConfig.ChaseAnim.Count)];
                ActorAnimator.CrossFade(randomAnimState, 0.25f);
            }
        }
        else
        {
            if (AttributesConfig.ChaseSlowAnim.Count > 0)
            {
                var randomAnimState = AttributesConfig.ChaseAnim[Random.Range(0, AttributesConfig.ChaseAnim.Count)];
                ActorAnimator.CrossFade(randomAnimState, 0.25f);
            }
        }
    }

    public void PlayMoveAnimation()
    {
        if (AttributesConfig.MoveAnim.Count > 0)
        {
            var randomAnimState = AttributesConfig.MoveAnim[Random.Range(0, AttributesConfig.MoveAnim.Count)];
            ActorAnimator.CrossFade(randomAnimState, 0.5f);
        }
    }

    public void PlayAttackAnimation()
    {
        if (AttributesConfig.AttackAnim.Count > 0)
        {
            var randomAnimState = AttributesConfig.AttackAnim[Random.Range(0, AttributesConfig.AttackAnim.Count)];
            ActorAnimator.CrossFade(randomAnimState, 0.25f);
        }
    }

    public void PlayStunAnimation()
    {
        if (AttributesConfig.StunAnim.Count > 0)
        {
            var randomAnimState = AttributesConfig.StunAnim[Random.Range(0, AttributesConfig.StunAnim.Count)];
            ActorAnimator.CrossFade(randomAnimState, 0.5f);
        }
    }

    public void PlayDeadAnimation()
    {
        if (AttributesConfig.DeadAnim.Count > 0)
        {
            var randomAnimState = AttributesConfig.DeadAnim[Random.Range(0, AttributesConfig.DeadAnim.Count)];
            ActorAnimator.CrossFade(randomAnimState, 0.5f);
        }
    }
    #endregion

    #region _audio
    public void PlayIdleAudio()
    {
        if (AttributesConfig.IdleAudioClips.Length > 0)
        {
            var randomClip = AttributesConfig.IdleAudioClips[Random.Range(0, AttributesConfig.IdleAudioClips.Length)];
            ActorAudioSource.PlayOneShot(randomClip);
        }
    }
    public void PlayChaseAudio()
    {
        if (AttributesConfig.ChaseAudioClips.Length > 0)
        {
            var randomClip = AttributesConfig.ChaseAudioClips[Random.Range(0, AttributesConfig.ChaseAudioClips.Length)];
            ActorAudioSource.PlayOneShot(randomClip);
        }
    }
    public void PlayMoveAudio()
    {
        if (AttributesConfig.MoveAudioClips.Length > 0)
        {
            var randomClip = AttributesConfig.MoveAudioClips[Random.Range(0, AttributesConfig.MoveAudioClips.Length)];
            ActorAudioSource.PlayOneShot(randomClip);
        }
    }
    public void PlayAttackAudio()
    {
        if (AttributesConfig.AttackAudioClips.Length > 0)
        {
            var randomClip = AttributesConfig.AttackAudioClips[Random.Range(0, AttributesConfig.AttackAudioClips.Length)];
            ActorAudioSource.PlayOneShot(randomClip);
        }
    }
    public void PlayHurtAudio()
    {
        if (AttributesConfig.HurtAudioClips.Length > 0)
        {
            var randomClip = AttributesConfig.HurtAudioClips[Random.Range(0, AttributesConfig.HurtAudioClips.Length)];
            ActorAudioSource.PlayOneShot(randomClip);
        }
    }
    public void PlayDeadAudio()
    {
        if (AttributesConfig.DeadAudioClips.Length > 0)
        {
            var randomClip = AttributesConfig.DeadAudioClips[Random.Range(0, AttributesConfig.DeadAudioClips.Length)];
            ActorAudioSource.PlayOneShot(randomClip);
        }
    }
    #endregion

    #endregion

    #region SUPPORTTIVE
    private void PlayCrossfadeAnimation(string animState, float fadeDuration = 0.5f)
    {
        if (ActorAnimator != null && !string.IsNullOrEmpty(animState))
        {
            ActorAnimator.CrossFade(animState, fadeDuration);
        }
    }
    #endregion
}
