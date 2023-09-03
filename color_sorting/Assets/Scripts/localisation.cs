using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class localisation
{
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
