using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

public class AudioManager : MonoBehaviourSingleton<AudioManager>
{
    [SerializeField] AudioMixer mixer;
    [SerializeField] AudioSource[] dingSources;

    private float pitch = 1;

    int consecutive = 0;
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
        consecutive = 0;
    }

    public void IncreasePitch()
    {
        Pitch += GameUtil.PitchIncrement;
        consecutive++;
    }

    private void SetPitch()
    {
        mixer.SetFloat("DingPitch", Pitch);
    }

    public void Play()
    {
        for (int i = 0; i < (consecutive > 4 ? dingSources.Length : 1); i++)
        {
            var d = dingSources[i];
            d.Stop();
            d.Play();
        }
    }
}
