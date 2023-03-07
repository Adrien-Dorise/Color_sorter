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
    [SerializeField] GameObject levelIconPrefab;
    [SerializeField] private GameObject rightArrow, leftArrow;
    [SerializeField] private GameObject musicManager;


    private Color arrowsColor;

    public int currentMaxLevel;
    public string textLevel = "1"; //"1" is for init, do not touch
    private int levelPerScreen;

    // Start is called before the first frame update
    void Start()
    {
        levelPerScreen = 9;
        musicManager = GameObject.Find("Music Manager");
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
        if(currentMaxLevel + levelPerScreen*2 >= gameManager.availableLevels)
        {
            currentMaxLevel = gameManager.availableLevels;
            rightArrow.SetActive(false);
        }
        else
        {
            rightArrow.SetActive(true); 
            currentMaxLevel = currentMaxLevel + levelPerScreen;
        }

        displayLevelButton(currentMaxLevel);

    }


    /// <summary>
    /// Method <c>leftScroll</c> manage left arrow button behaviour
    /// </summary>
    public void leftScroll()
    {
        rightArrow.SetActive(true);
        if (currentMaxLevel - levelPerScreen < 10)
        {
            currentMaxLevel = 1;
            leftArrow.SetActive(false);
        }
        else
        {
            currentMaxLevel = currentMaxLevel - levelPerScreen;
            leftArrow.SetActive(true);
        }

        displayLevelButton(currentMaxLevel);
    }


    /// <summary>
    /// Method <c>onQuitApp</c> manage quit button behaviour in the main menu
    /// </summary>
    public void onQuitApp()
    {
        Application.Quit();
    }


    /// <summary>
    /// Method <c>onQuitMenu</c> manage quit button behaviour in a level
    /// </summary>
    public void onQuitToMenu()
    {
        SceneManager.LoadScene("MainMenu");
    }


    /// <summary>
    /// Method <c>onReplay</c> manage replay level button behaviour
    /// </summary>
    public void onReplay()
    {
        PlayerPrefs.SetInt("Music Timestamp", musicManager.GetComponent<AudioSource>().timeSamples);
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }


    /// <summary>
    /// Method <c>levelSelection</c> manage level button behaviour
    /// </summary>
    public void levelSelection()
    {
        SceneManager.LoadScene("Level" + textLevel);
        gameManager.currentScene = "Level" + textLevel;
        PlayerPrefs.SetInt("Current Level", int.Parse(textLevel));
    }


    /// <summary>
    /// Method <c>Reset</c> manage reset save button behaviour
    /// </summary>
    public void Reset()
    {
        PlayerPrefs.DeleteAll();
        SceneManager.LoadScene("MainMenu");
    }

    /// <summary>
    /// Method <c>displayLevelButton</c> decide which main menu's selection arrows have to be displayed
    /// Depending on current screen and available levels, it is decide to display right, left or both selection arrows
    /// </summary>
    public void displayLevelButton(int maxLevel)
    {
        GameObject levelsButton = GameObject.Find("Level Buttons");
        int currentLevel = maxLevel;
        if(currentLevel >= 10)
        {
            currentLevel = currentLevel - (currentLevel % 9) + 1;
        }
        else
        {
            currentLevel = 1;
        }
        foreach (Image childImage in levelsButton.GetComponentsInChildren<Image>(true))
        {
            
            childImage.GetComponentInChildren<Text>(true).text = currentLevel.ToString();
            if (gameManager.availableLevels < currentLevel || currentLevel > PlayerPrefs.GetInt("MAXMAX"))
            {
                childImage.gameObject.SetActive(false);
            }
            else
            {
                childImage.gameObject.SetActive(true);
                if(currentLevel < PlayerPrefs.GetInt("Available Levels"))
                {
                    if(levels.Debug)
                    {
                        childImage.color = gameManager.colors[0];
                    }
                    else
                    {
                        childImage.color = levels.robotColorPerLevel[currentLevel];
                    }
                }

            }
            currentLevel++;
        }
    }

}


}
