using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting.Antlr3.Runtime;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

public class powerManager : MonoBehaviour
{
    public List<int> maxlevelTokenIdxPerPower; //List indicating the level creting the max token strike for each power. The list length is equal to the number of power. 
    private List<List<int>> powersNeededTokens; //List containing the tokens needed for each power to be activated
    private robotPower powerScript;
    private gameManager managerScript;
    private int powerAvailables = 4;
    private List<GameObject> tokensObjects; // List containing the tokens object in the <Token Canvas> GameObject. It is possible to access image (child(0)) and text (child(1))

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
        powerScript = GameObject.Find("Robot").GetComponent<robotPower>();
        managerScript = GameObject.Find("Game Manager").GetComponent<gameManager>();
        
        tokensObjects = new List<GameObject>();
        for(int i = 0; i < GameObject.Find("Token Canvas").transform.childCount - 1; i++)
        {
            tokensObjects.Add(GameObject.Find("Token Canvas").transform.GetChild(i).gameObject);
        }
        initTokenObject(gameManager.colors);

        maxlevelTokenIdxPerPower = new List<int>();
        powersNeededTokens = new List<List<int>>();
        for(int i=0; i<powerAvailables; i++)
        {
            maxlevelTokenIdxPerPower.Add(0);   
            powersNeededTokens.Add(new List<int>());
            setPowerToken(i, 5);
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
    }

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
                Debug.LogWarning("WARNING: Impossible to load color or text for token object " + id);
            }
            Debug.Log(tokensValues[id]);
        }
    }
    
    /// <summary>
    /// Create a random token list from the colors available. 
    /// The tokens are integers representing the color in gameManager "colors" variable.
    /// This method only check the token chain of the given power index
    /// </summary>
    /// <param name="powerIdx"></param>
    /// <param name="necessaryTokens"></param>
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
        for(int i=0; i<powerAvailables; i++)
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

    public bool checkPowerAvailable()
    {
        return false;
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
        for(int i=0; i<powerAvailables; i++)
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
