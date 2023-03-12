using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.SceneManagement;
using UnityEngine.UI;



/// <summary>
/// Class <c>setup</c> to attach to setup gameObject of each scene.
/// This class set the whole scene depending on what is stored in PlayerPrefs.
/// For main menu, it sets the necessary buttons depending on unlocked levels.
/// For level, it sets the correct number of tubes, with appropriate color layers.
/// </summary>
public class setup : MonoBehaviour
{
    //Init objects
    [SerializeField] private GameObject tubeParent;
    [SerializeField] private GameObject tubePrefab;
    [SerializeField] private GameObject robot;

    //Scene parameters
    //This is to be set in editor according to the level disposition.
    [SerializeField] private int numberOfTube = 3;
    [SerializeField] private int numberOfEmptyTube = 1;
    [SerializeField] private int numberOfMaxLayers = 4;
    [SerializeField] private int numberOfInitLayers = 2;
    [SerializeField] private int maxColors = 3;
    [SerializeField] public int completeTubeToWin = 2;

    //Tube positions
    private List<Vector3> posTubes = new List<Vector3>(); //To set in start func: Possible position for each tube on the scene.


    //Color
    private Color colorArrow, colorButtons;
    private GameObject musicManager;

    // Start is called before the first frame update
    void Start()
    {
        levels.robotColorPerLevel.Clear();
        Debug.Log(PlayerPrefs.GetString(save.robotColor));
        foreach(string colorVal in PlayerPrefs.GetString(save.robotColor).Split(' '))
        {
            Debug.Log(colorVal);
            try
            {
                levels.robotColorPerLevel.Add(gameManager.colors[int.Parse(colorVal)]);
            }
            catch(Exception e)
            {
                Debug.Log("String is: \'" + colorVal + "\'\n" + e);
            }
        }

        musicManager = GameObject.Find("Music Manager");

        if (SceneManager.GetActiveScene() == SceneManager.GetSceneByName("MainMenu")) //If we are in the main menu screen.
        {
            PlayerPrefs.SetInt(save.musicTime, 0);
            initMainMenu();
        }
        else //If we are in a level scene
        {
            //Tube positions
            posTubes.Add(new Vector3(-0.45f, 1.1f, 0f));
            posTubes.Add(new Vector3(0.45f, 1.1f, 0f));
            posTubes.Add(new Vector3(-0.45f, -0.2f, 0f));
            posTubes.Add(new Vector3(0.45f, -0.2f, 0f));
            posTubes.Add(new Vector3(-1.35f, 1.1f, 0f));
            posTubes.Add(new Vector3(1.35f, 1.1f, 0f));
            posTubes.Add(new Vector3(-1.35f, -0.2f, 0f));
            posTubes.Add(new Vector3(1.35f, -0.2f, 0f));
            posTubes.Add(new Vector3(-0.9f, -1.4f, 0f));
            posTubes.Add(new Vector3(0.9f, -1.4f, 0f));
            musicManager.GetComponent<AudioSource>().timeSamples = PlayerPrefs.GetInt(save.musicTime);
            initLevel();
        }
    }


