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
        Debug.Log("Level" + PlayerPrefs.GetInt(save.currentLevel));
        Debug.Log("Seed: " + seed);
        /*
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
        */
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
        switch (PlayerPrefs.GetInt(save.currentLevel)) 
        {
            
                
            case 1:
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

            case 2:
                setupObject.initLevelParameters(7, 2, 4, 4, 5);
                numberTube = 7;
                numberEmptyTube = 2;
                numberInitLayers = 4;
                numberMaxLayers = 4;
                tubeToWin = 5;
                maxLevelColor = 5;
                seed = 2039950124;

                setupObject.initLevelParameters(numberTube, numberEmptyTube, numberInitLayers, numberMaxLayers, tubeToWin);
                generatedLevel = levels.levelGenerator(seed,numberTube, numberEmptyTube, numberInitLayers, numberMaxLayers, tubeToWin, maxLevelColor);
                return generatedLevel;
                
            case 3:
                setupObject.initLevelParameters(8, 2, 4, 4, 6);
                numberTube = 8;
                numberEmptyTube = 2;
                numberInitLayers = 4;
                numberMaxLayers = 4;
                tubeToWin = 6;
                maxLevelColor = 6;
                seed = -612813029;

                setupObject.initLevelParameters(numberTube, numberEmptyTube, numberInitLayers, numberMaxLayers, tubeToWin);
                generatedLevel = levels.levelGenerator(seed,numberTube, numberEmptyTube, numberInitLayers, numberMaxLayers, tubeToWin, maxLevelColor);
                return generatedLevel;

            case 4:
                setupObject.initLevelParameters(9, 2, 4, 4, 7);
                numberTube = 9;
                numberEmptyTube = 2;
                numberInitLayers = 4;
                numberMaxLayers = 4;
                tubeToWin = 7;
                maxLevelColor = 7;
                seed = -1838037443;

                setupObject.initLevelParameters(numberTube, numberEmptyTube, numberInitLayers, numberMaxLayers, tubeToWin);
                generatedLevel = levels.levelGenerator(seed,numberTube, numberEmptyTube, numberInitLayers, numberMaxLayers, tubeToWin, maxLevelColor);
                return generatedLevel;

            case 5:
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
            
            case 6:
                setupObject.initLevelParameters(5, 1, 4, 4, 4);
                numberTube = 5;
                numberEmptyTube = 1;
                numberInitLayers = 4;
                numberMaxLayers = 4;
                tubeToWin = 4;
                maxLevelColor = 4;
                seed = 142450910;

                setupObject.initLevelParameters(numberTube, numberEmptyTube, numberInitLayers, numberMaxLayers, tubeToWin);
                generatedLevel = levels.levelGenerator(seed,numberTube, numberEmptyTube, numberInitLayers, numberMaxLayers, tubeToWin, maxLevelColor);
                return generatedLevel;

            
            case 7:
                setupObject.initLevelParameters(5, 1, 4, 4, 4);
                numberTube = 12;
                numberEmptyTube = 1;
                numberInitLayers = 4;
                numberMaxLayers = 4;
                tubeToWin = 11;
                maxLevelColor = 11;
                seed = -667877602;

                setupObject.initLevelParameters(numberTube, numberEmptyTube, numberInitLayers, numberMaxLayers, tubeToWin);
                generatedLevel = levels.levelGenerator(seed,numberTube, numberEmptyTube, numberInitLayers, numberMaxLayers, tubeToWin, maxLevelColor);
                return generatedLevel;
                
            case 8:
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

            case 9:
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

            case 10:
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

            case 11:
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

            case 12:
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

            case 13:
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

            case 14:
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

            case 15:
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

            case 16:
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

            case 17:
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

            case 18:
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

            case 19:
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

            case 20:
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

            case 21:
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

            case 22:
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

            case 23:
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

            case 24:
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

            case 25:
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

            case 26:
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

            case 27:
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

            case 28:
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

            case 29:
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

            case 30:
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

            case 31:
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

            case 32:
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

            case 33:
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

            case 34:
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

            case 35:
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

            case 36:
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

            case 37:
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

            case 38:
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

            case 39:
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

            case 40:
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

            case 41:
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

            case 42:
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

            case 43:
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

            case 44:
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

            case 45:
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

            case 46:
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

            case 47:
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

            case 48:
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

            case 49:
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

            case 50:
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

            case 51:
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

            case 52:
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

            case 53:
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

            case 54:
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

            case 55:
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

            case 56:
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

            case 57:
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

            case 58:
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

            case 59:
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

            case 60:
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

            case 61:
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

            case 62:
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

            case 63:
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

            case 64:
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

            case 65:
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

            case 66:
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

            case 67:
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

            case 68:
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

            case 69:
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

            case 70:
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

            case 71:
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

            case 72:
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

            case 73:
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

            case 74:
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

            case 75:
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

            case 76:
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

            case 77:
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

            case 78:
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

            case 79:
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

            case 80:
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

            case 81:
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

            case 82:
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

            case 83:
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

            case 84:
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

            case 85:
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

            case 86:
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

            case 87:
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

            case 88:
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

            case 89:
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

            case 90:
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

            case 91:
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

            case 92:
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

            case 93:
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

            case 94:
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

            case 95:
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

            case 96:
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

            case 97:
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

            case 98:
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

            case 99:
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

            case 100:
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

            case 101:
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

            case 102:
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

            case 103:
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

            case 104:
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

            case 105:
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

            case 106:
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

            case 107:
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

            case 108:
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

            case 109:
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

            case 110:
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

            case 111:
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

            case 112:
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

            case 113:
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

            case 114:
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

            case 115:
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

            case 116:
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

            case 117:
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

            case 118:
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

            case 119:
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

            case 120:
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

            case 121:
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

            case 122:
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

            case 123:
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

            case 124:
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

            case 125:
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

            case 126:
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

            case 127:
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

            case 128:
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

            case 129:
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

            case 130:
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

            case 131:
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

            case 132:
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

            case 133:
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

            case 134:
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

            case 135:
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

            case 136:
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

            case 137:
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

            case 138:
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

            case 139:
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

            case 140:
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

            case 141:
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

            case 142:
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

            case 143:
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

            case 144:
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

            case 145:
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

            case 146:
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

            case 147:
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

            case 148:
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

            case 149:
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

            case 150:
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

            default: 
                setupObject.initLevelParameters(3, 1, 3, 3, 2);
                generatedLevel = levels.levelGenerator(-1635485455,3,1,3,3,2,2);
                return generatedLevel;
                    }

                }

}
