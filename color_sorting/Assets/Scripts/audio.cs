using System.Collections;
using System.Collections.Generic;
using UnityEngine;


/// <summary>
/// Class <c>audio</c> manages both music and sound design of each scene.
/// It has to be attached to the audio manager game object.
/// </summary>
public class audio : MonoBehaviour
{
    private AudioSource speakerVictory, speakerPooringSource, speakerCompleteSource, speakerPowerOK, speakerPowerNOK;

    private void Start()
    {
        speakerVictory = transform.GetChild(0).GetComponent<AudioSource>();
        speakerPooringSource = transform.GetChild(1).GetComponent<AudioSource>();
        speakerCompleteSource = transform.GetChild(2).GetComponent<AudioSource>();
        speakerPowerOK = transform.GetChild(3).GetComponent<AudioSource>();
        speakerPowerNOK = transform.GetChild(4).GetComponent<AudioSource>();
    }

    public void pooringSound()
    {
        speakerPooringSource.Stop();
        speakerPooringSource.volume = 0.450f;
        speakerPooringSource.loop = false;
        speakerPooringSource.Play();
    }
    
    public void tubeCompleteSound()
    {
        speakerCompleteSource.Stop();
        speakerCompleteSource.volume = 1f;
        speakerCompleteSource.loop = false;
        speakerCompleteSource.Play();
    }
    
    public void victorySound()
    {
        speakerVictory.Stop();
        speakerVictory.volume = 1f;
        speakerVictory.loop = false;
        speakerVictory.Play();
    }

    public void powerOK()
    {
        speakerPowerOK.Stop();
        speakerPowerOK.volume = 0.6f;
        speakerPowerOK.loop = false;
        speakerPowerOK.Play();
    }

    public void powerNOK()
    {
        speakerPowerNOK.Stop();
        speakerPowerNOK.volume = 0.85f;
        speakerPowerNOK.loop = false;
        speakerPowerNOK.Play();
    }
}
