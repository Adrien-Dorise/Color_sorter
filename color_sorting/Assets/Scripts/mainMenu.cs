using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class mainMenu : MonoBehaviour
{

    [SerializeField] private GameObject mainCanvas, optionsCanvas, colorBlindCanvas, musicCanvas;

    // Start is called before the first frame update
    void Start()
    {
        mainCanvas.SetActive(true);
        optionsCanvas.SetActive(false);
        colorBlindCanvas.SetActive(false);
        musicCanvas.SetActive(false);
    }

    //Main Canvas
    public void quitButton()
    {
        Application.Quit();
    }
    
    public void levelSelectionButton()
    {
        SceneManager.LoadScene("Level Selection");
    }

    public void mayhemButton()
    {
        //SceneManager.LoadScene("Mayhem");
    }


    //Option button
    public void optionsButton()
    {
        mainCanvas.SetActive(false);
        optionsCanvas.SetActive(true);
        colorBlindCanvas.SetActive(false);
        musicCanvas.SetActive(false);
    }

    public void colorblindButton()
    {
        mainCanvas.SetActive(false);
        optionsCanvas.SetActive(false);
        colorBlindCanvas.SetActive(true);
        musicCanvas.SetActive(false);
    }

    public void musicButton()
    {
        mainCanvas.SetActive(false);
        optionsCanvas.SetActive(false);
        colorBlindCanvas.SetActive(false);
        musicCanvas.SetActive(true);
    }

    //Colorblind settings
    public void resetColorblind()
    {

    }

    

    public void backButton(string currentCanvas)
    {
        if (currentCanvas == "options")
        {
            mainCanvas.SetActive(true);
            optionsCanvas.SetActive(false);
            colorBlindCanvas.SetActive(false);
            musicCanvas.SetActive(false);
        } 
        else if (currentCanvas == "colorblind")
        {
            mainCanvas.SetActive(false);
            optionsCanvas.SetActive(true);
            colorBlindCanvas.SetActive(false);
            musicCanvas.SetActive(false);
        }
        else if(currentCanvas == "music")
        {
            mainCanvas.SetActive(false);
            optionsCanvas.SetActive(true);
            colorBlindCanvas.SetActive(false);
            musicCanvas.SetActive(false);
        }
        else
        {
            mainCanvas.SetActive(true);
            optionsCanvas.SetActive(false);
            colorBlindCanvas.SetActive(false);
            musicCanvas.SetActive(false);
        }
    }
}
