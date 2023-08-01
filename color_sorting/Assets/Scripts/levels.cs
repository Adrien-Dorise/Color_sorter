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

    /// <summary>
    /// LevelGenerator procedurally created a new level based on specificied parameters. 
    /// When laoding a level, the generator is setup to create the required tubes with color layer.
    /// When creating a new level, specify the wanted paramters, and reload the level until desired setup is found.
    /// When done, freeze the current seed to recover the exact same setup.
    /// </summary>
    /// <param name="seed"> Seed selected to create the level. Set seed=0 for random generation </param>
    /// <param name="numberTube"> Number of total tubes desired for the level (totalTube = colorTube + emptyTube) </param>
    /// <param name="numberEmptyTube"> Number of empty tupe (aka bonus tube) desired for the level </param>
    /// <param name="numberInitLayers"> Number of color layers inside each tubes when strating the level </param>
    /// <param name="numberMaxLayers"> Maximum number of layers required to fill a tube </param>
    /// <param name="tubeToWin"> Number of sorted tubes required to win the level </param>
    /// <param name="maxLevelColor"> Maximum number of different colors in the level </param>
    /// <param name="generatorVersion"> Generator version used to create the level 
    /// v1: Fully randomize generation
    /// v2: Occasionally reuse same tube to poor same color, creating easier to setup levels. The probability to reuse a tube is set by "rewindPercentage"
    /// v3: Authorises the use if the same color multiple time in the level </param>
    /// <param name="rewindPercentage">When the v2 generator is selected with "generatorVersion", defines the probability to reuse a tube with same color + version 1</param>
    /// <returns></returns>
    /// <exception cref="System.Exception"></exception>
    static public List<List<Color>> levelGenerator(int seed, int numberTube, int numberEmptyTube, int numberInitLayers, int numberMaxLayers, int tubeToWin, int maxLevelColor, int generatorVersion, float rewindPercentage=0.4f)
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
        if(maxLevelColor > gameManager.colors.Count)
        {
            throw new System.Exception("The number of colors wanted in this level is higher than the total number of available colors");
        }
        else
        {
            while(colorsUsed.Count < maxLevelColor)
            {
                randColor = UnityEngine.Random.Range(0,gameManager.colors.Count);
                if(!colorsUsed.Contains(randColor) || generatorVersion == 3)
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

        switch(generatorVersion)
        {
            case 1:
            case 3:
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
                            if(availableTubes[randTube] >= numberInitLayers)
                            {
                                availableTubes.RemoveAt(randTube);
                                tubesUsed.RemoveAt(randTube);
                            }
                            if(availableColors[randColor] >= numberMaxLayers)
                            {
                                availableColors.RemoveAt(randColor);
                                colorsUsed.RemoveAt(randColor);
                            }
                            randTubeMemory = randTube;
                            foundLayer = true;
                        }
                    }
                }
                break;

            case 2: //We occasionally repoor the same color in the same tube
                int memoryTube = 0, memoryColor = 0;
                float reuseTube;
                for(int lay = 0; lay < totalLayers; lay++)
                {
                    foundLayer = false;
                    while(!foundLayer)
                    {
                        randTube = UnityEngine.Random.Range(0,availableTubes.Count); //Finding correct tube
                        randColor = UnityEngine.Random.Range(0,availableColors.Count); //Finding correct color
                        reuseTube =  UnityEngine.Random.Range(0f,1f); //Set a number to reuse the tube. 
                        if(reuseTube < rewindPercentage && (memoryTube < availableTubes.Count && memoryColor < availableColors.Count)) //Change the reuse tube to modify rewind probability (< 0.2f means 20% chances of rewind)
                        {
                                randTube = memoryTube;
                                randColor = memoryColor;
                                //Debug.Log("Rewind");  
                        }
                        memoryTube = randTube;
                        memoryColor = randColor;


                        if(true)
                        {

                            //Setting up new color
                            associatedColor = gameManager.colors[colorsUsed[randColor]];
                            generatedLevel[tubesUsed[randTube]].Add(associatedColor);
                            debugList[tubesUsed[randTube]].Add(colorsUsed[randColor]);

                            //Removing completely used colors or tubes
                            availableColors[randColor] += 1;
                            availableTubes[randTube] += 1;

                            if(availableTubes[randTube] >= numberInitLayers)
                            {
                                availableTubes.RemoveAt(randTube);
                                tubesUsed.RemoveAt(randTube);
                            }
                            if(availableColors[randColor] >= numberMaxLayers)
                            {
                                availableColors.RemoveAt(randColor);
                                colorsUsed.RemoveAt(randColor);
                            }
                        }
                        foundLayer = true;
                    }
                }
                break;

            }

        //Print generated level in debug console
        Debug.Log("Level" + PlayerPrefs.GetInt(save.currentLevel));
        Debug.Log("Seed: " + seed);
        /*
        string str = "";
        int count = 1;
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
    static public List<List<Color>> Level1 = new List<List<Color>>()
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
        int seed, generatorVersion, numberTube, numberEmptyTube, numberInitLayers, numberMaxLayers, tubeToWin, maxLevelColor;
        float rewindPercentage;
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
                generatorVersion = 1;

                setupObject.initLevelParameters(numberTube, numberEmptyTube, numberInitLayers, numberMaxLayers, tubeToWin);
                generatedLevel = levels.levelGenerator(seed,numberTube, numberEmptyTube, numberInitLayers, numberMaxLayers, tubeToWin, maxLevelColor, generatorVersion);
                return generatedLevel;

            case 2:
                numberTube = 7;
                numberEmptyTube = 2;
                numberInitLayers = 4;
                numberMaxLayers = 4;
                tubeToWin = 5;
                maxLevelColor = 5;
                seed = 2039950124;
                generatorVersion = 1;

                setupObject.initLevelParameters(numberTube, numberEmptyTube, numberInitLayers, numberMaxLayers, tubeToWin);
                generatedLevel = levels.levelGenerator(seed,numberTube, numberEmptyTube, numberInitLayers, numberMaxLayers, tubeToWin, maxLevelColor, generatorVersion);
                return generatedLevel;
                
            case 3:
                numberTube = 8;
                numberEmptyTube = 2;
                numberInitLayers = 4;
                numberMaxLayers = 4;
                tubeToWin = 6;
                maxLevelColor = 6;
                seed = -612813029;
                generatorVersion = 1;

                setupObject.initLevelParameters(numberTube, numberEmptyTube, numberInitLayers, numberMaxLayers, tubeToWin);
                generatedLevel = levels.levelGenerator(seed,numberTube, numberEmptyTube, numberInitLayers, numberMaxLayers, tubeToWin, maxLevelColor, generatorVersion);
                return generatedLevel;

            case 4:
                numberTube = 9;
                numberEmptyTube = 2;
                numberInitLayers = 4;
                numberMaxLayers = 4;
                tubeToWin = 7;
                maxLevelColor = 7;
                seed = -1838037443;
                generatorVersion = 1;

                setupObject.initLevelParameters(numberTube, numberEmptyTube, numberInitLayers, numberMaxLayers, tubeToWin);
                generatedLevel = levels.levelGenerator(seed,numberTube, numberEmptyTube, numberInitLayers, numberMaxLayers, tubeToWin, maxLevelColor, generatorVersion);
                return generatedLevel;

            case 5:
                numberTube = 10;
                numberEmptyTube = 2;
                numberInitLayers = 4;
                numberMaxLayers = 4;
                tubeToWin = 8;
                maxLevelColor = 8;
                seed = 36095268;
                generatorVersion = 1;

                setupObject.initLevelParameters(numberTube, numberEmptyTube, numberInitLayers, numberMaxLayers, tubeToWin);
                generatedLevel = levels.levelGenerator(seed,numberTube, numberEmptyTube, numberInitLayers, numberMaxLayers, tubeToWin, maxLevelColor, generatorVersion);
                return generatedLevel;
            
            case 6:
                numberTube = 11;
                numberEmptyTube = 2;
                numberInitLayers = 4;
                numberMaxLayers = 4;
                tubeToWin = 9;
                maxLevelColor = 9;
                seed = -1614514401;
                generatorVersion = 1;

                setupObject.initLevelParameters(numberTube, numberEmptyTube, numberInitLayers, numberMaxLayers, tubeToWin);
                generatedLevel = levels.levelGenerator(seed,numberTube, numberEmptyTube, numberInitLayers, numberMaxLayers, tubeToWin, maxLevelColor, generatorVersion);
                return generatedLevel;
               
            case 7:
                numberTube = 12;
                numberEmptyTube = 2;
                numberInitLayers = 4;
                numberMaxLayers = 4;
                tubeToWin = 10;
                maxLevelColor = 10;
                seed = -1021136869;
                generatorVersion = 1;

                setupObject.initLevelParameters(numberTube, numberEmptyTube, numberInitLayers, numberMaxLayers, tubeToWin);
                generatedLevel = levels.levelGenerator(seed,numberTube, numberEmptyTube, numberInitLayers, numberMaxLayers, tubeToWin, maxLevelColor, generatorVersion);
                return generatedLevel;

            case 8:
                numberTube = 6;
                numberEmptyTube = 2;
                numberInitLayers = 5;
                numberMaxLayers = 5;
                tubeToWin = 4;
                maxLevelColor = 4;
                seed = 1;
                generatorVersion = 1;

                setupObject.initLevelParameters(numberTube, numberEmptyTube, numberInitLayers, numberMaxLayers, tubeToWin);
                generatedLevel = levels.levelGenerator(seed,numberTube, numberEmptyTube, numberInitLayers, numberMaxLayers, tubeToWin, maxLevelColor, generatorVersion);
                return generatedLevel;

            case 9:
                numberTube = 7;
                numberEmptyTube = 2;
                numberInitLayers = 5;
                numberMaxLayers = 5;
                tubeToWin = 5;
                maxLevelColor = 5;
                seed = 1;
                generatorVersion = 1;

                setupObject.initLevelParameters(numberTube, numberEmptyTube, numberInitLayers, numberMaxLayers, tubeToWin);
                generatedLevel = levels.levelGenerator(seed,numberTube, numberEmptyTube, numberInitLayers, numberMaxLayers, tubeToWin, maxLevelColor, generatorVersion);
                return generatedLevel;

            case 10:
                numberTube = 8;
                numberEmptyTube = 2;
                numberInitLayers = 5;
                numberMaxLayers = 5;
                tubeToWin = 6;
                maxLevelColor = 6;
                seed = 1;
                generatorVersion = 1;

                setupObject.initLevelParameters(numberTube, numberEmptyTube, numberInitLayers, numberMaxLayers, tubeToWin);
                generatedLevel = levels.levelGenerator(seed,numberTube, numberEmptyTube, numberInitLayers, numberMaxLayers, tubeToWin, maxLevelColor, generatorVersion);
                return generatedLevel;

            case 11:
                numberTube = 9;
                numberEmptyTube = 2;
                numberInitLayers = 5;
                numberMaxLayers = 5;
                tubeToWin = 7;
                maxLevelColor = 7;
                seed = 1;
                generatorVersion = 1;

                setupObject.initLevelParameters(numberTube, numberEmptyTube, numberInitLayers, numberMaxLayers, tubeToWin);
                generatedLevel = levels.levelGenerator(seed,numberTube, numberEmptyTube, numberInitLayers, numberMaxLayers, tubeToWin, maxLevelColor, generatorVersion);
                return generatedLevel;

            case 12:
                numberTube = 10;
                numberEmptyTube = 2;
                numberInitLayers = 5;
                numberMaxLayers = 5;
                tubeToWin = 8;
                maxLevelColor = 8;
                seed = 1;
                generatorVersion = 1;

                setupObject.initLevelParameters(numberTube, numberEmptyTube, numberInitLayers, numberMaxLayers, tubeToWin);
                generatedLevel = levels.levelGenerator(seed,numberTube, numberEmptyTube, numberInitLayers, numberMaxLayers, tubeToWin, maxLevelColor, generatorVersion);
                return generatedLevel;

            case 13:
                numberTube = 11;
                numberEmptyTube = 2;
                numberInitLayers = 5;
                numberMaxLayers = 5;
                tubeToWin = 9;
                maxLevelColor = 9;
                seed = 1;
                generatorVersion = 1;

                setupObject.initLevelParameters(numberTube, numberEmptyTube, numberInitLayers, numberMaxLayers, tubeToWin);
                generatedLevel = levels.levelGenerator(seed,numberTube, numberEmptyTube, numberInitLayers, numberMaxLayers, tubeToWin, maxLevelColor, generatorVersion);
                return generatedLevel;

            case 14:
                numberTube = 12;
                numberEmptyTube = 2;
                numberInitLayers = 5;
                numberMaxLayers = 5;
                tubeToWin = 10;
                maxLevelColor = 10;
                seed = 1;
                generatorVersion = 1;

                setupObject.initLevelParameters(numberTube, numberEmptyTube, numberInitLayers, numberMaxLayers, tubeToWin);
                generatedLevel = levels.levelGenerator(seed,numberTube, numberEmptyTube, numberInitLayers, numberMaxLayers, tubeToWin, maxLevelColor, generatorVersion);
                return generatedLevel;

            case 15:
                numberTube = 6;
                numberEmptyTube = 2;
                numberInitLayers = 6;
                numberMaxLayers = 6;
                tubeToWin = 4;
                maxLevelColor = 4;
                seed = 1;
                generatorVersion = 1;

                setupObject.initLevelParameters(numberTube, numberEmptyTube, numberInitLayers, numberMaxLayers, tubeToWin);
                generatedLevel = levels.levelGenerator(seed,numberTube, numberEmptyTube, numberInitLayers, numberMaxLayers, tubeToWin, maxLevelColor, generatorVersion);
                return generatedLevel;

            case 16:
                numberTube = 7;
                numberEmptyTube = 2;
                numberInitLayers = 6;
                numberMaxLayers = 6;
                tubeToWin = 5;
                maxLevelColor = 5;
                seed = 1;
                generatorVersion = 1;

                setupObject.initLevelParameters(numberTube, numberEmptyTube, numberInitLayers, numberMaxLayers, tubeToWin);
                generatedLevel = levels.levelGenerator(seed,numberTube, numberEmptyTube, numberInitLayers, numberMaxLayers, tubeToWin, maxLevelColor, generatorVersion);
                return generatedLevel;

            case 17:
                numberTube = 8;
                numberEmptyTube = 2;
                numberInitLayers = 6;
                numberMaxLayers = 6;
                tubeToWin = 6;
                maxLevelColor = 6;
                seed = 1;
                generatorVersion = 1;

                setupObject.initLevelParameters(numberTube, numberEmptyTube, numberInitLayers, numberMaxLayers, tubeToWin);
                generatedLevel = levels.levelGenerator(seed,numberTube, numberEmptyTube, numberInitLayers, numberMaxLayers, tubeToWin, maxLevelColor, generatorVersion);
                return generatedLevel;

            case 18:
                numberTube = 9;
                numberEmptyTube = 2;
                numberInitLayers = 6;
                numberMaxLayers = 6;
                tubeToWin = 7;
                maxLevelColor = 7;
                seed = 1;
                generatorVersion = 1;

                setupObject.initLevelParameters(numberTube, numberEmptyTube, numberInitLayers, numberMaxLayers, tubeToWin);
                generatedLevel = levels.levelGenerator(seed,numberTube, numberEmptyTube, numberInitLayers, numberMaxLayers, tubeToWin, maxLevelColor, generatorVersion);
                return generatedLevel;

            case 19:
                numberTube = 10;
                numberEmptyTube = 2;
                numberInitLayers = 6;
                numberMaxLayers = 6;
                tubeToWin = 8;
                maxLevelColor = 8;
                seed = 1;
                generatorVersion = 1;

                setupObject.initLevelParameters(numberTube, numberEmptyTube, numberInitLayers, numberMaxLayers, tubeToWin);
                generatedLevel = levels.levelGenerator(seed,numberTube, numberEmptyTube, numberInitLayers, numberMaxLayers, tubeToWin, maxLevelColor, generatorVersion);
                return generatedLevel;

            case 20:
                numberTube = 11;
                numberEmptyTube = 2;
                numberInitLayers = 6;
                numberMaxLayers = 6;
                tubeToWin = 9;
                maxLevelColor = 9;
                seed = 1;
                generatorVersion = 1;

                setupObject.initLevelParameters(numberTube, numberEmptyTube, numberInitLayers, numberMaxLayers, tubeToWin);
                generatedLevel = levels.levelGenerator(seed,numberTube, numberEmptyTube, numberInitLayers, numberMaxLayers, tubeToWin, maxLevelColor, generatorVersion);
                return generatedLevel;

            case 21:
                numberTube = 12;
                numberEmptyTube = 2;
                numberInitLayers = 6;
                numberMaxLayers = 6;
                tubeToWin = 10;
                maxLevelColor = 10;
                seed = -710306331;
                generatorVersion = 1;

                setupObject.initLevelParameters(numberTube, numberEmptyTube, numberInitLayers, numberMaxLayers, tubeToWin);
                generatedLevel = levels.levelGenerator(seed,numberTube, numberEmptyTube, numberInitLayers, numberMaxLayers, tubeToWin, maxLevelColor, generatorVersion);
                return generatedLevel;

            case 22:
                numberTube = 6;
                numberEmptyTube = 2;
                numberInitLayers = 7;
                numberMaxLayers = 7;
                tubeToWin = 4;
                maxLevelColor = 4;
                seed = 1;
                generatorVersion = 1;

                setupObject.initLevelParameters(numberTube, numberEmptyTube, numberInitLayers, numberMaxLayers, tubeToWin);
                generatedLevel = levels.levelGenerator(seed,numberTube, numberEmptyTube, numberInitLayers, numberMaxLayers, tubeToWin, maxLevelColor, generatorVersion);
                return generatedLevel;

            case 23:
                numberTube = 7;
                numberEmptyTube = 2;
                numberInitLayers = 7;
                numberMaxLayers = 7;
                tubeToWin = 5;
                maxLevelColor = 5;
                seed = 1;
                generatorVersion = 1;

                setupObject.initLevelParameters(numberTube, numberEmptyTube, numberInitLayers, numberMaxLayers, tubeToWin);
                generatedLevel = levels.levelGenerator(seed,numberTube, numberEmptyTube, numberInitLayers, numberMaxLayers, tubeToWin, maxLevelColor, generatorVersion);
                return generatedLevel;

            case 24:
                numberTube = 8;
                numberEmptyTube = 2;
                numberInitLayers = 7;
                numberMaxLayers = 7;
                tubeToWin = 6;
                maxLevelColor = 6;
                seed = 1;
                generatorVersion = 1;

                setupObject.initLevelParameters(numberTube, numberEmptyTube, numberInitLayers, numberMaxLayers, tubeToWin);
                generatedLevel = levels.levelGenerator(seed,numberTube, numberEmptyTube, numberInitLayers, numberMaxLayers, tubeToWin, maxLevelColor, generatorVersion);
                return generatedLevel;

            case 25:
                numberTube = 9;
                numberEmptyTube = 2;
                numberInitLayers = 7;
                numberMaxLayers = 7;
                tubeToWin = 7;
                maxLevelColor = 7;
                seed = 1;
                generatorVersion = 1;

                setupObject.initLevelParameters(numberTube, numberEmptyTube, numberInitLayers, numberMaxLayers, tubeToWin);
                generatedLevel = levels.levelGenerator(seed,numberTube, numberEmptyTube, numberInitLayers, numberMaxLayers, tubeToWin, maxLevelColor, generatorVersion);
                return generatedLevel;

            case 26:
                numberTube = 10;
                numberEmptyTube = 2;
                numberInitLayers = 7;
                numberMaxLayers = 7;
                tubeToWin = 8;
                maxLevelColor = 8;
                seed = 1;
                generatorVersion = 1;

                setupObject.initLevelParameters(numberTube, numberEmptyTube, numberInitLayers, numberMaxLayers, tubeToWin);
                generatedLevel = levels.levelGenerator(seed,numberTube, numberEmptyTube, numberInitLayers, numberMaxLayers, tubeToWin, maxLevelColor, generatorVersion);
                return generatedLevel;

            case 27:
                numberTube = 11;
                numberEmptyTube = 2;
                numberInitLayers = 7;
                numberMaxLayers = 7;
                tubeToWin = 9;
                maxLevelColor = 9;
                seed = 1;
                generatorVersion = 1;

                setupObject.initLevelParameters(numberTube, numberEmptyTube, numberInitLayers, numberMaxLayers, tubeToWin);
                generatedLevel = levels.levelGenerator(seed,numberTube, numberEmptyTube, numberInitLayers, numberMaxLayers, tubeToWin, maxLevelColor, generatorVersion);
                return generatedLevel;

            case 28:
                numberTube = 12;
                numberEmptyTube = 2;
                numberInitLayers = 7;
                numberMaxLayers = 7;
                tubeToWin = 10;
                maxLevelColor = 10;
                seed = -710306331; // -43873520
                generatorVersion = 1;

                setupObject.initLevelParameters(numberTube, numberEmptyTube, numberInitLayers, numberMaxLayers, tubeToWin);
                generatedLevel = levels.levelGenerator(seed,numberTube, numberEmptyTube, numberInitLayers, numberMaxLayers, tubeToWin, maxLevelColor, generatorVersion);
                return generatedLevel;

            case 29:
                numberTube = 6;
                numberEmptyTube = 2;
                numberInitLayers = 8;
                numberMaxLayers = 8;
                tubeToWin = 4;
                maxLevelColor = 4;
                seed = 1;
                generatorVersion = 1;

                setupObject.initLevelParameters(numberTube, numberEmptyTube, numberInitLayers, numberMaxLayers, tubeToWin);
                generatedLevel = levels.levelGenerator(seed,numberTube, numberEmptyTube, numberInitLayers, numberMaxLayers, tubeToWin, maxLevelColor, generatorVersion);
                return generatedLevel;

            case 30:
                numberTube = 7;
                numberEmptyTube = 2;
                numberInitLayers = 8;
                numberMaxLayers = 8;
                tubeToWin = 5;
                maxLevelColor = 5;
                seed = 1;
                generatorVersion = 1;

                setupObject.initLevelParameters(numberTube, numberEmptyTube, numberInitLayers, numberMaxLayers, tubeToWin);
                generatedLevel = levels.levelGenerator(seed,numberTube, numberEmptyTube, numberInitLayers, numberMaxLayers, tubeToWin, maxLevelColor, generatorVersion);
                return generatedLevel;

            case 31:
                numberTube = 8;
                numberEmptyTube = 2;
                numberInitLayers = 8;
                numberMaxLayers = 8;
                tubeToWin = 6;
                maxLevelColor = 6;
                seed = 1;
                generatorVersion = 1;

                setupObject.initLevelParameters(numberTube, numberEmptyTube, numberInitLayers, numberMaxLayers, tubeToWin);
                generatedLevel = levels.levelGenerator(seed,numberTube, numberEmptyTube, numberInitLayers, numberMaxLayers, tubeToWin, maxLevelColor, generatorVersion);
                return generatedLevel;

            case 32:
                numberTube = 9;
                numberEmptyTube = 2;
                numberInitLayers = 8;
                numberMaxLayers = 8;
                tubeToWin = 7;
                maxLevelColor = 7;
                seed = 1;
                generatorVersion = 1;

                setupObject.initLevelParameters(numberTube, numberEmptyTube, numberInitLayers, numberMaxLayers, tubeToWin);
                generatedLevel = levels.levelGenerator(seed,numberTube, numberEmptyTube, numberInitLayers, numberMaxLayers, tubeToWin, maxLevelColor, generatorVersion);
                return generatedLevel;

            case 33:
                numberTube = 10;
                numberEmptyTube = 2;
                numberInitLayers = 8;
                numberMaxLayers = 8;
                tubeToWin = 8;
                maxLevelColor = 8;
                seed = 1;
                generatorVersion = 1;

                setupObject.initLevelParameters(numberTube, numberEmptyTube, numberInitLayers, numberMaxLayers, tubeToWin);
                generatedLevel = levels.levelGenerator(seed,numberTube, numberEmptyTube, numberInitLayers, numberMaxLayers, tubeToWin, maxLevelColor, generatorVersion);
                return generatedLevel;

            case 34:
                numberTube = 11;
                numberEmptyTube = 2;
                numberInitLayers = 8;
                numberMaxLayers = 8;
                tubeToWin = 9;
                maxLevelColor = 9;
                seed = 1;
                generatorVersion = 1;

                setupObject.initLevelParameters(numberTube, numberEmptyTube, numberInitLayers, numberMaxLayers, tubeToWin);
                generatedLevel = levels.levelGenerator(seed,numberTube, numberEmptyTube, numberInitLayers, numberMaxLayers, tubeToWin, maxLevelColor, generatorVersion);
                return generatedLevel;

            case 35:
                numberTube = 12;
                numberEmptyTube = 2;
                numberInitLayers = 8;
                numberMaxLayers = 8;
                tubeToWin = 10;
                maxLevelColor = 10;
                seed = 1;
                generatorVersion = 1;

                setupObject.initLevelParameters(numberTube, numberEmptyTube, numberInitLayers, numberMaxLayers, tubeToWin);
                generatedLevel = levels.levelGenerator(seed,numberTube, numberEmptyTube, numberInitLayers, numberMaxLayers, tubeToWin, maxLevelColor, generatorVersion);
                return generatedLevel;

            case 36:
                numberTube = 5;
                numberEmptyTube = 1;
                numberInitLayers = 4;
                numberMaxLayers = 4;
                tubeToWin = 4;
                maxLevelColor = 4;
                seed = -487812485;
                generatorVersion = 1;

                setupObject.initLevelParameters(numberTube, numberEmptyTube, numberInitLayers, numberMaxLayers, tubeToWin);
                generatedLevel = levels.levelGenerator(seed,numberTube, numberEmptyTube, numberInitLayers, numberMaxLayers, tubeToWin, maxLevelColor, generatorVersion);
                return generatedLevel;

            case 37:
                numberTube = 6;
                numberEmptyTube = 1;
                numberInitLayers = 4;
                numberMaxLayers = 4;
                tubeToWin = 5;
                maxLevelColor = 5;
                seed = -59955976;
                generatorVersion = 1;

                setupObject.initLevelParameters(numberTube, numberEmptyTube, numberInitLayers, numberMaxLayers, tubeToWin);
                generatedLevel = levels.levelGenerator(seed,numberTube, numberEmptyTube, numberInitLayers, numberMaxLayers, tubeToWin, maxLevelColor, generatorVersion);
                return generatedLevel;

            case 38:
                numberTube = 7;
                numberEmptyTube = 1;
                numberInitLayers = 4;
                numberMaxLayers = 4;
                tubeToWin = 6;
                maxLevelColor = 6;
                seed = 1436246592;
                generatorVersion = 1;

                setupObject.initLevelParameters(numberTube, numberEmptyTube, numberInitLayers, numberMaxLayers, tubeToWin);
                generatedLevel = levels.levelGenerator(seed,numberTube, numberEmptyTube, numberInitLayers, numberMaxLayers, tubeToWin, maxLevelColor, generatorVersion);
                return generatedLevel;

            case 39:
                numberTube = 8;
                numberEmptyTube = 1;
                numberInitLayers = 4;
                numberMaxLayers = 4;
                tubeToWin = 7;
                maxLevelColor = 7;
                seed = 1933681151;
                generatorVersion = 1;

                setupObject.initLevelParameters(numberTube, numberEmptyTube, numberInitLayers, numberMaxLayers, tubeToWin);
                generatedLevel = levels.levelGenerator(seed,numberTube, numberEmptyTube, numberInitLayers, numberMaxLayers, tubeToWin, maxLevelColor, generatorVersion);
                return generatedLevel;

            case 40:
                numberTube = 9;
                numberEmptyTube = 1;
                numberInitLayers = 4;
                numberMaxLayers = 4;
                tubeToWin = 8;
                maxLevelColor = 8;
                seed = -2147076337;
                generatorVersion = 1;

                setupObject.initLevelParameters(numberTube, numberEmptyTube, numberInitLayers, numberMaxLayers, tubeToWin);
                generatedLevel = levels.levelGenerator(seed,numberTube, numberEmptyTube, numberInitLayers, numberMaxLayers, tubeToWin, maxLevelColor, generatorVersion);
                return generatedLevel;

            case 41:
                numberTube = 10;
                numberEmptyTube = 1;
                numberInitLayers = 4;
                numberMaxLayers = 4;
                tubeToWin = 9;
                maxLevelColor = 9;
                seed = 877639030;
                generatorVersion = 1;

                setupObject.initLevelParameters(numberTube, numberEmptyTube, numberInitLayers, numberMaxLayers, tubeToWin);
                generatedLevel = levels.levelGenerator(seed,numberTube, numberEmptyTube, numberInitLayers, numberMaxLayers, tubeToWin, maxLevelColor, generatorVersion);
                return generatedLevel;

            case 42:
                numberTube = 11;
                numberEmptyTube = 1;
                numberInitLayers = 4;
                numberMaxLayers = 4;
                tubeToWin = 10;
                maxLevelColor = 10;
                seed = 1814500245;
                generatorVersion = 1;

                setupObject.initLevelParameters(numberTube, numberEmptyTube, numberInitLayers, numberMaxLayers, tubeToWin);
                generatedLevel = levels.levelGenerator(seed,numberTube, numberEmptyTube, numberInitLayers, numberMaxLayers, tubeToWin, maxLevelColor, generatorVersion);
                return generatedLevel;

            case 43:
                numberTube = 12;
                numberEmptyTube = 1;
                numberInitLayers = 4;
                numberMaxLayers = 4;
                tubeToWin = 11;
                maxLevelColor = 11;
                seed = 0;
                generatorVersion = 1;

                setupObject.initLevelParameters(numberTube, numberEmptyTube, numberInitLayers, numberMaxLayers, tubeToWin);
                generatedLevel = levels.levelGenerator(seed,numberTube, numberEmptyTube, numberInitLayers, numberMaxLayers, tubeToWin, maxLevelColor, generatorVersion);
                return generatedLevel;

            case 44:
                numberTube = 5;
                numberEmptyTube = 1;
                numberInitLayers = 5;
                numberMaxLayers = 5;
                tubeToWin = 4;
                maxLevelColor = 4;
                seed = 492952610;
                generatorVersion = 1;

                setupObject.initLevelParameters(numberTube, numberEmptyTube, numberInitLayers, numberMaxLayers, tubeToWin);
                generatedLevel = levels.levelGenerator(seed,numberTube, numberEmptyTube, numberInitLayers, numberMaxLayers, tubeToWin, maxLevelColor, generatorVersion);
                return generatedLevel;

            case 45:
                numberTube = 6;
                numberEmptyTube = 1;
                numberInitLayers = 5;
                numberMaxLayers = 5;
                tubeToWin = 5;
                maxLevelColor = 5;
                seed = 1470882018;
                generatorVersion = 1;

                setupObject.initLevelParameters(numberTube, numberEmptyTube, numberInitLayers, numberMaxLayers, tubeToWin);
                generatedLevel = levels.levelGenerator(seed,numberTube, numberEmptyTube, numberInitLayers, numberMaxLayers, tubeToWin, maxLevelColor, generatorVersion);
                return generatedLevel;

            case 46:
                numberTube = 7;
                numberEmptyTube = 1;
                numberInitLayers = 5;
                numberMaxLayers = 5;
                tubeToWin = 6;
                maxLevelColor = 6;
                seed = -103140882;
                generatorVersion = 1;

                setupObject.initLevelParameters(numberTube, numberEmptyTube, numberInitLayers, numberMaxLayers, tubeToWin);
                generatedLevel = levels.levelGenerator(seed,numberTube, numberEmptyTube, numberInitLayers, numberMaxLayers, tubeToWin, maxLevelColor, generatorVersion);
                return generatedLevel;

            case 47:
                numberTube = 8;
                numberEmptyTube = 1;
                numberInitLayers = 5;
                numberMaxLayers = 5;
                tubeToWin = 7;
                maxLevelColor = 7;
                seed = -889384934;
                generatorVersion = 1;

                setupObject.initLevelParameters(numberTube, numberEmptyTube, numberInitLayers, numberMaxLayers, tubeToWin);
                generatedLevel = levels.levelGenerator(seed,numberTube, numberEmptyTube, numberInitLayers, numberMaxLayers, tubeToWin, maxLevelColor, generatorVersion);
                return generatedLevel;

            case 48:
                numberTube = 9;
                numberEmptyTube = 1;
                numberInitLayers = 5;
                numberMaxLayers = 5;
                tubeToWin = 8;
                maxLevelColor = 8;
                seed = 2139609325;
                generatorVersion = 1;

                setupObject.initLevelParameters(numberTube, numberEmptyTube, numberInitLayers, numberMaxLayers, tubeToWin);
                generatedLevel = levels.levelGenerator(seed,numberTube, numberEmptyTube, numberInitLayers, numberMaxLayers, tubeToWin, maxLevelColor, generatorVersion);
                return generatedLevel;

            case 49:
                numberTube = 10;
                numberEmptyTube = 1;
                numberInitLayers = 5;
                numberMaxLayers = 5;
                tubeToWin = 9;
                maxLevelColor = 9;
                seed = 1699502929;
                generatorVersion = 1;

                setupObject.initLevelParameters(numberTube, numberEmptyTube, numberInitLayers, numberMaxLayers, tubeToWin);
                generatedLevel = levels.levelGenerator(seed,numberTube, numberEmptyTube, numberInitLayers, numberMaxLayers, tubeToWin, maxLevelColor, generatorVersion);
                return generatedLevel;

            case 50:
                numberTube = 11;
                numberEmptyTube = 1;
                numberInitLayers = 5;
                numberMaxLayers = 5;
                tubeToWin = 10;
                maxLevelColor = 10;
                seed = 1911788288;
                generatorVersion = 1;

                setupObject.initLevelParameters(numberTube, numberEmptyTube, numberInitLayers, numberMaxLayers, tubeToWin);
                generatedLevel = levels.levelGenerator(seed,numberTube, numberEmptyTube, numberInitLayers, numberMaxLayers, tubeToWin, maxLevelColor, generatorVersion);
                return generatedLevel;

            case 51:
                numberTube = 12;
                numberEmptyTube = 1;
                numberInitLayers = 5;
                numberMaxLayers = 5;
                tubeToWin = 11;
                maxLevelColor = 11;
                seed = 0;
                generatorVersion = 1;

                setupObject.initLevelParameters(numberTube, numberEmptyTube, numberInitLayers, numberMaxLayers, tubeToWin);
                generatedLevel = levels.levelGenerator(seed,numberTube, numberEmptyTube, numberInitLayers, numberMaxLayers, tubeToWin, maxLevelColor, generatorVersion);
                return generatedLevel;

            case 52:
                numberTube = 5;
                numberEmptyTube = 1;
                numberInitLayers = 6;
                numberMaxLayers = 6;
                tubeToWin = 4;
                maxLevelColor = 4;
                seed = 232916072;
                generatorVersion = 1;

                setupObject.initLevelParameters(numberTube, numberEmptyTube, numberInitLayers, numberMaxLayers, tubeToWin);
                generatedLevel = levels.levelGenerator(seed,numberTube, numberEmptyTube, numberInitLayers, numberMaxLayers, tubeToWin, maxLevelColor, generatorVersion);
                return generatedLevel;

            case 53:
                numberTube = 6;
                numberEmptyTube = 1;
                numberInitLayers = 6;
                numberMaxLayers = 6;
                tubeToWin = 5;
                maxLevelColor = 5;
                seed = 464550047;
                generatorVersion = 1;

                setupObject.initLevelParameters(numberTube, numberEmptyTube, numberInitLayers, numberMaxLayers, tubeToWin);
                generatedLevel = levels.levelGenerator(seed,numberTube, numberEmptyTube, numberInitLayers, numberMaxLayers, tubeToWin, maxLevelColor, generatorVersion);
                return generatedLevel;

            case 54:
                numberTube = 7;
                numberEmptyTube = 1;
                numberInitLayers = 6;
                numberMaxLayers = 6;
                tubeToWin = 6;
                maxLevelColor = 6;
                seed = 1230198419;
                generatorVersion = 1;

                setupObject.initLevelParameters(numberTube, numberEmptyTube, numberInitLayers, numberMaxLayers, tubeToWin);
                generatedLevel = levels.levelGenerator(seed,numberTube, numberEmptyTube, numberInitLayers, numberMaxLayers, tubeToWin, maxLevelColor, generatorVersion);
                return generatedLevel;

            case 55:
                numberTube = 8;
                numberEmptyTube = 1;
                numberInitLayers = 6;
                numberMaxLayers = 6;
                tubeToWin = 7;
                maxLevelColor = 7;
                seed = 1230198419;
                generatorVersion = 1;

                setupObject.initLevelParameters(numberTube, numberEmptyTube, numberInitLayers, numberMaxLayers, tubeToWin);
                generatedLevel = levels.levelGenerator(seed,numberTube, numberEmptyTube, numberInitLayers, numberMaxLayers, tubeToWin, maxLevelColor, generatorVersion);
                return generatedLevel;

            case 56:
                numberTube = 9;
                numberEmptyTube = 1;
                numberInitLayers = 6;
                numberMaxLayers = 6;
                tubeToWin = 8;
                maxLevelColor = 8;
                seed = 740350752;//1876527899;
                generatorVersion = 2;

                setupObject.initLevelParameters(numberTube, numberEmptyTube, numberInitLayers, numberMaxLayers, tubeToWin);
                generatedLevel = levels.levelGenerator(seed,numberTube, numberEmptyTube, numberInitLayers, numberMaxLayers, tubeToWin, maxLevelColor, generatorVersion);
                return generatedLevel;

            case 57:
                numberTube = 10;
                numberEmptyTube = 1;
                numberInitLayers = 6;
                numberMaxLayers = 6;
                tubeToWin = 9;
                maxLevelColor = 9;
                seed = 315066914;
                generatorVersion = 2;

                setupObject.initLevelParameters(numberTube, numberEmptyTube, numberInitLayers, numberMaxLayers, tubeToWin);
                generatedLevel = levels.levelGenerator(seed,numberTube, numberEmptyTube, numberInitLayers, numberMaxLayers, tubeToWin, maxLevelColor, generatorVersion);
                return generatedLevel;

            case 58:
                numberTube = 11;
                numberEmptyTube = 1;
                numberInitLayers = 6;
                numberMaxLayers = 6;
                tubeToWin = 10;
                maxLevelColor = 10;
                seed = -2061995040;
                generatorVersion = 2;

                setupObject.initLevelParameters(numberTube, numberEmptyTube, numberInitLayers, numberMaxLayers, tubeToWin);
                generatedLevel = levels.levelGenerator(seed,numberTube, numberEmptyTube, numberInitLayers, numberMaxLayers, tubeToWin, maxLevelColor, generatorVersion);
                return generatedLevel;

            case 59:
                numberTube = 12;
                numberEmptyTube = 1;
                numberInitLayers = 6;
                numberMaxLayers = 6;
                tubeToWin = 11;
                maxLevelColor = 11;
                seed = 0;
                generatorVersion = 1;

                setupObject.initLevelParameters(numberTube, numberEmptyTube, numberInitLayers, numberMaxLayers, tubeToWin);
                generatedLevel = levels.levelGenerator(seed,numberTube, numberEmptyTube, numberInitLayers, numberMaxLayers, tubeToWin, maxLevelColor, generatorVersion);
                return generatedLevel;

            case 60:
                numberTube = 5;
                numberEmptyTube = 1;
                numberInitLayers = 7;
                numberMaxLayers = 7;
                tubeToWin = 4;
                maxLevelColor = 4;
                seed = 1230198419;
                generatorVersion = 1;

                setupObject.initLevelParameters(numberTube, numberEmptyTube, numberInitLayers, numberMaxLayers, tubeToWin);
                generatedLevel = levels.levelGenerator(seed,numberTube, numberEmptyTube, numberInitLayers, numberMaxLayers, tubeToWin, maxLevelColor, generatorVersion);
                return generatedLevel;

            case 61:
                numberTube = 6;
                numberEmptyTube = 1;
                numberInitLayers = 7;
                numberMaxLayers = 7;
                tubeToWin = 5;
                maxLevelColor = 5;
                seed = 915497011;
                generatorVersion = 1;

                setupObject.initLevelParameters(numberTube, numberEmptyTube, numberInitLayers, numberMaxLayers, tubeToWin);
                generatedLevel = levels.levelGenerator(seed,numberTube, numberEmptyTube, numberInitLayers, numberMaxLayers, tubeToWin, maxLevelColor, generatorVersion);
                return generatedLevel;

            case 62:
                numberTube = 7;
                numberEmptyTube = 1;
                numberInitLayers = 7;
                numberMaxLayers = 7;
                tubeToWin = 6;
                maxLevelColor = 6;
                seed = 0;
                generatorVersion = 1;

                setupObject.initLevelParameters(numberTube, numberEmptyTube, numberInitLayers, numberMaxLayers, tubeToWin);
                generatedLevel = levels.levelGenerator(seed,numberTube, numberEmptyTube, numberInitLayers, numberMaxLayers, tubeToWin, maxLevelColor, generatorVersion);
                return generatedLevel;

            case 63:
                numberTube = 8;
                numberEmptyTube = 1;
                numberInitLayers = 7;
                numberMaxLayers = 7;
                tubeToWin = 7;
                maxLevelColor = 7;
                seed = -1118133291;
                generatorVersion = 2;

                setupObject.initLevelParameters(numberTube, numberEmptyTube, numberInitLayers, numberMaxLayers, tubeToWin);
                generatedLevel = levels.levelGenerator(seed,numberTube, numberEmptyTube, numberInitLayers, numberMaxLayers, tubeToWin, maxLevelColor, generatorVersion);
                return generatedLevel;

            case 64:
                numberTube = 9;
                numberEmptyTube = 1;
                numberInitLayers = 7;
                numberMaxLayers = 7;
                tubeToWin = 8;
                maxLevelColor = 8;
                seed = -781584217;
                generatorVersion = 2;

                setupObject.initLevelParameters(numberTube, numberEmptyTube, numberInitLayers, numberMaxLayers, tubeToWin);
                generatedLevel = levels.levelGenerator(seed,numberTube, numberEmptyTube, numberInitLayers, numberMaxLayers, tubeToWin, maxLevelColor, generatorVersion);
                return generatedLevel;

            case 65:
                numberTube = 10;
                numberEmptyTube = 1;
                numberInitLayers = 7;
                numberMaxLayers = 7;
                tubeToWin = 9;
                maxLevelColor = 9;
                seed = 1589765535;
                generatorVersion = 2;
                rewindPercentage = 0.55f;

                setupObject.initLevelParameters(numberTube, numberEmptyTube, numberInitLayers, numberMaxLayers, tubeToWin);
                generatedLevel = levels.levelGenerator(seed,numberTube, numberEmptyTube, numberInitLayers, numberMaxLayers, tubeToWin, maxLevelColor, generatorVersion, rewindPercentage);
                return generatedLevel;

            case 66:
                numberTube = 11;
                numberEmptyTube = 1;
                numberInitLayers = 7;
                numberMaxLayers = 7;
                tubeToWin = 10;
                maxLevelColor = 10;
                seed = 1921444667;
                generatorVersion = 2;
                rewindPercentage = 0.55f;

                setupObject.initLevelParameters(numberTube, numberEmptyTube, numberInitLayers, numberMaxLayers, tubeToWin);
                generatedLevel = levels.levelGenerator(seed,numberTube, numberEmptyTube, numberInitLayers, numberMaxLayers, tubeToWin, maxLevelColor, generatorVersion, rewindPercentage);
                return generatedLevel;

            case 67:
                numberTube = 12;
                numberEmptyTube = 1;
                numberInitLayers = 7;
                numberMaxLayers = 7;
                tubeToWin = 11;
                maxLevelColor = 11;
                seed = 0;
                generatorVersion = 2;
                rewindPercentage = 0.55f;

                setupObject.initLevelParameters(numberTube, numberEmptyTube, numberInitLayers, numberMaxLayers, tubeToWin);
                generatedLevel = levels.levelGenerator(seed,numberTube, numberEmptyTube, numberInitLayers, numberMaxLayers, tubeToWin, maxLevelColor, generatorVersion, rewindPercentage);
                return generatedLevel;

            case 68:
                numberTube = 5;
                numberEmptyTube = 1;
                numberInitLayers = 8;
                numberMaxLayers = 8;
                tubeToWin = 4;
                maxLevelColor = 4;
                seed = -63437094;
                generatorVersion = 1;

                setupObject.initLevelParameters(numberTube, numberEmptyTube, numberInitLayers, numberMaxLayers, tubeToWin);
                generatedLevel = levels.levelGenerator(seed,numberTube, numberEmptyTube, numberInitLayers, numberMaxLayers, tubeToWin, maxLevelColor, generatorVersion);
                return generatedLevel;

            case 69:
                numberTube = 6;
                numberEmptyTube = 1;
                numberInitLayers = 8;
                numberMaxLayers = 8;
                tubeToWin = 5;
                maxLevelColor = 5;
                seed = 304135680;
                generatorVersion = 2;
                rewindPercentage = 0.40f;

                setupObject.initLevelParameters(numberTube, numberEmptyTube, numberInitLayers, numberMaxLayers, tubeToWin);
                generatedLevel = levels.levelGenerator(seed,numberTube, numberEmptyTube, numberInitLayers, numberMaxLayers, tubeToWin, maxLevelColor, generatorVersion, rewindPercentage);
                return generatedLevel;

            case 70:
                numberTube = 7;
                numberEmptyTube = 1;
                numberInitLayers = 8;
                numberMaxLayers = 8;
                tubeToWin = 6;
                maxLevelColor = 6;
                seed = -2137212480;
                generatorVersion = 2;
                rewindPercentage = 0.40f;

                setupObject.initLevelParameters(numberTube, numberEmptyTube, numberInitLayers, numberMaxLayers, tubeToWin);
                generatedLevel = levels.levelGenerator(seed,numberTube, numberEmptyTube, numberInitLayers, numberMaxLayers, tubeToWin, maxLevelColor, generatorVersion, rewindPercentage);
                return generatedLevel;

            case 71:
                numberTube = 8;
                numberEmptyTube = 1;
                numberInitLayers = 8;
                numberMaxLayers = 8;
                tubeToWin = 7;
                maxLevelColor = 7;
                seed = -655505300;
                generatorVersion = 2;
                rewindPercentage = 0.5f;

                setupObject.initLevelParameters(numberTube, numberEmptyTube, numberInitLayers, numberMaxLayers, tubeToWin);
                generatedLevel = levels.levelGenerator(seed,numberTube, numberEmptyTube, numberInitLayers, numberMaxLayers, tubeToWin, maxLevelColor, generatorVersion, rewindPercentage);
                return generatedLevel;

            case 72:
                numberTube = 9;
                numberEmptyTube = 1;
                numberInitLayers = 8;
                numberMaxLayers = 8;
                tubeToWin = 8;
                maxLevelColor = 8;
                seed = 1031627519;
                generatorVersion = 2;
                rewindPercentage = 0.60f;

                setupObject.initLevelParameters(numberTube, numberEmptyTube, numberInitLayers, numberMaxLayers, tubeToWin);
                generatedLevel = levels.levelGenerator(seed,numberTube, numberEmptyTube, numberInitLayers, numberMaxLayers, tubeToWin, maxLevelColor, generatorVersion, rewindPercentage);
                return generatedLevel;

            case 73:
                numberTube = 10;
                numberEmptyTube = 1;
                numberInitLayers = 8;
                numberMaxLayers = 8;
                tubeToWin = 9;
                maxLevelColor = 9;
                seed = 224941955;
                generatorVersion = 2;
                rewindPercentage = 0.60f;

                setupObject.initLevelParameters(numberTube, numberEmptyTube, numberInitLayers, numberMaxLayers, tubeToWin);
                generatedLevel = levels.levelGenerator(seed,numberTube, numberEmptyTube, numberInitLayers, numberMaxLayers, tubeToWin, maxLevelColor, generatorVersion, rewindPercentage);
                return generatedLevel;

            case 74:
                numberTube = 11;
                numberEmptyTube = 1;
                numberInitLayers = 8;
                numberMaxLayers = 8;
                tubeToWin = 10;
                maxLevelColor = 10;
                seed = 1305240332;
                generatorVersion = 2;
                rewindPercentage = 0.70f;

                setupObject.initLevelParameters(numberTube, numberEmptyTube, numberInitLayers, numberMaxLayers, tubeToWin);
                generatedLevel = levels.levelGenerator(seed,numberTube, numberEmptyTube, numberInitLayers, numberMaxLayers, tubeToWin, maxLevelColor, generatorVersion, rewindPercentage);
                return generatedLevel;

            case 75:
                numberTube = 12;
                numberEmptyTube = 1;
                numberInitLayers = 8;
                numberMaxLayers = 8;
                tubeToWin = 11;
                maxLevelColor = 11;
                seed = 0;
                generatorVersion = 2;
                rewindPercentage = 0.55f;

                setupObject.initLevelParameters(numberTube, numberEmptyTube, numberInitLayers, numberMaxLayers, tubeToWin);
                generatedLevel = levels.levelGenerator(seed,numberTube, numberEmptyTube, numberInitLayers, numberMaxLayers, tubeToWin, maxLevelColor, generatorVersion, rewindPercentage);
                return generatedLevel;

            case 76:
                numberTube = 4;
                numberEmptyTube = 0;
                numberInitLayers = 3;
                numberMaxLayers = 4;
                tubeToWin = 3;
                maxLevelColor = 3;
                seed = -105213848;
                generatorVersion = 2;

                setupObject.initLevelParameters(numberTube, numberEmptyTube, numberInitLayers, numberMaxLayers, tubeToWin);
                generatedLevel = levels.levelGenerator(seed,numberTube, numberEmptyTube, numberInitLayers, numberMaxLayers, tubeToWin, maxLevelColor, generatorVersion);
                return generatedLevel;

            case 77:
                numberTube = 6;
                numberEmptyTube = 0;
                numberInitLayers = 4;
                numberMaxLayers = 6;
                tubeToWin = 4;
                maxLevelColor = 4;
                seed = -1727871735;
                generatorVersion = 1;

                setupObject.initLevelParameters(numberTube, numberEmptyTube, numberInitLayers, numberMaxLayers, tubeToWin);
                generatedLevel = levels.levelGenerator(seed,numberTube, numberEmptyTube, numberInitLayers, numberMaxLayers, tubeToWin, maxLevelColor, generatorVersion);
                return generatedLevel;

            case 78:
                
                numberTube = 11;
                numberEmptyTube = 1;
                numberInitLayers = 4;
                numberMaxLayers = 5;
                tubeToWin = 8;
                maxLevelColor = tubeToWin;
                seed = 0;
                generatorVersion = 1;

                setupObject.initLevelParameters(numberTube, numberEmptyTube, numberInitLayers, numberMaxLayers, tubeToWin);
                generatedLevel = levels.levelGenerator(seed,numberTube, numberEmptyTube, numberInitLayers, numberMaxLayers, tubeToWin, maxLevelColor, generatorVersion);
                return generatedLevel;

            case 79:
                
                numberTube = 5;
                numberEmptyTube = 1;
                numberInitLayers = 4;
                numberMaxLayers = 4;
                tubeToWin = 4;
                maxLevelColor = 4;
                seed = 0;
                generatorVersion = 1;

                setupObject.initLevelParameters(numberTube, numberEmptyTube, numberInitLayers, numberMaxLayers, tubeToWin);
                generatedLevel = levels.levelGenerator(seed,numberTube, numberEmptyTube, numberInitLayers, numberMaxLayers, tubeToWin, maxLevelColor, generatorVersion);
                return generatedLevel;

            case 80:
                
                numberTube = 5;
                numberEmptyTube = 1;
                numberInitLayers = 4;
                numberMaxLayers = 4;
                tubeToWin = 4;
                maxLevelColor = 4;
                seed = 0;
                generatorVersion = 1;

                setupObject.initLevelParameters(numberTube, numberEmptyTube, numberInitLayers, numberMaxLayers, tubeToWin);
                generatedLevel = levels.levelGenerator(seed,numberTube, numberEmptyTube, numberInitLayers, numberMaxLayers, tubeToWin, maxLevelColor, generatorVersion);
                return generatedLevel;

            case 81:
                
                numberTube = 5;
                numberEmptyTube = 1;
                numberInitLayers = 4;
                numberMaxLayers = 4;
                tubeToWin = 4;
                maxLevelColor = 4;
                seed = 0;
                generatorVersion = 1;

                setupObject.initLevelParameters(numberTube, numberEmptyTube, numberInitLayers, numberMaxLayers, tubeToWin);
                generatedLevel = levels.levelGenerator(seed,numberTube, numberEmptyTube, numberInitLayers, numberMaxLayers, tubeToWin, maxLevelColor, generatorVersion);
                return generatedLevel;

            case 82:
                
                numberTube = 5;
                numberEmptyTube = 1;
                numberInitLayers = 4;
                numberMaxLayers = 4;
                tubeToWin = 4;
                maxLevelColor = 4;
                seed = 0;
                generatorVersion = 1;

                setupObject.initLevelParameters(numberTube, numberEmptyTube, numberInitLayers, numberMaxLayers, tubeToWin);
                generatedLevel = levels.levelGenerator(seed,numberTube, numberEmptyTube, numberInitLayers, numberMaxLayers, tubeToWin, maxLevelColor, generatorVersion);
                return generatedLevel;

            case 83:
                
                numberTube = 5;
                numberEmptyTube = 1;
                numberInitLayers = 4;
                numberMaxLayers = 4;
                tubeToWin = 4;
                maxLevelColor = 4;
                seed = 0;
                generatorVersion = 1;

                setupObject.initLevelParameters(numberTube, numberEmptyTube, numberInitLayers, numberMaxLayers, tubeToWin);
                generatedLevel = levels.levelGenerator(seed,numberTube, numberEmptyTube, numberInitLayers, numberMaxLayers, tubeToWin, maxLevelColor, generatorVersion);
                return generatedLevel;

            case 84:
                
                numberTube = 5;
                numberEmptyTube = 1;
                numberInitLayers = 4;
                numberMaxLayers = 4;
                tubeToWin = 4;
                maxLevelColor = 4;
                seed = 0;
                generatorVersion = 1;

                setupObject.initLevelParameters(numberTube, numberEmptyTube, numberInitLayers, numberMaxLayers, tubeToWin);
                generatedLevel = levels.levelGenerator(seed,numberTube, numberEmptyTube, numberInitLayers, numberMaxLayers, tubeToWin, maxLevelColor, generatorVersion);
                return generatedLevel;

            case 85:
                
                numberTube = 5;
                numberEmptyTube = 1;
                numberInitLayers = 4;
                numberMaxLayers = 4;
                tubeToWin = 4;
                maxLevelColor = 4;
                seed = 0;
                generatorVersion = 1;

                setupObject.initLevelParameters(numberTube, numberEmptyTube, numberInitLayers, numberMaxLayers, tubeToWin);
                generatedLevel = levels.levelGenerator(seed,numberTube, numberEmptyTube, numberInitLayers, numberMaxLayers, tubeToWin, maxLevelColor, generatorVersion);
                return generatedLevel;

            case 86:
                
                numberTube = 5;
                numberEmptyTube = 1;
                numberInitLayers = 4;
                numberMaxLayers = 4;
                tubeToWin = 4;
                maxLevelColor = 4;
                seed = 0;
                generatorVersion = 1;

                setupObject.initLevelParameters(numberTube, numberEmptyTube, numberInitLayers, numberMaxLayers, tubeToWin);
                generatedLevel = levels.levelGenerator(seed,numberTube, numberEmptyTube, numberInitLayers, numberMaxLayers, tubeToWin, maxLevelColor, generatorVersion);
                return generatedLevel;

            case 87:
                
                numberTube = 5;
                numberEmptyTube = 1;
                numberInitLayers = 4;
                numberMaxLayers = 4;
                tubeToWin = 4;
                maxLevelColor = 4;
                seed = 0;
                generatorVersion = 1;

                setupObject.initLevelParameters(numberTube, numberEmptyTube, numberInitLayers, numberMaxLayers, tubeToWin);
                generatedLevel = levels.levelGenerator(seed,numberTube, numberEmptyTube, numberInitLayers, numberMaxLayers, tubeToWin, maxLevelColor, generatorVersion);
                return generatedLevel;

            case 88:
                
                numberTube = 5;
                numberEmptyTube = 1;
                numberInitLayers = 4;
                numberMaxLayers = 4;
                tubeToWin = 4;
                maxLevelColor = 4;
                seed = 0;
                generatorVersion = 1;

                setupObject.initLevelParameters(numberTube, numberEmptyTube, numberInitLayers, numberMaxLayers, tubeToWin);
                generatedLevel = levels.levelGenerator(seed,numberTube, numberEmptyTube, numberInitLayers, numberMaxLayers, tubeToWin, maxLevelColor, generatorVersion);
                return generatedLevel;

            case 89:
                
                numberTube = 5;
                numberEmptyTube = 1;
                numberInitLayers = 4;
                numberMaxLayers = 4;
                tubeToWin = 4;
                maxLevelColor = 4;
                seed = 0;
                generatorVersion = 1;

                setupObject.initLevelParameters(numberTube, numberEmptyTube, numberInitLayers, numberMaxLayers, tubeToWin);
                generatedLevel = levels.levelGenerator(seed,numberTube, numberEmptyTube, numberInitLayers, numberMaxLayers, tubeToWin, maxLevelColor, generatorVersion);
                return generatedLevel;

            case 90:
                
                numberTube = 5;
                numberEmptyTube = 1;
                numberInitLayers = 4;
                numberMaxLayers = 4;
                tubeToWin = 4;
                maxLevelColor = 4;
                seed = 0;
                generatorVersion = 1;

                setupObject.initLevelParameters(numberTube, numberEmptyTube, numberInitLayers, numberMaxLayers, tubeToWin);
                generatedLevel = levels.levelGenerator(seed,numberTube, numberEmptyTube, numberInitLayers, numberMaxLayers, tubeToWin, maxLevelColor, generatorVersion);
                return generatedLevel;

            case 91:
                
                numberTube = 5;
                numberEmptyTube = 1;
                numberInitLayers = 4;
                numberMaxLayers = 4;
                tubeToWin = 4;
                maxLevelColor = 4;
                seed = 0;
                generatorVersion = 1;

                setupObject.initLevelParameters(numberTube, numberEmptyTube, numberInitLayers, numberMaxLayers, tubeToWin);
                generatedLevel = levels.levelGenerator(seed,numberTube, numberEmptyTube, numberInitLayers, numberMaxLayers, tubeToWin, maxLevelColor, generatorVersion);
                return generatedLevel;

            case 92:
                
                numberTube = 5;
                numberEmptyTube = 1;
                numberInitLayers = 4;
                numberMaxLayers = 4;
                tubeToWin = 4;
                maxLevelColor = 4;
                seed = 0;
                generatorVersion = 1;

                setupObject.initLevelParameters(numberTube, numberEmptyTube, numberInitLayers, numberMaxLayers, tubeToWin);
                generatedLevel = levels.levelGenerator(seed,numberTube, numberEmptyTube, numberInitLayers, numberMaxLayers, tubeToWin, maxLevelColor, generatorVersion);
                return generatedLevel;

            case 93:
                
                numberTube = 5;
                numberEmptyTube = 1;
                numberInitLayers = 4;
                numberMaxLayers = 4;
                tubeToWin = 4;
                maxLevelColor = 4;
                seed = 0;
                generatorVersion = 1;

                setupObject.initLevelParameters(numberTube, numberEmptyTube, numberInitLayers, numberMaxLayers, tubeToWin);
                generatedLevel = levels.levelGenerator(seed,numberTube, numberEmptyTube, numberInitLayers, numberMaxLayers, tubeToWin, maxLevelColor, generatorVersion);
                return generatedLevel;

            case 94:
                
                numberTube = 5;
                numberEmptyTube = 1;
                numberInitLayers = 4;
                numberMaxLayers = 4;
                tubeToWin = 4;
                maxLevelColor = 4;
                seed = 0;
                generatorVersion = 1;

                setupObject.initLevelParameters(numberTube, numberEmptyTube, numberInitLayers, numberMaxLayers, tubeToWin);
                generatedLevel = levels.levelGenerator(seed,numberTube, numberEmptyTube, numberInitLayers, numberMaxLayers, tubeToWin, maxLevelColor, generatorVersion);
                return generatedLevel;

            case 95:
                
                numberTube = 5;
                numberEmptyTube = 1;
                numberInitLayers = 4;
                numberMaxLayers = 4;
                tubeToWin = 4;
                maxLevelColor = 4;
                seed = 0;
                generatorVersion = 1;

                setupObject.initLevelParameters(numberTube, numberEmptyTube, numberInitLayers, numberMaxLayers, tubeToWin);
                generatedLevel = levels.levelGenerator(seed,numberTube, numberEmptyTube, numberInitLayers, numberMaxLayers, tubeToWin, maxLevelColor, generatorVersion);
                return generatedLevel;

            case 96:
                
                numberTube = 5;
                numberEmptyTube = 1;
                numberInitLayers = 4;
                numberMaxLayers = 4;
                tubeToWin = 4;
                maxLevelColor = 4;
                seed = 0;
                generatorVersion = 1;

                setupObject.initLevelParameters(numberTube, numberEmptyTube, numberInitLayers, numberMaxLayers, tubeToWin);
                generatedLevel = levels.levelGenerator(seed,numberTube, numberEmptyTube, numberInitLayers, numberMaxLayers, tubeToWin, maxLevelColor, generatorVersion);
                return generatedLevel;

            case 97:
                
                numberTube = 5;
                numberEmptyTube = 1;
                numberInitLayers = 4;
                numberMaxLayers = 4;
                tubeToWin = 4;
                maxLevelColor = 4;
                seed = 0;
                generatorVersion = 1;

                setupObject.initLevelParameters(numberTube, numberEmptyTube, numberInitLayers, numberMaxLayers, tubeToWin);
                generatedLevel = levels.levelGenerator(seed,numberTube, numberEmptyTube, numberInitLayers, numberMaxLayers, tubeToWin, maxLevelColor, generatorVersion);
                return generatedLevel;

            case 98:
                
                numberTube = 5;
                numberEmptyTube = 1;
                numberInitLayers = 4;
                numberMaxLayers = 4;
                tubeToWin = 4;
                maxLevelColor = 4;
                seed = 0;
                generatorVersion = 1;

                setupObject.initLevelParameters(numberTube, numberEmptyTube, numberInitLayers, numberMaxLayers, tubeToWin);
                generatedLevel = levels.levelGenerator(seed,numberTube, numberEmptyTube, numberInitLayers, numberMaxLayers, tubeToWin, maxLevelColor, generatorVersion);
                return generatedLevel;

            case 99:
                
                numberTube = 5;
                numberEmptyTube = 1;
                numberInitLayers = 4;
                numberMaxLayers = 4;
                tubeToWin = 4;
                maxLevelColor = 4;
                seed = 0;
                generatorVersion = 1;

                setupObject.initLevelParameters(numberTube, numberEmptyTube, numberInitLayers, numberMaxLayers, tubeToWin);
                generatedLevel = levels.levelGenerator(seed,numberTube, numberEmptyTube, numberInitLayers, numberMaxLayers, tubeToWin, maxLevelColor, generatorVersion);
                return generatedLevel;

            case 100:
                
                numberTube = 5;
                numberEmptyTube = 1;
                numberInitLayers = 4;
                numberMaxLayers = 4;
                tubeToWin = 4;
                maxLevelColor = 4;
                seed = 0;
                generatorVersion = 1;

                setupObject.initLevelParameters(numberTube, numberEmptyTube, numberInitLayers, numberMaxLayers, tubeToWin);
                generatedLevel = levels.levelGenerator(seed,numberTube, numberEmptyTube, numberInitLayers, numberMaxLayers, tubeToWin, maxLevelColor, generatorVersion);
                return generatedLevel;

            case 101:
                
                numberTube = 5;
                numberEmptyTube = 1;
                numberInitLayers = 4;
                numberMaxLayers = 4;
                tubeToWin = 4;
                maxLevelColor = 4;
                seed = 0;
                generatorVersion = 1;

                setupObject.initLevelParameters(numberTube, numberEmptyTube, numberInitLayers, numberMaxLayers, tubeToWin);
                generatedLevel = levels.levelGenerator(seed,numberTube, numberEmptyTube, numberInitLayers, numberMaxLayers, tubeToWin, maxLevelColor, generatorVersion);
                return generatedLevel;

            case 102:
                
                numberTube = 5;
                numberEmptyTube = 1;
                numberInitLayers = 4;
                numberMaxLayers = 4;
                tubeToWin = 4;
                maxLevelColor = 4;
                seed = 0;
                generatorVersion = 1;

                setupObject.initLevelParameters(numberTube, numberEmptyTube, numberInitLayers, numberMaxLayers, tubeToWin);
                generatedLevel = levels.levelGenerator(seed,numberTube, numberEmptyTube, numberInitLayers, numberMaxLayers, tubeToWin, maxLevelColor, generatorVersion);
                return generatedLevel;

            case 103:
                
                numberTube = 5;
                numberEmptyTube = 1;
                numberInitLayers = 4;
                numberMaxLayers = 4;
                tubeToWin = 4;
                maxLevelColor = 4;
                seed = 0;
                generatorVersion = 1;

                setupObject.initLevelParameters(numberTube, numberEmptyTube, numberInitLayers, numberMaxLayers, tubeToWin);
                generatedLevel = levels.levelGenerator(seed,numberTube, numberEmptyTube, numberInitLayers, numberMaxLayers, tubeToWin, maxLevelColor, generatorVersion);
                return generatedLevel;

            case 104:
                
                numberTube = 5;
                numberEmptyTube = 1;
                numberInitLayers = 4;
                numberMaxLayers = 4;
                tubeToWin = 4;
                maxLevelColor = 4;
                seed = 0;
                generatorVersion = 1;

                setupObject.initLevelParameters(numberTube, numberEmptyTube, numberInitLayers, numberMaxLayers, tubeToWin);
                generatedLevel = levels.levelGenerator(seed,numberTube, numberEmptyTube, numberInitLayers, numberMaxLayers, tubeToWin, maxLevelColor, generatorVersion);
                return generatedLevel;

            case 105:
                
                numberTube = 5;
                numberEmptyTube = 1;
                numberInitLayers = 4;
                numberMaxLayers = 4;
                tubeToWin = 4;
                maxLevelColor = 4;
                seed = 0;
                generatorVersion = 1;

                setupObject.initLevelParameters(numberTube, numberEmptyTube, numberInitLayers, numberMaxLayers, tubeToWin);
                generatedLevel = levels.levelGenerator(seed,numberTube, numberEmptyTube, numberInitLayers, numberMaxLayers, tubeToWin, maxLevelColor, generatorVersion);
                return generatedLevel;

            case 106:
                
                numberTube = 5;
                numberEmptyTube = 1;
                numberInitLayers = 4;
                numberMaxLayers = 4;
                tubeToWin = 4;
                maxLevelColor = 4;
                seed = 0;
                generatorVersion = 1;

                setupObject.initLevelParameters(numberTube, numberEmptyTube, numberInitLayers, numberMaxLayers, tubeToWin);
                generatedLevel = levels.levelGenerator(seed,numberTube, numberEmptyTube, numberInitLayers, numberMaxLayers, tubeToWin, maxLevelColor, generatorVersion);
                return generatedLevel;

            case 107:
                
                numberTube = 5;
                numberEmptyTube = 1;
                numberInitLayers = 4;
                numberMaxLayers = 4;
                tubeToWin = 4;
                maxLevelColor = 4;
                seed = 0;
                generatorVersion = 1;

                setupObject.initLevelParameters(numberTube, numberEmptyTube, numberInitLayers, numberMaxLayers, tubeToWin);
                generatedLevel = levels.levelGenerator(seed,numberTube, numberEmptyTube, numberInitLayers, numberMaxLayers, tubeToWin, maxLevelColor, generatorVersion);
                return generatedLevel;

            case 108:
                
                numberTube = 5;
                numberEmptyTube = 1;
                numberInitLayers = 4;
                numberMaxLayers = 4;
                tubeToWin = 4;
                maxLevelColor = 4;
                seed = 0;
                generatorVersion = 1;

                setupObject.initLevelParameters(numberTube, numberEmptyTube, numberInitLayers, numberMaxLayers, tubeToWin);
                generatedLevel = levels.levelGenerator(seed,numberTube, numberEmptyTube, numberInitLayers, numberMaxLayers, tubeToWin, maxLevelColor, generatorVersion);
                return generatedLevel;

            case 109:
                
                numberTube = 5;
                numberEmptyTube = 1;
                numberInitLayers = 4;
                numberMaxLayers = 4;
                tubeToWin = 4;
                maxLevelColor = 4;
                seed = 0;
                generatorVersion = 1;

                setupObject.initLevelParameters(numberTube, numberEmptyTube, numberInitLayers, numberMaxLayers, tubeToWin);
                generatedLevel = levels.levelGenerator(seed,numberTube, numberEmptyTube, numberInitLayers, numberMaxLayers, tubeToWin, maxLevelColor, generatorVersion);
                return generatedLevel;

            case 110:
                
                numberTube = 5;
                numberEmptyTube = 1;
                numberInitLayers = 4;
                numberMaxLayers = 4;
                tubeToWin = 4;
                maxLevelColor = 4;
                seed = 0;
                generatorVersion = 1;

                setupObject.initLevelParameters(numberTube, numberEmptyTube, numberInitLayers, numberMaxLayers, tubeToWin);
                generatedLevel = levels.levelGenerator(seed,numberTube, numberEmptyTube, numberInitLayers, numberMaxLayers, tubeToWin, maxLevelColor, generatorVersion);
                return generatedLevel;

            case 111:
                
                numberTube = 5;
                numberEmptyTube = 1;
                numberInitLayers = 4;
                numberMaxLayers = 4;
                tubeToWin = 4;
                maxLevelColor = 4;
                seed = 0;
                generatorVersion = 1;

                setupObject.initLevelParameters(numberTube, numberEmptyTube, numberInitLayers, numberMaxLayers, tubeToWin);
                generatedLevel = levels.levelGenerator(seed,numberTube, numberEmptyTube, numberInitLayers, numberMaxLayers, tubeToWin, maxLevelColor, generatorVersion);
                return generatedLevel;

            case 112:
                
                numberTube = 5;
                numberEmptyTube = 1;
                numberInitLayers = 4;
                numberMaxLayers = 4;
                tubeToWin = 4;
                maxLevelColor = 4;
                seed = 0;
                generatorVersion = 1;

                setupObject.initLevelParameters(numberTube, numberEmptyTube, numberInitLayers, numberMaxLayers, tubeToWin);
                generatedLevel = levels.levelGenerator(seed,numberTube, numberEmptyTube, numberInitLayers, numberMaxLayers, tubeToWin, maxLevelColor, generatorVersion);
                return generatedLevel;

            case 113:
                
                numberTube = 5;
                numberEmptyTube = 1;
                numberInitLayers = 4;
                numberMaxLayers = 4;
                tubeToWin = 4;
                maxLevelColor = 4;
                seed = 0;
                generatorVersion = 1;

                setupObject.initLevelParameters(numberTube, numberEmptyTube, numberInitLayers, numberMaxLayers, tubeToWin);
                generatedLevel = levels.levelGenerator(seed,numberTube, numberEmptyTube, numberInitLayers, numberMaxLayers, tubeToWin, maxLevelColor, generatorVersion);
                return generatedLevel;

            case 114:
                
                numberTube = 5;
                numberEmptyTube = 1;
                numberInitLayers = 4;
                numberMaxLayers = 4;
                tubeToWin = 4;
                maxLevelColor = 4;
                seed = 0;
                generatorVersion = 1;

                setupObject.initLevelParameters(numberTube, numberEmptyTube, numberInitLayers, numberMaxLayers, tubeToWin);
                generatedLevel = levels.levelGenerator(seed,numberTube, numberEmptyTube, numberInitLayers, numberMaxLayers, tubeToWin, maxLevelColor, generatorVersion);
                return generatedLevel;

            case 115:
                
                numberTube = 5;
                numberEmptyTube = 1;
                numberInitLayers = 4;
                numberMaxLayers = 4;
                tubeToWin = 4;
                maxLevelColor = 4;
                seed = 0;
                generatorVersion = 1;

                setupObject.initLevelParameters(numberTube, numberEmptyTube, numberInitLayers, numberMaxLayers, tubeToWin);
                generatedLevel = levels.levelGenerator(seed,numberTube, numberEmptyTube, numberInitLayers, numberMaxLayers, tubeToWin, maxLevelColor, generatorVersion);
                return generatedLevel;

            case 116:
                
                numberTube = 5;
                numberEmptyTube = 1;
                numberInitLayers = 4;
                numberMaxLayers = 4;
                tubeToWin = 4;
                maxLevelColor = 4;
                seed = 0;
                generatorVersion = 1;

                setupObject.initLevelParameters(numberTube, numberEmptyTube, numberInitLayers, numberMaxLayers, tubeToWin);
                generatedLevel = levels.levelGenerator(seed,numberTube, numberEmptyTube, numberInitLayers, numberMaxLayers, tubeToWin, maxLevelColor, generatorVersion);
                return generatedLevel;

            case 117:
                
                numberTube = 5;
                numberEmptyTube = 1;
                numberInitLayers = 4;
                numberMaxLayers = 4;
                tubeToWin = 4;
                maxLevelColor = 4;
                seed = 0;
                generatorVersion = 1;

                setupObject.initLevelParameters(numberTube, numberEmptyTube, numberInitLayers, numberMaxLayers, tubeToWin);
                generatedLevel = levels.levelGenerator(seed,numberTube, numberEmptyTube, numberInitLayers, numberMaxLayers, tubeToWin, maxLevelColor, generatorVersion);
                return generatedLevel;

            case 118:
                
                numberTube = 5;
                numberEmptyTube = 1;
                numberInitLayers = 4;
                numberMaxLayers = 4;
                tubeToWin = 4;
                maxLevelColor = 4;
                seed = 0;
                generatorVersion = 1;

                setupObject.initLevelParameters(numberTube, numberEmptyTube, numberInitLayers, numberMaxLayers, tubeToWin);
                generatedLevel = levels.levelGenerator(seed,numberTube, numberEmptyTube, numberInitLayers, numberMaxLayers, tubeToWin, maxLevelColor, generatorVersion);
                return generatedLevel;

            case 119:
                
                numberTube = 5;
                numberEmptyTube = 1;
                numberInitLayers = 4;
                numberMaxLayers = 4;
                tubeToWin = 4;
                maxLevelColor = 4;
                seed = 0;
                generatorVersion = 1;

                setupObject.initLevelParameters(numberTube, numberEmptyTube, numberInitLayers, numberMaxLayers, tubeToWin);
                generatedLevel = levels.levelGenerator(seed,numberTube, numberEmptyTube, numberInitLayers, numberMaxLayers, tubeToWin, maxLevelColor, generatorVersion);
                return generatedLevel;

            case 120:
                
                numberTube = 5;
                numberEmptyTube = 1;
                numberInitLayers = 4;
                numberMaxLayers = 4;
                tubeToWin = 4;
                maxLevelColor = 4;
                seed = 0;
                generatorVersion = 1;

                setupObject.initLevelParameters(numberTube, numberEmptyTube, numberInitLayers, numberMaxLayers, tubeToWin);
                generatedLevel = levels.levelGenerator(seed,numberTube, numberEmptyTube, numberInitLayers, numberMaxLayers, tubeToWin, maxLevelColor, generatorVersion);
                return generatedLevel;

            case 121:
                
                numberTube = 5;
                numberEmptyTube = 1;
                numberInitLayers = 4;
                numberMaxLayers = 4;
                tubeToWin = 4;
                maxLevelColor = 4;
                seed = 0;
                generatorVersion = 1;

                setupObject.initLevelParameters(numberTube, numberEmptyTube, numberInitLayers, numberMaxLayers, tubeToWin);
                generatedLevel = levels.levelGenerator(seed,numberTube, numberEmptyTube, numberInitLayers, numberMaxLayers, tubeToWin, maxLevelColor, generatorVersion);
                return generatedLevel;

            case 122:
                
                numberTube = 5;
                numberEmptyTube = 1;
                numberInitLayers = 4;
                numberMaxLayers = 4;
                tubeToWin = 4;
                maxLevelColor = 4;
                seed = 0;
                generatorVersion = 1;

                setupObject.initLevelParameters(numberTube, numberEmptyTube, numberInitLayers, numberMaxLayers, tubeToWin);
                generatedLevel = levels.levelGenerator(seed,numberTube, numberEmptyTube, numberInitLayers, numberMaxLayers, tubeToWin, maxLevelColor, generatorVersion);
                return generatedLevel;

            case 123:
                
                numberTube = 5;
                numberEmptyTube = 1;
                numberInitLayers = 4;
                numberMaxLayers = 4;
                tubeToWin = 4;
                maxLevelColor = 4;
                seed = 0;
                generatorVersion = 1;

                setupObject.initLevelParameters(numberTube, numberEmptyTube, numberInitLayers, numberMaxLayers, tubeToWin);
                generatedLevel = levels.levelGenerator(seed,numberTube, numberEmptyTube, numberInitLayers, numberMaxLayers, tubeToWin, maxLevelColor, generatorVersion);
                return generatedLevel;

            case 124:
                
                numberTube = 5;
                numberEmptyTube = 1;
                numberInitLayers = 4;
                numberMaxLayers = 4;
                tubeToWin = 4;
                maxLevelColor = 4;
                seed = 0;
                generatorVersion = 1;

                setupObject.initLevelParameters(numberTube, numberEmptyTube, numberInitLayers, numberMaxLayers, tubeToWin);
                generatedLevel = levels.levelGenerator(seed,numberTube, numberEmptyTube, numberInitLayers, numberMaxLayers, tubeToWin, maxLevelColor, generatorVersion);
                return generatedLevel;

            case 125:
                
                numberTube = 5;
                numberEmptyTube = 1;
                numberInitLayers = 4;
                numberMaxLayers = 4;
                tubeToWin = 4;
                maxLevelColor = 4;
                seed = 0;
                generatorVersion = 1;

                setupObject.initLevelParameters(numberTube, numberEmptyTube, numberInitLayers, numberMaxLayers, tubeToWin);
                generatedLevel = levels.levelGenerator(seed,numberTube, numberEmptyTube, numberInitLayers, numberMaxLayers, tubeToWin, maxLevelColor, generatorVersion);
                return generatedLevel;

            case 126:
                
                numberTube = 5;
                numberEmptyTube = 1;
                numberInitLayers = 4;
                numberMaxLayers = 4;
                tubeToWin = 4;
                maxLevelColor = 4;
                seed = 0;
                generatorVersion = 1;

                setupObject.initLevelParameters(numberTube, numberEmptyTube, numberInitLayers, numberMaxLayers, tubeToWin);
                generatedLevel = levels.levelGenerator(seed,numberTube, numberEmptyTube, numberInitLayers, numberMaxLayers, tubeToWin, maxLevelColor, generatorVersion);
                return generatedLevel;

            case 127:
                
                numberTube = 5;
                numberEmptyTube = 1;
                numberInitLayers = 4;
                numberMaxLayers = 4;
                tubeToWin = 4;
                maxLevelColor = 4;
                seed = 0;
                generatorVersion = 1;

                setupObject.initLevelParameters(numberTube, numberEmptyTube, numberInitLayers, numberMaxLayers, tubeToWin);
                generatedLevel = levels.levelGenerator(seed,numberTube, numberEmptyTube, numberInitLayers, numberMaxLayers, tubeToWin, maxLevelColor, generatorVersion);
                return generatedLevel;

            case 128:
                
                numberTube = 5;
                numberEmptyTube = 1;
                numberInitLayers = 4;
                numberMaxLayers = 4;
                tubeToWin = 4;
                maxLevelColor = 4;
                seed = 0;
                generatorVersion = 1;

                setupObject.initLevelParameters(numberTube, numberEmptyTube, numberInitLayers, numberMaxLayers, tubeToWin);
                generatedLevel = levels.levelGenerator(seed,numberTube, numberEmptyTube, numberInitLayers, numberMaxLayers, tubeToWin, maxLevelColor, generatorVersion);
                return generatedLevel;

            case 129:
                
                numberTube = 5;
                numberEmptyTube = 1;
                numberInitLayers = 4;
                numberMaxLayers = 4;
                tubeToWin = 4;
                maxLevelColor = 4;
                seed = 0;
                generatorVersion = 1;

                setupObject.initLevelParameters(numberTube, numberEmptyTube, numberInitLayers, numberMaxLayers, tubeToWin);
                generatedLevel = levels.levelGenerator(seed,numberTube, numberEmptyTube, numberInitLayers, numberMaxLayers, tubeToWin, maxLevelColor, generatorVersion);
                return generatedLevel;

            case 130:
                
                numberTube = 5;
                numberEmptyTube = 1;
                numberInitLayers = 4;
                numberMaxLayers = 4;
                tubeToWin = 4;
                maxLevelColor = 4;
                seed = 0;
                generatorVersion = 1;

                setupObject.initLevelParameters(numberTube, numberEmptyTube, numberInitLayers, numberMaxLayers, tubeToWin);
                generatedLevel = levels.levelGenerator(seed,numberTube, numberEmptyTube, numberInitLayers, numberMaxLayers, tubeToWin, maxLevelColor, generatorVersion);
                return generatedLevel;

            case 131:
                
                numberTube = 5;
                numberEmptyTube = 1;
                numberInitLayers = 4;
                numberMaxLayers = 4;
                tubeToWin = 4;
                maxLevelColor = 4;
                seed = 0;
                generatorVersion = 1;

                setupObject.initLevelParameters(numberTube, numberEmptyTube, numberInitLayers, numberMaxLayers, tubeToWin);
                generatedLevel = levels.levelGenerator(seed,numberTube, numberEmptyTube, numberInitLayers, numberMaxLayers, tubeToWin, maxLevelColor, generatorVersion);
                return generatedLevel;

            case 132:
                
                numberTube = 5;
                numberEmptyTube = 1;
                numberInitLayers = 4;
                numberMaxLayers = 4;
                tubeToWin = 4;
                maxLevelColor = 4;
                seed = 0;
                generatorVersion = 1;

                setupObject.initLevelParameters(numberTube, numberEmptyTube, numberInitLayers, numberMaxLayers, tubeToWin);
                generatedLevel = levels.levelGenerator(seed,numberTube, numberEmptyTube, numberInitLayers, numberMaxLayers, tubeToWin, maxLevelColor, generatorVersion);
                return generatedLevel;

            case 133:
                
                numberTube = 5;
                numberEmptyTube = 1;
                numberInitLayers = 4;
                numberMaxLayers = 4;
                tubeToWin = 4;
                maxLevelColor = 4;
                seed = 0;
                generatorVersion = 1;

                setupObject.initLevelParameters(numberTube, numberEmptyTube, numberInitLayers, numberMaxLayers, tubeToWin);
                generatedLevel = levels.levelGenerator(seed,numberTube, numberEmptyTube, numberInitLayers, numberMaxLayers, tubeToWin, maxLevelColor, generatorVersion);
                return generatedLevel;

            case 134:
                
                numberTube = 5;
                numberEmptyTube = 1;
                numberInitLayers = 4;
                numberMaxLayers = 4;
                tubeToWin = 4;
                maxLevelColor = 4;
                seed = 0;
                generatorVersion = 1;

                setupObject.initLevelParameters(numberTube, numberEmptyTube, numberInitLayers, numberMaxLayers, tubeToWin);
                generatedLevel = levels.levelGenerator(seed,numberTube, numberEmptyTube, numberInitLayers, numberMaxLayers, tubeToWin, maxLevelColor, generatorVersion);
                return generatedLevel;

            case 135:
                
                numberTube = 5;
                numberEmptyTube = 1;
                numberInitLayers = 4;
                numberMaxLayers = 4;
                tubeToWin = 4;
                maxLevelColor = 4;
                seed = 0;
                generatorVersion = 1;

                setupObject.initLevelParameters(numberTube, numberEmptyTube, numberInitLayers, numberMaxLayers, tubeToWin);
                generatedLevel = levels.levelGenerator(seed,numberTube, numberEmptyTube, numberInitLayers, numberMaxLayers, tubeToWin, maxLevelColor, generatorVersion);
                return generatedLevel;

            case 136:
                
                numberTube = 5;
                numberEmptyTube = 1;
                numberInitLayers = 4;
                numberMaxLayers = 4;
                tubeToWin = 4;
                maxLevelColor = 4;
                seed = 0;
                generatorVersion = 1;

                setupObject.initLevelParameters(numberTube, numberEmptyTube, numberInitLayers, numberMaxLayers, tubeToWin);
                generatedLevel = levels.levelGenerator(seed,numberTube, numberEmptyTube, numberInitLayers, numberMaxLayers, tubeToWin, maxLevelColor, generatorVersion);
                return generatedLevel;

            case 137:
                
                numberTube = 5;
                numberEmptyTube = 1;
                numberInitLayers = 4;
                numberMaxLayers = 4;
                tubeToWin = 4;
                maxLevelColor = 4;
                seed = 0;
                generatorVersion = 1;

                setupObject.initLevelParameters(numberTube, numberEmptyTube, numberInitLayers, numberMaxLayers, tubeToWin);
                generatedLevel = levels.levelGenerator(seed,numberTube, numberEmptyTube, numberInitLayers, numberMaxLayers, tubeToWin, maxLevelColor, generatorVersion);
                return generatedLevel;

            case 138:
                
                numberTube = 5;
                numberEmptyTube = 1;
                numberInitLayers = 4;
                numberMaxLayers = 4;
                tubeToWin = 4;
                maxLevelColor = 4;
                seed = 0;
                generatorVersion = 1;

                setupObject.initLevelParameters(numberTube, numberEmptyTube, numberInitLayers, numberMaxLayers, tubeToWin);
                generatedLevel = levels.levelGenerator(seed,numberTube, numberEmptyTube, numberInitLayers, numberMaxLayers, tubeToWin, maxLevelColor, generatorVersion);
                return generatedLevel;

            case 139:
                
                numberTube = 5;
                numberEmptyTube = 1;
                numberInitLayers = 4;
                numberMaxLayers = 4;
                tubeToWin = 4;
                maxLevelColor = 4;
                seed = 0;
                generatorVersion = 1;

                setupObject.initLevelParameters(numberTube, numberEmptyTube, numberInitLayers, numberMaxLayers, tubeToWin);
                generatedLevel = levels.levelGenerator(seed,numberTube, numberEmptyTube, numberInitLayers, numberMaxLayers, tubeToWin, maxLevelColor, generatorVersion);
                return generatedLevel;

            case 140:
                
                numberTube = 5;
                numberEmptyTube = 1;
                numberInitLayers = 4;
                numberMaxLayers = 4;
                tubeToWin = 4;
                maxLevelColor = 4;
                seed = 0;
                generatorVersion = 1;

                setupObject.initLevelParameters(numberTube, numberEmptyTube, numberInitLayers, numberMaxLayers, tubeToWin);
                generatedLevel = levels.levelGenerator(seed,numberTube, numberEmptyTube, numberInitLayers, numberMaxLayers, tubeToWin, maxLevelColor, generatorVersion);
                return generatedLevel;

            case 141:
                
                numberTube = 5;
                numberEmptyTube = 1;
                numberInitLayers = 4;
                numberMaxLayers = 4;
                tubeToWin = 4;
                maxLevelColor = 4;
                seed = 0;
                generatorVersion = 1;

                setupObject.initLevelParameters(numberTube, numberEmptyTube, numberInitLayers, numberMaxLayers, tubeToWin);
                generatedLevel = levels.levelGenerator(seed,numberTube, numberEmptyTube, numberInitLayers, numberMaxLayers, tubeToWin, maxLevelColor, generatorVersion);
                return generatedLevel;

            case 142:
                
                numberTube = 5;
                numberEmptyTube = 1;
                numberInitLayers = 4;
                numberMaxLayers = 4;
                tubeToWin = 4;
                maxLevelColor = 4;
                seed = 0;
                generatorVersion = 1;

                setupObject.initLevelParameters(numberTube, numberEmptyTube, numberInitLayers, numberMaxLayers, tubeToWin);
                generatedLevel = levels.levelGenerator(seed,numberTube, numberEmptyTube, numberInitLayers, numberMaxLayers, tubeToWin, maxLevelColor, generatorVersion);
                return generatedLevel;

            case 143:
                
                numberTube = 5;
                numberEmptyTube = 1;
                numberInitLayers = 4;
                numberMaxLayers = 4;
                tubeToWin = 4;
                maxLevelColor = 4;
                seed = 0;
                generatorVersion = 1;

                setupObject.initLevelParameters(numberTube, numberEmptyTube, numberInitLayers, numberMaxLayers, tubeToWin);
                generatedLevel = levels.levelGenerator(seed,numberTube, numberEmptyTube, numberInitLayers, numberMaxLayers, tubeToWin, maxLevelColor, generatorVersion);
                return generatedLevel;

            case 144:
                
                numberTube = 5;
                numberEmptyTube = 1;
                numberInitLayers = 4;
                numberMaxLayers = 4;
                tubeToWin = 4;
                maxLevelColor = 4;
                seed = 0;
                generatorVersion = 1;

                setupObject.initLevelParameters(numberTube, numberEmptyTube, numberInitLayers, numberMaxLayers, tubeToWin);
                generatedLevel = levels.levelGenerator(seed,numberTube, numberEmptyTube, numberInitLayers, numberMaxLayers, tubeToWin, maxLevelColor, generatorVersion);
                return generatedLevel;

            case 145:
                
                numberTube = 5;
                numberEmptyTube = 1;
                numberInitLayers = 4;
                numberMaxLayers = 4;
                tubeToWin = 4;
                maxLevelColor = 4;
                seed = 0;
                generatorVersion = 1;

                setupObject.initLevelParameters(numberTube, numberEmptyTube, numberInitLayers, numberMaxLayers, tubeToWin);
                generatedLevel = levels.levelGenerator(seed,numberTube, numberEmptyTube, numberInitLayers, numberMaxLayers, tubeToWin, maxLevelColor, generatorVersion);
                return generatedLevel;

            case 146:
                
                numberTube = 5;
                numberEmptyTube = 1;
                numberInitLayers = 4;
                numberMaxLayers = 4;
                tubeToWin = 4;
                maxLevelColor = 4;
                seed = 0;
                generatorVersion = 1;

                setupObject.initLevelParameters(numberTube, numberEmptyTube, numberInitLayers, numberMaxLayers, tubeToWin);
                generatedLevel = levels.levelGenerator(seed,numberTube, numberEmptyTube, numberInitLayers, numberMaxLayers, tubeToWin, maxLevelColor, generatorVersion);
                return generatedLevel;

            case 147:
                
                numberTube = 5;
                numberEmptyTube = 1;
                numberInitLayers = 4;
                numberMaxLayers = 4;
                tubeToWin = 4;
                maxLevelColor = 4;
                seed = 0;
                generatorVersion = 1;

                setupObject.initLevelParameters(numberTube, numberEmptyTube, numberInitLayers, numberMaxLayers, tubeToWin);
                generatedLevel = levels.levelGenerator(seed,numberTube, numberEmptyTube, numberInitLayers, numberMaxLayers, tubeToWin, maxLevelColor, generatorVersion);
                return generatedLevel;

            case 148:
                
                numberTube = 5;
                numberEmptyTube = 1;
                numberInitLayers = 4;
                numberMaxLayers = 4;
                tubeToWin = 4;
                maxLevelColor = 4;
                seed = 0;
                generatorVersion = 1;

                setupObject.initLevelParameters(numberTube, numberEmptyTube, numberInitLayers, numberMaxLayers, tubeToWin);
                generatedLevel = levels.levelGenerator(seed,numberTube, numberEmptyTube, numberInitLayers, numberMaxLayers, tubeToWin, maxLevelColor, generatorVersion);
                return generatedLevel;

            case 149:
                
                numberTube = 5;
                numberEmptyTube = 1;
                numberInitLayers = 4;
                numberMaxLayers = 4;
                tubeToWin = 4;
                maxLevelColor = 4;
                seed = 0;
                generatorVersion = 1;

                setupObject.initLevelParameters(numberTube, numberEmptyTube, numberInitLayers, numberMaxLayers, tubeToWin);
                generatedLevel = levels.levelGenerator(seed,numberTube, numberEmptyTube, numberInitLayers, numberMaxLayers, tubeToWin, maxLevelColor, generatorVersion);
                return generatedLevel;

            case 150:
                
                numberTube = 5;
                numberEmptyTube = 1;
                numberInitLayers = 4;
                numberMaxLayers = 4;
                tubeToWin = 4;
                maxLevelColor = 4;
                seed = 0;
                generatorVersion = 1;

                setupObject.initLevelParameters(numberTube, numberEmptyTube, numberInitLayers, numberMaxLayers, tubeToWin);
                generatedLevel = levels.levelGenerator(seed,numberTube, numberEmptyTube, numberInitLayers, numberMaxLayers, tubeToWin, maxLevelColor, generatorVersion);
                return generatedLevel;

            default: 
                setupObject.initLevelParameters(3, 1, 3, 3, 2);
                generatedLevel = levels.levelGenerator(-1635485455,3,1,3,3,2,2,1);
                return generatedLevel;
                    }

                }

}
