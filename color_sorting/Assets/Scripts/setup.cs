using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
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
    private int numberOfTube;
    private int numberOfEmptyTube;
    private int numberOfMaxLayers;
    private int numberOfInitLayers;
    [SerializeField] public int completeTubeToWin;
    private List<List<Color>> colorTubesList;
    

    //Tube positions
    private List<Vector3> posTubes = new List<Vector3>(); //To set in start func: Possible position for each tube on the scene.


    //Color
    private Color colorArrow, colorButtons;

    private GameObject musicManager;

    // Start is called before the first frame update
    void Start()
    {
        levels.robotColorPerLevel.Clear();
        foreach(string colorVal in PlayerPrefs.GetString(save.robotColor).Split(' '))
        {
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

        if (SceneManager.GetActiveScene() == SceneManager.GetSceneByName("Level Selection")) //If we are in the main menu screen.
        {
            PlayerPrefs.SetInt(save.musicTime, 0);
            initMainMenu();
        }
        else //If we are in a level scene
        {
            //Tube positions
            int x1 = -833, x2 = 833, x3 = -2500, x4 = 2500;
            int y1 = 1500, y2 = -1200, y3 = 3800; 
            posTubes.Add(new Vector3(x1, y1, 0f));
            posTubes.Add(new Vector3(x2, y1, 0f));
            posTubes.Add(new Vector3(x1, y2, 0f));
            posTubes.Add(new Vector3(x2, y2, 0f));
            posTubes.Add(new Vector3(x1, y3, 0f));
            posTubes.Add(new Vector3(x2, y3, 0f));
            posTubes.Add(new Vector3(x3, y1, 0f));
            posTubes.Add(new Vector3(x4, y1, 0f));
            posTubes.Add(new Vector3(x3, y2, 0f));
            posTubes.Add(new Vector3(x4, y2, 0f));
            posTubes.Add(new Vector3(x3, y3, 0f));
            posTubes.Add(new Vector3(x4, y3, 0f));
            musicManager.GetComponent<AudioSource>().timeSamples = PlayerPrefs.GetInt(save.musicTime);
            colorTubesList = levels.getLevelColors();
            initLevel();
        }
    }

    public void initLevelParameters(int numberTube, int numberEmptyTube, int numberInitLayers, int numberMaxLayers, int tubeToWin)
    {
        numberOfTube = numberTube;
        numberOfEmptyTube = numberEmptyTube;
        numberOfMaxLayers = numberMaxLayers;
        numberOfInitLayers = numberInitLayers;
        completeTubeToWin = tubeToWin;
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
        for(int i = 0; i < numberOfInitLayers; i++)
        {
            randomCol.Add(gameManager.colors[UnityEngine.Random.Range(0, gameManager.colors.Count())]);
        }

        Color colorRob;
        colorRob = levels.robotColorPerLevel[PlayerPrefs.GetInt(save.currentLevel)-1];
        robot.GetComponent<robot>().initialise(colorRob);


        //We verify that we can change a color with the robot !
        bool isColorRobotChanged = false;
        foreach (List<Color> col in colorTubesList) 
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
            Color switchColor = colorTubesList[0].LastOrDefault();
            for (int i = 0; i < colorTubesList.Count; i++)
            {
                for(int j = 0; j < colorTubesList[i].Count; j++)
                {
                    try
                    {
                        if (colorTubesList[i][j] == colorRob) //Switch colors similar to robot by the switch color
                        {
                            colorTubesList[i][j] = switchColor;
                        }
                        else if (colorTubesList[i][j] == switchColor) 
                        {
                            colorTubesList[i][j] = colorRob;
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
                tube.GetComponent<testTube>().initialise(numberOfMaxLayers, colorTubesList[i]);

                //Change color according to robot
                try
                {
                    if(tube.GetComponent<testTube>().colorList.Peek() == colorRob && !isTubeChangedByRobot)
                    {
                        tube.GetComponent<testTube>().removeColorLayer();
                        Color newColor = gameManager.colors[0];
                        for(int iCol = 0; iCol < gameManager.colors.Count(); iCol++)
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
        selection selectScript = GameObject.Find("Buttons Canvas").GetComponent<selection>();
        GameObject levelsButton = GameObject.Find("Level Buttons");
        GameObject quitButton = GameObject.Find("Quit");
        GameObject buttonsCanvas = GameObject.Find("Buttons Canvas");

        //Initialise
        robot.GetComponent<robot>().initialise(levels.robotColorPerLevel.LastOrDefault());
        selectScript.initialise(colorArrow);
        quitButton.GetComponent<Image>().color = colorArrow;
        selectScript.displayLevelButton(PlayerPrefs.GetInt(save.availableLevels));
        selectScript.maxDisplayedLevel = PlayerPrefs.GetInt(save.availableLevels); ;

        //Arrows hiding
        if(PlayerPrefs.GetInt(save.availableLevels) <= 9)
        {
            buttonsCanvas.gameObject.transform.GetChild(1).gameObject.SetActive(false);
        }
        buttonsCanvas.gameObject.transform.GetChild(0).gameObject.SetActive(false);


    }
}


    