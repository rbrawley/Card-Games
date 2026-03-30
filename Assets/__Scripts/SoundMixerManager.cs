using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;

public class SoundMixerManager : MonoBehaviour
{
   [SerializeField] private AudioMixer audioMixer;
   [SerializeField] private Slider     masterSlider;
   [SerializeField] private Slider     musicSlider;
   [SerializeField] private Slider     SFXSlider;

   

   private void Start()
    {
        if (PlayerPrefs.HasKey("PlayerMaster")) 
        {
            SetMasterVolume(PlayerPrefs.GetFloat("PlayerMaster"));
            masterSlider.value = PlayerPrefs.GetFloat("PlayerMaster");
        }
        else SetMasterVolume(1);

        if (PlayerPrefs.HasKey("PlayerSFX")) 
        {
            SetSoundFXVolume(PlayerPrefs.GetFloat("PlayerSFX"));
            SFXSlider.value = PlayerPrefs.GetFloat("PlayerSFX");
        }
        else SetSoundFXVolume(1);
  
        
        if (PlayerPrefs.HasKey("PlayerMusic")) 
        {
            SetMusicVolume(PlayerPrefs.GetFloat("PlayerMusic"));
            musicSlider.value = PlayerPrefs.GetFloat("PlayerMusic");
        }
        else SetMusicVolume(1);
    }

    public void SetMasterVolume(float level)
    {
        //float level = masterSlider.value;
    //   audioMixer.SetFloat("masterVolume", level); // logrithmic volume, poor choice
        audioMixer.SetFloat("masterVolume", Mathf.Log10(level) * 20f);  //sets volume from logrithmic volume to linear
        //Debug.Log("The master vol level is: " + level);
        PlayerPrefs.SetFloat("PlayerMaster", level);
    }

    public void SetSoundFXVolume(float level)
    {
        //float level = SFXSlider.value;
        // audioMixer.SetFloat("soundFXVolume", level);
        audioMixer.SetFloat("soundFXVolume", Mathf.Log10(level) * 20f);
        PlayerPrefs.SetFloat("PlayerSFX", level);
    }

    public void SetMusicVolume(float level)
    {
        //float level = musicSlider.value;
        // audioMixer.SetFloat("musicVolume", level);
        audioMixer.SetFloat("musicVolume", Mathf.Log10(level) * 20f);
        PlayerPrefs.SetFloat("PlayerMusic", level);


    }
}
