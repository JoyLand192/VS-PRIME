using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class AudioManager : MonoBehaviour
{
    [SerializeField] Slider volumeSlider;
    public int SfxChannelSlotAmount;
    public float SfxVolume;
    public AudioClip[] Sfxs;
    public AudioSource[] SfxChannels;
    int SfxChannelIndex;

    public enum SfxList
    {
        Click,
        Slash,
        Damage,
        Ding,
        Gwamp,
        Taat,
        TP,
        Remote,
        Select,
        Charge,
        PowerCharge,
    }
    void Awake()
    {
        volumeSlider.onValueChanged.AddListener(delegate { SFXVolumeChanged(); });
        SfxChannels = new AudioSource[SfxChannelSlotAmount];
        GameObject SfxManager = new GameObject("SFX_Manager");

        for (int i = 0; i < SfxChannels.Length; i++)
        {
            SfxChannels[i] = SfxManager.AddComponent<AudioSource>();
            SfxChannels[i].playOnAwake = false;
            SfxChannels[i].loop = false;
            SfxChannels[i].volume = SfxVolume;
        }
        DontDestroyOnLoad(SfxManager);
    }
    public void SFXVolumeChanged()
    {
        SfxVolume = volumeSlider.value * GameStatus.MasterVolume;
        foreach (var a in SfxChannels)
        {
            a.volume = volumeSlider.value * GameStatus.MasterVolume;
        }
    }

    public void PlaySfx(SfxList sfx, float volume)
    {
        for (int i = 0; i < SfxChannels.Length; i++)
        {
            int FixedIndex = (i + SfxChannelIndex) % SfxChannels.Length;
            if (SfxChannels[FixedIndex].isPlaying) continue;

            SfxChannels[FixedIndex].clip = Sfxs[(int)sfx];
            SfxChannels[FixedIndex].volume = SfxVolume * volume / 100f;
            SfxChannels[FixedIndex].Play();
            break;
        }
    }
}
