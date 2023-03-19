using JetBrains.Annotations;
using System.Collections;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;


/// <summary>
/// Class <c>levels</c> manages each level design
/// The design are stored in a color array that has to be manually filled.
/// The setup variables has to be adjusted with the size of the array.
/// </summary>
public class levels
{
    
    static public List<Color> robotColorPerLevel = new List<Color>();


    private void levelGenerator(int seed, int numberTube, int numberEmptyTube, int numberInitLayers, int numberMaxLayers, int tubeToWin, int maxLevelColor)
    {
        UnityEngine.Random.InitState(seed);
        List<List<Color>> generatedLevel = new List<List<Color>>();
        for(int i = 0; i < numberTube - numberEmptyTube; i++)
        {
            generatedLevel.Add(new List<Color>());
        }
        int totalLayers = numberInitLayers * (numberTube - numberEmptyTube);
        
        //We select the colors used in this new level;
        List<int> colorsUsed = new List<int>();
        int rand, randMemory;
        if(maxLevelColor > gameManager.colors.Length)
        {
            throw new System.Exception("The number of colors wanted in this level is higher than the total number of available colors");
        }
        else
        {
            while(colorsUsed.Count < maxLevelColor)
            {
                rand = UnityEngine.Random.Range(0,gameManager.colors.Length);
                if(!colorsUsed.Contains(rand))
                {
                    colorsUsed.Add(rand);
                }
            }
        }

        randMemory = -1;
        for(int lay = 0; lay < totalLayers; lay++)
        {
            rand = UnityEngine.Random.Range(0,gameManager.colors.Length);
            if(rand != randMemory)
            {
                colorsUsed.Add(rand);
            }
            randMemory = rand;
        }

        //Get layers order
        
        UnityEngine.Random.InitState((int)DateTime.Now.Ticks);
    }
    //setupObject.initLevelParameters(3, 1, 3, 3, 2);



    //Levels are done like:
    // Tube1: Color1, color2, Color3
    // Tube2: Color1, color2, Color3

    static public List<List<Color>> testLevel = new List<List<Color>>()
    {
        new List<Color>{ gameManager.colors[2], gameManager.colors[1], gameManager.colors[1] },
        new List<Color>{gameManager.colors[2], gameManager.colors[1],gameManager.colors[2]},
    };

    
    static public List<List<Color>> Level1 = new List<List<Color>>()
    {
        new List<Color>{gameManager.colors[0], gameManager.colors[1], gameManager.colors[3], gameManager.colors[1] },
        new List<Color>{gameManager.colors[1], gameManager.colors[0], gameManager.colors[2], gameManager.colors[3]},
        new List<Color>{gameManager.colors[0], gameManager.colors[1], gameManager.colors[3], gameManager.colors[2]},
        new List<Color>{gameManager.colors[2], gameManager.colors[0], gameManager.colors[2], gameManager.colors[3]},
    };

    static public List<List<Color>> Level2 = new List<List<Color>>()
    {
        new List<Color>{gameManager.colors[0], gameManager.colors[1], gameManager.colors[0], gameManager.colors[2] },
        new List<Color>{gameManager.colors[1], gameManager.colors[2], gameManager.colors[4], gameManager.colors[0]},
        new List<Color>{gameManager.colors[1], gameManager.colors[3], gameManager.colors[2], gameManager.colors[4]},
        new List<Color>{gameManager.colors[0], gameManager.colors[3], gameManager.colors[4], gameManager.colors[3]},
        new List<Color>{gameManager.colors[2], gameManager.colors[1], gameManager.colors[4], gameManager.colors[3]},
    };

    static public List<List<Color>> Level3 = new List<List<Color>>()
    {
        new List<Color>{gameManager.colors[5], gameManager.colors[0], gameManager.colors[2], gameManager.colors[2] },
        new List<Color>{gameManager.colors[4], gameManager.colors[1], gameManager.colors[3], gameManager.colors[1]},
        new List<Color>{gameManager.colors[3], gameManager.colors[2], gameManager.colors[4], gameManager.colors[0]},
        new List<Color>{gameManager.colors[2], gameManager.colors[3], gameManager.colors[5], gameManager.colors[5]},
        new List<Color>{gameManager.colors[1], gameManager.colors[4], gameManager.colors[0], gameManager.colors[4]},
        new List<Color>{gameManager.colors[0], gameManager.colors[5], gameManager.colors[1], gameManager.colors[3]},
    };

