using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class powerManager : MonoBehaviour
{
    public List<int> maxlevelTokenIdxPerPower; //List indicating the level creating the max token strike for each power. The list length is equal to the number of power. 
    private List<List<int>> powersNeededTokens; //List containing the tokens needed for each power to be activated
    private robotPower powerScript;
    private gameManager managerScript;
    private List<GameObject> tokensObjects; // List containing the tokens object in the <Token Canvas> GameObject. It is possible to access image (child(0)) and text (child(1))

    private GameObject powerButtonsCanvas;

    private bool deleteColorUsed;  

    [SerializeField] private bool debug_check;


    // Start is called before the first frame update
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
        managerScript = GameObject.Find("Game Manager").GetComponent<gameManager>();
        powerButtonsCanvas = GameObject.Find("Power Buttons");
    
        tokensObjects = new List<GameObject>();
        for(int i = 0; i < GameObject.Find("Token Canvas").transform.childCount; i++)
        {
            tokensObjects.Add(GameObject.Find("Token Canvas").transform.GetChild(i).gameObject);
        }
        initTokenObject(gameManager.colors);

        maxlevelTokenIdxPerPower = new List<int>();
        powersNeededTokens = new List<List<int>>();
        
        powersNeededTokens = new List<List<int>>{
            new List<int>{0}, //rollBack
            new List<int>{1}, //nextMove
            new List<int>{0}, //isWind
            new List<int>{0} //deleteColor
        };
        for(int i=0; i<powersNeededTokens.Count; i++)
        {
            maxlevelTokenIdxPerPower.Add(0);   
            
            //powersNeededTokens.Add(new List<int>());
            //setPowerToken(i, 5);
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
        if(selection == "rollBack")
        {
            foreach(int token in powersNeededTokens[0])
            {
                updateOneToken(token,-1);
            }
            powerScript.rollBackOne();
        }
        
            
        else if(selection == "nextMove")
        {
            foreach(int token in powersNeededTokens[1])
            {
                updateOneToken(token,-1);
            }
            yield return StartCoroutine(powerScript.findNextMove());
            if(powerScript.isStateWinnable)
            {
                GameObject pooredTube = powerScript.nextPooredTube;
                GameObject pooringTube = powerScript.nextPooringTube;
            }
            else
            {
                
            }
        }
        
        else if(selection == "isWin")
        {
            foreach(int token in powersNeededTokens[2])
            {
                updateOneToken(token,-1);
            }
            yield return StartCoroutine(powerScript.isWinnable());
            if(powerScript.isStateWinnable)
            {
                
            }
            else
            {

            }
        }

        else if (selection == "deleteColor")
        {
            foreach(int token in powersNeededTokens[3])
            {
                updateOneToken(token,-1);
            }
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

        if(powerID == 3 && deleteColorUsed) //Player can only used the delete color power once in a game.
        {
            return false;
        }
        
        foreach(int neededToken in powersNeededTokens[powerID])
        {
            if(availableTokens[neededToken] <= 0) //No tokens left available -> power can't be used
            {
                return false;
            }
            else
            {
                availableTokens[neededToken] --;
            }
        }
        return true;
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
    
    /// <summary>
    /// Create a random token list from the colors available. 
    /// The tokens are integers representing the color in gameManager "colors" variable.
    /// For now, the token chain is created at random, meaning a random list of integer is creating of length given by necessary token parameter
    /// </summary>
    /// <param name="powerIdx"></param>
    /// <param name="necessaryTokens">Number of tokens needed for that power</param>
    private void setPowerToken(int powerIdx, int necessaryTokens)
    {
        powersNeededTokens[powerIdx].Clear();
        for(int i=0; i<necessaryTokens; i++)
        {
            powersNeededTokens[powerIdx].Add(UnityEngine.Random.Range(0, gameManager.colors.Count));
        }
    }

    /// <summary>
    /// Check the state of the token chain of the desired power.
    /// This function uses the information gathered when calling "loadTokenChains()" method.
    /// The info given is the first leven of the token chain, therefore this function checks the following levels' finishing color in regards to the power needed tokens.
    /// It return the number consecutive tokens unlocked in the chain.
    /// </summary>
    /// <param name="powerIdx"></param>
    public int powerTokenState(int powerIdx)
    {
        string[] levels = PlayerPrefs.GetString(save.robotColor).Split(' ');
        int startIdx = maxlevelTokenIdxPerPower[powerIdx];
        int endIdx = Mathf.Max(startIdx + powersNeededTokens[powerIdx].Count, levels.Length);
        int strike = 0;
        for(int i = startIdx; i<endIdx; i++)
        {
            if(int.Parse(levels[i]) != powersNeededTokens[powerIdx][strike])
            {
                break;
            }
            strike++;
        }
        return strike;
    }

    /// <summary>
    /// Verify the current states of power tokens in regards to the levels finished by the player.
    /// To do so, we iterate through each level and compare the finishing color with the tokens chain needed for each power.
    /// If a power's token chain is even partially started, the level starting the chain is saved in global variable "maxlevelTokenIdxPerPower"
    /// </summary>
    public void loadTokenChains()
    {
        List<int> tmpTokenStrike = new List<int>();
        List<int> maxTokenStrikePerPower = new List<int>();
        for(int i=0; i<powersNeededTokens.Count; i++)
        {
            maxTokenStrikePerPower.Add(0);   
            tmpTokenStrike.Add(0);
        }
        int levelIdx = 0;
        foreach(string colorVal in PlayerPrefs.GetString(save.robotColor).Split(' '))
        {
            levelIdx++;
            int currentPower = 0;
            foreach(List<int> neededToken in powersNeededTokens)
            {
                if(tmpTokenStrike[currentPower] >= 0) //If negative number, it means we already found a token chain. And I'm too lazy to code a way to remove it from the list RN.
                {
                    if(neededToken[tmpTokenStrike[currentPower]] == int.Parse(colorVal)) //The token of the level contained in the actual colorVal is matching with the token needed for the current power considered
                    {
                        tmpTokenStrike[currentPower]++;
                        if(tmpTokenStrike[currentPower] >= neededToken.Count)
                        {
                            maxTokenStrikePerPower[currentPower] = -1;
                            maxlevelTokenIdxPerPower[currentPower] = levelIdx - neededToken.Count; 
                        }
                    }
                    else
                    {
                        if(tmpTokenStrike[currentPower] > maxTokenStrikePerPower[currentPower])
                        {
                            maxTokenStrikePerPower[currentPower] = tmpTokenStrike[currentPower];
                            maxlevelTokenIdxPerPower[currentPower] = levelIdx - tmpTokenStrike[currentPower] - 1; 
                        }
                    }
                }
                currentPower++; 
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
            foreach(int token in powersNeededTokens[i])
            {
                str1 += " " + token;
            }
            Debug.Log(str1);
        }

        loadTokenChains();
        string str2 = "";
        foreach(int token in maxlevelTokenIdxPerPower)
        {
            str2 += " " + token;
        }
        Debug.Log("level starting power:" + str2);

        Debug.Log(powerTokenState(0));
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
