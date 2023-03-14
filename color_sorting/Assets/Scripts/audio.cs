using System.Collections;
using System.Collections.Generic;
using UnityEngine;


/// <summary>
/// Class <c>audio</c> manages both music and sound design of each scene.
/// It has to be attached to the audio manager game object.
/// </summary>
public class audio : MonoBehaviour
{
    [SerializeField] AudioClip pooring;
    [SerializeField] AudioClip tubeComplete;
    [SerializeField] AudioClip victory;
    private AudioSource pooringSource, completeSource;

    private void Start()
    {
        pooringSource = transform.GetChild(0).GetComponent<AudioSource>();
        completeSource = transform.GetChild(1).GetComponent<AudioSource>();
    }

    public void pooringSound()
    {
        pooringSource.Stop();
        pooringSource.volume = 0.450f;
        pooringSource.clip = pooring;
        pooringSource.loop = false;
        pooringSource.Play();
    }
    
    public void tubeCompleteSound()
    {
        completeSource.Stop();
        completeSource.volume = 1f;
        completeSource.clip = tubeComplete;
        completeSource.loop = false;
        completeSource.Play();
    }
    
    public void victorySound()
    {
        completeSource.Stop();
        completeSource.volume = 1f;
        completeSource.clip = victory;
        completeSource.loop = false;
        completeSource.Play();
    }

}
