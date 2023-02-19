using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class audio : MonoBehaviour
{
    [SerializeField] AudioClip pooring;
    [SerializeField] AudioClip tubeComplete;
    [SerializeField] AudioClip victory;
    private AudioSource source;

    private void Start()
    {
        source = GetComponent<AudioSource>();
    }

    public void pooringSound()
    {
        source.Stop();
        source.volume = 0.450f;
        source.clip = pooring;
        source.loop = false;
        source.Play();
    }
    
    public void tubeCompleteSound()
    {
        source.Stop();
        source.volume = 1f;
        source.clip = tubeComplete;
        source.loop = false;
        source.Play();
    }
    
    public void victorySound()
    {
        source.Stop();
        source.volume = 1f;
        source.clip = victory;
        source.loop = false;
        source.Play();
    }

}
