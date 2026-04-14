using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SFXManager : MonoBehaviour
{
    public static SFXManager S;

    [SerializeField] private AudioSource SFXObject;

    private void Awake()
    {
        if (S == null)
        {
            S = this;
        }
    }

    public void PlaySFX(AudioClip audioClip, Transform spawn, float volume)
    {
        //spawn gameobject
        AudioSource audioSource = Instantiate(SFXObject, spawn.position, Quaternion.identity);

        //assigns audioclip
        audioSource.clip = audioClip;

        //assign volume
        audioSource.volume = volume;

        //play sound
        audioSource.Play();

        //get length of clip
        float clipLength = audioSource.clip.length;

        //destroy clip after playing
        Destroy(audioSource.gameObject, clipLength);
    } 

    public void PlayRandomSFX(AudioClip[] audioClip, Transform spawn, float volume)
    {

        //assign a random index
        int rand = Random.Range(0, audioClip.Length);
        Debug.Log("Length = " + audioClip.Length);
        Debug.Log("Rand = " + rand);

        //spawn gameobject
        AudioSource audioSource = Instantiate(SFXObject, spawn.position, Quaternion.identity);

        //assigns audioclip
        audioSource.clip = audioClip[rand];

        //assign volume
        audioSource.volume = volume;

        //play sound
        audioSource.Play();

        //get length of clip
        float clipLength = audioSource.clip.length;

        //destroy clip after playing
        Destroy(audioSource.gameObject, clipLength);
    } 
}
