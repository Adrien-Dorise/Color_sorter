using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

/// <summary>
/// Class <c>gameManager</c> is to attach to the game manager gameObject of each scenes.
/// This class manages the state machine of the game (more details in the state machine file).
/// It is this class that is used to update the game after each player's interaction with the game
/// </summary>
public class gameManager : MonoBehaviour
{
    
    public enum states { wait, powerSelection, robotPower, idleNoTube, idleTube, poorColor, endLevel, levelSelection, def }
    public enum actions { noAction, clickedTube, clickedRobot, clickedBackground, finishAction, usePower }
    [SerializeField] public states currentState; //{ get; private set; }
    static public List<Color> colors;
    [SerializeField] public GameObject memoryTube;
    private GameObject tubesGroupObject;
    [SerializeField] private Image victorySprite;
    private robot robotScript;
    private robotPower robotPowerScript;
    private GameObject robotCanvas;
    private IEnumerator inactivityRoutine;
    private powerManager powerManagerScript;
    private audio audioManager;
    private GameObject powerCanvas;
    private GameObject tokenCanvas;
    private musicManager musicManagerScript;

    private setup setupScript;
    [SerializeField] private int completedTube;
    public bool isNewTubeCompleted;



    //Pooring animation
    //To set in Start function!
    [SerializeField] private float pooringTime;
    [SerializeField] private float translationTime;
    [SerializeField] private float xOffset;
    [SerializeField] private float yOffset;


    static public string currentScene { get; set; }




    private void Awake()
    {
        Application.targetFrameRate = 60;
        tubesGroupObject = GameObject.Find("Tubes");
        robotScript = GameObject.Find("Robot").GetComponent<robot>();
        audioManager = GameObject.Find("Audio Manager").GetComponent<audio>();
        setupScript = GameObject.Find("Setup").GetComponent<setup>();
        musicManagerScript = GameObject.Find("Music Manager").GetComponent<musicManager>();
        robotCanvas = GameObject.Find("Robot Canvas");
        if(SceneManager.GetActiveScene() == SceneManager.GetSceneByName("Level"))
        {
            victorySprite = GameObject.Find("Victory Canvas").transform.GetChild(0).GetComponent<Image>();
            robotPowerScript = GameObject.Find("Power Manager").GetComponent<robotPower>();
            powerManagerScript = GameObject.Find("Power Manager").GetComponent<powerManager>();
            powerCanvas = GameObject.Find("Power Buttons");
            tokenCanvas = GameObject.Find("Token Canvas");
        }


        //These parts have to be done in Awake so it can be used by other scripts in during their Start initialisation
        currentScene = "Level Selection";
        completedTube = 0;
        isNewTubeCompleted = false;
        if(SceneManager.GetActiveScene() == SceneManager.GetSceneByName("Level Selection"))
        {
            currentState = states.levelSelection;
        }
        else if(SceneManager.GetActiveScene() == SceneManager.GetSceneByName("Main Menu"))
        {
            currentState = states.def;
        }
        else 
        {
            currentState = states.idleNoTube;
        }
        colors = colorBlindSettings.loadColors();


        if (!PlayerPrefs.HasKey(save.availableLevels)) //First play ! 
        {
            if(save.debugDev)
            {
                string str = "";
                PlayerPrefs.SetInt(save.availableLevels, save.maxAvailableLevels);
                for(int i = 0; i < save.maxAvailableLevels; i++)
                {
                    str += "255 ";
                }
                str = str.Remove(str.Length - 1);
                PlayerPrefs.SetString(save.robotColor,str);
            }
            else
            {
                PlayerPrefs.SetInt(save.availableLevels, 1);
                PlayerPrefs.SetString(save.robotColor, "0");
            }
        }

    }

    // Start is called before the first frame update
    private void Start()
    {
        //Pooring animation
        pooringTime = 0.5f;
        translationTime = 0.2f;
        xOffset = 1500f;
        yOffset = 1250f;

        
        if(SceneManager.GetActiveScene() == SceneManager.GetSceneByName("Level"))
        {
            powerCanvas.GetComponent<Canvas>().enabled = false;
            foreach(Button tokenButton in tokenCanvas.GetComponentsInChildren<Button>())
            {
                tokenButton.interactable = false;
            }
            inactivityRoutine = inactivityAction();
            robotCanvas.transform.GetChild(0).GetChild(1).GetChild(0).GetComponent<Text>().text = PlayerPrefs.GetInt(save.currentLevel).ToString();
        }
        memoryTube = null;
    }


