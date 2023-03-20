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


    static public List<List<Color>> levelGenerator(int seed, int numberTube, int numberEmptyTube, int numberInitLayers, int numberMaxLayers, int tubeToWin, int maxLevelColor)
    {
        if(seed == 0)
        {
            seed = (int)DateTime.Now.Ticks;
        }
        UnityEngine.Random.InitState(seed);
            
        List<List<int>> debugList =  new List<List<int>>();
        List<List<Color>> generatedLevel = new List<List<Color>>();
        List<int> availableTubes = new List<int>();
        List<int> tubesUsed = new List<int>();
        List<int> availableColors = new List<int>();
        for(int i = 0; i < numberTube - numberEmptyTube; i++)
        {
            generatedLevel.Add(new List<Color>());
            debugList.Add(new List<int>());
            availableTubes.Add(0);
            tubesUsed.Add(i);
        }
        int totalLayers = numberInitLayers * (numberTube - numberEmptyTube);
        
        //We select the colors used in this new level;
        List<int> colorsUsed = new List<int>();
        int randTube, randTubeMemory, randColor;
        if(maxLevelColor > gameManager.colors.Length)
        {
            throw new System.Exception("The number of colors wanted in this level is higher than the total number of available colors");
        }
        else
        {
            while(colorsUsed.Count < maxLevelColor)
            {
                randColor = UnityEngine.Random.Range(0,gameManager.colors.Length);
                if(!colorsUsed.Contains(randColor))
                {
                    colorsUsed.Add(randColor);
                    availableColors.Add(0);
                }
            }
        }

        //Layer creation
        randTubeMemory = -1; //Use to avoid two same consecutive color
        bool foundLayer = false;
        Color associatedColor;
        for(int lay = 0; lay < totalLayers; lay++)
        {
            foundLayer = false;
            while(!foundLayer)
            {

                randTube = UnityEngine.Random.Range(0,availableTubes.Count); //Finding correct tube
                if(randTube != randTubeMemory || availableTubes.Count <= 1)
                {
                    randColor = UnityEngine.Random.Range(0,availableColors.Count); //Finding correct color

                    //Setting up new color
                    associatedColor = gameManager.colors[colorsUsed[randColor]];
                    generatedLevel[tubesUsed[randTube]].Add(associatedColor);
                    debugList[tubesUsed[randTube]].Add(colorsUsed[randColor]);

                    //Removing completely used colors or tubes
                    availableColors[randColor] += 1;
                    availableTubes[randTube] += 1;
                    /*string s ="";
                    foreach(int a in availableColors)
                    {
                        s+=a + " ";
                    }
                    Debug.Log(s);*/
                    if(availableColors[randColor] >= numberMaxLayers)
                    {
                        availableColors.RemoveAt(randColor);
                        colorsUsed.RemoveAt(randColor);
                    }
                    if(availableTubes[randTube] >= numberInitLayers)
                    {
                        availableTubes.RemoveAt(randTube);
                        tubesUsed.RemoveAt(randTube);
                    }
                    randTubeMemory = randTube;
                    foundLayer = true;
                }
            }
        }

        //Print generated level in debug console
        string str = "";
        int count = 1;
        Debug.Log("Seed: " + seed);
        foreach(List<int> tube in debugList)
        {
            str = "";
            foreach(int col in tube)
            {
                str += " " + col;
            }
            Debug.Log("tube <" + count + ">:" + str);
            count++;
        }
        
        UnityEngine.Random.InitState((int)DateTime.Now.Ticks);
        return generatedLevel;
    }


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
        List<List<Color>> generatedLevel;
        int seed, numberTube, numberEmptyTube, numberInitLayers, numberMaxLayers, tubeToWin, maxLevelColor;
        switch (SceneManager.GetActiveScene().name) 
        {
            case "testLevel":
                setupObject.initLevelParameters(3, 1, 3, 3, 2);
                generatedLevel = levels.levelGenerator(-1635485455,3,1,3,3,2,2);
                return generatedLevel;
                
            case "Level1":
                numberTube = 6;
                numberEmptyTube = 2;
                numberInitLayers = 4;
                numberMaxLayers = 4;
                tubeToWin = 4;
                maxLevelColor = 4;
                seed = 1008674052;

                setupObject.initLevelParameters(numberTube, numberEmptyTube, numberInitLayers, numberMaxLayers, tubeToWin);
                generatedLevel = levels.levelGenerator(seed,numberTube, numberEmptyTube, numberInitLayers, numberMaxLayers, tubeToWin, maxLevelColor);
                return generatedLevel;

            case "Level2":

                setupObject.initLevelParameters(7, 2, 4, 4, 5);
                numberTube = 7;
                numberEmptyTube = 2;
                numberInitLayers = 4;
                numberMaxLayers = 4;
                tubeToWin = 5;
                maxLevelColor = 5;
                seed = 0;

                setupObject.initLevelParameters(numberTube, numberEmptyTube, numberInitLayers, numberMaxLayers, tubeToWin);
                generatedLevel = levels.levelGenerator(seed,numberTube, numberEmptyTube, numberInitLayers, numberMaxLayers, tubeToWin, maxLevelColor);
                return generatedLevel;
                
            case "Level3":
                setupObject.initLevelParameters(8, 2, 4, 4, 6);
                numberTube = 8;
                numberEmptyTube = 2;
                numberInitLayers = 4;
                numberMaxLayers = 4;
                tubeToWin = 6;
                maxLevelColor = 6;
                seed = 0;

                setupObject.initLevelParameters(numberTube, numberEmptyTube, numberInitLayers, numberMaxLayers, tubeToWin);
                generatedLevel = levels.levelGenerator(seed,numberTube, numberEmptyTube, numberInitLayers, numberMaxLayers, tubeToWin, maxLevelColor);
                return generatedLevel;

            case "Level4":
                setupObject.initLevelParameters(9, 2, 4, 4, 7);
                numberTube = 9;
                numberEmptyTube = 2;
                numberInitLayers = 4;
                numberMaxLayers = 4;
                tubeToWin = 7;
                maxLevelColor = 7;
                seed = 0;

                setupObject.initLevelParameters(numberTube, numberEmptyTube, numberInitLayers, numberMaxLayers, tubeToWin);
                generatedLevel = levels.levelGenerator(seed,numberTube, numberEmptyTube, numberInitLayers, numberMaxLayers, tubeToWin, maxLevelColor);
                return generatedLevel;

            case "Level5":
                setupObject.initLevelParameters(10, 2, 4, 4, 8);
                numberTube = 10;
                numberEmptyTube = 2;
                numberInitLayers = 4;
                numberMaxLayers = 4;
                tubeToWin = 8;
                maxLevelColor = 8;
                seed = 36095268;

                setupObject.initLevelParameters(numberTube, numberEmptyTube, numberInitLayers, numberMaxLayers, tubeToWin);
                generatedLevel = levels.levelGenerator(seed,numberTube, numberEmptyTube, numberInitLayers, numberMaxLayers, tubeToWin, maxLevelColor);
                return generatedLevel;
            
            case "Level6":
                setupObject.initLevelParameters(5, 1, 4, 4, 4);
                numberTube = 5;
                numberEmptyTube = 1;
                numberInitLayers = 4;
                numberMaxLayers = 4;
                tubeToWin = 4;
                maxLevelColor = 4;
                seed = 0;

                setupObject.initLevelParameters(numberTube, numberEmptyTube, numberInitLayers, numberMaxLayers, tubeToWin);
                generatedLevel = levels.levelGenerator(seed,numberTube, numberEmptyTube, numberInitLayers, numberMaxLayers, tubeToWin, maxLevelColor);
                return generatedLevel;

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