    /// <summary>
    /// Method <c>initLevel</c> Initialise a level screen after loading
    /// First, the robot color is loaded according to saved in "Current Level" PlayerPrefs 
    /// Next, the tube are spawned and set in the right position.
    /// Next, the tubes are filled according to the setup written in levels script.
    /// Also, for now, one of the top color of the tube is modified accordingly to the robot color. If no change is possible, we switch all colors value to fit the robot: The level stays the same, but the colors are different.
    /// </summary>
    private void initLevel()
    {

        List<Color> randomCol = new List<Color>();
        for(int i = 0; i < Mathf.Min(numberOfInitLayers,maxColors); i++)
        {
            randomCol.Add(gameManager.colors[UnityEngine.Random.Range(0, gameManager.colors.Count())]);
        }

        Color colorRob;
        colorRob = levels.robotColorPerLevel[PlayerPrefs.GetInt(save.currentLevel)-1];
        robot.GetComponent<robot>().initialise(colorRob);


        //We verify that we can change a color with the robot !
        bool isColorRobotChanged = false;
        foreach (List<Color> col in levels.getLevelColors()) 
        {
            try
            {
                if (col.LastOrDefault() == colorRob)
                {
                    isColorRobotChanged = true;
                    break;
                }
            }
            catch (Exception e) { Debug.Log(e); }
        }

        //We switch all layers of one color if impossible to find a colors corresponding to robot
        if(!isColorRobotChanged) 
        {
            Color switchColor = levels.getLevelColors()[0].LastOrDefault();
            for (int i = 0; i < levels.getLevelColors().Count; i++)
            {
                for(int j = 0; j < levels.getLevelColors()[i].Count; j++)
                {
                    try
                    {
                        if (levels.getLevelColors()[i][j] == colorRob) //Switch colors similar to robot by the switch color
                        {
                            levels.getLevelColors()[i][j] = switchColor;
                        }
                        else if (levels.getLevelColors()[i][j] == switchColor) 
                        {
                            levels.getLevelColors()[i][j] = colorRob;
                        }
                    }
                    catch (Exception e) { Debug.Log(e); }

                }
            }
        }

        //Initialise tubes
        bool isTubeChangedByRobot = false;
        for (int i = 0; i < numberOfTube; i++)
        {
            GameObject tube = Instantiate(tubePrefab, tubeParent.transform);
            tube.transform.localPosition = posTubes[i];
            if(i < numberOfTube - numberOfEmptyTube)
            {
                tube.GetComponent<testTube>().initialise(numberOfMaxLayers, levels.getLevelColors()[i]);

                //Change color according to robot
                try
                {
                    if(tube.GetComponent<testTube>().colorList.Peek() == colorRob && !isTubeChangedByRobot)
                    {
                        tube.GetComponent<testTube>().removeColorLayer();
                        /*
                         * Strategy with randomize switched color
                        Color newColor = gameManager.colors[UnityEngine.Random.Range(0, maxColors)];
                        while(newColor == colorRob)
                        {
                            newColor = gameManager.colors[UnityEngine.Random.Range(0, maxColors)];
                        }
                        */
                        Color newColor = gameManager.colors[0];
                        for(int iCol = 0; iCol < maxColors; iCol++)
                        {
                            newColor = gameManager.colors[iCol]; 
                            if(newColor != colorRob)
                            {
                                break;
                            }
                        }


                        tube.GetComponent<testTube>().addColorLayer(newColor);
                        isTubeChangedByRobot = true;
                    }
                }
                catch(Exception ex)
                {
                    Debug.Log(ex);
                }
            }
            else
            {
                tube.GetComponent<testTube>().initialise(numberOfMaxLayers, new List<Color>());
            }


        }
        
        //gameManager.colors[UnityEngine.Random.Range(0, gameManager.colors.Count())];
    }


    /// <summary>
    /// Method <c>initMainMeny</c> Initialise the main screen after loading.
    /// Set all level + quit + arrows buttons and colors.
    /// The menu start at the last available level. In this setup, only left arrow is available if number of availabe levels > 9
    /// </summary>
    private void initMainMenu()
    {
        //Find components
        colorArrow = gameManager.colors[UnityEngine.Random.Range(0, gameManager.colors.Count())];
        colorButtons = gameManager.colors[UnityEngine.Random.Range(0, gameManager.colors.Count())];
        GameObject buttons = GameObject.Find("Level Buttons");
        selection selectScript = GameObject.Find("Selection Canvas").GetComponent<selection>();
        GameObject levelsButton = GameObject.Find("Level Buttons");
        GameObject quitButton = GameObject.Find("Quit");

        //Initialise
        robot.GetComponent<robot>().initialise(levels.robotColorPerLevel.LastOrDefault());
        selectScript.initialise(colorArrow);
        quitButton.GetComponent<Image>().color = colorArrow;
        selectScript.displayLevelButton(PlayerPrefs.GetInt(save.availableLevels));
        selectScript.maxDisplayedLevel = PlayerPrefs.GetInt(save.availableLevels); ;

        //Arrows hiding
        if(PlayerPrefs.GetInt(save.availableLevels) <= 9)
        {
            selectScript.gameObject.transform.GetChild(1).gameObject.SetActive(false);
        }
        selectScript.gameObject.transform.GetChild(0).gameObject.SetActive(false);


    }
}


    