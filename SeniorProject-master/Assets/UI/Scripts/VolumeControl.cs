using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VolumeControl : MonoBehaviour
{

    private AudioSource audioSrc;
    private float soundVolume = 1f;

   
    // Start is called before the first frame update
    void Start()
    {
        audioSrc = GetComponent<AudioSource>();
    }

    // Update is called once per frame
    void Update()
    {
        audioSrc.volume = soundVolume;
    }

    public void SetVolume(float vol)
    {
        soundVolume = vol;
    }
}
