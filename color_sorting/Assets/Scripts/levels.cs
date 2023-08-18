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
                numberTube = 4;
                numberEmptyTube = 0;
                numberInitLayers = 3;
                numberMaxLayers = 4;
                tubeToWin = 3;
                maxLevelColor = 3;
                seed = -105213848;
                generatorVersion = 2;
                rewindPercentage = 0.4f;

                setupObject.initLevelParameters(numberTube, numberEmptyTube, numberInitLayers, numberMaxLayers, tubeToWin);
                generatedLevel = levels.levelGenerator(seed,numberTube, numberEmptyTube, numberInitLayers, numberMaxLayers, tubeToWin, maxLevelColor, generatorVersion, rewindPercentage);
                return generatedLevel;

            case 2:
                numberTube = 5;
                numberEmptyTube = 1;
                numberInitLayers = 5;
                numberMaxLayers = 10;
                tubeToWin = 2;
                maxLevelColor = 2;
                generatorVersion= 1;
                seed = 1132154279;

                setupObject.initLevelParameters(numberTube, numberEmptyTube, numberInitLayers, numberMaxLayers, tubeToWin);
                generatedLevel = levels.levelGenerator(seed,numberTube, numberEmptyTube, numberInitLayers, numberMaxLayers, tubeToWin, maxLevelColor, generatorVersion);
                return generatedLevel;

            case 3:
                numberTube = 5;
                numberEmptyTube = 1;
                numberInitLayers = 3;
                numberMaxLayers = 6;
                tubeToWin = 2;
                maxLevelColor = 2;
                generatorVersion= 1;
                seed = 2005539349;

                setupObject.initLevelParameters(numberTube, numberEmptyTube, numberInitLayers, numberMaxLayers, tubeToWin);
                generatedLevel = levels.levelGenerator(seed,numberTube, numberEmptyTube, numberInitLayers, numberMaxLayers, tubeToWin, maxLevelColor, generatorVersion);
                return generatedLevel;

            case 4:
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

            case 5:
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

            case 6:
                numberTube = 5;
                numberEmptyTube = 0;
                numberInitLayers = 4;
                numberMaxLayers = 10;
                tubeToWin = 2;
                maxLevelColor = 2;
                generatorVersion= 1;
                seed = 492865492;

                setupObject.initLevelParameters(numberTube, numberEmptyTube, numberInitLayers, numberMaxLayers, tubeToWin);
                generatedLevel = levels.levelGenerator(seed,numberTube, numberEmptyTube, numberInitLayers, numberMaxLayers, tubeToWin, maxLevelColor, generatorVersion);
                return generatedLevel;

            case 7:
                numberTube = 5;
                numberEmptyTube = 1;
                numberInitLayers = 5;
                numberMaxLayers = 5;
                tubeToWin = 4;
                maxLevelColor = 4;
                seed = 960622186;
                generatorVersion = 1;

                setupObject.initLevelParameters(numberTube, numberEmptyTube, numberInitLayers, numberMaxLayers, tubeToWin);
                generatedLevel = levels.levelGenerator(seed,numberTube, numberEmptyTube, numberInitLayers, numberMaxLayers, tubeToWin, maxLevelColor, generatorVersion);
                return generatedLevel;

            case 8:
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

            case 9:
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

            case 10:
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

            case 11:
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

            case 12:
                numberTube = 6;
                numberEmptyTube = 0;
                numberInitLayers = 3;
                numberMaxLayers = 9;
                tubeToWin = 2;
                maxLevelColor = 2;
                generatorVersion= 1;
                seed = 223410078;

                setupObject.initLevelParameters(numberTube, numberEmptyTube, numberInitLayers, numberMaxLayers, tubeToWin);
                generatedLevel = levels.levelGenerator(seed,numberTube, numberEmptyTube, numberInitLayers, numberMaxLayers, tubeToWin, maxLevelColor, generatorVersion);
                return generatedLevel;
                

            case 13:
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

            case 14:
                numberTube = 6;
                numberEmptyTube = 2;
                numberInitLayers = 5;
                numberMaxLayers = 5;
                tubeToWin = 4;
                maxLevelColor = 4;
                seed = 1970090662;
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
                seed = -2016101703;
                generatorVersion = 1;

                setupObject.initLevelParameters(numberTube, numberEmptyTube, numberInitLayers, numberMaxLayers, tubeToWin);
                generatedLevel = levels.levelGenerator(seed,numberTube, numberEmptyTube, numberInitLayers, numberMaxLayers, tubeToWin, maxLevelColor, generatorVersion);
                return generatedLevel;

            case 16:
                numberTube = 6;
                numberEmptyTube = 2;
                numberInitLayers = 7;
                numberMaxLayers = 7;
                tubeToWin = 4;
                maxLevelColor = 4;
                seed = 5;
                generatorVersion = 1;

                setupObject.initLevelParameters(numberTube, numberEmptyTube, numberInitLayers, numberMaxLayers, tubeToWin);
                generatedLevel = levels.levelGenerator(seed,numberTube, numberEmptyTube, numberInitLayers, numberMaxLayers, tubeToWin, maxLevelColor, generatorVersion);
                return generatedLevel;

            case 17:
                numberTube = 6;
                numberEmptyTube = 2;
                numberInitLayers = 8;
                numberMaxLayers = 8;
                tubeToWin = 4;
                maxLevelColor = 4;
                seed = 10;
                generatorVersion = 1;

                setupObject.initLevelParameters(numberTube, numberEmptyTube, numberInitLayers, numberMaxLayers, tubeToWin);
                generatedLevel = levels.levelGenerator(seed,numberTube, numberEmptyTube, numberInitLayers, numberMaxLayers, tubeToWin, maxLevelColor, generatorVersion);
                return generatedLevel;

            case 18:
                numberTube = 6;
                numberEmptyTube = 2;
                numberInitLayers = 8;
                numberMaxLayers = 8;
                tubeToWin = 4;
                maxLevelColor = 4;
                seed = 15;
                generatorVersion = 1;

                setupObject.initLevelParameters(numberTube, numberEmptyTube, numberInitLayers, numberMaxLayers, tubeToWin);
                generatedLevel = levels.levelGenerator(seed,numberTube, numberEmptyTube, numberInitLayers, numberMaxLayers, tubeToWin, maxLevelColor, generatorVersion);
                return generatedLevel;

            case 19:
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

            case 20:
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

            case 21:
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

            case 22:
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

            case 23:
                numberTube = 6;
                numberEmptyTube = 1;
                numberInitLayers = 7;
                numberMaxLayers = 7;
                tubeToWin = 5;
                maxLevelColor = 5;
                seed = -1644799438;
                generatorVersion = 1;

                setupObject.initLevelParameters(numberTube, numberEmptyTube, numberInitLayers, numberMaxLayers, tubeToWin);
                generatedLevel = levels.levelGenerator(seed,numberTube, numberEmptyTube, numberInitLayers, numberMaxLayers, tubeToWin, maxLevelColor, generatorVersion);
                return generatedLevel;

            case 24:
                numberTube = 6;
                numberEmptyTube = 0;
                numberInitLayers = 4;
                numberMaxLayers = 6;
                tubeToWin = 4;
                maxLevelColor = 4;
                generatorVersion= 1;
                seed = 803499897;

                setupObject.initLevelParameters(numberTube, numberEmptyTube, numberInitLayers, numberMaxLayers, tubeToWin);
                generatedLevel = levels.levelGenerator(seed,numberTube, numberEmptyTube, numberInitLayers, numberMaxLayers, tubeToWin, maxLevelColor, generatorVersion);
                return generatedLevel;

            case 25:
                numberTube = 6;
                numberEmptyTube = 0;
                numberInitLayers = 4;
                numberMaxLayers = 6;
                tubeToWin = 4;
                maxLevelColor = 4;
                generatorVersion= 1;
                seed = -893899191;

                setupObject.initLevelParameters(numberTube, numberEmptyTube, numberInitLayers, numberMaxLayers, tubeToWin);
                generatedLevel = levels.levelGenerator(seed,numberTube, numberEmptyTube, numberInitLayers, numberMaxLayers, tubeToWin, maxLevelColor, generatorVersion);
                return generatedLevel;

            case 26:
                numberTube = 6;
                numberEmptyTube = 1;
                numberInitLayers = 8;
                numberMaxLayers = 10;
                tubeToWin = 4;
                maxLevelColor = 4;
                generatorVersion= 1;
                seed = 1483886993;

                setupObject.initLevelParameters(numberTube, numberEmptyTube, numberInitLayers, numberMaxLayers, tubeToWin);
                generatedLevel = levels.levelGenerator(seed,numberTube, numberEmptyTube, numberInitLayers, numberMaxLayers, tubeToWin, maxLevelColor, generatorVersion);
                return generatedLevel;

            case 27:
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

            case 28:
                numberTube = 6;
                numberEmptyTube = 1;
                numberInitLayers = 8;
                numberMaxLayers = 8;
                tubeToWin = 5;
                maxLevelColor = 5;
                seed = 574847686;
                generatorVersion = 2;
                rewindPercentage = 0.40f;

                setupObject.initLevelParameters(numberTube, numberEmptyTube, numberInitLayers, numberMaxLayers, tubeToWin);
                generatedLevel = levels.levelGenerator(seed,numberTube, numberEmptyTube, numberInitLayers, numberMaxLayers, tubeToWin, maxLevelColor, generatorVersion, rewindPercentage);
                return generatedLevel;

            case 29:
                numberTube = 6;
                numberEmptyTube = 1;
                numberInitLayers = 8;
                numberMaxLayers = 10;
                tubeToWin = 4;
                maxLevelColor = 4;
                generatorVersion= 1;
                seed = -982775495;

                setupObject.initLevelParameters(numberTube, numberEmptyTube, numberInitLayers, numberMaxLayers, tubeToWin);
                generatedLevel = levels.levelGenerator(seed,numberTube, numberEmptyTube, numberInitLayers, numberMaxLayers, tubeToWin, maxLevelColor, generatorVersion);
                return generatedLevel;

            case 30:
                numberTube = 6;
                numberEmptyTube = 1;
                numberInitLayers = 8;
                numberMaxLayers = 10;
                tubeToWin = 4;
                maxLevelColor = 4;
                generatorVersion= 1;
                seed = 1273849643;

                setupObject.initLevelParameters(numberTube, numberEmptyTube, numberInitLayers, numberMaxLayers, tubeToWin);
                generatedLevel = levels.levelGenerator(seed,numberTube, numberEmptyTube, numberInitLayers, numberMaxLayers, tubeToWin, maxLevelColor, generatorVersion);
                return generatedLevel;

            case 31:
                numberTube = 6;
                numberEmptyTube = 0;
                numberInitLayers = 6;
                numberMaxLayers = 9;
                tubeToWin = 4;
                maxLevelColor = 4;
                generatorVersion= 1;
                seed = 631927984;

                setupObject.initLevelParameters(numberTube, numberEmptyTube, numberInitLayers, numberMaxLayers, tubeToWin);
                generatedLevel = levels.levelGenerator(seed,numberTube, numberEmptyTube, numberInitLayers, numberMaxLayers, tubeToWin, maxLevelColor, generatorVersion);
                return generatedLevel;

            case 32:
                numberTube = 6;
                numberEmptyTube = 0;
                numberInitLayers = 6;
                numberMaxLayers = 9;
                tubeToWin = 4;
                maxLevelColor = 4;
                generatorVersion= 1;
                seed = 1382416453;

                setupObject.initLevelParameters(numberTube, numberEmptyTube, numberInitLayers, numberMaxLayers, tubeToWin);
                generatedLevel = levels.levelGenerator(seed,numberTube, numberEmptyTube, numberInitLayers, numberMaxLayers, tubeToWin, maxLevelColor, generatorVersion);
                return generatedLevel;

            case 33:
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

            case 34:
                numberTube = 7;
                numberEmptyTube = 2;
                numberInitLayers = 5;
                numberMaxLayers = 5;
                tubeToWin = 5;
                maxLevelColor = 5;
                seed = 1746708291;
                generatorVersion = 1;

                setupObject.initLevelParameters(numberTube, numberEmptyTube, numberInitLayers, numberMaxLayers, tubeToWin);
                generatedLevel = levels.levelGenerator(seed,numberTube, numberEmptyTube, numberInitLayers, numberMaxLayers, tubeToWin, maxLevelColor, generatorVersion);
                return generatedLevel;

            case 35:
                numberTube = 7;
                numberEmptyTube = 2;
                numberInitLayers = 6;
                numberMaxLayers = 6;
                tubeToWin = 5;
                maxLevelColor = 5;
                seed = -1161113695;
                generatorVersion = 1;

                setupObject.initLevelParameters(numberTube, numberEmptyTube, numberInitLayers, numberMaxLayers, tubeToWin);
                generatedLevel = levels.levelGenerator(seed,numberTube, numberEmptyTube, numberInitLayers, numberMaxLayers, tubeToWin, maxLevelColor, generatorVersion);
                return generatedLevel;

            case 36:
                numberTube = 7;
                numberEmptyTube = 2;
                numberInitLayers = 7;
                numberMaxLayers = 7;
                tubeToWin = 5;
                maxLevelColor = 5;
                seed = 1598076211;
                generatorVersion = 1;

                setupObject.initLevelParameters(numberTube, numberEmptyTube, numberInitLayers, numberMaxLayers, tubeToWin);
                generatedLevel = levels.levelGenerator(seed,numberTube, numberEmptyTube, numberInitLayers, numberMaxLayers, tubeToWin, maxLevelColor, generatorVersion);
                return generatedLevel;

            case 37:
                numberTube = 7;
                numberEmptyTube = 1;
                numberInitLayers = 4;
                numberMaxLayers = 6;
                tubeToWin = 4;
                maxLevelColor = 4;
                generatorVersion= 1;
                seed = -984744563;

                setupObject.initLevelParameters(numberTube, numberEmptyTube, numberInitLayers, numberMaxLayers, tubeToWin);
                generatedLevel = levels.levelGenerator(seed,numberTube, numberEmptyTube, numberInitLayers, numberMaxLayers, tubeToWin, maxLevelColor, generatorVersion);
                return generatedLevel;

            case 38:
                numberTube = 7;
                numberEmptyTube = 2;
                numberInitLayers = 8;
                numberMaxLayers = 8;
                tubeToWin = 5;
                maxLevelColor = 5;
                seed = -1717718467;
                generatorVersion = 1;

                setupObject.initLevelParameters(numberTube, numberEmptyTube, numberInitLayers, numberMaxLayers, tubeToWin);
                generatedLevel = levels.levelGenerator(seed,numberTube, numberEmptyTube, numberInitLayers, numberMaxLayers, tubeToWin, maxLevelColor, generatorVersion);
                return generatedLevel;

            case 39:
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

            case 40:
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

            case 41:
                numberTube = 7;
                numberEmptyTube = 1;
                numberInitLayers = 6;
                numberMaxLayers = 9;
                tubeToWin = 4;
                maxLevelColor = 4;
                generatorVersion= 1;
                seed = -1340694164;

                setupObject.initLevelParameters(numberTube, numberEmptyTube, numberInitLayers, numberMaxLayers, tubeToWin);
                generatedLevel = levels.levelGenerator(seed,numberTube, numberEmptyTube, numberInitLayers, numberMaxLayers, tubeToWin, maxLevelColor, generatorVersion);
                return generatedLevel;

            case 42:
                numberTube = 7;
                numberEmptyTube = 1;
                numberInitLayers = 6;
                numberMaxLayers = 9;
                tubeToWin = 4;
                maxLevelColor = 4;
                generatorVersion= 1;
                seed = -729924699;

                setupObject.initLevelParameters(numberTube, numberEmptyTube, numberInitLayers, numberMaxLayers, tubeToWin);
                generatedLevel = levels.levelGenerator(seed,numberTube, numberEmptyTube, numberInitLayers, numberMaxLayers, tubeToWin, maxLevelColor, generatorVersion);
                return generatedLevel;

            case 43:
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

            case 44:
                numberTube = 7;
                numberEmptyTube = 1;
                numberInitLayers = 6;
                numberMaxLayers = 6;
                tubeToWin = 6;
                maxLevelColor = 6;
                seed = 733736003;
                generatorVersion = 1;

                setupObject.initLevelParameters(numberTube, numberEmptyTube, numberInitLayers, numberMaxLayers, tubeToWin);
                generatedLevel = levels.levelGenerator(seed,numberTube, numberEmptyTube, numberInitLayers, numberMaxLayers, tubeToWin, maxLevelColor, generatorVersion);
                return generatedLevel;

            case 45:
                numberTube = 7;
                numberEmptyTube = 1;
                numberInitLayers = 7;
                numberMaxLayers = 7;
                tubeToWin = 6;
                maxLevelColor = 6;
                seed = 1425987826;
                generatorVersion = 1;

                setupObject.initLevelParameters(numberTube, numberEmptyTube, numberInitLayers, numberMaxLayers, tubeToWin);
                generatedLevel = levels.levelGenerator(seed,numberTube, numberEmptyTube, numberInitLayers, numberMaxLayers, tubeToWin, maxLevelColor, generatorVersion);
                return generatedLevel;

            case 46:
                numberTube = 7;
                numberEmptyTube = 1;
                numberInitLayers = 7;
                numberMaxLayers = 7;
                tubeToWin = 6;
                maxLevelColor = 6;
                seed = -1064459665;
                generatorVersion = 1;

                setupObject.initLevelParameters(numberTube, numberEmptyTube, numberInitLayers, numberMaxLayers, tubeToWin);
                generatedLevel = levels.levelGenerator(seed,numberTube, numberEmptyTube, numberInitLayers, numberMaxLayers, tubeToWin, maxLevelColor, generatorVersion);
                return generatedLevel;

            case 47:
                numberTube = 7;
                numberEmptyTube = 1;
                numberInitLayers = 8;
                numberMaxLayers = 8;
                tubeToWin = 6;
                maxLevelColor = 6;
                seed = -954521313;
                generatorVersion = 2;
                rewindPercentage = 0.40f;

                setupObject.initLevelParameters(numberTube, numberEmptyTube, numberInitLayers, numberMaxLayers, tubeToWin);
                generatedLevel = levels.levelGenerator(seed,numberTube, numberEmptyTube, numberInitLayers, numberMaxLayers, tubeToWin, maxLevelColor, generatorVersion, rewindPercentage);
                return generatedLevel;

            case 48:
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

            case 49:
                numberTube = 7;
                numberEmptyTube = 0;
                numberInitLayers = 4;
                numberMaxLayers = 7;
                tubeToWin = 4;
                maxLevelColor = 4;
                generatorVersion= 1;
                seed = 1482304785;

                setupObject.initLevelParameters(numberTube, numberEmptyTube, numberInitLayers, numberMaxLayers, tubeToWin);
                generatedLevel = levels.levelGenerator(seed,numberTube, numberEmptyTube, numberInitLayers, numberMaxLayers, tubeToWin, maxLevelColor, generatorVersion);
                return generatedLevel;

            case 50:
                numberTube = 7;
                numberEmptyTube = 0;
                numberInitLayers = 4;
                numberMaxLayers = 7;
                tubeToWin = 4;
                maxLevelColor = 4;
                generatorVersion= 1;
                seed = -94809751;

                setupObject.initLevelParameters(numberTube, numberEmptyTube, numberInitLayers, numberMaxLayers, tubeToWin);
                generatedLevel = levels.levelGenerator(seed,numberTube, numberEmptyTube, numberInitLayers, numberMaxLayers, tubeToWin, maxLevelColor, generatorVersion);
                return generatedLevel;

            case 51:
                numberTube = 7;
                numberEmptyTube = 0;
                numberInitLayers = 6;
                numberMaxLayers = 7;
                tubeToWin = 6;
                maxLevelColor = 6;
                generatorVersion= 2;
                seed = 628424006;
                rewindPercentage = 0.40f;

                setupObject.initLevelParameters(numberTube, numberEmptyTube, numberInitLayers, numberMaxLayers, tubeToWin);
                generatedLevel = levels.levelGenerator(seed,numberTube, numberEmptyTube, numberInitLayers, numberMaxLayers, tubeToWin, maxLevelColor, generatorVersion, rewindPercentage);
                return generatedLevel;

            case 52:
                numberTube = 7;
                numberEmptyTube = 0;
                numberInitLayers = 6;
                numberMaxLayers = 7;
                tubeToWin = 6;
                maxLevelColor = 6;
                generatorVersion= 1;
                seed = -601799129;

                setupObject.initLevelParameters(numberTube, numberEmptyTube, numberInitLayers, numberMaxLayers, tubeToWin);
                generatedLevel = levels.levelGenerator(seed,numberTube, numberEmptyTube, numberInitLayers, numberMaxLayers, tubeToWin, maxLevelColor, generatorVersion);
                return generatedLevel;

            case 53:
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

            case 54:
                numberTube = 8;
                numberEmptyTube = 2;
                numberInitLayers = 5;
                numberMaxLayers = 5;
                tubeToWin = 6;
                maxLevelColor = 6;
                seed = 1056744348;
                generatorVersion = 1;

                setupObject.initLevelParameters(numberTube, numberEmptyTube, numberInitLayers, numberMaxLayers, tubeToWin);
                generatedLevel = levels.levelGenerator(seed,numberTube, numberEmptyTube, numberInitLayers, numberMaxLayers, tubeToWin, maxLevelColor, generatorVersion);
                return generatedLevel;

            case 55:
                numberTube = 8;
                numberEmptyTube = 0;
                numberInitLayers = 3;
                numberMaxLayers = 6;
                tubeToWin = 4;
                maxLevelColor = 4;
                generatorVersion= 1;
                seed = 478624601;

                setupObject.initLevelParameters(numberTube, numberEmptyTube, numberInitLayers, numberMaxLayers, tubeToWin);
                generatedLevel = levels.levelGenerator(seed,numberTube, numberEmptyTube, numberInitLayers, numberMaxLayers, tubeToWin, maxLevelColor, generatorVersion);
                return generatedLevel;

            case 56:
                numberTube = 8;
                numberEmptyTube = 2;
                numberInitLayers = 7;
                numberMaxLayers = 7;
                tubeToWin = 6;
                maxLevelColor = 6;
                seed = 1347817683;
                generatorVersion = 1;

                setupObject.initLevelParameters(numberTube, numberEmptyTube, numberInitLayers, numberMaxLayers, tubeToWin);
                generatedLevel = levels.levelGenerator(seed,numberTube, numberEmptyTube, numberInitLayers, numberMaxLayers, tubeToWin, maxLevelColor, generatorVersion);
                return generatedLevel;

            case 57:
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

            case 58:
                numberTube = 8;
                numberEmptyTube = 2;
                numberInitLayers = 6;
                numberMaxLayers = 6;
                tubeToWin = 6;
                maxLevelColor = 6;
                seed = 1578270614;
                generatorVersion = 1;

                setupObject.initLevelParameters(numberTube, numberEmptyTube, numberInitLayers, numberMaxLayers, tubeToWin);
                generatedLevel = levels.levelGenerator(seed,numberTube, numberEmptyTube, numberInitLayers, numberMaxLayers, tubeToWin, maxLevelColor, generatorVersion);
                return generatedLevel;

            case 59:
                numberTube = 8;
                numberEmptyTube = 2;
                numberInitLayers = 8;
                numberMaxLayers = 8;
                tubeToWin = 6;
                maxLevelColor = 6;
                seed = 1718036064;
                generatorVersion = 1;

                setupObject.initLevelParameters(numberTube, numberEmptyTube, numberInitLayers, numberMaxLayers, tubeToWin);
                generatedLevel = levels.levelGenerator(seed,numberTube, numberEmptyTube, numberInitLayers, numberMaxLayers, tubeToWin, maxLevelColor, generatorVersion);
                return generatedLevel;

            case 60:
                numberTube = 8;
                numberEmptyTube = 2;
                numberInitLayers = 8;
                numberMaxLayers = 8;
                tubeToWin = 6;
                maxLevelColor = 6;
                seed = 1988553870;
                generatorVersion = 1;

                setupObject.initLevelParameters(numberTube, numberEmptyTube, numberInitLayers, numberMaxLayers, tubeToWin);
                generatedLevel = levels.levelGenerator(seed,numberTube, numberEmptyTube, numberInitLayers, numberMaxLayers, tubeToWin, maxLevelColor, generatorVersion);
                return generatedLevel;

            case 61:
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

            case 62:
                numberTube = 8;
                numberEmptyTube = 1;
                numberInitLayers = 5;
                numberMaxLayers = 5;
                tubeToWin = 7;
                maxLevelColor = 7;
                seed =-1024034830;
                generatorVersion = 1;

                setupObject.initLevelParameters(numberTube, numberEmptyTube, numberInitLayers, numberMaxLayers, tubeToWin);
                generatedLevel = levels.levelGenerator(seed,numberTube, numberEmptyTube, numberInitLayers, numberMaxLayers, tubeToWin, maxLevelColor, generatorVersion);
                return generatedLevel;

            case 63:
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

            case 64:
                numberTube = 8;
                numberEmptyTube = 1;
                numberInitLayers = 6;
                numberMaxLayers = 6;
                tubeToWin = 7;
                maxLevelColor = 7;
                seed = -110670929;
                generatorVersion = 1;

                setupObject.initLevelParameters(numberTube, numberEmptyTube, numberInitLayers, numberMaxLayers, tubeToWin);
                generatedLevel = levels.levelGenerator(seed,numberTube, numberEmptyTube, numberInitLayers, numberMaxLayers, tubeToWin, maxLevelColor, generatorVersion);
                return generatedLevel;

            case 65:
                numberTube = 8;
                numberEmptyTube = 1;
                numberInitLayers = 4;
                numberMaxLayers = 7;
                tubeToWin = 4;
                maxLevelColor = 4;
                generatorVersion= 1;
                seed = 1409360253;

                setupObject.initLevelParameters(numberTube, numberEmptyTube, numberInitLayers, numberMaxLayers, tubeToWin);
                generatedLevel = levels.levelGenerator(seed,numberTube, numberEmptyTube, numberInitLayers, numberMaxLayers, tubeToWin, maxLevelColor, generatorVersion);
                return generatedLevel;

            case 66:
                numberTube = 8;
                numberEmptyTube = 1;
                numberInitLayers = 4;
                numberMaxLayers = 7;
                tubeToWin = 4;
                maxLevelColor = 4;
                generatorVersion= 1;
                seed = 356013544;

                setupObject.initLevelParameters(numberTube, numberEmptyTube, numberInitLayers, numberMaxLayers, tubeToWin);
                generatedLevel = levels.levelGenerator(seed,numberTube, numberEmptyTube, numberInitLayers, numberMaxLayers, tubeToWin, maxLevelColor, generatorVersion);
                return generatedLevel;

            case 67:
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

            case 68:
                numberTube = 8;
                numberEmptyTube = 1;
                numberInitLayers = 8;
                numberMaxLayers = 8;
                tubeToWin = 7;
                maxLevelColor = 7;
                seed = 1329687937;
                generatorVersion = 2;
                rewindPercentage = 0.5f;

                setupObject.initLevelParameters(numberTube, numberEmptyTube, numberInitLayers, numberMaxLayers, tubeToWin);
                generatedLevel = levels.levelGenerator(seed,numberTube, numberEmptyTube, numberInitLayers, numberMaxLayers, tubeToWin, maxLevelColor, generatorVersion, rewindPercentage);
                return generatedLevel;

            case 69:
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

            case 70:
                numberTube = 8;
                numberEmptyTube = 1;
                numberInitLayers = 7;
                numberMaxLayers = 7;
                tubeToWin = 7;
                maxLevelColor = 7;
                seed = 1329687937;
                generatorVersion = 2;

                setupObject.initLevelParameters(numberTube, numberEmptyTube, numberInitLayers, numberMaxLayers, tubeToWin);
                generatedLevel = levels.levelGenerator(seed,numberTube, numberEmptyTube, numberInitLayers, numberMaxLayers, tubeToWin, maxLevelColor, generatorVersion);
                return generatedLevel;

            case 71:
                numberTube = 8;
                numberEmptyTube = 0;
                numberInitLayers = 4;
                numberMaxLayers = 8;
                tubeToWin = 4;
                maxLevelColor = 4;
                generatorVersion= 1;
                seed = -149699361;

                setupObject.initLevelParameters(numberTube, numberEmptyTube, numberInitLayers, numberMaxLayers, tubeToWin);
                generatedLevel = levels.levelGenerator(seed,numberTube, numberEmptyTube, numberInitLayers, numberMaxLayers, tubeToWin, maxLevelColor, generatorVersion);
                return generatedLevel;

            case 72:
                numberTube = 8;
                numberEmptyTube = 0;
                numberInitLayers = 4;
                numberMaxLayers = 8;
                tubeToWin = 4;
                maxLevelColor = 4;
                generatorVersion= 1;
                seed = -1530392790;

                setupObject.initLevelParameters(numberTube, numberEmptyTube, numberInitLayers, numberMaxLayers, tubeToWin);
                generatedLevel = levels.levelGenerator(seed,numberTube, numberEmptyTube, numberInitLayers, numberMaxLayers, tubeToWin, maxLevelColor, generatorVersion);
                return generatedLevel;

            case 73:
                numberTube = 8;
                numberEmptyTube = 1;
                numberInitLayers = 6;
                numberMaxLayers = 7;
                tubeToWin = 6;
                maxLevelColor = 6;
                generatorVersion= 1;
                seed = 1896476837;

                setupObject.initLevelParameters(numberTube, numberEmptyTube, numberInitLayers, numberMaxLayers, tubeToWin);
                generatedLevel = levels.levelGenerator(seed,numberTube, numberEmptyTube, numberInitLayers, numberMaxLayers, tubeToWin, maxLevelColor, generatorVersion);
                return generatedLevel;

            case 74:
                numberTube = 8;
                numberEmptyTube = 1;
                numberInitLayers = 6;
                numberMaxLayers = 7;
                tubeToWin = 6;
                maxLevelColor = 6;
                generatorVersion= 1;
                seed = -1367173237;

                setupObject.initLevelParameters(numberTube, numberEmptyTube, numberInitLayers, numberMaxLayers, tubeToWin);
                generatedLevel = levels.levelGenerator(seed,numberTube, numberEmptyTube, numberInitLayers, numberMaxLayers, tubeToWin, maxLevelColor, generatorVersion);
                return generatedLevel;

            case 75:
                numberTube = 9;
                numberEmptyTube = 1;
                numberInitLayers = 3;
                numberMaxLayers = 6;
                tubeToWin = 4;
                maxLevelColor = 4;
                generatorVersion= 1;
                seed = -1927062992;

                setupObject.initLevelParameters(numberTube, numberEmptyTube, numberInitLayers, numberMaxLayers, tubeToWin);
                generatedLevel = levels.levelGenerator(seed,numberTube, numberEmptyTube, numberInitLayers, numberMaxLayers, tubeToWin, maxLevelColor, generatorVersion);
                return generatedLevel;

            case 76:
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

            case 77:
                numberTube = 9;
                numberEmptyTube = 2;
                numberInitLayers = 5;
                numberMaxLayers = 5;
                tubeToWin = 7;
                maxLevelColor = 7;
                seed = -1155797977;
                generatorVersion = 1;

                setupObject.initLevelParameters(numberTube, numberEmptyTube, numberInitLayers, numberMaxLayers, tubeToWin);
                generatedLevel = levels.levelGenerator(seed,numberTube, numberEmptyTube, numberInitLayers, numberMaxLayers, tubeToWin, maxLevelColor, generatorVersion);
                return generatedLevel;

            case 78:
                numberTube = 9;
                numberEmptyTube = 2;
                numberInitLayers = 7;
                numberMaxLayers = 7;
                tubeToWin = 7;
                maxLevelColor = 7;
                seed = -990163566;
                generatorVersion = 1;

                setupObject.initLevelParameters(numberTube, numberEmptyTube, numberInitLayers, numberMaxLayers, tubeToWin);
                generatedLevel = levels.levelGenerator(seed,numberTube, numberEmptyTube, numberInitLayers, numberMaxLayers, tubeToWin, maxLevelColor, generatorVersion);
                return generatedLevel;

            case 79:
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

            case 80:
                numberTube = 9;
                numberEmptyTube = 2;
                numberInitLayers = 6;
                numberMaxLayers = 6;
                tubeToWin = 7;
                maxLevelColor = 7;
                seed = -762294273;
                generatorVersion = 1;

                setupObject.initLevelParameters(numberTube, numberEmptyTube, numberInitLayers, numberMaxLayers, tubeToWin);
                generatedLevel = levels.levelGenerator(seed,numberTube, numberEmptyTube, numberInitLayers, numberMaxLayers, tubeToWin, maxLevelColor, generatorVersion);
                return generatedLevel;

            case 81:
                numberTube = 9;
                numberEmptyTube = 2;
                numberInitLayers = 6;
                numberMaxLayers = 6;
                tubeToWin = 7;
                maxLevelColor = 7;
                seed = -479341369;
                generatorVersion = 1;

                setupObject.initLevelParameters(numberTube, numberEmptyTube, numberInitLayers, numberMaxLayers, tubeToWin);
                generatedLevel = levels.levelGenerator(seed,numberTube, numberEmptyTube, numberInitLayers, numberMaxLayers, tubeToWin, maxLevelColor, generatorVersion);
                return generatedLevel;

            case 82:
                numberTube = 9;
                numberEmptyTube = 1;
                numberInitLayers = 4;
                numberMaxLayers = 8;
                tubeToWin = 4;
                maxLevelColor = 4;
                generatorVersion= 1;
                seed = -2046844590;

                setupObject.initLevelParameters(numberTube, numberEmptyTube, numberInitLayers, numberMaxLayers, tubeToWin);
                generatedLevel = levels.levelGenerator(seed,numberTube, numberEmptyTube, numberInitLayers, numberMaxLayers, tubeToWin, maxLevelColor, generatorVersion);
                return generatedLevel;

            case 83:
                numberTube = 9;
                numberEmptyTube = 1;
                numberInitLayers = 4;
                numberMaxLayers = 8;
                tubeToWin = 4;
                maxLevelColor = 4;
                generatorVersion= 1;
                seed = -266936091;

                setupObject.initLevelParameters(numberTube, numberEmptyTube, numberInitLayers, numberMaxLayers, tubeToWin);
                generatedLevel = levels.levelGenerator(seed,numberTube, numberEmptyTube, numberInitLayers, numberMaxLayers, tubeToWin, maxLevelColor, generatorVersion);
                return generatedLevel;

            case 84:
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

            case 85:
                numberTube = 9;
                numberEmptyTube = 1;
                numberInitLayers = 5;
                numberMaxLayers = 5;
                tubeToWin = 8;
                maxLevelColor = 8;
                seed = -325151281;
                generatorVersion = 1;

                setupObject.initLevelParameters(numberTube, numberEmptyTube, numberInitLayers, numberMaxLayers, tubeToWin);
                generatedLevel = levels.levelGenerator(seed,numberTube, numberEmptyTube, numberInitLayers, numberMaxLayers, tubeToWin, maxLevelColor, generatorVersion);
                return generatedLevel;

            case 86:
                numberTube = 9;
                numberEmptyTube = 1;
                numberInitLayers = 5;
                numberMaxLayers = 10;
                tubeToWin = 4;
                maxLevelColor = 4;
                generatorVersion= 1;
                seed = 1285573601;

                setupObject.initLevelParameters(numberTube, numberEmptyTube, numberInitLayers, numberMaxLayers, tubeToWin);
                generatedLevel = levels.levelGenerator(seed,numberTube, numberEmptyTube, numberInitLayers, numberMaxLayers, tubeToWin, maxLevelColor, generatorVersion);
                return generatedLevel;

            case 87:
                numberTube = 9;
                numberEmptyTube = 1;
                numberInitLayers = 5;
                numberMaxLayers = 10;
                tubeToWin = 4;
                maxLevelColor = 4;
                generatorVersion= 1;
                seed = 564226151;

                setupObject.initLevelParameters(numberTube, numberEmptyTube, numberInitLayers, numberMaxLayers, tubeToWin);
                generatedLevel = levels.levelGenerator(seed,numberTube, numberEmptyTube, numberInitLayers, numberMaxLayers, tubeToWin, maxLevelColor, generatorVersion);
                return generatedLevel;

            case 88:
                numberTube = 9;
                numberEmptyTube = 0;
                numberInitLayers = 4;
                numberMaxLayers = 6;
                tubeToWin = 6;
                maxLevelColor = 6;
                generatorVersion= 1;
                seed = 923417619;

                setupObject.initLevelParameters(numberTube, numberEmptyTube, numberInitLayers, numberMaxLayers, tubeToWin);
                generatedLevel = levels.levelGenerator(seed,numberTube, numberEmptyTube, numberInitLayers, numberMaxLayers, tubeToWin, maxLevelColor, generatorVersion);
                return generatedLevel;

            case 89:
                numberTube = 9;
                numberEmptyTube = 0;
                numberInitLayers = 4;
                numberMaxLayers = 6;
                tubeToWin = 6;
                maxLevelColor = 6;
                generatorVersion= 1;
                seed = 793778351;

                setupObject.initLevelParameters(numberTube, numberEmptyTube, numberInitLayers, numberMaxLayers, tubeToWin);
                generatedLevel = levels.levelGenerator(seed,numberTube, numberEmptyTube, numberInitLayers, numberMaxLayers, tubeToWin, maxLevelColor, generatorVersion);
                return generatedLevel;

            case 90:
                numberTube = 9;
                numberEmptyTube = 0;
                numberInitLayers = 4;
                numberMaxLayers = 6;
                tubeToWin = 6;
                maxLevelColor = 6;
                generatorVersion= 1;
                seed = 1183047786;

                setupObject.initLevelParameters(numberTube, numberEmptyTube, numberInitLayers, numberMaxLayers, tubeToWin);
                generatedLevel = levels.levelGenerator(seed,numberTube, numberEmptyTube, numberInitLayers, numberMaxLayers, tubeToWin, maxLevelColor, generatorVersion);
                return generatedLevel;

            case 91:
                numberTube = 9;
                numberEmptyTube = 0;
                numberInitLayers = 4;
                numberMaxLayers = 9;
                tubeToWin = 4;
                maxLevelColor = 4;
                generatorVersion= 1;
                seed = 474822742;

                setupObject.initLevelParameters(numberTube, numberEmptyTube, numberInitLayers, numberMaxLayers, tubeToWin);
                generatedLevel = levels.levelGenerator(seed,numberTube, numberEmptyTube, numberInitLayers, numberMaxLayers, tubeToWin, maxLevelColor, generatorVersion);
                return generatedLevel;

            case 92:
                numberTube = 9;
                numberEmptyTube = 0;
                numberInitLayers = 4;
                numberMaxLayers = 9;
                tubeToWin = 4;
                maxLevelColor = 4;
                generatorVersion= 1;
                seed = 1350963604;

                setupObject.initLevelParameters(numberTube, numberEmptyTube, numberInitLayers, numberMaxLayers, tubeToWin);
                generatedLevel = levels.levelGenerator(seed,numberTube, numberEmptyTube, numberInitLayers, numberMaxLayers, tubeToWin, maxLevelColor, generatorVersion);
                return generatedLevel;

            case 93:
                numberTube = 9;
                numberEmptyTube = 0;
                numberInitLayers = 4;
                numberMaxLayers = 9;
                tubeToWin = 4;
                maxLevelColor = 4;
                generatorVersion= 1;
                seed = 1528262772;

                setupObject.initLevelParameters(numberTube, numberEmptyTube, numberInitLayers, numberMaxLayers, tubeToWin);
                generatedLevel = levels.levelGenerator(seed,numberTube, numberEmptyTube, numberInitLayers, numberMaxLayers, tubeToWin, maxLevelColor, generatorVersion);
                return generatedLevel;

            case 94:
                numberTube = 9;
                numberEmptyTube = 2;
                numberInitLayers = 8;
                numberMaxLayers = 8;
                tubeToWin = 7;
                maxLevelColor = 7;
                seed = 1808499529;
                generatorVersion = 1;

                setupObject.initLevelParameters(numberTube, numberEmptyTube, numberInitLayers, numberMaxLayers, tubeToWin);
                generatedLevel = levels.levelGenerator(seed,numberTube, numberEmptyTube, numberInitLayers, numberMaxLayers, tubeToWin, maxLevelColor, generatorVersion);
                return generatedLevel;

            case 95:
                numberTube = 9;
                numberEmptyTube = 2;
                numberInitLayers = 8;
                numberMaxLayers = 8;
                tubeToWin = 7;
                maxLevelColor = 7;
                seed = 1932743670;
                generatorVersion = 1;

                setupObject.initLevelParameters(numberTube, numberEmptyTube, numberInitLayers, numberMaxLayers, tubeToWin);
                generatedLevel = levels.levelGenerator(seed,numberTube, numberEmptyTube, numberInitLayers, numberMaxLayers, tubeToWin, maxLevelColor, generatorVersion);
                return generatedLevel;

            case 96:
                numberTube = 9;
                numberEmptyTube = 2;
                numberInitLayers = 8;
                numberMaxLayers = 8;
                tubeToWin = 7;
                maxLevelColor = 7;
                seed = 2130181210;
                generatorVersion = 1;

                setupObject.initLevelParameters(numberTube, numberEmptyTube, numberInitLayers, numberMaxLayers, tubeToWin);
                generatedLevel = levels.levelGenerator(seed,numberTube, numberEmptyTube, numberInitLayers, numberMaxLayers, tubeToWin, maxLevelColor, generatorVersion);
                return generatedLevel;

            case 97:
                numberTube = 9;
                numberEmptyTube = 2;
                numberInitLayers = 8;
                numberMaxLayers = 8;
                tubeToWin = 7;
                maxLevelColor = 7;
                seed = 2130181219;
                generatorVersion = 1;

                setupObject.initLevelParameters(numberTube, numberEmptyTube, numberInitLayers, numberMaxLayers, tubeToWin);
                generatedLevel = levels.levelGenerator(seed,numberTube, numberEmptyTube, numberInitLayers, numberMaxLayers, tubeToWin, maxLevelColor, generatorVersion);
                return generatedLevel;

            case 98:
                numberTube = 9;
                numberEmptyTube = 1;
                numberInitLayers = 8;
                numberMaxLayers = 8;
                tubeToWin = 8;
                maxLevelColor = 8;
                seed = -411529851;
                generatorVersion = 2;
                rewindPercentage = 0.60f;

                setupObject.initLevelParameters(numberTube, numberEmptyTube, numberInitLayers, numberMaxLayers, tubeToWin);
                generatedLevel = levels.levelGenerator(seed,numberTube, numberEmptyTube, numberInitLayers, numberMaxLayers, tubeToWin, maxLevelColor, generatorVersion, rewindPercentage);
                return generatedLevel;

            case 99:
                numberTube = 9;
                numberEmptyTube = 1;
                numberInitLayers = 8;
                numberMaxLayers = 8;
                tubeToWin = 8;
                maxLevelColor = 8;
                seed = -2004987813;
                generatorVersion = 2;
                rewindPercentage = 0.60f;

                setupObject.initLevelParameters(numberTube, numberEmptyTube, numberInitLayers, numberMaxLayers, tubeToWin);
                generatedLevel = levels.levelGenerator(seed,numberTube, numberEmptyTube, numberInitLayers, numberMaxLayers, tubeToWin, maxLevelColor, generatorVersion, rewindPercentage);
                return generatedLevel;

            case 100:
                numberTube = 9;
                numberEmptyTube = 1;
                numberInitLayers = 8;
                numberMaxLayers = 8;
                tubeToWin = 8;
                maxLevelColor = 8;
                seed = 740350752;
                generatorVersion = 2;
                rewindPercentage = 0.60f;

                setupObject.initLevelParameters(numberTube, numberEmptyTube, numberInitLayers, numberMaxLayers, tubeToWin);
                generatedLevel = levels.levelGenerator(seed,numberTube, numberEmptyTube, numberInitLayers, numberMaxLayers, tubeToWin, maxLevelColor, generatorVersion, rewindPercentage);
                return generatedLevel;

            case 101:
                numberTube = 9;
                numberEmptyTube = 1;
                numberInitLayers = 6;
                numberMaxLayers = 6;
                tubeToWin = 8;
                maxLevelColor = 8;
                seed = 506311203;
                generatorVersion = 2;
                rewindPercentage = 0.40f;

                setupObject.initLevelParameters(numberTube, numberEmptyTube, numberInitLayers, numberMaxLayers, tubeToWin);
                generatedLevel = levels.levelGenerator(seed,numberTube, numberEmptyTube, numberInitLayers, numberMaxLayers, tubeToWin, maxLevelColor, generatorVersion);
                return generatedLevel;

            case 102:
                numberTube = 9;
                numberEmptyTube = 1;
                numberInitLayers = 6;
                numberMaxLayers = 6;
                tubeToWin = 8;
                maxLevelColor = 8;
                seed = 1031627519 ;
                generatorVersion = 2;
                rewindPercentage = 0.40f;

                setupObject.initLevelParameters(numberTube, numberEmptyTube, numberInitLayers, numberMaxLayers, tubeToWin);
                generatedLevel = levels.levelGenerator(seed,numberTube, numberEmptyTube, numberInitLayers, numberMaxLayers, tubeToWin, maxLevelColor, generatorVersion);
                return generatedLevel;

            case 103:
                numberTube = 9;
                numberEmptyTube = 1;
                numberInitLayers = 6;
                numberMaxLayers = 6;
                tubeToWin = 8;
                maxLevelColor = 8;
                seed = 1136064280;
                generatorVersion = 2;
                rewindPercentage = 0.40f;

                setupObject.initLevelParameters(numberTube, numberEmptyTube, numberInitLayers, numberMaxLayers, tubeToWin);
                generatedLevel = levels.levelGenerator(seed,numberTube, numberEmptyTube, numberInitLayers, numberMaxLayers, tubeToWin, maxLevelColor, generatorVersion);
                return generatedLevel;

            case 104:
                numberTube = 9;
                numberEmptyTube = 0;
                numberInitLayers = 8;
                numberMaxLayers = 9;
                tubeToWin = 8;
                maxLevelColor = 8;
                generatorVersion= 2;
                seed = -793996965;
                rewindPercentage = 0.70f;

                setupObject.initLevelParameters(numberTube, numberEmptyTube, numberInitLayers, numberMaxLayers, tubeToWin);
                generatedLevel = levels.levelGenerator(seed,numberTube, numberEmptyTube, numberInitLayers, numberMaxLayers, tubeToWin, maxLevelColor, generatorVersion, rewindPercentage);
                return generatedLevel;

            case 105:
                numberTube = 9;
                numberEmptyTube = 1;
                numberInitLayers = 6;
                numberMaxLayers = 8;
                tubeToWin = 6;
                maxLevelColor = 6;
                generatorVersion= 1;
                seed = -1888362038;

                setupObject.initLevelParameters(numberTube, numberEmptyTube, numberInitLayers, numberMaxLayers, tubeToWin);
                generatedLevel = levels.levelGenerator(seed,numberTube, numberEmptyTube, numberInitLayers, numberMaxLayers, tubeToWin, maxLevelColor, generatorVersion);
                return generatedLevel;

            case 106:
                numberTube = 9;
                numberEmptyTube = 1;
                numberInitLayers = 6;
                numberMaxLayers = 8;
                tubeToWin = 6;
                maxLevelColor = 6;
                generatorVersion= 1;
                seed = -1888362037;

                setupObject.initLevelParameters(numberTube, numberEmptyTube, numberInitLayers, numberMaxLayers, tubeToWin);
                generatedLevel = levels.levelGenerator(seed,numberTube, numberEmptyTube, numberInitLayers, numberMaxLayers, tubeToWin, maxLevelColor, generatorVersion);
                return generatedLevel;

            case 107:
                numberTube = 9;
                numberEmptyTube = 1;
                numberInitLayers = 6;
                numberMaxLayers = 8;
                tubeToWin = 6;
                maxLevelColor = 6;
                generatorVersion= 1;
                seed = -754344547;

                setupObject.initLevelParameters(numberTube, numberEmptyTube, numberInitLayers, numberMaxLayers, tubeToWin);
                generatedLevel = levels.levelGenerator(seed,numberTube, numberEmptyTube, numberInitLayers, numberMaxLayers, tubeToWin, maxLevelColor, generatorVersion);
                return generatedLevel;

            case 108:
                numberTube = 9;
                numberEmptyTube = 1;
                numberInitLayers = 7;
                numberMaxLayers = 7;
                tubeToWin = 8;
                maxLevelColor = 8;
                seed = -781584217;
                generatorVersion = 2;
                rewindPercentage = 0.40f;

                setupObject.initLevelParameters(numberTube, numberEmptyTube, numberInitLayers, numberMaxLayers, tubeToWin);
                generatedLevel = levels.levelGenerator(seed,numberTube, numberEmptyTube, numberInitLayers, numberMaxLayers, tubeToWin, maxLevelColor, generatorVersion);
                return generatedLevel;


            case 109:
                numberTube = 9;
                numberEmptyTube = 0;
                numberInitLayers = 6;
                numberMaxLayers = 9;
                tubeToWin = 6;
                maxLevelColor = 6;
                generatorVersion= 1;
                seed = 821413580;

                setupObject.initLevelParameters(numberTube, numberEmptyTube, numberInitLayers, numberMaxLayers, tubeToWin);
                generatedLevel = levels.levelGenerator(seed,numberTube, numberEmptyTube, numberInitLayers, numberMaxLayers, tubeToWin, maxLevelColor, generatorVersion);
                return generatedLevel;

            case 110:
                numberTube = 9;
                numberEmptyTube = 0;
                numberInitLayers = 6;
                numberMaxLayers = 9;
                tubeToWin = 6;
                maxLevelColor = 6;
                generatorVersion= 1;
                seed = 1769465023;

                setupObject.initLevelParameters(numberTube, numberEmptyTube, numberInitLayers, numberMaxLayers, tubeToWin);
                generatedLevel = levels.levelGenerator(seed,numberTube, numberEmptyTube, numberInitLayers, numberMaxLayers, tubeToWin, maxLevelColor, generatorVersion);
                return generatedLevel;

            case 111:
                numberTube = 9;
                numberEmptyTube = 0;
                numberInitLayers = 6;
                numberMaxLayers = 9;
                tubeToWin = 6;
                maxLevelColor = 6;
                generatorVersion= 1;
                seed = 1989398864;

                setupObject.initLevelParameters(numberTube, numberEmptyTube, numberInitLayers, numberMaxLayers, tubeToWin);
                generatedLevel = levels.levelGenerator(seed,numberTube, numberEmptyTube, numberInitLayers, numberMaxLayers, tubeToWin, maxLevelColor, generatorVersion);
                return generatedLevel;

            case 112:
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

            case 113:
                numberTube = 10;
                numberEmptyTube = 2;
                numberInitLayers = 5;
                numberMaxLayers = 5;
                tubeToWin = 8;
                maxLevelColor = 8;
                seed = 2143395459;
                generatorVersion = 1;

                setupObject.initLevelParameters(numberTube, numberEmptyTube, numberInitLayers, numberMaxLayers, tubeToWin);
                generatedLevel = levels.levelGenerator(seed,numberTube, numberEmptyTube, numberInitLayers, numberMaxLayers, tubeToWin, maxLevelColor, generatorVersion);
                return generatedLevel;

            case 114:
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

            case 115:
                numberTube = 10;
                numberEmptyTube = 1;
                numberInitLayers = 4;
                numberMaxLayers = 4;
                tubeToWin = 9;
                maxLevelColor = 9;
                seed = -1875330715;
                generatorVersion = 1;

                setupObject.initLevelParameters(numberTube, numberEmptyTube, numberInitLayers, numberMaxLayers, tubeToWin);
                generatedLevel = levels.levelGenerator(seed,numberTube, numberEmptyTube, numberInitLayers, numberMaxLayers, tubeToWin, maxLevelColor, generatorVersion);
                return generatedLevel;

            case 116:
                numberTube = 10;
                numberEmptyTube = 1;
                numberInitLayers = 4;
                numberMaxLayers = 9;
                tubeToWin = 4;
                maxLevelColor = 4;
                generatorVersion= 1;
                seed = -1722283952;

                setupObject.initLevelParameters(numberTube, numberEmptyTube, numberInitLayers, numberMaxLayers, tubeToWin);
                generatedLevel = levels.levelGenerator(seed,numberTube, numberEmptyTube, numberInitLayers, numberMaxLayers, tubeToWin, maxLevelColor, generatorVersion);
                return generatedLevel;

            case 117:
                numberTube = 10;
                numberEmptyTube = 1;
                numberInitLayers = 4;
                numberMaxLayers = 9;
                tubeToWin = 4;
                maxLevelColor = 4;
                generatorVersion= 1;
                seed = -1541290371;

                setupObject.initLevelParameters(numberTube, numberEmptyTube, numberInitLayers, numberMaxLayers, tubeToWin);
                generatedLevel = levels.levelGenerator(seed,numberTube, numberEmptyTube, numberInitLayers, numberMaxLayers, tubeToWin, maxLevelColor, generatorVersion);
                return generatedLevel;

            case 118:
                numberTube = 10;
                numberEmptyTube = 2;
                numberInitLayers = 6;
                numberMaxLayers = 6;
                tubeToWin = 8;
                maxLevelColor = 8;
                seed = -1345271879;
                generatorVersion = 1;

                setupObject.initLevelParameters(numberTube, numberEmptyTube, numberInitLayers, numberMaxLayers, tubeToWin);
                generatedLevel = levels.levelGenerator(seed,numberTube, numberEmptyTube, numberInitLayers, numberMaxLayers, tubeToWin, maxLevelColor, generatorVersion);
                return generatedLevel;

            case 119:
                numberTube = 10;
                numberEmptyTube = 2;
                numberInitLayers = 6;
                numberMaxLayers = 6;
                tubeToWin = 8;
                maxLevelColor = 8;
                seed = -1213342466;
                generatorVersion = 1;

                setupObject.initLevelParameters(numberTube, numberEmptyTube, numberInitLayers, numberMaxLayers, tubeToWin);
                generatedLevel = levels.levelGenerator(seed,numberTube, numberEmptyTube, numberInitLayers, numberMaxLayers, tubeToWin, maxLevelColor, generatorVersion);
                return generatedLevel;

            case 120:
                numberTube = 10;
                numberEmptyTube = 2;
                numberInitLayers = 6;
                numberMaxLayers = 6;
                tubeToWin = 8;
                maxLevelColor = 8;
                seed = -1813342466;
                generatorVersion = 1;

                setupObject.initLevelParameters(numberTube, numberEmptyTube, numberInitLayers, numberMaxLayers, tubeToWin);
                generatedLevel = levels.levelGenerator(seed,numberTube, numberEmptyTube, numberInitLayers, numberMaxLayers, tubeToWin, maxLevelColor, generatorVersion);
                return generatedLevel;

            case 121:
                numberTube = 10;
                numberEmptyTube = 1;
                numberInitLayers = 4;
                numberMaxLayers = 6;
                tubeToWin = 6;
                maxLevelColor = 6;
                generatorVersion= 1;
                seed = -840910099;

                setupObject.initLevelParameters(numberTube, numberEmptyTube, numberInitLayers, numberMaxLayers, tubeToWin);
                generatedLevel = levels.levelGenerator(seed,numberTube, numberEmptyTube, numberInitLayers, numberMaxLayers, tubeToWin, maxLevelColor, generatorVersion);
                return generatedLevel;

            case 122:
                numberTube = 10;
                numberEmptyTube = 1;
                numberInitLayers = 4;
                numberMaxLayers = 6;
                tubeToWin = 6;
                maxLevelColor = 6;
                generatorVersion= 1;
                seed = -1063955128;

                setupObject.initLevelParameters(numberTube, numberEmptyTube, numberInitLayers, numberMaxLayers, tubeToWin);
                generatedLevel = levels.levelGenerator(seed,numberTube, numberEmptyTube, numberInitLayers, numberMaxLayers, tubeToWin, maxLevelColor, generatorVersion);
                return generatedLevel;

            case 123:
                numberTube = 10;
                numberEmptyTube = 1;
                numberInitLayers = 4;
                numberMaxLayers = 6;
                tubeToWin = 6;
                maxLevelColor = 6;
                generatorVersion= 1;
                seed = -865439389;

                setupObject.initLevelParameters(numberTube, numberEmptyTube, numberInitLayers, numberMaxLayers, tubeToWin);
                generatedLevel = levels.levelGenerator(seed,numberTube, numberEmptyTube, numberInitLayers, numberMaxLayers, tubeToWin, maxLevelColor, generatorVersion);
                return generatedLevel;

            case 124:
                numberTube = 10;
                numberEmptyTube = 1;
                numberInitLayers = 5;
                numberMaxLayers = 5;
                tubeToWin = 9;
                maxLevelColor = 9;
                seed = 1145885638;
                generatorVersion = 2;
                rewindPercentage = 0.40f;

                setupObject.initLevelParameters(numberTube, numberEmptyTube, numberInitLayers, numberMaxLayers, tubeToWin);
                generatedLevel = levels.levelGenerator(seed,numberTube, numberEmptyTube, numberInitLayers, numberMaxLayers, tubeToWin, maxLevelColor, generatorVersion, rewindPercentage);
                return generatedLevel;

            case 125:
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

            case 126:
                numberTube = 10;
                numberEmptyTube = 2;
                numberInitLayers = 7;
                numberMaxLayers = 7;
                tubeToWin = 8;
                maxLevelColor = 8;
                seed = 1553016852;
                generatorVersion = 1;

                setupObject.initLevelParameters(numberTube, numberEmptyTube, numberInitLayers, numberMaxLayers, tubeToWin);
                generatedLevel = levels.levelGenerator(seed,numberTube, numberEmptyTube, numberInitLayers, numberMaxLayers, tubeToWin, maxLevelColor, generatorVersion);
                return generatedLevel;

            case 127:
                numberTube = 10;
                numberEmptyTube = 2;
                numberInitLayers = 7;
                numberMaxLayers = 7;
                tubeToWin = 8;
                maxLevelColor = 8;
                seed = -551229731;
                generatorVersion = 1;

                setupObject.initLevelParameters(numberTube, numberEmptyTube, numberInitLayers, numberMaxLayers, tubeToWin);
                generatedLevel = levels.levelGenerator(seed,numberTube, numberEmptyTube, numberInitLayers, numberMaxLayers, tubeToWin, maxLevelColor, generatorVersion);
                return generatedLevel;

            case 128:
                numberTube = 10;
                numberEmptyTube = 2;
                numberInitLayers = 7;
                numberMaxLayers = 7;
                tubeToWin = 8;
                maxLevelColor = 8;
                seed = -365118969;
                generatorVersion = 1;

                setupObject.initLevelParameters(numberTube, numberEmptyTube, numberInitLayers, numberMaxLayers, tubeToWin);
                generatedLevel = levels.levelGenerator(seed,numberTube, numberEmptyTube, numberInitLayers, numberMaxLayers, tubeToWin, maxLevelColor, generatorVersion);
                return generatedLevel;

            case 129:
                numberTube = 10;
                numberEmptyTube = 1;
                numberInitLayers = 6;
                numberMaxLayers = 9;
                tubeToWin = 6;
                maxLevelColor = 6;
                generatorVersion= 1;
                seed = -219620295;

                setupObject.initLevelParameters(numberTube, numberEmptyTube, numberInitLayers, numberMaxLayers, tubeToWin);
                generatedLevel = levels.levelGenerator(seed,numberTube, numberEmptyTube, numberInitLayers, numberMaxLayers, tubeToWin, maxLevelColor, generatorVersion);
                return generatedLevel;
            
            case 130:
                numberTube = 10;
                numberEmptyTube = 1;
                numberInitLayers = 6;
                numberMaxLayers = 9;
                tubeToWin = 6;
                maxLevelColor = 6;
                generatorVersion= 1;
                seed = 6219933;

                setupObject.initLevelParameters(numberTube, numberEmptyTube, numberInitLayers, numberMaxLayers, tubeToWin);
                generatedLevel = levels.levelGenerator(seed,numberTube, numberEmptyTube, numberInitLayers, numberMaxLayers, tubeToWin, maxLevelColor, generatorVersion);
                return generatedLevel;

            case 131:
                numberTube = 10;
                numberEmptyTube = 1;
                numberInitLayers = 6;
                numberMaxLayers = 9;
                tubeToWin = 6;
                maxLevelColor = 6;
                generatorVersion= 1;
                seed = -74814692;

                setupObject.initLevelParameters(numberTube, numberEmptyTube, numberInitLayers, numberMaxLayers, tubeToWin);
                generatedLevel = levels.levelGenerator(seed,numberTube, numberEmptyTube, numberInitLayers, numberMaxLayers, tubeToWin, maxLevelColor, generatorVersion);
                return generatedLevel;

            case 132:
                numberTube = 10;
                numberEmptyTube = 2;
                numberInitLayers = 6;
                numberMaxLayers = 6;
                tubeToWin = 8;
                maxLevelColor = 8;
                seed = 621652988;
                generatorVersion = 1;

                setupObject.initLevelParameters(numberTube, numberEmptyTube, numberInitLayers, numberMaxLayers, tubeToWin);
                generatedLevel = levels.levelGenerator(seed,numberTube, numberEmptyTube, numberInitLayers, numberMaxLayers, tubeToWin, maxLevelColor, generatorVersion);
                return generatedLevel;

            case 133:
                numberTube = 10;
                numberEmptyTube = 2;
                numberInitLayers = 6;
                numberMaxLayers = 6;
                tubeToWin = 8;
                maxLevelColor = 8;
                seed = 521652988;
                generatorVersion = 1;

                setupObject.initLevelParameters(numberTube, numberEmptyTube, numberInitLayers, numberMaxLayers, tubeToWin);
                generatedLevel = levels.levelGenerator(seed,numberTube, numberEmptyTube, numberInitLayers, numberMaxLayers, tubeToWin, maxLevelColor, generatorVersion);
                return generatedLevel;

            case 134:
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

            case 135:
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
            
            case 136:
                numberTube = 10;
                numberEmptyTube = 2;
                numberInitLayers = 7;
                numberMaxLayers = 7;
                tubeToWin = 8;
                maxLevelColor = 8;
                seed = -1375289533;
                generatorVersion = 1;
                rewindPercentage = 0.40f;

                setupObject.initLevelParameters(numberTube, numberEmptyTube, numberInitLayers, numberMaxLayers, tubeToWin);
                generatedLevel = levels.levelGenerator(seed,numberTube, numberEmptyTube, numberInitLayers, numberMaxLayers, tubeToWin, maxLevelColor, generatorVersion, rewindPercentage);
                return generatedLevel;

            case 137:
                numberTube = 10;
                numberEmptyTube = 2;
                numberInitLayers = 8;
                numberMaxLayers = 8;
                tubeToWin = 8;
                maxLevelColor = 8;
                seed = -1585527908;
                generatorVersion = 1;
                rewindPercentage = 0.40f;

                setupObject.initLevelParameters(numberTube, numberEmptyTube, numberInitLayers, numberMaxLayers, tubeToWin);
                generatedLevel = levels.levelGenerator(seed,numberTube, numberEmptyTube, numberInitLayers, numberMaxLayers, tubeToWin, maxLevelColor, generatorVersion, rewindPercentage);
                return generatedLevel;            

            case 138:
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

            case 139:
                numberTube = 10;
                numberEmptyTube = 2;
                numberInitLayers = 8;
                numberMaxLayers = 8;
                tubeToWin = 8;
                maxLevelColor = 8;
                seed = -632719381;
                generatorVersion = 1;
                rewindPercentage = 0.55f;

                setupObject.initLevelParameters(numberTube, numberEmptyTube, numberInitLayers, numberMaxLayers, tubeToWin);
                generatedLevel = levels.levelGenerator(seed,numberTube, numberEmptyTube, numberInitLayers, numberMaxLayers, tubeToWin, maxLevelColor, generatorVersion, rewindPercentage);
                return generatedLevel;

            case 140:
                numberTube = 10;
                numberEmptyTube = 2;
                numberInitLayers = 9;
                numberMaxLayers = 9;
                tubeToWin = 8;
                maxLevelColor = 8;
                seed = -141254748;
                generatorVersion = 1;

                setupObject.initLevelParameters(numberTube, numberEmptyTube, numberInitLayers, numberMaxLayers, tubeToWin);
                generatedLevel = levels.levelGenerator(seed,numberTube, numberEmptyTube, numberInitLayers, numberMaxLayers, tubeToWin, maxLevelColor, generatorVersion);
                return generatedLevel;

            case 141:
                numberTube = 10;
                numberEmptyTube = 2;
                numberInitLayers = 9;
                numberMaxLayers = 9;
                tubeToWin = 8;
                maxLevelColor = 8;
                seed = 294526577;
                generatorVersion = 1;

                setupObject.initLevelParameters(numberTube, numberEmptyTube, numberInitLayers, numberMaxLayers, tubeToWin);
                generatedLevel = levels.levelGenerator(seed,numberTube, numberEmptyTube, numberInitLayers, numberMaxLayers, tubeToWin, maxLevelColor, generatorVersion);
                return generatedLevel;

            case 142:
                numberTube = 8;
                numberEmptyTube = 0;
                numberInitLayers = 6;
                numberMaxLayers = 8;
                tubeToWin = 6;
                maxLevelColor = 6;
                generatorVersion= 1;
                seed = 87273640;

                setupObject.initLevelParameters(numberTube, numberEmptyTube, numberInitLayers, numberMaxLayers, tubeToWin);
                generatedLevel = levels.levelGenerator(seed,numberTube, numberEmptyTube, numberInitLayers, numberMaxLayers, tubeToWin, maxLevelColor, generatorVersion);
                return generatedLevel;

            case 143:
                numberTube = 10;
                numberEmptyTube = 1;
                numberInitLayers = 8;
                numberMaxLayers = 9;
                tubeToWin = 8;
                maxLevelColor = 8;
                generatorVersion= 1;
                seed = 54776766;

                setupObject.initLevelParameters(numberTube, numberEmptyTube, numberInitLayers, numberMaxLayers, tubeToWin);
                generatedLevel = levels.levelGenerator(seed,numberTube, numberEmptyTube, numberInitLayers, numberMaxLayers, tubeToWin, maxLevelColor, generatorVersion);
                return generatedLevel;

            case 144:
                numberTube = 10;
                numberEmptyTube = 0;
                numberInitLayers = 8;
                numberMaxLayers = 10;
                tubeToWin = 8;
                maxLevelColor = 8;
                generatorVersion= 1;
                seed = -2125947658;

                setupObject.initLevelParameters(numberTube, numberEmptyTube, numberInitLayers, numberMaxLayers, tubeToWin);
                generatedLevel = levels.levelGenerator(seed,numberTube, numberEmptyTube, numberInitLayers, numberMaxLayers, tubeToWin, maxLevelColor, generatorVersion);
                return generatedLevel;

            case 145:
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

            case 146:
                numberTube = 11;
                numberEmptyTube = 2;
                numberInitLayers = 5;
                numberMaxLayers = 5;
                tubeToWin = 9;
                maxLevelColor = 9;
                seed = 554174695;
                generatorVersion = 1;

                setupObject.initLevelParameters(numberTube, numberEmptyTube, numberInitLayers, numberMaxLayers, tubeToWin);
                generatedLevel = levels.levelGenerator(seed,numberTube, numberEmptyTube, numberInitLayers, numberMaxLayers, tubeToWin, maxLevelColor, generatorVersion);
                return generatedLevel;

            case 147:
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

            case 148:
                numberTube = 11;
                numberEmptyTube = 1;
                numberInitLayers = 4;
                numberMaxLayers = 10;
                tubeToWin = 4;
                maxLevelColor = 4;
                generatorVersion= 1;
                seed = 646346138;

                setupObject.initLevelParameters(numberTube, numberEmptyTube, numberInitLayers, numberMaxLayers, tubeToWin);
                generatedLevel = levels.levelGenerator(seed,numberTube, numberEmptyTube, numberInitLayers, numberMaxLayers, tubeToWin, maxLevelColor, generatorVersion);
                return generatedLevel;

            case 149:
                numberTube = 11;
                numberEmptyTube = 1;
                numberInitLayers = 4;
                numberMaxLayers = 10;
                tubeToWin = 4;
                maxLevelColor = 4;
                generatorVersion= 1;
                seed = 759419543;

                setupObject.initLevelParameters(numberTube, numberEmptyTube, numberInitLayers, numberMaxLayers, tubeToWin);
                generatedLevel = levels.levelGenerator(seed,numberTube, numberEmptyTube, numberInitLayers, numberMaxLayers, tubeToWin, maxLevelColor, generatorVersion);
                return generatedLevel;

            case 150:
                numberTube = 11;
                numberEmptyTube = 1;
                numberInitLayers = 4;
                numberMaxLayers = 10;
                tubeToWin = 4;
                maxLevelColor = 4;
                generatorVersion= 1;
                seed = 610243122;

                setupObject.initLevelParameters(numberTube, numberEmptyTube, numberInitLayers, numberMaxLayers, tubeToWin);
                generatedLevel = levels.levelGenerator(seed,numberTube, numberEmptyTube, numberInitLayers, numberMaxLayers, tubeToWin, maxLevelColor, generatorVersion);
                return generatedLevel;

            case 151:
                numberTube = 11;
                numberEmptyTube = 1;
                numberInitLayers = 4;
                numberMaxLayers = 10;
                tubeToWin = 4;
                maxLevelColor = 4;
                generatorVersion= 1;
                seed = 1001611234;

                setupObject.initLevelParameters(numberTube, numberEmptyTube, numberInitLayers, numberMaxLayers, tubeToWin);
                generatedLevel = levels.levelGenerator(seed,numberTube, numberEmptyTube, numberInitLayers, numberMaxLayers, tubeToWin, maxLevelColor, generatorVersion);
                return generatedLevel;

            case 152:
                numberTube = 11;
                numberEmptyTube = 2;
                numberInitLayers = 6;
                numberMaxLayers = 6;
                tubeToWin = 9;
                maxLevelColor = 9;
                seed = 2118981315;
                generatorVersion = 1;

                setupObject.initLevelParameters(numberTube, numberEmptyTube, numberInitLayers, numberMaxLayers, tubeToWin);
                generatedLevel = levels.levelGenerator(seed,numberTube, numberEmptyTube, numberInitLayers, numberMaxLayers, tubeToWin, maxLevelColor, generatorVersion);
                return generatedLevel;

            case 153:
                numberTube = 11;
                numberEmptyTube = 2;
                numberInitLayers = 6;
                numberMaxLayers = 6;
                tubeToWin = 9;
                maxLevelColor = 9;
                seed = -2028040970;
                generatorVersion = 1;

                setupObject.initLevelParameters(numberTube, numberEmptyTube, numberInitLayers, numberMaxLayers, tubeToWin);
                generatedLevel = levels.levelGenerator(seed,numberTube, numberEmptyTube, numberInitLayers, numberMaxLayers, tubeToWin, maxLevelColor, generatorVersion);
                return generatedLevel;

            case 154:
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

            case 155:
                numberTube = 11;
                numberEmptyTube = 2;
                numberInitLayers = 6;
                numberMaxLayers = 6;
                tubeToWin = 9;
                maxLevelColor = 9;
                seed = -1464191312;
                generatorVersion = 1;

                setupObject.initLevelParameters(numberTube, numberEmptyTube, numberInitLayers, numberMaxLayers, tubeToWin);
                generatedLevel = levels.levelGenerator(seed,numberTube, numberEmptyTube, numberInitLayers, numberMaxLayers, tubeToWin, maxLevelColor, generatorVersion);
                return generatedLevel;

            case 156:
                numberTube = 11;
                numberEmptyTube = 0;
                numberInitLayers = 4;
                numberMaxLayers = 11;
                tubeToWin = 4;
                maxLevelColor = 4;
                generatorVersion= 1;
                seed = -1875498331;

                setupObject.initLevelParameters(numberTube, numberEmptyTube, numberInitLayers, numberMaxLayers, tubeToWin);
                generatedLevel = levels.levelGenerator(seed,numberTube, numberEmptyTube, numberInitLayers, numberMaxLayers, tubeToWin, maxLevelColor, generatorVersion);
                return generatedLevel;

            case 157:
                numberTube = 11;
                numberEmptyTube = 0;
                numberInitLayers = 4;
                numberMaxLayers = 11;
                tubeToWin = 4;
                maxLevelColor = 4;
                generatorVersion= 1;
                seed = -1180603961;

                setupObject.initLevelParameters(numberTube, numberEmptyTube, numberInitLayers, numberMaxLayers, tubeToWin);
                generatedLevel = levels.levelGenerator(seed,numberTube, numberEmptyTube, numberInitLayers, numberMaxLayers, tubeToWin, maxLevelColor, generatorVersion);
                return generatedLevel;

            case 158:
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

            case 159:
                numberTube = 11;
                numberEmptyTube = 2;
                numberInitLayers = 7;
                numberMaxLayers = 7;
                tubeToWin = 9;
                maxLevelColor = 9;
                seed = 186483505;
                generatorVersion = 1;

                setupObject.initLevelParameters(numberTube, numberEmptyTube, numberInitLayers, numberMaxLayers, tubeToWin);
                generatedLevel = levels.levelGenerator(seed,numberTube, numberEmptyTube, numberInitLayers, numberMaxLayers, tubeToWin, maxLevelColor, generatorVersion);
                return generatedLevel;

            case 160:
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

            case 161:
                numberTube = 11;
                numberEmptyTube = 2;
                numberInitLayers = 8;
                numberMaxLayers = 8;
                tubeToWin = 9;
                maxLevelColor = 9;
                seed = 455809077;
                generatorVersion = 1;
                rewindPercentage = 0.70f;

                setupObject.initLevelParameters(numberTube, numberEmptyTube, numberInitLayers, numberMaxLayers, tubeToWin);
                generatedLevel = levels.levelGenerator(seed,numberTube, numberEmptyTube, numberInitLayers, numberMaxLayers, tubeToWin, maxLevelColor, generatorVersion, rewindPercentage);
                return generatedLevel;

            case 162:
                numberTube = 11;
                numberEmptyTube = 1;
                numberInitLayers = 6;
                numberMaxLayers = 10;
                tubeToWin = 6;
                maxLevelColor = 6;
                generatorVersion= 1;
                seed = -1902081932;

                setupObject.initLevelParameters(numberTube, numberEmptyTube, numberInitLayers, numberMaxLayers, tubeToWin);
                generatedLevel = levels.levelGenerator(seed,numberTube, numberEmptyTube, numberInitLayers, numberMaxLayers, tubeToWin, maxLevelColor, generatorVersion);
                return generatedLevel;

            case 163:
                numberTube = 11;
                numberEmptyTube = 1;
                numberInitLayers = 6;
                numberMaxLayers = 10;
                tubeToWin = 6;
                maxLevelColor = 6;
                generatorVersion= 1;
                seed = 0;

                setupObject.initLevelParameters(numberTube, numberEmptyTube, numberInitLayers, numberMaxLayers, tubeToWin);
                generatedLevel = levels.levelGenerator(seed,numberTube, numberEmptyTube, numberInitLayers, numberMaxLayers, tubeToWin, maxLevelColor, generatorVersion);
                return generatedLevel;

            case 164:
                numberTube = 11;
                numberEmptyTube = 2;
                numberInitLayers = 7;
                numberMaxLayers = 7;
                tubeToWin = 9;
                maxLevelColor = 9;
                seed = 829406647;
                generatorVersion = 1;

                setupObject.initLevelParameters(numberTube, numberEmptyTube, numberInitLayers, numberMaxLayers, tubeToWin);
                generatedLevel = levels.levelGenerator(seed,numberTube, numberEmptyTube, numberInitLayers, numberMaxLayers, tubeToWin, maxLevelColor, generatorVersion);
                return generatedLevel;

            case 165:
                numberTube = 11;
                numberEmptyTube = 2;
                numberInitLayers = 8;
                numberMaxLayers = 8;
                tubeToWin = 9;
                maxLevelColor = 9;
                seed = 1670878279;
                generatorVersion = 1;

                setupObject.initLevelParameters(numberTube, numberEmptyTube, numberInitLayers, numberMaxLayers, tubeToWin);
                generatedLevel = levels.levelGenerator(seed,numberTube, numberEmptyTube, numberInitLayers, numberMaxLayers, tubeToWin, maxLevelColor, generatorVersion);
                return generatedLevel;

            case 166:
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

            case 167:
                numberTube = 11;
                numberEmptyTube = 2;
                numberInitLayers = 9;
                numberMaxLayers = 9;
                tubeToWin = 9;
                maxLevelColor = 9;
                seed = 2049713172;
                generatorVersion = 2;
                rewindPercentage = 0.30f;

                setupObject.initLevelParameters(numberTube, numberEmptyTube, numberInitLayers, numberMaxLayers, tubeToWin);
                generatedLevel = levels.levelGenerator(seed,numberTube, numberEmptyTube, numberInitLayers, numberMaxLayers, tubeToWin, maxLevelColor, generatorVersion, rewindPercentage);
                return generatedLevel;

            case 168:
                numberTube = 11;
                numberEmptyTube = 0;
                numberInitLayers = 6;
                numberMaxLayers = 11;
                tubeToWin = 6;
                maxLevelColor = 6;
                generatorVersion= 1;
                seed = -1755762346;

                setupObject.initLevelParameters(numberTube, numberEmptyTube, numberInitLayers, numberMaxLayers, tubeToWin);
                generatedLevel = levels.levelGenerator(seed,numberTube, numberEmptyTube, numberInitLayers, numberMaxLayers, tubeToWin, maxLevelColor, generatorVersion);
                return generatedLevel;

            case 169:
                numberTube = 11;
                numberEmptyTube = 0;
                numberInitLayers = 6;
                numberMaxLayers = 11;
                tubeToWin = 6;
                maxLevelColor = 6;
                generatorVersion= 1;
                seed = -2046840417;

                setupObject.initLevelParameters(numberTube, numberEmptyTube, numberInitLayers, numberMaxLayers, tubeToWin);
                generatedLevel = levels.levelGenerator(seed,numberTube, numberEmptyTube, numberInitLayers, numberMaxLayers, tubeToWin, maxLevelColor, generatorVersion);
                return generatedLevel;

            case 170:
                numberTube = 11;
                numberEmptyTube = 2;
                numberInitLayers = 9;
                numberMaxLayers = 9;
                tubeToWin = 9;
                maxLevelColor = 9;
                seed = -244076299;
                generatorVersion = 1;

                setupObject.initLevelParameters(numberTube, numberEmptyTube, numberInitLayers, numberMaxLayers, tubeToWin);
                generatedLevel = levels.levelGenerator(seed,numberTube, numberEmptyTube, numberInitLayers, numberMaxLayers, tubeToWin, maxLevelColor, generatorVersion);
                return generatedLevel;

            case 171:
                numberTube = 11;
                numberEmptyTube = 2;
                numberInitLayers = 9;
                numberMaxLayers = 9;
                tubeToWin = 9;
                maxLevelColor = 9;
                seed = 104482085;
                generatorVersion = 1;

                setupObject.initLevelParameters(numberTube, numberEmptyTube, numberInitLayers, numberMaxLayers, tubeToWin);
                generatedLevel = levels.levelGenerator(seed,numberTube, numberEmptyTube, numberInitLayers, numberMaxLayers, tubeToWin, maxLevelColor, generatorVersion);
                return generatedLevel;

            case 172:
                numberTube = 11;
                numberEmptyTube = 1;
                numberInitLayers = 8;
                numberMaxLayers = 10;
                tubeToWin = 8;
                maxLevelColor = 8;
                generatorVersion= 1;
                seed = 305494875;

                setupObject.initLevelParameters(numberTube, numberEmptyTube, numberInitLayers, numberMaxLayers, tubeToWin);
                generatedLevel = levels.levelGenerator(seed,numberTube, numberEmptyTube, numberInitLayers, numberMaxLayers, tubeToWin, maxLevelColor, generatorVersion);
                return generatedLevel;

            case 173:
                numberTube = 11;
                numberEmptyTube = 0;
                numberInitLayers = 8;
                numberMaxLayers = 11;
                tubeToWin = 8;
                maxLevelColor = 8;
                generatorVersion= 1;
                seed = -1494388785;

                setupObject.initLevelParameters(numberTube, numberEmptyTube, numberInitLayers, numberMaxLayers, tubeToWin);
                generatedLevel = levels.levelGenerator(seed,numberTube, numberEmptyTube, numberInitLayers, numberMaxLayers, tubeToWin, maxLevelColor, generatorVersion);
                return generatedLevel;

            case 174:
                numberTube = 12;
                numberEmptyTube = 0;
                numberInitLayers = 3;
                numberMaxLayers = 9;
                tubeToWin = 4;
                maxLevelColor = 4;
                generatorVersion= 1;
                seed = 347795249;

                setupObject.initLevelParameters(numberTube, numberEmptyTube, numberInitLayers, numberMaxLayers, tubeToWin);
                generatedLevel = levels.levelGenerator(seed,numberTube, numberEmptyTube, numberInitLayers, numberMaxLayers, tubeToWin, maxLevelColor, generatorVersion);
                return generatedLevel;

            case 175:
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

            case 176:
                numberTube = 12;
                numberEmptyTube = 0;
                numberInitLayers = 3;
                numberMaxLayers = 6;
                tubeToWin = 6;
                maxLevelColor = 6;
                generatorVersion= 1;
                seed = 633061102;

                setupObject.initLevelParameters(numberTube, numberEmptyTube, numberInitLayers, numberMaxLayers, tubeToWin);
                generatedLevel = levels.levelGenerator(seed,numberTube, numberEmptyTube, numberInitLayers, numberMaxLayers, tubeToWin, maxLevelColor, generatorVersion);
                return generatedLevel;

            case 177:
                numberTube = 12;
                numberEmptyTube = 0;
                numberInitLayers = 3;
                numberMaxLayers = 6;
                tubeToWin = 6;
                maxLevelColor = 6;
                generatorVersion= 1;
                seed = 723169680;

                setupObject.initLevelParameters(numberTube, numberEmptyTube, numberInitLayers, numberMaxLayers, tubeToWin);
                generatedLevel = levels.levelGenerator(seed,numberTube, numberEmptyTube, numberInitLayers, numberMaxLayers, tubeToWin, maxLevelColor, generatorVersion);
                return generatedLevel;

            case 178:
                numberTube = 12;
                numberEmptyTube = 2;
                numberInitLayers = 5;
                numberMaxLayers = 5;
                tubeToWin = 10;
                maxLevelColor = 10;
                seed = 959452291;
                generatorVersion = 1;

                setupObject.initLevelParameters(numberTube, numberEmptyTube, numberInitLayers, numberMaxLayers, tubeToWin);
                generatedLevel = levels.levelGenerator(seed,numberTube, numberEmptyTube, numberInitLayers, numberMaxLayers, tubeToWin, maxLevelColor, generatorVersion);
                return generatedLevel;

            case 179:
                numberTube = 12;
                numberEmptyTube = 2;
                numberInitLayers = 5;
                numberMaxLayers = 5;
                tubeToWin = 10;
                maxLevelColor = 10;
                seed = 1225663265;
                generatorVersion = 1;

                setupObject.initLevelParameters(numberTube, numberEmptyTube, numberInitLayers, numberMaxLayers, tubeToWin);
                generatedLevel = levels.levelGenerator(seed,numberTube, numberEmptyTube, numberInitLayers, numberMaxLayers, tubeToWin, maxLevelColor, generatorVersion);
                return generatedLevel;

            case 180:
                numberTube = 12;
                numberEmptyTube = 1;
                numberInitLayers = 4;
                numberMaxLayers = 11;
                tubeToWin = 4;
                maxLevelColor = 4;
                generatorVersion= 1;
                seed = 201537639;

                setupObject.initLevelParameters(numberTube, numberEmptyTube, numberInitLayers, numberMaxLayers, tubeToWin);
                generatedLevel = levels.levelGenerator(seed,numberTube, numberEmptyTube, numberInitLayers, numberMaxLayers, tubeToWin, maxLevelColor, generatorVersion);
                return generatedLevel;

            case 181:
                numberTube = 12;
                numberEmptyTube = 1;
                numberInitLayers = 4;
                numberMaxLayers = 11;
                tubeToWin = 4;
                maxLevelColor = 4;
                generatorVersion= 1;
                seed = 1447333159;

                setupObject.initLevelParameters(numberTube, numberEmptyTube, numberInitLayers, numberMaxLayers, tubeToWin);
                generatedLevel = levels.levelGenerator(seed,numberTube, numberEmptyTube, numberInitLayers, numberMaxLayers, tubeToWin, maxLevelColor, generatorVersion);
                return generatedLevel;

            case 182:
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

            case 183:
                numberTube = 12;
                numberEmptyTube = 2;
                numberInitLayers = 6;
                numberMaxLayers = 6;
                tubeToWin = 10;
                maxLevelColor = 10;
                seed = 1846870114;
                generatorVersion = 1;

                setupObject.initLevelParameters(numberTube, numberEmptyTube, numberInitLayers, numberMaxLayers, tubeToWin);
                generatedLevel = levels.levelGenerator(seed,numberTube, numberEmptyTube, numberInitLayers, numberMaxLayers, tubeToWin, maxLevelColor, generatorVersion);
                return generatedLevel;

            case 184:
                numberTube = 12;
                numberEmptyTube = 0;
                numberInitLayers = 4;
                numberMaxLayers = 6;
                tubeToWin = 8;
                maxLevelColor = 8;
                generatorVersion= 1;
                seed = 1040119417;

                setupObject.initLevelParameters(numberTube, numberEmptyTube, numberInitLayers, numberMaxLayers, tubeToWin);
                generatedLevel = levels.levelGenerator(seed,numberTube, numberEmptyTube, numberInitLayers, numberMaxLayers, tubeToWin, maxLevelColor, generatorVersion);
                return generatedLevel;

            case 185:
                numberTube = 12;
                numberEmptyTube = 0;
                numberInitLayers = 4;
                numberMaxLayers = 6;
                tubeToWin = 8;
                maxLevelColor = 8;
                generatorVersion= 1;
                seed = 2039252729;

                setupObject.initLevelParameters(numberTube, numberEmptyTube, numberInitLayers, numberMaxLayers, tubeToWin);
                generatedLevel = levels.levelGenerator(seed,numberTube, numberEmptyTube, numberInitLayers, numberMaxLayers, tubeToWin, maxLevelColor, generatorVersion);
                return generatedLevel;

            case 186:
                numberTube = 12;
                numberEmptyTube = 0;
                numberInitLayers = 4;
                numberMaxLayers = 8;
                tubeToWin = 6;
                maxLevelColor = 6;
                generatorVersion= 1;
                seed = -39126835;

                setupObject.initLevelParameters(numberTube, numberEmptyTube, numberInitLayers, numberMaxLayers, tubeToWin);
                generatedLevel = levels.levelGenerator(seed,numberTube, numberEmptyTube, numberInitLayers, numberMaxLayers, tubeToWin, maxLevelColor, generatorVersion);
                return generatedLevel;

            case 187:
                numberTube = 12;
                numberEmptyTube = 0;
                numberInitLayers = 4;
                numberMaxLayers = 8;
                tubeToWin = 6;
                maxLevelColor = 6;
                generatorVersion= 1;
                seed = -2051255234;

                setupObject.initLevelParameters(numberTube, numberEmptyTube, numberInitLayers, numberMaxLayers, tubeToWin);
                generatedLevel = levels.levelGenerator(seed,numberTube, numberEmptyTube, numberInitLayers, numberMaxLayers, tubeToWin, maxLevelColor, generatorVersion);
                return generatedLevel;

            case 188:
                numberTube = 8;
                numberEmptyTube = 0;
                numberInitLayers = 5;
                numberMaxLayers = 10;
                tubeToWin = 4;
                maxLevelColor = 4;
                generatorVersion= 1;
                seed = 700135141;

                setupObject.initLevelParameters(numberTube, numberEmptyTube, numberInitLayers, numberMaxLayers, tubeToWin);
                generatedLevel = levels.levelGenerator(seed,numberTube, numberEmptyTube, numberInitLayers, numberMaxLayers, tubeToWin, maxLevelColor, generatorVersion);
                return generatedLevel;

            case 189:
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

            case 190:
                numberTube = 12;
                numberEmptyTube = 2;
                numberInitLayers = 7;
                numberMaxLayers = 7;
                tubeToWin = 10;
                maxLevelColor = 10;
                seed = 147397908;
                generatorVersion = 1;

                setupObject.initLevelParameters(numberTube, numberEmptyTube, numberInitLayers, numberMaxLayers, tubeToWin);
                generatedLevel = levels.levelGenerator(seed,numberTube, numberEmptyTube, numberInitLayers, numberMaxLayers, tubeToWin, maxLevelColor, generatorVersion);
                return generatedLevel;

            case 191:
                numberTube = 12;
                numberEmptyTube = 2;
                numberInitLayers = 8;
                numberMaxLayers = 8;
                tubeToWin = 10;
                maxLevelColor = 10;
                seed = 662930882;
                generatorVersion = 1;

                setupObject.initLevelParameters(numberTube, numberEmptyTube, numberInitLayers, numberMaxLayers, tubeToWin);
                generatedLevel = levels.levelGenerator(seed,numberTube, numberEmptyTube, numberInitLayers, numberMaxLayers, tubeToWin, maxLevelColor, generatorVersion);
                return generatedLevel;
            case 192:
                numberTube = 12;
                numberEmptyTube = 1;
                numberInitLayers = 6;
                numberMaxLayers = 11;
                tubeToWin = 6;
                maxLevelColor = 6;
                generatorVersion= 1;
                seed = 373609458;

                setupObject.initLevelParameters(numberTube, numberEmptyTube, numberInitLayers, numberMaxLayers, tubeToWin);
                generatedLevel = levels.levelGenerator(seed,numberTube, numberEmptyTube, numberInitLayers, numberMaxLayers, tubeToWin, maxLevelColor, generatorVersion);
                return generatedLevel;

            case 193:
                numberTube = 12;
                numberEmptyTube = 1;
                numberInitLayers = 6;
                numberMaxLayers = 11;
                tubeToWin = 6;
                maxLevelColor = 6;
                generatorVersion= 1;
                seed = 843416103;

                setupObject.initLevelParameters(numberTube, numberEmptyTube, numberInitLayers, numberMaxLayers, tubeToWin);
                generatedLevel = levels.levelGenerator(seed,numberTube, numberEmptyTube, numberInitLayers, numberMaxLayers, tubeToWin, maxLevelColor, generatorVersion);
                return generatedLevel;

            case 194:
                numberTube = 12;
                numberEmptyTube = 2;
                numberInitLayers = 9;
                numberMaxLayers = 9;
                tubeToWin = 10;
                maxLevelColor = 10;
                seed = 977787953;
                generatorVersion = 1;

                setupObject.initLevelParameters(numberTube, numberEmptyTube, numberInitLayers, numberMaxLayers, tubeToWin);
                generatedLevel = levels.levelGenerator(seed,numberTube, numberEmptyTube, numberInitLayers, numberMaxLayers, tubeToWin, maxLevelColor, generatorVersion);
                return generatedLevel;

            case 195:
                numberTube = 12;
                numberEmptyTube = 1;
                numberInitLayers = 8;
                numberMaxLayers = 11;
                tubeToWin = 8;
                maxLevelColor = 8;
                generatorVersion= 1;
                seed = 577477681;

                setupObject.initLevelParameters(numberTube, numberEmptyTube, numberInitLayers, numberMaxLayers, tubeToWin);
                generatedLevel = levels.levelGenerator(seed,numberTube, numberEmptyTube, numberInitLayers, numberMaxLayers, tubeToWin, maxLevelColor, generatorVersion);
                return generatedLevel;

            case 196:
                numberTube = 12;
                numberEmptyTube = 0;
                numberInitLayers = 5;
                numberMaxLayers = 6;
                tubeToWin = 10;
                maxLevelColor = 10;
                generatorVersion= 1;
                seed = 1181655638;

                setupObject.initLevelParameters(numberTube, numberEmptyTube, numberInitLayers, numberMaxLayers, tubeToWin);
                generatedLevel = levels.levelGenerator(seed,numberTube, numberEmptyTube, numberInitLayers, numberMaxLayers, tubeToWin, maxLevelColor, generatorVersion);
                return generatedLevel;

            case 197:
                numberTube = 12;
                numberEmptyTube = 0;
                numberInitLayers = 6;
                numberMaxLayers = 9;
                tubeToWin = 8;
                maxLevelColor = 8;
                generatorVersion= 1;
                seed = 939943497;

                setupObject.initLevelParameters(numberTube, numberEmptyTube, numberInitLayers, numberMaxLayers, tubeToWin);
                generatedLevel = levels.levelGenerator(seed,numberTube, numberEmptyTube, numberInitLayers, numberMaxLayers, tubeToWin, maxLevelColor, generatorVersion);
                return generatedLevel;

            case 198:
                numberTube = 10;
                numberEmptyTube = 0;
                numberInitLayers = 6;
                numberMaxLayers = 10;
                tubeToWin = 6;
                maxLevelColor = 6;
                generatorVersion= 1;
                seed = 1063618400;

                setupObject.initLevelParameters(numberTube, numberEmptyTube, numberInitLayers, numberMaxLayers, tubeToWin);
                generatedLevel = levels.levelGenerator(seed,numberTube, numberEmptyTube, numberInitLayers, numberMaxLayers, tubeToWin, maxLevelColor, generatorVersion);
                return generatedLevel;
            
            case 199:
                numberTube = 12;
                numberEmptyTube = 2;
                numberInitLayers = 10;
                numberMaxLayers = 10;
                tubeToWin = 10;
                maxLevelColor = 10;
                seed = 1306641232;
                generatorVersion = 1;

                setupObject.initLevelParameters(numberTube, numberEmptyTube, numberInitLayers, numberMaxLayers, tubeToWin);
                generatedLevel = levels.levelGenerator(seed,numberTube, numberEmptyTube, numberInitLayers, numberMaxLayers, tubeToWin, maxLevelColor, generatorVersion);
                return generatedLevel;

            

            case 200:
                numberTube = 12;
                numberEmptyTube = 1;
                numberInitLayers = 10;
                numberMaxLayers = 11;
                tubeToWin = 10;
                maxLevelColor = 10;
                generatorVersion= 1;
                seed =  -223190038;

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