    /// <summary>
    /// Method <c>areSameolor</c> checks if two tubes's top layers are identical
    /// It is used to verify is it is possible to poor one tube into the other. 
    /// </summary>
    /// <param name="tube1"> First tube to check </param>
    /// <param name="tube2"> Second tube to check </param>
    /// <returns> True if same color or at least one tube empty, false otherwise </returns>
    static public bool areSameColor(GameObject tube1, GameObject tube2)
    {
        if(tube1.GetComponent<testTube>().colorList.Count == 0 || tube2.GetComponent<testTube>().colorList.Count == 0)
        {
            return true;
        }
        else
        {
            return tube1.GetComponent<testTube>().colorList.Peek() == tube2.GetComponent<testTube>().colorList.Peek();
        }
    }


    /// <summary>
    /// Method <c>setSortOrder</c> manage the order of two tubes on a same canvas
    /// It is used during pooring animation to ensure that the pooring tube is in front of the poored tube and avoid sprite overlapping.
    /// </summary>
    /// <param name="tube1"> Tube moved that poors the layer </param>
    /// <param name="tube2"> Tube that receive the color layer </param>
    /// <param name="sortOrder"> sorting value taken as a reference to set the two tubes.
    private void setSortOrder(GameObject tube1, GameObject tube2, int sortOrder)
    {
        tube1.GetComponent<Canvas>().sortingOrder = sortOrder+5; //Tube order
        foreach(Canvas canv in tube1.transform.GetChild(0).GetComponentsInChildren<Canvas>()) //Pooring order
        {
            canv.sortingOrder = sortOrder;
        }
        foreach(Canvas canv in tube1.transform.GetChild(1).GetComponentsInChildren<Canvas>()) //Layer order
        {
            canv.sortingOrder = sortOrder+4;
        }

        tube2.GetComponent<Canvas>().sortingOrder = sortOrder +3;
        foreach(Canvas canv in tube2.transform.GetChild(1).GetComponentsInChildren<Canvas>())
        {
            canv.sortingOrder = sortOrder + 2;
        }
    }


    static public int pooringAction(GameObject tube1, GameObject tube2)
    {
        int pooredLayers = 0;
        bool stillNotMax = tube2.GetComponent<testTube>().colorList.Count < tube2.GetComponent<testTube>().maxLiquid;
        bool notEmpty = tube1.GetComponent<testTube>().colorList.Count != 0;
        int safeGuard = 0;    
        while (areSameColor(tube2, tube1) && stillNotMax && notEmpty)
        {
            if (safeGuard > 100)
            {
                Debug.LogWarning("Safeguard reached while pooring!");
                break;
            }
            safeGuard++;

            //Switch colors
            tube2.GetComponent<testTube>().addColorLayer(tube1.GetComponent<testTube>().colorList.Peek());
            tube1.GetComponent<testTube>().removeColorLayer();
            stillNotMax = tube2.GetComponent<testTube>().colorList.Count < tube2.GetComponent<testTube>().maxLiquid;
            notEmpty = tube1.GetComponent<testTube>().colorList.Count != 0;
            pooredLayers++;
        }  
        return pooredLayers;
    }

