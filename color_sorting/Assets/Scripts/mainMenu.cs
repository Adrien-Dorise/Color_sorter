using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class mainMenu : MonoBehaviour
{

    [SerializeField] private GameObject mainCanvas, optionsCanvas, colorBlindCanvas, wheelCanvas, musicCanvas;

    //ColorBlind
    private int colorInSelection;
    private GameObject colorButtonsParent, backColorBlind, resetButton, dummyColorButton, dummyNewColor;

    public Color[] dummycolors;

    // Start is called before the first frame update
    void Start()
    {

        //ColorBlind
        colorInSelection = 0;

        colorButtonsParent = colorBlindCanvas.transform.GetChild(0).gameObject;
        resetButton = colorBlindCanvas.transform.GetChild(1).gameObject;
        backColorBlind = colorBlindCanvas.transform.GetChild(2).gameObject;
        dummyColorButton = wheelCanvas.transform.GetChild(1).gameObject;
        dummyNewColor = wheelCanvas.transform.GetChild(2).gameObject;

        mainCanvas.SetActive(true);
        optionsCanvas.SetActive(false);
        colorBlindCanvas.SetActive(false);
        musicCanvas.SetActive(false);
        wheelCanvas.SetActive(false);

        colorButtonsParent.SetActive(true);
        resetButton.SetActive(true);
        backColorBlind.SetActive(true);
        dummyColorButton.SetActive(true);
        
        dummycolors = wheelCanvas.transform.GetChild(0).GetComponent<Image>().sprite.texture.GetPixels();
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
        wheelCanvas.SetActive(false);
        musicCanvas.SetActive(false);
    }

    public void colorblindButton()
    {
        //We set the correct colors for the colorblind buttons
        for(int i = 0; i < colorButtonsParent.transform.childCount-1; i++)
        {
            try
            {
                colorButtonsParent.transform.GetChild(i).GetComponent<Image>().color = gameManager.colors[i];
            }
            catch(System.Exception e)
            {
                Debug.Log("Not enough color available in game manager\n" + e);
            }
        }

        mainCanvas.SetActive(false);
        optionsCanvas.SetActive(false);
        colorBlindCanvas.SetActive(true);
        wheelCanvas.SetActive(false);
        musicCanvas.SetActive(false);
    }

    public void musicButton()
    {
        mainCanvas.SetActive(false);
        optionsCanvas.SetActive(false);
        colorBlindCanvas.SetActive(false);
        wheelCanvas.SetActive(false);
        musicCanvas.SetActive(true);
    }

    //Colorblind settings
    public void resetColorblind()
    {
        colorBlindSettings.resetColorSettings();
    }

    //!!! Color blind menu !!!
    public void colorblindIndexButton(int colorIndex)
    {
        colorInSelection = colorIndex;
        colorBlindCanvas.SetActive(false);

        dummyColorButton.GetComponent<Image>().color = colorBlindSettings.returnSavedColor(colorIndex);
        wheelCanvas.SetActive(true);
    }

    public void wheelButton()
    {
        //Touch input = Input.GetTouch(0);
        Vector3 playerPick = Input.mousePosition;

        Color[] wheelData = wheelCanvas.transform.GetChild(0).GetComponent<Image>().sprite.texture.GetPixels();
        Color newColor = wheelCanvas.transform.GetChild(0).GetComponent<Image>().sprite.texture.GetPixel((int)playerPick.x, (int)playerPick.y);
        Debug.Log((int)playerPick.x + "/" + (int)playerPick.y + " = " + newColor);
        dummyNewColor.GetComponent<Image>().color = newColor;
    }

    

    public void backButton(string currentCanvas)
    {
        if (currentCanvas == "options")
        {
            mainCanvas.SetActive(true);
            optionsCanvas.SetActive(false);
            colorBlindCanvas.SetActive(false);
            wheelCanvas.SetActive(false);
            musicCanvas.SetActive(false);
        } 
        else if (currentCanvas == "colorblind")
        {
            mainCanvas.SetActive(false);
            optionsCanvas.SetActive(true);
            colorBlindCanvas.SetActive(false);
            wheelCanvas.SetActive(false);
            musicCanvas.SetActive(false);
        }
        else if (currentCanvas == "colorWheel")
        {
            mainCanvas.SetActive(false);
            optionsCanvas.SetActive(false);
            colorBlindCanvas.SetActive(true);
            wheelCanvas.SetActive(false);
            musicCanvas.SetActive(false);
        }
        else if(currentCanvas == "music")
        {
            mainCanvas.SetActive(false);
            optionsCanvas.SetActive(true);
            colorBlindCanvas.SetActive(false);
            wheelCanvas.SetActive(false);
            musicCanvas.SetActive(false);
        }
        else
        {
            mainCanvas.SetActive(true);
            optionsCanvas.SetActive(false);
            colorBlindCanvas.SetActive(false);
            wheelCanvas.SetActive(false);
            musicCanvas.SetActive(false);
        }
    }
}
