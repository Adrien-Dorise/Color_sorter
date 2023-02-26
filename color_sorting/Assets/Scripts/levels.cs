using JetBrains.Annotations;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class levels : MonoBehaviour
{
    static public bool Debug = true;
    static public List<Color> robotColorPerLevel = new List<Color>();


    //Levels are done like:
    // Tube1: Color1, color2, Color3
    // Tube2: Color1, color2, Color3

    static public List<List<Color>> testLevel = new List<List<Color>>()
    {
        new List<Color>{ gameManager.colors[2], gameManager.colors[1], gameManager.colors[1] },
        new List<Color>{gameManager.colors[1], gameManager.colors[1],},
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
        switch (SceneManager.GetActiveScene().name) 
        {
            case "testScene":
                return testLevel;
                break;
            case "Level1":
                return Level1;
                break;
            case "Level2":
                return Level2;
                break;
                
            case "Level3":
                return Level3;
                break;
            case "Level4":
                return Level4;
                break;
            case "Level5":
                return Level5;
                break;
            
            case "Level6":
                return Level6;
                break;
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
