using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

public class AudioManager : MonoBehaviourSingleton<AudioManager>
{
    [SerializeField] AudioMixer mixer;
    [SerializeField] AudioSource[] dingSources;

    private float pitch = 1;

    public float Pitch
    { 
        get 
        { 
            return pitch; 
        } 
        set
        {
            pitch = value;
            if (pitch >= GameUtil.MaxPitch)
                pitch = GameUtil.MaxPitch;

            SetPitch();
        }
    }

    public void ResetPitch()
    {
        Pitch = 1;
    }

    public void IncreasePitch()
    {
        Pitch += GameUtil.PitchIncrement;
    }

    private void SetPitch()
    {
        mixer.SetFloat("DingPitch", Pitch);
    }

    public void Play()
    {
        foreach(var d in dingSources)
        {
            d.Stop();
            d.Play();
        }
    }
}
