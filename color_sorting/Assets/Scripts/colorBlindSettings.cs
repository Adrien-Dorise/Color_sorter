using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class colorBlindSettings
{

    private gameManager managerScript; 
    
    const int numberOfColors = 11;    
    private Color[] baseColor =  new Color[numberOfColors] { 
            new Color(0.067f, 0.235f, 0.859f, 1), //Blue
            new Color(0.235f, 0.067f, 0.863f, 1), //BluePurple
            new Color(0.435f,0.043f,0.859f), //Purple
            new Color(0.847f, 0.0f, 0.847f), //Pink
            new Color(0.5f, 0.0f, 0.5f), //PinkRed
            new Color(1.0f, 0.118f, 0.0f), //Red
            new Color(1.0f, 0.424f, 0.0f), //Orange
            new Color(0.047f, 0.373f, 0.851f), //CyanBlue
            new Color(0.004f, 0.792f, 0.827f), //Cyan
            new Color(0.0f, 0.894f, 0.166f), //Green
            new Color(1.0f, 0.859f, 0.0f), //Yellow
            //new Color(0.0f, 0.894f, 0.5f), //Green2
        };

    


    //Save color system
    private void setBaseColors()
    {
        int iter = 0;
        foreach(Color col in baseColor)
        {
            PlayerPrefs.SetString(save.colors + iter, ColorUtility.ToHtmlStringRGBA(col));
            iter++;
        }
    }

    private void switchSavedColor(int colorIndex, Color newColor)
    {
        PlayerPrefs.SetString(save.colors + colorIndex, ColorUtility.ToHtmlStringRGBA(newColor));
    }


    static public void resetColorSettings()
    {
        colorBlindSettings c = new colorBlindSettings();
        c.setBaseColors();
    }

    static public Color[] initColors()
    {
        Color[] colors = new Color[numberOfColors];
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
                colors[iter] = color; 
            }
            else
            {
                colors[iter] = c.baseColor[iter];
            }
            iter++;
        }
        return colors;
    }

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







