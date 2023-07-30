using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;


public class mainMenu : MonoBehaviour
{

    [SerializeField] private GameObject mainCanvas, colorBlindCanvas, wheelCanvas, musicCanvas;

    //ColorBlind
    private int colorInSelection;
    private GameObject colorButtonsParent, backColorBlind, resetButton, dummyColorButton, dummyNewColor, wheel;
    private Color saveColor;
    private Color[] textureColor;
    private List<List<int>> colorGroups;
    private List<Color> colorGroupsIndex;
    private bool isLoadingWheelComplete;
    [SerializeField] private Sprite referenceColorWheelSprite;
    private Texture2D colorWheelTexture;
    private musicManager musicManagerScript;

    // Start is called before the first frame update
    void Start()
    {
        musicManagerScript = GameObject.Find("Music Manager").GetComponent<musicManager>();

        //ColorBlind
        colorInSelection = 0;

        colorButtonsParent = colorBlindCanvas.transform.GetChild(0).gameObject;
        resetButton = colorBlindCanvas.transform.GetChild(1).gameObject;
        backColorBlind = colorBlindCanvas.transform.GetChild(2).gameObject;
        dummyColorButton = wheelCanvas.transform.GetChild(1).gameObject;
        dummyNewColor = wheelCanvas.transform.GetChild(2).gameObject;
        wheel = wheelCanvas.transform.GetChild(0).gameObject;
        isLoadingWheelComplete = false;
        StartCoroutine(initWheelColors());


        mainCanvas.SetActive(true);
        colorBlindCanvas.SetActive(false);
        musicCanvas.SetActive(false);
        wheelCanvas.SetActive(false);

        colorButtonsParent.SetActive(true);
        resetButton.SetActive(true);
        backColorBlind.SetActive(true);
        dummyColorButton.SetActive(true);
        
    }

    //Main Canvas
    public void quitButton()
    {
        musicManagerScript.setMusicState(save.mainMenuMusicState, false);
        Application.Quit();
    }
    
    public void levelSelectionButton()
    {
        musicManagerScript.setMusicState(save.mainMenuMusicState, true);
        SceneManager.LoadScene("Level Selection");
    }

    public void mayhemButton()
    {
        //SceneManager.LoadScene("Mayhem");
    }



    public void setColorForColorBlindSettings()
    {

        //We set the correct colors for the colorblind buttons
        for(int i = 0; i < colorButtonsParent.transform.childCount; i++)
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
    }

    public void colorblindButton()
    {
        setColorForColorBlindSettings();
        mainCanvas.SetActive(false);
        colorBlindCanvas.SetActive(true);
        wheelCanvas.SetActive(false);
        musicCanvas.SetActive(false);
    }

    public void musicButton()
    {
        mainCanvas.SetActive(false);
        colorBlindCanvas.SetActive(false);
        wheelCanvas.SetActive(false);
        musicCanvas.SetActive(true);
    }

    //Colorblind settings
    public void resetColorblind()
    {
        colorBlindSettings.resetColorSettings();
        setColorForColorBlindSettings();
    }

    //!!! Color blind menu !!!

    /// <summary>
    /// Method <c>colorblindIndexButton</c> activate the wheel canvas when a color setting button is pressed by the player.
    /// The color of the button is temporay stored in case the player choose to modify it.
    /// As an algorithm setting up the wheel is set when starting the scene, a freeze can be seen if the player select the wheel canvas to before completely loading the wheel. 
    /// </summary>
    /// <param name="colorIndex"> Index of the color stored in case of switch. </param>
    public void colorblindIndexButton(int colorIndex)
    {
        colorInSelection = colorIndex;
        colorBlindCanvas.SetActive(false);

        dummyColorButton.GetComponent<Image>().color = colorBlindSettings.returnSavedColor(colorIndex);
        dummyNewColor.GetComponent<Image>().color = colorBlindSettings.returnSavedColor(colorIndex);
        saveColor = colorBlindSettings.returnSavedColor(colorIndex);
        
        setWheelColors();
        while(!isLoadingWheelComplete)
        {}
        wheelCanvas.SetActive(true);
    }