    static public List<List<Color>> Level4 = new List<List<Color>>()
    {
        new List<Color>{gameManager.colors[2], gameManager.colors[1], gameManager.colors[4], gameManager.colors[6] },
        new List<Color>{gameManager.colors[3], gameManager.colors[0], gameManager.colors[5], gameManager.colors[1]},
        new List<Color>{gameManager.colors[4], gameManager.colors[2], gameManager.colors[1], gameManager.colors[0]},
        new List<Color>{gameManager.colors[5], gameManager.colors[3], gameManager.colors[6], gameManager.colors[0]},
        new List<Color>{gameManager.colors[2], gameManager.colors[5], gameManager.colors[4], gameManager.colors[5]},
        new List<Color>{gameManager.colors[3], gameManager.colors[1], gameManager.colors[3], gameManager.colors[6]},
        new List<Color>{gameManager.colors[4], gameManager.colors[6], gameManager.colors[2], gameManager.colors[0]},
    };

    
    static public List<List<Color>> Level5 = new List<List<Color>>()
    {
        new List<Color>{gameManager.colors[7], gameManager.colors[0], gameManager.colors[5], gameManager.colors[2]},
        new List<Color>{gameManager.colors[5], gameManager.colors[1], gameManager.colors[4], gameManager.colors[3]},
        new List<Color>{gameManager.colors[3], gameManager.colors[2], gameManager.colors[3], gameManager.colors[5]},
        new List<Color>{gameManager.colors[1], gameManager.colors[1], gameManager.colors[2], gameManager.colors[7]},
        new List<Color>{gameManager.colors[6], gameManager.colors[2], gameManager.colors[0], gameManager.colors[6]},
        new List<Color>{gameManager.colors[4], gameManager.colors[0], gameManager.colors[7], gameManager.colors[4]},
        new List<Color>{gameManager.colors[4], gameManager.colors[1], gameManager.colors[7], gameManager.colors[6]},
        new List<Color>{gameManager.colors[5], gameManager.colors[0], gameManager.colors[6], gameManager.colors[3]},
    };

    static public List<List<Color>> Level6 = new List<List<Color>>()
    {
        new List<Color>{gameManager.colors[0], gameManager.colors[1], gameManager.colors[3], gameManager.colors[1] },
        new List<Color>{gameManager.colors[1], gameManager.colors[0], gameManager.colors[2], gameManager.colors[3]},
        new List<Color>{gameManager.colors[0], gameManager.colors[1], gameManager.colors[3], gameManager.colors[2]},
        new List<Color>{gameManager.colors[2], gameManager.colors[0], gameManager.colors[2], gameManager.colors[3]},
    };


    static public List<List<Color>> getLevelColors()
    {
        setup setupObject = GameObject.Find("Setup").GetComponent<setup>(); 
        //( numberTube,  numberEmptyTube,  numberInitLayers,  numberMaxLayers,  tubeToWin)

        switch (SceneManager.GetActiveScene().name) 
        {
            case "testLevel":
                setupObject.initLevelParameters(3, 1, 3, 3, 2);
                return testLevel;
                
            case "Level1":
                setupObject.initLevelParameters(6, 2, 4, 4, 4);
                return Level1;

            case "Level2":
                setupObject.initLevelParameters(7, 2, 4, 4, 5);
                return Level2;
                
            case "Level3":
                setupObject.initLevelParameters(8, 2, 4, 4, 6);
                return Level3;

            case "Level4":
                setupObject.initLevelParameters(9, 2, 4, 4, 7);
                return Level4;

            case "Level5":
                setupObject.initLevelParameters(10, 2, 4, 4, 8);
                return Level5;
            
            case "Level6":
                setupObject.initLevelParameters(5, 1, 4, 4, 4);
                return Level6;

            /*
            case "Level7":
                return Level7;
                break;
            case "Level8":
                return Level8;
                break;
            case "Level9":
                return Level9;
                break;
            case "Level10":
                return Level10;
                break;
            case "Level11":
                return Level11;
                break;
            case "Level12":
                return Level12;
                break;
            case "Level13":
                return Level13;
                break;
            case "Level14":
                return Level14;
                break;
            case "Level15":
                return Level15;
                break;
            case "Level16":
                return Level16;
                break;
                */
            default: return null;
        }
    }


}