    /// <summary>
    /// Method <c>pooringAnimation</c> manage the animation when a tube is porring a layer into another one.
    /// The moved tube is both translated and rotated into position. Then a small tempo is set while pooring layer. Finally, the tube is set back into position.
    /// </summary>
    /// <param name="tube1"> Tube moved that poors the layer </param>
    /// <param name="tube2"> Tube that receive the color layer </param>
    private IEnumerator pooringAnimation(GameObject tube1, GameObject tube2)
    {
        int xDir = 1;
        float rotation = 40f;
        Vector3 initialPosition = tube1.transform.localPosition;
        float initialRotation = tube1.transform.localRotation.eulerAngles.z;
        Color pooredColor = Color.black;

        if(tube1.transform.localPosition.x < tube2.transform.localPosition.x) //pooring tube is at right
        {
            xDir = -1;
            rotation *= -1;
        }
        robotScript.switchEyeColor(tube1.GetComponent<testTube>().colorList.Peek());
        try
        {
            pooredColor = tube1.GetComponent<testTube>().colorList.Peek();
            if(!save.debugDev)
            {
                tube1.GetComponent<Image>().raycastTarget = false;
                tube2.GetComponent<Image>().raycastTarget = false;
            }
        }
        catch(Exception e)
        {
            Debug.Log(e);
        }
    


        //Animate
        if(!save.debugLevel)
        {
            //Sorting order update
            setSortOrder(tube1,tube2, 10);
        
            //Move memory tube to selected tube
            Vector3 newPos = new Vector3(tube2.transform.localPosition.x + xOffset * xDir, tube2.transform.localPosition.y + yOffset, 0f);
            float newRot = rotation;
            yield return StartCoroutine(tube1.GetComponent<testTube>().moveTube(newPos,newRot,translationTime));

            //Add poored liquid
            tube1.transform.GetChild(0).localScale = new Vector3(-xDir,1,1);
            foreach(Image sprite in tube1.transform.GetChild(0).GetComponentsInChildren<Image>())
            {
                sprite.enabled = true;
                sprite.color = pooredColor;
            }
            audioManager.pooringSound();   
        }

        //tube's liquid management
        int layerPoored = pooringAction(tube1, tube2);
        robotPowerScript.updateMove(tube1,tube2,layerPoored);

        if(tube2.GetComponent<testTube>().tubeComplete)
        {
            isNewTubeCompleted = true;
        }
        gameState(actions.finishAction);

        //End animations
        if(!save.debugLevel)
        {
            yield return new WaitForSeconds(pooringTime);

            //Return to initial position
            tube2.GetComponent<Image>().raycastTarget = true;
            foreach(Image sprite in tube1.transform.GetChild(0).GetComponentsInChildren<Image>())
            {
                sprite.enabled = false;
            }
        }

        tube1.GetComponent<testTube>().tubeScaling(false);
        if(!save.debugLevel)
        { 
            yield return StartCoroutine(tube1.GetComponent<testTube>().moveTube(initialPosition,initialRotation,translationTime));
            tube1.GetComponent<Image>().raycastTarget = true;
            setSortOrder(tube1,tube2, 0);
        }
    }

    
    /// <summary>
    /// Method <c>saveRobotColorManagement</c> change the saved value in playerprefs of the robot color for the given level.
    /// If the level was already cleared, the robot color is modified, otherwise, we add a new character to the save variable.
    /// </summary>
    /// <param name="levelNumber"> Level to change the save variable </param>
    /// <param name="colorValue"> New value </param>
    public void saveRobotColorManagement(int levelNumber, int colorValue)
    {
        if(PlayerPrefs.GetString(save.robotColor).Split(' ').Count() <= levelNumber) //If first time this level is completed
        {
            PlayerPrefs.SetString(save.robotColor, PlayerPrefs.GetString(save.robotColor) + " " + colorValue);
        }
        else
        {
            string newSave = "";
            string[] str = PlayerPrefs.GetString(save.robotColor).Split(' ');
            str[levelNumber] = colorValue.ToString();
            foreach(string val in str)
            {
                newSave += val + ' ';
            }
            newSave = newSave.Remove(newSave.Length - 1);
            PlayerPrefs.SetString(save.robotColor, newSave);
        }
    }


    public void updateCompletedTubes()
    {
        completedTube = 0;
        foreach(testTube tubeScript in tubesGroupObject.GetComponentsInChildren<testTube>())
        {
            if(tubeScript.isComplete())
            {
                completedTube++;
            }
        }
    }

    private IEnumerator victory()
    {
        audioManager.GetComponent<audio>().victorySound();
        robotScript.eyesStateMachine(robot.eyesActions.animate, robot.avalaibleAnim.heart);
        victorySprite.enabled = true;

        if(PlayerPrefs.GetInt(save.currentLevel) >= PlayerPrefs.GetInt(save.availableLevels)) //Player unlock a new level!
        {
            int level = PlayerPrefs.GetInt(save.currentLevel) + 1;
            PlayerPrefs.SetInt(save.availableLevels, level);
        }

        int savedColor = 0;
        foreach(Color color in colors)
        {
            if(robotScript.eyeColor == color)
            {
                break;
            }
            savedColor++;
        }
        powerManagerScript.updateOneToken(savedColor, 1);
        saveRobotColorManagement(PlayerPrefs.GetInt(save.currentLevel), savedColor);
        yield return new WaitForSeconds(2f);
        SceneManager.LoadScene("Level Selection");

    }


