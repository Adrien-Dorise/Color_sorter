using JetBrains.Annotations;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class levels : MonoBehaviour
{
    static public List<Color> robotColorPerLevel = new List<Color>();

    static public List<List<Color>> Level1 = new List<List<Color>>()
    {
        new List<Color>{ gameManager.colors[0], gameManager.colors[1], gameManager.colors[3] },
        new List<Color>{gameManager.colors[3], gameManager.colors[2],},

    };

    static List<List<Color>> Level2 = new List<List<Color>>()
    {
        new List<Color>{ gameManager.colors[0], gameManager.colors[1], gameManager.colors[3] },
        new List<Color>{ Color.blue,Color.yellow},

    };

    static public List<List<Color>> getLevelColors()
    {
        switch (SceneManager.GetActiveScene().name) 
        {
            case "testScene":
                return Level1;
                break;
            case "Level1":
                return Level1;
                break;
            case "Level2":
                return Level2;
                break;
                /*
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
