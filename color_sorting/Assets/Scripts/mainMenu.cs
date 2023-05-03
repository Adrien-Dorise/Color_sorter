using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class mainMenu : MonoBehaviour
{

    [SerializeField] private GameObject mainCanvas, optionsCanvas, colorBlindCanvas, wheelCanvas, musicCanvas;

    //ColorBlind
    private int colorInSelection;
    private GameObject colorButtonsParent, backColorBlind, resetButton, dummyColorButton, dummyNewColor, wheel;
    private Color saveColor;
    private Color[] textureColor;
    private List<List<int>> colorGroups;
    private List<Color> colorGroupsIndex;
    [SerializeField] private Sprite referenceColorWheelSprite;
    private Texture2D colorWheelTexture;

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
        wheel = wheelCanvas.transform.GetChild(0).gameObject;
        StartCoroutine(initWheelColors());


        mainCanvas.SetActive(true);
        optionsCanvas.SetActive(false);
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
        setColorForColorBlindSettings();
    }

    //!!! Color blind menu !!!
    public void colorblindIndexButton(int colorIndex)
    {
        colorInSelection = colorIndex;
        colorBlindCanvas.SetActive(false);

        dummyColorButton.GetComponent<Image>().color = colorBlindSettings.returnSavedColor(colorIndex);
        dummyNewColor.GetComponent<Image>().color = colorBlindSettings.returnSavedColor(colorIndex);
        saveColor = colorBlindSettings.returnSavedColor(colorIndex);
        
        setWheelColors();
        wheelCanvas.SetActive(true);
    }


    [SerializeField] List<Color> tempColor = new List<Color>();
    private IEnumerator initWheelColors()
    {
        colorWheelTexture = new Texture2D(referenceColorWheelSprite.texture.width, referenceColorWheelSprite.texture.height, referenceColorWheelSprite.texture.format, false);
        Graphics.CopyTexture(referenceColorWheelSprite.texture,colorWheelTexture);
        wheel.GetComponent<Image>().sprite = Sprite.Create(colorWheelTexture, new Rect(0.0f, 0.0f, colorWheelTexture.width, colorWheelTexture.height), new Vector2(0.0f, 0.0f), 100.0f);
        Debug.Log(colorWheelTexture.isReadable);

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
                    tempColor.Add(textureColor[pxl]);
                    colorGroupsIndex.Add(textureColor[pxl]);
                    colorGroups.Add(new List<int>());
                    colorGroups[colorGroupsIndex.Count - 1].Add(idx);
                }
            }
            else
            {
                textureColor[pxl] = new Color(0f,0f,0f,0f);
            }
            
            if(idx %500000 == 0)
            {
                Debug.Log(idx + " / " + size);
                Debug.Log(colorGroupsIndex.Count);
                yield return new WaitForSeconds(0f);
            }
            idx++;
        }
        Debug.Log("Finished");
    }


    public bool isSameColor(Color colorA, Color colorB)
    {
        float threshold = 0.08f;
        if(Mathf.Abs(colorA.r - colorB.r) >= threshold)
        { return false; }
        if(Mathf.Abs(colorA.b - colorB.b) >= threshold)
        { return false; }
        if(Mathf.Abs(colorA.g - colorB.g) >= threshold)
        { return false; }

        return true;
    }


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
            colorBlindSettings.switchSavedColor(colorInSelection, saveColor);
            setColorForColorBlindSettings();
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
