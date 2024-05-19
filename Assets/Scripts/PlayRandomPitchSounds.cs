using System.Collections;
using System.Collections.Generic;
using UnityEditor.Timeline.Actions;
using UnityEngine;

public class PlayRandomPitchSounds : MonoBehaviour
{
    
    [SerializeField]  AudioSource source;
    [SerializeField] AudioClip[] sounds;
    [SerializeField] float minPitch = 75;
    [SerializeField] float maxPitch = 120;
    [SerializeField] bool oneShot =false;
    
    public void Play(int index)
    {
        source.pitch = Random.Range(minPitch, maxPitch) / 100;
        source.PlayOneShot(sounds[index]);
    }
    public void PlayRandom()
    {
        if (sounds.Length > 0)
        {
            source.pitch = Random.Range(minPitch, maxPitch) / 100;
            
            AudioClip clip = sounds[Random.Range(0, sounds.Length)];
            source.PlayOneShot(clip);
            if (oneShot)
                Destroy(this.gameObject, clip.length);
        }
    }
    public bool IsPlaying() 
    { 
        return source.isPlaying; 
    }

}
