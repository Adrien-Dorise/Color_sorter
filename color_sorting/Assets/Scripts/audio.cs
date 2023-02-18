using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class audio : MonoBehaviour
{
    [SerializeField] AudioClip pooring;
    private AudioSource source;

    private void Start()
    {
        source = GetComponent<AudioSource>();
    }

    public void pooringSound()
    {
        source.clip = pooring;
        source.loop = false;
        source.Play();
    }

}
