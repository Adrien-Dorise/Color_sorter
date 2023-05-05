using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class colorBlindSettings
{   
    private List<Color> baseColor_v1 =  new List<Color> { 
            new Color(0.067f, 0.235f, 0.859f), //Blue
            new Color(0.235f, 0.067f, 0.863f), //BluePurple
            new Color(0.435f,0.043f,0.859f), //Purple
            new Color(0.847f, 0.0f, 0.847f), //Pink
            new Color(0.5f, 0.0f, 0.5f), //PinkRed
            new Color(1.0f, 0.118f, 0.0f), //Red
            new Color(1.0f, 0.424f, 0.0f), //Orange
            new Color(0.047f, 0.373f, 0.851f), //CyanBlue
            new Color(0.004f, 0.792f, 0.827f), //Cyan
            new Color(0.0f, 0.894f, 0.166f), //Green
            //new Color(1.0f, 0.859f, 0.0f), //Yellow
            //new Color(0.0f, 0.894f, 0.5f), //Green2
        };

        private List<Color> baseColor_v2 =  new List<Color> { 
            new Color(0.839f, 0.016f, 0.106f), //Red
            new Color(0.914f,0.396f,0.043f), //Orange
            new Color(1.0f,0.894f,0.0f), //Yellow
            new Color(0.192f,0.643f,0.169f), //Dark Green
            new Color(0.710f,0.796f,0.0f), //Light Green
            new Color(0.082f,0.675f,0.871f), //Cyan
            new Color(0.0f,0.420f,0.698f), //Blue
            new Color(0.310f,0.227f,0.549f), //Purple
            new Color(0.624f,0.424f,0.655f), //Light Purple
            new Color(0.882f,0.0f,0.420f), //Pink
        };

    private List<Color> baseColor =  new List<Color> { 
            new Color(0.698f, 0.000f, 0.071f), //Red
            new Color(0.859f,0.271f,0.000f), //Orange
            new Color(1.0f,0.765f,0.000f), //Yellow
            new Color(0.580f,0.714f,0.000f), //Light Green
            new Color(0.086f,0.580f,0.086f), //Dark Green
            new Color(0.082f,0.675f,0.871f), //Cyan
            new Color(0.000f,0.310f,0.569f), //Blue
            new Color(0.247f,0.141f,0.439f), //Purple
            new Color(0.624f,0.000f,0.588f), //Light Purple
            new Color(0.906f,0.388f,0.741f), //Pink
        };
    



    //Save color system

    /// <summary>
    /// Method <c>setBaseColors</c> Saves the colors set in the baseColor variable
    /// The PlayerPrefs feature is used to save the colors in a string format. Each color is concatenated into a single string.
    /// By doing so, only one key is necessary to save all colors. 
    private void setBaseColors()
    {
        int iter = 0;
        foreach(Color col in baseColor)
        {
            PlayerPrefs.SetString(save.colors + iter, ColorUtility.ToHtmlStringRGBA(col));
            iter++;
        }
        gameManager.colors = loadColors();
    }

    /// <summary>
    /// Method <c>switchSavedColor</c> replaces a used color to a new one.
    /// It is implemented in the colorblind setting screen 
    /// It can be used in game when the player choose a new set of color for the game.
    /// </summary>
    /// <param name="colorIndex"> Index to the old color </param>
    /// <param name="newColor"> Nez color that will be placed in the index given by colorIndex </param>
    static public void switchSavedColor(int colorIndex, Color newColor)
    {
        PlayerPrefs.SetString(save.colors + colorIndex, ColorUtility.ToHtmlStringRGBA(newColor));
        gameManager.colors = loadColors();
    }

    /// <summary>
    /// Method <c>resetColorSettings</c> replaces all modification made by the player by the colors contained in the List <baseColor> 
    /// </summary>
    static public void resetColorSettings()
    {
        colorBlindSettings c = new colorBlindSettings();
        c.setBaseColors();
    }

    /// <summary>
    /// Method <c>loadColors</c> recovers the colors stored in PlayerPrefs.
    /// The string containing the colors is parsed and convert into a Color variable.
    /// If no colors are stored in PlayerPrefs (in case of first boot of the game), the colors in baseColor are used and saved. 
    /// </summary>
    /// <returns> True if same color or at least one tube empty, false otherwise </returns>
    static public List<Color> loadColors()
    {
        List<Color> colors = new List<Color>();
        int iter = 0;
        colorBlindSettings c = new colorBlindSettings();
        
        if(!PlayerPrefs.HasKey(save.colors + iter)) //No colors saved before (first game)
        {
            c.setBaseColors();      
        }

        Color color = Color.black;
        while(PlayerPrefs.HasKey(save.colors + iter))
        {
            if(ColorUtility.TryParseHtmlString("#"+PlayerPrefs.GetString(save.colors + iter), out color))
            {
                colors.Add(color); 
            }
            else
            {
                colors.Add(c.baseColor[iter]);
            }
            iter++;
        }
        return colors;
    }

    /// <summary>
    /// Method <c>returnSavedColor</c> recover the color referenced by the index in PLayerPref save
    /// If no color is found in the index (overflow), then the color black is returned
    /// </summary>
    /// <param name="index"> Index of the color to retrieve </param>
    /// <returns> type=Color: Returns the color at given index. Black if overflow </returns>
    static public Color returnSavedColor(int index)
    {
        Color color;
        if(ColorUtility.TryParseHtmlString("#"+PlayerPrefs.GetString(save.colors + index), out color))
            {
                return color; 
            }
            else
            {
                return Color.black;
            }
    }
    
    
}







