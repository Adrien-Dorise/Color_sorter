using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class localisation
{
    /// <summary>
    /// Display the current language depending of the player prefs setting
    /// </summary>
    /// <param name="localisationObject">Game Object containing the text in different languages</param>
    static public void displayCorrectLocalisation(GameObject localisationObject)
    {
        int localisationChoice = getLocalisation();
        for(int i=0; i<localisationObject.transform.childCount; i++)
        {
            if(i == localisationChoice)
            {
                localisationObject.transform.GetChild(i).gameObject.SetActive(true);
            }
            else
            {
                localisationObject.transform.GetChild(i).gameObject.SetActive(false);
            }
        }   
    }

    /// <summary>
    /// Get the player setting for localisation
    /// </summary>
    /// <returns></returns>
    static public int getLocalisation()
    {
        int language = 0;
        if(PlayerPrefs.GetString(save.localisation) == "English")
        {
            language = 0;
        }
        else if(PlayerPrefs.GetString(save.localisation) == "French")
        {
            language = 1;
        }
        else if(PlayerPrefs.GetString(save.localisation) == "Spanish")
        {
            language = 2;
        }
        else if(PlayerPrefs.GetString(save.localisation) == "Italian")
        {
            language = 3;
        }
        else if(PlayerPrefs.GetString(save.localisation) == "German")
        {
            language = 4;
        }
        return language;
    }
}
