using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using System;

public class musicManager : MonoBehaviour
{
    //Music to used -> To setup in editor
    //Audio clip for one shot musics
    //List for transitional musics
    [SerializeField] private AudioClip mainMenuMusic;
    [SerializeField] public List<AudioClip> levelMusicParts;
    [HideInInspector] public int currentLevelMusicSection;
    
    //User parameters
    [SerializeField][Range(0.0f, 1.0f)] float maxVolume = 1.0f;

    //Speakers for main musics + transitions
    private AudioSource[] speakers;

    //Transition parameters
    private float fadeDelay;
    private float fadeCoeff;

    // Start is called before the first frame update
    void Start()
    {
        //Transition parameters
        fadeDelay = 0.01f;
        fadeCoeff = 0.002f;

        //Speakers initialisation
        speakers = new AudioSource[2];
        speakers[0] = this.gameObject.AddComponent<AudioSource>();
        speakers[0].loop = true;
        speakers[0].playOnAwake = false;
        speakers[1] = this.gameObject.AddComponent<AudioSource>();
        speakers[1].loop = true;
        speakers[1].playOnAwake = false;

        //Set up the music for current scene
        musicChoiceManager();
        currentLevelMusicSection = 0;

    }

    /// <summary>
    ///musicChoiceManager setup the music to be played depending on the level.
    ///It is here that all choices of music is performed.
    ///Each music are chosen depending on criteria defined by the game dev. Usually, the scene name is a pretty straightforward way of choosing musics.
    /// </summary>
    private void musicChoiceManager()
    {
        string currentScene = SceneManager.GetActiveScene().name;
        speakers[0].volume = maxVolume;
        
        if(currentScene == "Main Menu" || currentScene == "Level Selection")
        {
            speakers[0].clip = mainMenuMusic;
            resumeMusic(speakers[0], save.mainMenuMusicState);
        }
        else if(currentScene == "Level")
        {
            speakers[0].clip = levelMusicParts[0];
            resumeMusic(speakers[0], save.levelMusicState);
        }
        
        speakers[0].Play();
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

    /// <summary>
    /// resumeMusic set the music played by the given AudioSource at the time saved in PlayerPrefs
    /// </summary>
    /// <param name="speaker">AudioSource containing the music to modify</param>
    /// <param name="saveName">string corresponding to the PlayerPrefs token referencing the time to set the music</param>
    private void resumeMusic(AudioSource speaker, string saveName)
    {
        int sample = 0;
        try
        {
            sample = PlayerPrefs.GetInt(saveName);
        }
        catch(System.Exception e)
        {
            Debug.LogWarning(e);
        }
        speaker.timeSamples = sample;
    }

    /// <summary>
    /// switchAudio enables to switch smoothly between two audio clips.
    /// This is used to transition between musics without a clear cut.
    /// To do so, a cross fade is perfomed between two speakers running the musics at the same time.
    /// </summary>
    /// <param name="newMusic">New audio clip to use as a replacement of the current one</param>
    public void switchAudio(AudioClip newMusic)
    {
            int currentSample = speakers[0].timeSamples;
            
            //Switch old audio to speaker 1
            speakers[1].clip = speakers[0].clip;
            speakers[1].timeSamples = currentSample;
            speakers[1].volume = maxVolume;
            speakers[1].Play();

            //Switch new audio to speaker 0
            speakers[0].Stop();
            speakers[0].clip = newMusic;
            speakers[0].volume = 0f;
            speakers[0].timeSamples = currentSample;
            
            //Cross Fade
            try
            {
                StartCoroutine(crossFade(fadeDelay, fadeCoeff));
            }
            catch(Exception e)
            {
                Debug.LogWarning("Warning during cross fade in musicManager. It is probable that the error comes from a wrong timeSample value\n" + e);
            }
            
        
    }

    /// <summary>
    ///crossFade performs a fade between two audio clips
    /// The audio contained in speaker 1 is smoothly replaced by the one contained in speaker 0
    /// </summary>
    /// <param name="delay">Speed at which the cross fade is performed</param>
    /// <param name="fadeCoeff">Strength at which the volume is modified. Higher values means faster cross fade</param>
    /// <returns></returns>
    private IEnumerator crossFade(float delay, float fadeCoeff)
    {
        speakers[0].volume = 0f;
        speakers[1].volume = maxVolume;
        speakers[0].Play();
        speakers[1].Play();

        while(speakers[1].volume >= 0.01f)
        {
            speakers[0].volume += fadeCoeff;
            speakers[1].volume -= fadeCoeff;
            yield return new WaitForSecondsRealtime(delay);
        }

        speakers[0].volume = maxVolume;
        speakers[1].Stop();

    }
}
