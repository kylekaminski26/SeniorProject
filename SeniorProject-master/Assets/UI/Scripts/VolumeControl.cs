using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VolumeControl : MonoBehaviour
{
    //This script is to be attached to each sound in order to allow
    //the Volume sliders to change the volume of each sound.

    private AudioSource audioSrc;
    private float soundVolume = 1f;

   
    // Start is called before the first frame update
    void Start()
    {
        //Locate audio source (the sound) to be changed.
        audioSrc = GetComponent<AudioSource>();
    }

    // Update is called once per frame
    void Update()
    {
        //Calling this once per frame allows the slider value to
        //immediately change the volume of the sound.
        audioSrc.volume = soundVolume;
    }

    public void SetVolume(float vol)
    {
        soundVolume = vol;
    }
}