    /// <summary>
    /// Method <c>initWheelColors</c> setup the algorithm for faster wheel display.
    /// When the player activate the wheel canvas, colors already selected are removed from selection.
    /// To do so, each pixel of the wheel texture are seen to check the similarity with already stored color.
    /// This process takes time and can end up in a severe freeze when displaying the wheel.
    /// This function speed up the process by grouping similar colors' index together in a list.
    /// Therefore, when needed, only the header of the list is checked for similarities with similar colors, and linked indexes are deactivated. 
    /// Color headers are stored in colorGroupsIndex List and pixels' index are stored in colorGroups variable
    /// </summary>
    private IEnumerator initWheelColors()
    {
        colorWheelTexture = new Texture2D(referenceColorWheelSprite.texture.width, referenceColorWheelSprite.texture.height, referenceColorWheelSprite.texture.format, false);
        Graphics.CopyTexture(referenceColorWheelSprite.texture,colorWheelTexture);
        wheel.GetComponent<Image>().sprite = Sprite.Create(colorWheelTexture, new Rect(0.0f, 0.0f, colorWheelTexture.width, colorWheelTexture.height), new Vector2(0.0f, 0.0f), 100.0f);

        //Creation of color array to speed up forbidden color research speed.
        colorGroupsIndex = new List<Color>();
        colorGroups = new List<List<int>>();
        
        int idx = 0;
        textureColor = wheel.GetComponent<Image>().sprite.texture.GetPixels();
        int size = textureColor.Length;
        bool isContained = false;
        for(int pxl=0; pxl < size; pxl++)
        {   
            if(textureColor[pxl].a == 1f)
            {
                isContained = false;
                foreach(Color col in colorGroupsIndex)
                {
                    if(isSameColor(col, textureColor[pxl]))
                    {
                        isContained = true;
                        colorGroups[colorGroupsIndex.IndexOf(col)].Add(idx);
                        break;
                    }
                }
                if(!isContained)
                {
                    colorGroupsIndex.Add(textureColor[pxl]);
                    colorGroups.Add(new List<int>());
                    colorGroups[colorGroupsIndex.Count - 1].Add(idx);
                }
            }
            else
            {
                textureColor[pxl] = new Color(0f,0f,0f,0f);
            }
            
            if(idx %1000000 == 0)
            {
                yield return null;
            }
            idx++;
        }
        isLoadingWheelComplete = true;
    }

    /// <summary>
    /// Method <c>isSameColor</c> checks if two colors are close enough from each other to be considered similar
    /// The strategy used checks each channel independantly to assess similarity
    /// </summary>
    /// <param name="color1"> First color </param>
    /// <param name="color2"> Second color </param>
    /// <returns> True if same colors, false otherwise </returns>
    public bool isSameColor(Color color1, Color color2)
    {
        float threshold = 0.08f;
        if(Mathf.Abs(color1.r - color2.r) >= threshold)
        { return false; }
        if(Mathf.Abs(color1.b - color2.b) >= threshold)
        { return false; }
        if(Mathf.Abs(color1.g - color2.g) >= threshold)
        { return false; }

        return true;
    }

