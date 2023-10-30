using System;
using System.Collections;
using System.Collections.Generic;
using System.Numerics;
using UnityEngine;
using UnityEngine.UI;

public class powerManager : MonoBehaviour
{
    public List<int> maxlevelTokenIdxPerPower; //List indicating the level creating the max token strike for each power. The list length is equal to the number of power. 
    private List<int> powersNeededTokens; //List containing the numbe roftokens needed for each power to be activated
    private robotPower powerScript;
    private gameManager managerScript;
    private List<GameObject> tokensObjects; // List containing the tokens object in the <Token Canvas> GameObject. It is possible to access image (child(0)) and text (child(1))

    private GameObject powerButtonsCanvas;
    private GameObject powerResultsCanvas;
    private audio audioScript;
    private robot robotScript;

    private bool deleteColorUsed;  

    [SerializeField] private bool debug_check;
    [SerializeField] private GameObject tubePrefab;


    private void Awake()
    {
        managerScript = GameObject.Find("Game Manager").GetComponent<gameManager>();
        powerButtonsCanvas = GameObject.Find("Power Buttons");
        powerResultsCanvas = GameObject.Find("Results");
        robotScript = GameObject.Find("Robot").GetComponent<robot>();
        audioScript = GameObject.Find("Audio Manager").GetComponent<audio>();
    }

    void Start()
    {
        if(!PlayerPrefs.HasKey(save.powerToken))
        {
            string tokens_values = "";
            for(int i=0; i<gameManager.colors.Count; i++)
            {
                tokens_values += "0 ";
            }
            tokens_values = tokens_values.Remove(tokens_values.Length - 1);
            PlayerPrefs.SetString(save.powerToken, tokens_values);
        }

        debug_check = false;
        deleteColorUsed = false;
        powerScript = this.GetComponent<robotPower>();
        powerResultsCanvas.SetActive(false);
    
        tokensObjects = new List<GameObject>();
        for(int i = 0; i < GameObject.Find("Token Canvas").transform.childCount; i++)
        {
            tokensObjects.Add(GameObject.Find("Token Canvas").transform.GetChild(i).gameObject);
        }
        initTokenObject(gameManager.colors);

        powersNeededTokens = new List<int>{
            1, //rollBack
            4, //nextMove
            2, //isWind
            7 //deleteColor
        };
        
        maxlevelTokenIdxPerPower = new List<int>();
        for(int i=0; i<powersNeededTokens.Count; i++)
        {
            maxlevelTokenIdxPerPower.Add(0);   
        }
    }

    /// <summary>
    /// Shuffles the elements of a list using the Fisher-Yates shuffle algorithm.
    /// </summary>
    /// <param name="list">The list to be shuffled.</param>
    private void shuffle(List<int> list)
    {
        System.Random rng = new System.Random();
        int n = list.Count;
        
        for (int i = n - 1; i > 0; i--)
        {
            int j = rng.Next(i + 1);
            int temp = list[i];
            list[i] = list[j];
            list[j] = temp;
        }
    }

    /// <summary>
    /// powerButton is called by the button, that call the state machine, that then call the coroutine 
    /// It performs the power action when a specific button is pressed. The power selected is laucnhed by calling the corresponding method
    /// </summary>
    /// <param name="selection">Power linked to the pressed button </param>
    public void powerButton(string selection)
    {
        managerScript.gameState(gameManager.actions.usePower,null,selection);
    }

