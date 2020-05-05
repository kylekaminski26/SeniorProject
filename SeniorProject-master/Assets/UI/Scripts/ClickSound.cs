using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Button))]
public class ClickSound : MonoBehaviour
{
    //The purpose of this script is to provide click sounds to the buttons in the
    //menus of the game.
    public AudioClip sound;

    private Button button { get { return GetComponent<Button>(); } } //Gets button to be used
    private AudioSource source { get { return GetComponent<AudioSource>(); } }//Gets sound to be used
    // Start is called before the first frame update
    void Start()
    {
        //Finds the sound to be used.
        gameObject.AddComponent<AudioSource>();
        source.clip = sound;
        source.playOnAwake = false;//Prevents the sound from playing when the game opens without being clicked
        //Plays the sound when the button object is clicked.
        button.onClick.AddListener(() => PlaySound());
    }

    void PlaySound()
    {
        //Plays the sound once, no looping.
        source.PlayOneShot(sound);
        
    }
}
