using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class setup : MonoBehaviour
{
    //Init objects
    [SerializeField] private GameObject tubeParent;
    [SerializeField] private GameObject tubePrefab;
    [SerializeField] private GameObject robot;

    //Scene parameters
    [SerializeField] private int numberOfTube = 3;
    [SerializeField] private int numberOfEmptyTube = 1;
    [SerializeField] private int numberOfMaxLayers = 4;
    [SerializeField] private int numberOfInitLayers = 2;
    [SerializeField] private int maxColors = 3;
    [SerializeField] private int completeTubeToWin = 2;

    //Tube positions
    private List<Vector3> posTubes = new List<Vector3>();


    //Color
    private Color colorArrow, colorButtons;
    private GameObject musicManager;

    // Start is called before the first frame update
    void Start()
    {

        musicManager = GameObject.Find("Music Manager");
        if (SceneManager.GetActiveScene() == SceneManager.GetSceneByName("MainMenu"))
        {
            PlayerPrefs.SetInt("Music Timestamp", 0);
            initMainMenu();
        }
        else
        {
            //Tube positions
            posTubes.Add(new Vector3(-0.45f, 1.1f, 0f));
            posTubes.Add(new Vector3(0.45f, 1.1f, 0f));
            posTubes.Add(new Vector3(-0.45f, -0.5f, 0f));
            posTubes.Add(new Vector3(0.45f, -0.5f, 0f));
            posTubes.Add(new Vector3(-1.35f, 1.1f, 0f));
            posTubes.Add(new Vector3(1.35f, 1.1f, 0f));
            posTubes.Add(new Vector3(-1.35f, -0.5f, 0f));
            posTubes.Add(new Vector3(1.35f, -0.5f, 0f));
            musicManager.GetComponent<AudioSource>().timeSamples = PlayerPrefs.GetInt("Music Timestamp");
            initLevel();
        }
    }

    private void initLevel()
    {

        List<Color> randomCol = new List<Color>();
        for(int i = 0; i < Mathf.Min(numberOfInitLayers,maxColors); i++)
        {
            randomCol.Add(gameManager.colors[UnityEngine.Random.Range(0, gameManager.colors.Count())]);
        }

        Color colorRob = gameManager.colors[PlayerPrefs.GetInt("Robot Color" + SceneManager.GetActiveScene().name)];
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
                        if (levels.getLevelColors()[i][j] == colorRob)
                        {
                            levels.getLevelColors()[i][j] = gameManager.colors.LastOrDefault();
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
        robot.GetComponent<robot>().initialise(colorArrow);
        selectScript.initialise(colorArrow);
        quitButton.GetComponent<Image>().color = colorArrow;
        displayLevelButton(gameManager.availableLevels);
        selectScript.currentMaxLevel = gameManager.availableLevels; ;

        //Arrows hiding
        if(gameManager.availableLevels <= 9)
        {
            selectScript.gameObject.transform.GetChild(1).gameObject.SetActive(false);
        }
        selectScript.gameObject.transform.GetChild(0).gameObject.SetActive(false);


    }

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
            if (gameManager.availableLevels < currentLevel)
            {
                childImage.gameObject.SetActive(false);
            }
            else
            {
                childImage.gameObject.SetActive(true);
                if(currentLevel < maxLevel)
                {
                    childImage.color = colorButtons;
                }
            }
            currentLevel++;
        }
    }

}