    public IEnumerator powerButtonRoutine(string selection)
    {   
        //We first set the tokens that will be consumed by using the power
        List<int> tokenUseOrder = new List<int>();
        List<int> possessedTokens = loadTokens();
        for(int i=0; i<possessedTokens.Count; i++)
        {
            if(possessedTokens[i] > 0)
            {
                tokenUseOrder.Add(i);
            }
        }
        shuffle(tokenUseOrder);

        if(selection == "rollBack")
        {
            robotScript.eyesStateMachine(robot.eyesActions.animate,robot.avalaibleAnim.solving);
            if(!save.debugPower)
            {
                for(int i=0; i<powersNeededTokens[0]; i++)
                {
                    updateOneToken(tokenUseOrder[i],-1);
                }
            }
            yield return new WaitForSeconds(0.5f);
            audioScript.powerOK();
            powerScript.rollBackOne();
        }
        
            
        else if(selection == "nextMove")
        {
            if(!save.debugPower)
            {
                for(int i=0; i<powersNeededTokens[1]; i++)
                {
                    updateOneToken(tokenUseOrder[i],-1);
                }
            }
            robotScript.eyesStateMachine(robot.eyesActions.animate,robot.avalaibleAnim.solving);
            yield return StartCoroutine(powerScript.findNextMove());
            robotScript.eyesStateMachine(robot.eyesActions.endAnimate);
            if(powerScript.isStateWinnable)
            {
                
                int canvasOrder = powerResultsCanvas.GetComponent<Canvas>().sortingOrder;

                robotScript.eyesStateMachine(robot.eyesActions.animate,robot.avalaibleAnim.goodSolution);
                audioScript.powerOK();

                //Displaying the two tubes found for the next move 
                GameObject[] nextTubes = {powerScript.nextPooringTube, powerScript.nextPooredTube};
                nextTubes[0].GetComponent<testTube>().tubeScaling(true);
                yield return new WaitForSeconds(0.3f);
                nextTubes[1].GetComponent<testTube>().tubeScaling(true);
                yield return new WaitForSeconds(1f);
                nextTubes[0].GetComponent<testTube>().tubeScaling(false);
                yield return new WaitForSeconds(0.3f);
                nextTubes[1].GetComponent<testTube>().tubeScaling(false);
            }
            else
            {
                robotScript.eyesStateMachine(robot.eyesActions.animate,robot.avalaibleAnim.badSolution);
                audioScript.powerNOK();
                powerResultsCanvas.SetActive(true);
                powerResultsCanvas.transform.GetChild(4).gameObject.SetActive(true);
                yield return new WaitForSeconds(1.5f);
                powerResultsCanvas.SetActive(false);
                powerResultsCanvas.transform.GetChild(4).gameObject.SetActive(false);
            }
        }
        
        else if(selection == "isWin")
        {
            if(!save.debugPower)
            {
                for(int i=0; i<powersNeededTokens[2]; i++)
                {
                    updateOneToken(tokenUseOrder[i],-1);
                }
            }
            robotScript.eyesStateMachine(robot.eyesActions.animate,robot.avalaibleAnim.solving);
            yield return StartCoroutine(powerScript.isWinnable());
            robotScript.eyesStateMachine(robot.eyesActions.endAnimate);
            if(powerScript.isStateWinnable)
            {
                robotScript.eyesStateMachine(robot.eyesActions.animate,robot.avalaibleAnim.goodSolution);
                audioScript.powerOK();
                powerResultsCanvas.SetActive(true);
                powerResultsCanvas.transform.GetChild(3).gameObject.SetActive(true);
                yield return new WaitForSeconds(1.5f);
                powerResultsCanvas.SetActive(false);
                powerResultsCanvas.transform.GetChild(3).gameObject.SetActive(false);
            }
            else
            {
                robotScript.eyesStateMachine(robot.eyesActions.animate,robot.avalaibleAnim.badSolution);
                audioScript.powerNOK();
                powerResultsCanvas.SetActive(true);
                powerResultsCanvas.transform.GetChild(4).gameObject.SetActive(true);
                yield return new WaitForSeconds(1.5f);
                powerResultsCanvas.SetActive(false);
                powerResultsCanvas.transform.GetChild(4).gameObject.SetActive(false);
            }
        }

        else if (selection == "deleteColor")
        {
            if(!save.debugPower)
            {
                for(int i=0; i<powersNeededTokens[3]; i++)
                {
                    updateOneToken(tokenUseOrder[i],-1);
                }
            }
            robotScript.eyesStateMachine(robot.eyesActions.animate,robot.avalaibleAnim.solving);
            yield return new WaitForSeconds(0.5f);
            audioScript.powerOK();
            powerScript.deleteColor();
            deleteColorUsed = true;
        }
        
        managerScript.gameState(gameManager.actions.finishAction);
    }

    
    /// <summary>
    /// checkPowerAvailable verify if a power can be used by the player at the moment.
    /// This method compare the needed token for the wanted power with the tokens possessed by the player
    /// </summary>
    /// <param name="powerID"> Power id to be checked (3 to check power identified by number 3 </param>
    /// <returns>Return True if power can be used, false otherwise</returns>
    private bool checkPowerAvailable(int powerID)
    {
        List<int> availableTokens = loadTokens(); 
        int numberOfUniqueTokens = 0;

        if(powerID == 3 && deleteColorUsed) //Player can only used the delete color power once in a game.
        {
            return false;
        }
        
        foreach(int token in availableTokens)
        {
            if(token > 0)
            {
                numberOfUniqueTokens++;
            }
        }

        if(numberOfUniqueTokens >= powersNeededTokens[powerID])
        {
            return true;
        }
        else
        {
            return false;
        }
    }

