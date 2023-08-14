using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

/// <summary>
/// Class <c>selection</c> to attach to selection gameObject of each scenes.
/// This class manage the buttons' behaviour in the menu
/// </summary>
public class selection : MonoBehaviour
{
    //All level setup
    private musicManager musicManagerScript;

    //Level selection setup
    [SerializeField] private GameObject rightArrow, leftArrow;


    private Color arrowsColor;

    [HideInInspector] public int maxDisplayedLevel; //Max level on the displayed icons in the selection screen
    [HideInInspector] public string textLevel = "1"; //"1" is for init, do not touch
    private int levelPerScreen;

    // Start is called before the first frame update
    void Start()
    {
        levelPerScreen = 9;
        musicManagerScript = GameObject.Find("Music Manager").GetComponent<musicManager>();
    }


    /// <summary>
    /// Method <c>initialise</c> setup the main menu arrows color
    /// </summary>
    public void initialise(Color color)
    {
        arrowsColor = color;
        rightArrow.GetComponent<Image>().color = arrowsColor;
        leftArrow.GetComponent<Image>().color = arrowsColor;
    }


    /// <summary>
    /// Method <c>rightScroll</c> manage right arrow button behaviour
    /// </summary>
    public void rightScroll()
    {
        leftArrow.SetActive(true);
        Debug.Log(maxDisplayedLevel);
        if(maxDisplayedLevel + (levelPerScreen) >= PlayerPrefs.GetInt(save.availableLevels)) //We are at the maximum available levels
        {
            maxDisplayedLevel = PlayerPrefs.GetInt(save.availableLevels);
            rightArrow.SetActive(false);
        }
        else //We can still scroll right
        {
            rightArrow.SetActive(true); 
            maxDisplayedLevel = maxDisplayedLevel + levelPerScreen;
        }

        displayLevelButton(maxDisplayedLevel);

    }


    /// <summary>
    /// Method <c>leftScroll</c> manage left arrow button behaviour
    /// </summary>
    public void leftScroll()
    {
        rightArrow.SetActive(true);
        if (maxDisplayedLevel - levelPerScreen < 10)
        {
            maxDisplayedLevel = 9;
            leftArrow.SetActive(false);
        }
        else
        {
            maxDisplayedLevel = maxDisplayedLevel - ((maxDisplayedLevel*1)%levelPerScreen) - 1;
            leftArrow.SetActive(true);
        }

        displayLevelButton(maxDisplayedLevel);
    }


    /// <summary>
    /// Method <c>onQuitApp</c> manage quit button behaviour in the main menu
    /// </summary>
    public void onQuitApp()
    {
        musicManagerScript.setMusicState(save.mainMenuMusicState, true);
        SceneManager.LoadScene("Main Menu");
    }


    /// <summary>
    /// Method <c>onQuitMenu</c> manage quit button behaviour in a level
    /// </summary>
    public void onQuitToMenu()
    {
        musicManagerScript.setMusicState(save.mainMenuMusicState, false);
        SceneManager.LoadScene("Level Selection");
    }


    /// <summary>
    /// Method <c>onReplay</c> manage replay level button behaviour
    /// </summary>
    public void onReplay()
    {
        musicManagerScript.setMusicState(save.levelMusicState, true);
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }


    /// <summary>
    /// Method <c>levelSelection</c> manage level button behaviour
    /// </summary>
    public void levelSelection()
    {
        gameManager.currentScene = "Level" + textLevel;
        PlayerPrefs.SetInt("Current Level", int.Parse(textLevel));
        musicManagerScript.setMusicState(save.mainMenuMusicState, false);
        musicManagerScript.setMusicState(save.levelMusicState, false);
        SceneManager.LoadScene("Level");
    }


    /// <summary>
    /// Method <c>Reset</c> manage reset button behaviour
    /// </summary>
    public void Reset()
    {
        int levelsAvailable = 21; // To set manually
        PlayerPrefs.DeleteAll();
        string str = "";

        gameManager managerScript = GameObject.Find("Game Manager").GetComponent<gameManager>();
        PlayerPrefs.SetInt(save.availableLevels, levelsAvailable);
        for(int i = 0; i < levelsAvailable; i++)
        {
            str += "5 ";
        }
        str = str.Remove(str.Length - 1);
        PlayerPrefs.SetString(save.robotColor,str);

        musicManagerScript.setMusicState(save.mainMenuMusicState, true);
        SceneManager.LoadScene("Main Menu");
    }


    /// <summary>
    /// Method <c>displayLevelButton</c> decide which main menu's selection arrows have to be displayed
    /// Depending on current screen and available levels, it is decide to display right, left or both selection arrows
    /// </summary>
    public void displayLevelButton(int maxLevel)
    {
        GameObject levelsButton = GameObject.Find("Level Buttons");

        int firstScreenLevel = maxLevel - ((maxLevel-1)%levelPerScreen);

        Debug.Log(maxLevel + " / " + firstScreenLevel + " / " + (firstScreenLevel-1)%levelPerScreen);
        foreach (Image childImage in levelsButton.GetComponentsInChildren<Image>(true))
        {
            
            childImage.GetComponentInChildren<Text>(true).text = firstScreenLevel.ToString();
            childImage.color = Color.white;
            if (PlayerPrefs.GetInt(save.availableLevels) < firstScreenLevel || firstScreenLevel > save.maxAvailableLevels)
            {
                childImage.gameObject.SetActive(false);
            }
            else
            {
                childImage.gameObject.SetActive(true);
                if(firstScreenLevel < PlayerPrefs.GetInt("Available Levels"))
                {
                    if(save.debugDev)
                    {
                        childImage.color = gameManager.colors[0];
                    }
                    else
                    {
                        try
                        {
                            childImage.color = levels.robotColorPerLevel[firstScreenLevel];
                            //Debug.Log("level " + firstScreenLevel + " / color" + levels.robotColorPerLevel[firstScreenLevel]);
                        }
                        catch(Exception e)
                        {
                            //Debug.LogWarning("Warning: Not enough colors saved in the playerPrefs to setup all levels. Save file might be corrupted. Non-available colors are replaced with white.");
                        }
                    }
                }

            }
            firstScreenLevel++;
        }
    }


}