    private IEnumerator inactivityAction()
    {
        robotCanvas.transform.GetChild(1).gameObject.SetActive(false);
        yield return new WaitForSeconds(7f);
        robotCanvas.transform.GetChild(1).gameObject.SetActive(true);
    }

    private void startInactivity()
    {
        try
        {
            StopCoroutine(inactivityRoutine);
            inactivityRoutine = inactivityAction();
            StartCoroutine(inactivityRoutine);
        }
        catch(Exception e)
        {
            Debug.LogWarning("Warning: Problem related to the inactivity routine" + e);
        }
    }

    //!!! State machine !!! 
    // See diagram for state info

    public void gameState(actions act, GameObject obj = null, string info = "")
    {
        switch(currentState)
        {
            case states.idleNoTube:
                try
                {
                    if(act == actions.clickedTube)
                    {
                        startInactivity();
                        bool notEmpty = obj.GetComponent<testTube>().colorList.Count != 0;
                        if (!obj.GetComponent<testTube>().tubeComplete && notEmpty)
                        {
                            currentState = states.idleTube;
                            //Debug.Log("tube clicked");
                            obj.GetComponent<testTube>().tubeScaling(true);
                            memoryTube = obj;
                        }
                    }
                }
                catch(Exception ex)
                {
                    Debug.LogWarning(ex);
                }

                if(act == actions.clickedRobot)
                {
                    currentState = states.powerSelection;
                    StopCoroutine(inactivityRoutine);
                    robotCanvas.transform.GetChild(1).gameObject.SetActive(false);
                    powerManagerScript.setInteractablePowerButtons();
                    foreach(Button tokenButton in tokenCanvas.GetComponentsInChildren<Button>())
                    {
                        tokenButton.interactable = true;
                    }
                    powerCanvas.GetComponent<Canvas>().enabled = true;
                }
                if(act == actions.clickedBackground)
                {
                    startInactivity();
                }
                break;
            
            case states.idleTube:
                try
                {
                    if(act == actions.clickedTube)
                    {
                        startInactivity();
                        bool stillNotMax = obj.GetComponent<testTube>().colorList.Count < obj.GetComponent<testTube>().maxLiquid;
                        bool notEmpty = memoryTube.GetComponent<testTube>().colorList.Count != 0;
                        if(obj == memoryTube) //Same tube selected
                        {
                            currentState = states.idleNoTube;
                            memoryTube.GetComponent<testTube>().tubeScaling(false);
                            memoryTube = null;
                        }
                        else if (obj.GetComponent<testTube>().tubeComplete) //completed tube selected
                        {
                            currentState = states.idleNoTube;
                            memoryTube.GetComponent<testTube>().tubeScaling(false);
                            memoryTube = null;
                        }
                        else if(!areSameColor(obj, memoryTube)) //New tube selected but different colors
                        {
                            memoryTube.GetComponent<testTube>().tubeScaling(false);
                            obj.GetComponent<testTube>().tubeScaling(true);
                            memoryTube = obj;
                        }
                        else if (areSameColor(memoryTube, obj) && stillNotMax) //New tube is ok to poor additional color
                        {
                            currentState = states.poorColor;
                            StartCoroutine(pooringAnimation(memoryTube, obj));
                            memoryTube = null;
                        }
                        else if (areSameColor(memoryTube, obj) && !stillNotMax) //New tube is too full to be poored
                        {
                            memoryTube.GetComponent<testTube>().tubeScaling(false);
                            obj.GetComponent<testTube>().tubeScaling(true);
                            memoryTube = obj;
                        }
                    }
                    else if(act == actions.clickedRobot)
                    {
                        currentState = states.powerSelection;
                        StopCoroutine(inactivityRoutine);
                        robotCanvas.transform.GetChild(1).gameObject.SetActive(false);
                        memoryTube.GetComponent<testTube>().tubeScaling(false);
                        memoryTube = null;
                        powerManagerScript.setInteractablePowerButtons();
                        foreach(Button tokenButton in tokenCanvas.GetComponentsInChildren<Button>())
                        {
                            tokenButton.interactable = true;
                        }
                        powerCanvas.GetComponent<Canvas>().enabled = true;
                    }
                    else if(act == actions.clickedBackground)
                    {
                        startInactivity();
                        currentState = states.idleNoTube;
                        memoryTube.GetComponent<testTube>().tubeScaling(false);
                        memoryTube = null;
                    }
                }
                catch (Exception ex)
                {
                    startInactivity();
                    currentState = states.idleNoTube;
                    Debug.LogWarning(ex);
                    memoryTube.GetComponent<testTube>().tubeScaling(false);
                    memoryTube = null;
                }
                break;
                               
                
            case states.poorColor:
                if(act == actions.finishAction)
                {
                    startInactivity();
                    if(isNewTubeCompleted)
                    {
                        completedTube++;
                        isNewTubeCompleted = false;
                        if(completedTube >= setupScript.completeTubeToWin) //Victory
                        {
                            currentState = states.endLevel;
                            gameState(actions.noAction);
                        }
                        else //New tube complete
                        {
                            currentState = states.idleNoTube;
                            audioManager.GetComponent<audio>().tubeCompleteSound();
                            if(musicManagerScript.currentLevelMusicSection + 1 < musicManagerScript.levelMusicParts.Count)
                            {
                                musicManagerScript.currentLevelMusicSection++;
                                musicManagerScript.switchAudio(musicManagerScript.levelMusicParts[musicManagerScript.currentLevelMusicSection]);
                            }
                            robotScript.eyesStateMachine(robot.eyesActions.animate, robot.avalaibleAnim.happy);
                        }
                    }
                    else //Nothing new in this world
                    {
                        startInactivity();
                        currentState = states.idleNoTube;
                    }
                }
                break;

            case states.powerSelection:
                if(act == actions.usePower)
                {
                    currentState = states.robotPower;
                    powerCanvas.GetComponent<Canvas>().enabled = false;
                    StopCoroutine(inactivityRoutine);
                    StartCoroutine(powerManagerScript.powerButtonRoutine(info));
                    foreach(Button tokenButton in tokenCanvas.GetComponentsInChildren<Button>())
                    {
                        tokenButton.interactable = false;
                    }
                }

                else if(act == actions.clickedRobot || act == actions.clickedBackground)
                {
                    startInactivity();
                    currentState = states.idleNoTube;                    
                    powerCanvas.GetComponent<Canvas>().enabled = false;
                    foreach(Button tokenButton in tokenCanvas.GetComponentsInChildren<Button>())
                    {
                        tokenButton.interactable = false;
                    }
                }

                try
                {
                    if(act == actions.clickedTube)
                    {
                        powerCanvas.GetComponent<Canvas>().enabled = false;
                        foreach(Button tokenButton in tokenCanvas.GetComponentsInChildren<Button>())
                        {
                            tokenButton.interactable = false;
                        }
                        bool notEmpty = obj.GetComponent<testTube>().colorList.Count != 0;
                        if (!obj.GetComponent<testTube>().tubeComplete && notEmpty)
                        {
                            currentState = states.idleTube;
                            //Debug.Log("tube clicked");
                            obj.GetComponent<testTube>().tubeScaling(true);
                            memoryTube = obj;
                        }
                    }
                }
                catch(Exception ex)
                {
                    startInactivity();
                    currentState = states.idleNoTube;
                    Debug.LogWarning(ex);
                }

                break;

            case states.robotPower:
                if(act == actions.finishAction)
                {
                    startInactivity();
                    currentState = states.idleNoTube;
                    robotScript.eyesStateMachine(robot.eyesActions.endAnimate);
                }
                break;

            case states.endLevel:
                StopCoroutine(inactivityRoutine);
                foreach(Button butt in tubesGroupObject.GetComponentsInChildren<Button>())
                {
                    butt.interactable = false;
                }
                StartCoroutine(victory());
                break;
        }
    }
    
}