    /// <summary>
    /// setInteractablePowerButtons activate or not each power buttons in the Power Canvas GameObject
    /// To do so, it verifies the availability of each powers in the game by using the "checkPowerAvailable" method.
    /// This method is mainly used when activating the power buttons canvas, so that only available powers can be selected by the player.
    /// </summary>
    public void setInteractablePowerButtons()
    {
        if(!save.debugPower)
        {
            for(int childID=2; childID < powerButtonsCanvas.transform.childCount; childID++)
            {
                int powerID = childID-2; //We remove first iteration to consider the background object as it is child(0) of the power canvas, and second for txt panel.
                if(checkPowerAvailable(powerID))
                {
                    powerButtonsCanvas.transform.GetChild(childID).GetComponent<Button>().interactable = true;
                }
                else
                {
                    powerButtonsCanvas.transform.GetChild(childID).GetComponent<Button>().interactable = false;
                }
            }
        }

    }

    public void updateOneToken(int tokenID, int increment)
    {
        string tokenSave = "";
        List<int> tokensValues = loadTokens();
        tokensValues[tokenID] += increment;
        foreach(int token in tokensValues)
        {
            tokenSave += token.ToString() + " ";
        }
        tokenSave = tokenSave.Remove(tokenSave.Length - 1);

        PlayerPrefs.SetString(save.powerToken, tokenSave);

        initTokenObject(gameManager.colors);
    }

    /// <summary>
    /// loadTokens returns the token possessed by the player in the form of a list.
    /// The PlayerPrefs of the game are checked to recover the current states of each token.
    /// The PlayerPrefs are parsed to be given in form of a list. Each indices represent the token available for a specific color of the game
    /// </summary>
    /// <returns> List of int representing the available tokens for each colors </returns>
    private List<int> loadTokens()
    {
        List<int> tokensValues = new List<int>();
        string[] savedTokens = PlayerPrefs.GetString(save.powerToken).Split(' ');
        foreach(string tok in savedTokens)
        {
            tokensValues.Add(int.Parse(tok));
        }
        return tokensValues;

    }

    private void initTokenObject(List<Color> colors)
    {
        List<int> tokensValues = loadTokens();
        for(int id=0; id<tokensObjects.Count; id++)
        {
            try
            {
                tokensObjects[id].transform.GetChild(0).GetComponent<Image>().color = colors[id];
                tokensObjects[id].transform.GetChild(1).GetComponent<Text>().text = tokensValues[id].ToString();
            }
            catch(Exception e)
            {
                Debug.LogWarning("WARNING: Impossible to load color or text for token object " + id + "\n" + e);
            }
        }
    }
    
    private void debug()
    {
        string strColor = "";
        foreach(string colorVal in PlayerPrefs.GetString(save.robotColor).Split(' '))
        {
            strColor += " " + colorVal;
        }
        Debug.Log(strColor);

        Debug.Log("Power tokens:");
        for(int i=0; i<powersNeededTokens.Count; i++)
        {
            string str1 = ""; 
            foreach(int tokenNeeded in powersNeededTokens)
            {
                str1 += " " + tokenNeeded;
            }
            Debug.Log(str1);
        }
    }

    // Update is called once per frame
    void Update()
    {
        if(debug_check)
        {
            debug_check = false;
            debug();
        }

    }
}
