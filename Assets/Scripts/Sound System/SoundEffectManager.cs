using System.Collections;
using UnityEngine;
using SnakeGame;

/// <summary>
/// Manages all the SFS's for the game, with the help of <seealso cref="SoundEffect"/> and <seealso cref="SoundEffectSO"/> classes.
/// </summary>
[DisallowMultipleComponent]
public class SoundEffectManager : SingletonMonoBehaviour<SoundEffectManager>
{
    public int soundsVolume = 9;

    private void Start()
    {
        if (PlayerPrefs.HasKey(nameof(soundsVolume)))
        {
            soundsVolume = PlayerPrefs.GetInt(nameof(soundsVolume));
        }

        SetSoundsVolume(soundsVolume);
    }

    private void OnDisable()
    {
        PlayerPrefs.SetInt(nameof(soundsVolume), soundsVolume);
    }

    /// <summary>
    /// Plays The Selected Sound Effect
    /// </summary>
    public void PlaySoundEffect(SoundEffectSO soundEffect)
    {
        SoundEffect sound = (SoundEffect)PoolManager.Instance.ReuseComponent(soundEffect.soundPrefab, Vector3.zero,
            Quaternion.identity);
        sound.SetSound(soundEffect);
        sound.gameObject.SetActive(true);
        StartCoroutine(DisableSound(sound, soundEffect.soundEffectClip.length));
    }

    /// <summary>
    /// Disable The Sound GameObject After It Has Finished Playing And Thus Returning It To The Pool
    /// </summary>
    private IEnumerator DisableSound(SoundEffect sound, float soundDuration)
    {
        yield return new WaitForSeconds(soundDuration);
        sound.gameObject.SetActive(false);
    }

    /// <summary>
    /// Increase the sounds volume
    /// </summary>
    public void IncreaseMusicVolume()
    {
        int maxVolume = 20;

        if (soundsVolume >= maxVolume) return;

        soundsVolume += 1;
        SetSoundsVolume(soundsVolume);
    }

    /// <summary>
    /// Decrease the sounds volume
    /// </summary>
    public void DecreaseMusicVolume()
    {
        if (soundsVolume == 0) return;

        soundsVolume -= 1;

        SetSoundsVolume(soundsVolume);
    }

    /// <summary>
    /// Sets The Volume Of The Sound Effects
    /// </summary>
    private void SetSoundsVolume(int soundsVolume)
    {
        float muteVolumeDecibels = -80f;

        if (soundsVolume == 0)
        {
            GameResources.Instance.soundMasterMixerGroup.audioMixer.SetFloat("SFXVolume", muteVolumeDecibels);
        }
        else
        {
            GameResources.Instance.soundMasterMixerGroup.audioMixer.SetFloat("SFXVolume",
                HelperUtilities.LinearToDecibels(soundsVolume));
        }
    }
}
