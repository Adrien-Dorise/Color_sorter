using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor.SceneManagement;
using UnityEngine.SceneManagement;

public class musicManager : MonoBehaviour
{
    //Music to used -> To setup in editor
    //Audio clip for one shot musics
    //List for transitional musics
    [SerializeField] private AudioClip mainMenuMusic;
    [SerializeField] private List<AudioClip> levelMusicParts;
    
    //Speakers for main musics + transitions
    private int currentSample;

    //Transition parameters
    private float fadeDelay;
    private float fadeCoeff;

    // Start is called before the first frame update
    void Start()
    {
        //Transition parameters
        fadeDelay = 0.01f;
        fadeCoeff = 0.002f;
    }

    private void musicChoiceManager()
    {
        string currentScene = SceneManager.GetActiveScene().name;
        AudioSource speaker = this.GetComponent<AudioSource>();

        if(currentScene == "Main Menu")
        {
            speaker.clip = mainMenuMusic;
        }

    }

    /// <summary>
    /// setMusicstate records the current sample state of a music in the PlayerPrefs
    /// It is to be called by other scripts before switching scenes.
    /// It is possible to reset the music to the beginning by applying 0 to sampleState parameter
    /// </summary>
    /// <param name="saveName">Name used to save the state of the desired music in the PlayerPrefs</param>
    /// <param name="sampleState">True to save the music at the current sample state. False to reinitialise the state to the beginning of the music</param>
    public void setMusicState(string saveName, bool saveCurrentState)
    {
        if(saveCurrentState)
        {
            PlayerPrefs.SetInt(saveName, this.GetComponent<AudioSource>().timeSamples);
        }
        else
        {
            PlayerPrefs.SetInt(saveName, 0);
        }
    }

    private void resumeMusic()
    {
        try
        {
            int sample = PlayerPrefs.GetInt(saveSample);
            GetComponent<AudioSource>().timeSamples = sample;
        }
        catch(System.Exception e)
        {
            Debug.Log(e);
        }
    }

    private void switchAudio()
    {
        if (currentAudio != gameManagerScript.closeness)
        {
            currentSample = speaker[0].timeSamples;
            //Switch old audio to speaker 1
            speaker[1].clip = clip[currentAudio];
            speaker[1].timeSamples = currentSample;

            //Switch new audio to speaker 0
            speaker[0].clip = clip[gameManagerScript.closeness];
            speaker[0].timeSamples = currentSample;
            currentAudio = gameManagerScript.closeness;

            //Cross Fade
            StartCoroutine(crossFade(fadeDelay));
            
        }
    }

    private IEnumerator crossFade(float delay)
    {
        speaker[0].volume = 0f;
        speaker[1].volume = 1f;

        speaker[0].Play();
        speaker[1].Play();

        while(speaker[1].volume >= 0.01f)
        {
            speaker[0].volume += fadeCoeff;
            speaker[1].volume -= fadeCoeff;
            yield return new WaitForSecondsRealtime(delay);
        }

        speaker[0].volume = 1f;
        speaker[1].Stop();

    }

}
