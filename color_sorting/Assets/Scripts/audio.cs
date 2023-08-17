using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

/// <summary>
/// Class <c>audio</c> manages both music and sound design of each scene.
/// It has to be attached to the audio manager game object.
/// </summary>
public class audio : MonoBehaviour
{
    private AudioSource speakerVictory, speakerPooringSource, speakerCompleteSource, speakerPowerOK, speakerPowerNOK;
    private Slider sliderVolume;
    private float volume;

    private void Awake()
    {
        if(SceneManager.GetActiveScene() == SceneManager.GetSceneByName("Main Menu"))
        {
            sliderVolume = GameObject.Find("Sound Slider").GetComponent<Slider>();
        }
    }

    private void Start()
    {
        speakerVictory = transform.GetChild(0).GetComponent<AudioSource>();
        speakerPooringSource = transform.GetChild(1).GetComponent<AudioSource>();
        speakerCompleteSource = transform.GetChild(2).GetComponent<AudioSource>();
        speakerPowerOK = transform.GetChild(3).GetComponent<AudioSource>();
        speakerPowerNOK = transform.GetChild(4).GetComponent<AudioSource>();
        if(!PlayerPrefs.HasKey(save.soundVolume))
        {
            PlayerPrefs.SetFloat(save.soundVolume, 1f);
            volume = 1f;
        }
        else
        {
            volume = PlayerPrefs.GetFloat(save.soundVolume);
        }
        if(SceneManager.GetActiveScene() == SceneManager.GetSceneByName("Main Menu"))
        {
            sliderVolume.value = volume;
        }
    }


    /// <summary>
    /// Set the volume for all sound design by using the connected slider.
    /// This function must be linked to a slider gameobject in the editor.
    /// </summary>
    public void setVolume()
    {
        volume = sliderVolume.value;
        PlayerPrefs.SetFloat(save.soundVolume, volume); 
    }

    public void pooringSound()
    {
        speakerPooringSource.Stop();
        speakerPooringSource.volume = 0.450f * volume;
        speakerPooringSource.loop = false;
        speakerPooringSource.Play();
    }
    
    public void tubeCompleteSound()
    {
        speakerCompleteSource.Stop();
        speakerCompleteSource.volume = 1f * volume;
        speakerCompleteSource.loop = false;
        speakerCompleteSource.Play();
    }
    
    public void victorySound()
    {
        speakerVictory.Stop();
        speakerVictory.volume = 1f * volume;
        speakerVictory.loop = false;
        speakerVictory.Play();
    }

    public void powerOK()
    {
        speakerPowerOK.Stop();
        speakerPowerOK.volume = 0.6f * volume;
        speakerPowerOK.loop = false;
        speakerPowerOK.Play();
    }

    public void powerNOK()
    {
        speakerPowerNOK.Stop();
        speakerPowerNOK.volume = 0.85f * volume;
        speakerPowerNOK.loop = false;
        speakerPowerNOK.Play();
    }
}
