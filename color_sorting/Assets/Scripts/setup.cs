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
    [SerializeField] private int numberOfInitLayers = 2;
    [SerializeField] private int numberOfMaxLayers = 4;
    [SerializeField] private int maxColors = 3;

    //Tube positions
    private List<Vector3> posTubes = new List<Vector3>();


    //Color
    private Color colorArrow, colorButtons;

    // Start is called before the first frame update
    void Start()
    {
        //Tube positions
        posTubes.Add(new Vector3 ( -0.45f, 1.1f, 0f ));
        posTubes.Add(new Vector3 ( 0.45f, 1.1f, 0f ));
        posTubes.Add(new Vector3 ( -0.45f, -0.5f, 0f ));
        posTubes.Add(new Vector3 ( 0.45f, -0.5f, 0f ));
        posTubes.Add(new Vector3 ( -1.35f, 1.1f, 0f ));
        posTubes.Add(new Vector3 ( 1.35f, 1.1f, 0f ));
        posTubes.Add(new Vector3 ( -1.35f, -0.5f, 0f ));
        posTubes.Add(new Vector3 ( 1.35f, -0.5f, 0f ));

        if (SceneManager.GetActiveScene() == SceneManager.GetSceneByName("MainMenu"))
        {
            initMainMenu();
        }
        else
        {
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

        for (int i = 0; i < numberOfTube; i++)
        {
            GameObject tube = Instantiate(tubePrefab, tubeParent.transform);
            tube.transform.localPosition = posTubes[i];
            tube.GetComponent<testTube>().initialise(numberOfMaxLayers, numberOfInitLayers, randomCol);
        }

        robot.GetComponent<robot>().initialise(randomCol[0]);
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
        if(gameManager.availableLevels >= 10)
        {
            selectScript.gameObject.transform.GetChild(0).gameObject.SetActive(false);
        }

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
            childImage.color = colorButtons;
            childImage.GetComponentInChildren<Text>(true).text = currentLevel.ToString();
            if (gameManager.availableLevels < currentLevel)
            {
                childImage.gameObject.SetActive(false);
            }
            else
            {
                childImage.gameObject.SetActive(true);
            }
            currentLevel++;
        }
    }

}
