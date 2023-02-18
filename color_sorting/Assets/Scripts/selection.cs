using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.UI;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class selection : MonoBehaviour
{
    [SerializeField] GameObject levelIconPrefab;
    [SerializeField] private GameObject rightArrow, leftArrow;
    [SerializeField] private GameObject setupObject;


    private Color arrowsColor;

    public int currentMaxLevel;
    public string textLevel = "1"; //"1" is for init, do not touch
    private int levelPerScreen;

    // Start is called before the first frame update
    void Start()
    {
        levelPerScreen = 9;
    }

    public void initialise(Color color)
    {
        arrowsColor = color;
        rightArrow.GetComponent<Image>().color = arrowsColor;
        leftArrow.GetComponent<Image>().color = arrowsColor;
    }

    public void rightScroll()
    {
        leftArrow.SetActive(true);
        if(currentMaxLevel + levelPerScreen*2 >= gameManager.availableLevels)
        {
            currentMaxLevel = gameManager.availableLevels;
            rightArrow.SetActive(false);
        }
        else
        {
            rightArrow.SetActive(true); 
            currentMaxLevel = currentMaxLevel + levelPerScreen;
        }

        setupObject.GetComponent<setup>().displayLevelButton(currentMaxLevel);

    }

    public void leftScroll()
    {
        rightArrow.SetActive(true);
        if (currentMaxLevel - levelPerScreen < 10)
        {
            currentMaxLevel = 1;
            leftArrow.SetActive(false);
        }
        else
        {
            currentMaxLevel = currentMaxLevel - levelPerScreen;
            leftArrow.SetActive(true);
        }

        setupObject.GetComponent<setup>().displayLevelButton(currentMaxLevel);
    }

    public void onQuit()
    {
        Application.Quit();
    }

    public void levelSelection()
    {
        SceneManager.LoadScene("Level" + textLevel);
    }


}