    /// <summary>
    /// Method <c>setWheelColors</c> modifies the wheel texture to deactivate colors that are already chosen. 
    /// It is done to ensure that each chosen colors are unique.
    /// The sstrategy used is based on colorGroups and colorGroupsIndex variables set in <initColorWheel> function.
    /// The similarity between the colors contained in colorGroupsIndex is checked. If similar, all pixels contained in the linked header are deactivated by using their index value.
    /// The texture is cloned beforehand to ensure that the original sprite stays intact when reloading the game.
    /// </summary>
    private void setWheelColors()
    {
        List<Color> forbidenColors = new List<Color>();
        forbidenColors.AddRange(gameManager.colors);
        
        Destroy(colorWheelTexture);
        colorWheelTexture = new Texture2D(referenceColorWheelSprite.texture.width, referenceColorWheelSprite.texture.height, referenceColorWheelSprite.texture.format, false);
        Graphics.CopyTexture(referenceColorWheelSprite.texture,colorWheelTexture);
        wheel.GetComponent<Image>().sprite = Sprite.Create(colorWheelTexture, new Rect(0.0f, 0.0f, colorWheelTexture.width, colorWheelTexture.height), new Vector2(0.0f, 0.0f), 100.0f);
        
        textureColor = colorWheelTexture.GetPixels();

        Debug.Log(forbidenColors[0]);
        foreach(Color forbidCol in forbidenColors)
        {
            foreach(Color wheelCol in colorGroupsIndex)
            {
                if(isSameColor(forbidCol, wheelCol))
                {
                    foreach(int idx in colorGroups[colorGroupsIndex.IndexOf(wheelCol)])
                    {
                        textureColor[idx] = Color.black;
                    }
                }
            }
        }
       
        wheel.GetComponent<Image>().sprite.texture.SetPixels(textureColor);
        wheel.GetComponent<Image>().sprite.texture.Apply();
    }

    /// <summary>
    /// Method <c>wheelButton</c> replaces a saved color by the one selected by the player on the wheel
    /// When the player clicks the wheel, the color value of the pixel selected is taken to replace the temporary saved color.
    /// </summary>
    public void wheelButton()
    {
        //Goal = get wheel color from click

        //First, we get click in regards to camera
        Vector2 playerPick = Camera.main.ScreenToWorldPoint(Input.mousePosition);

        //Then, we translate origin point to bottom left border of the sprite
        Vector2 spriteSize = new Vector2(wheel.GetComponent<Image>().rectTransform.rect.width,wheel.GetComponent<Image>().rectTransform.rect.height);
        spriteSize *= wheelCanvas.GetComponent<RectTransform>().localScale;
        Vector2 wheelPosition = wheel.GetComponent<RectTransform>().position;
        playerPick = playerPick - wheelPosition + spriteSize/2;

        //Then, we convert position to texture size
        Vector2 textureSize = new Vector2(wheel.GetComponent<Image>().sprite.texture.width,wheel.GetComponent<Image>().sprite.texture.height);
        Vector2 ratio = textureSize / spriteSize;
        playerPick = playerPick * ratio;

        //Finally, we select the color on the texture
        Color newColor = wheelCanvas.transform.GetChild(0).GetComponent<Image>().sprite.texture.GetPixel((int)playerPick.x, (int)playerPick.y);

        if(newColor != Color.white && newColor != Color.black &&  newColor.a == 1f)
        {
            //Debug.Log((int)playerPick.x + "/" + (int)playerPick.y + " = " + newColor);
            dummyNewColor.GetComponent<Image>().color = newColor;
            saveColor = newColor;
        }
        Debug.Log(newColor);
    }



    public void backButton(string currentCanvas)
    {
        if (currentCanvas == "colorblind")
        {
            mainCanvas.SetActive(true);
            colorBlindCanvas.SetActive(false);
            wheelCanvas.SetActive(false);
            musicCanvas.SetActive(false);
        }
        else if (currentCanvas == "colorWheel")
        {
            colorBlindSettings.switchSavedColor(colorInSelection, saveColor);
            setColorForColorBlindSettings();
            mainCanvas.SetActive(false);
            colorBlindCanvas.SetActive(true);
            wheelCanvas.SetActive(false);
            musicCanvas.SetActive(false);
        }
        else if(currentCanvas == "music")
        {
            mainCanvas.SetActive(true);
            colorBlindCanvas.SetActive(false);
            wheelCanvas.SetActive(false);
            musicCanvas.SetActive(false);
        }
        else
        {
            mainCanvas.SetActive(true);
            colorBlindCanvas.SetActive(false);
            wheelCanvas.SetActive(false);
            musicCanvas.SetActive(false);
        }
    }
}
