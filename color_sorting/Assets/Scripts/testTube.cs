using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class testTube : MonoBehaviour
{
    [SerializeField] private GameObject liquidPrefab;
    [SerializeField] public int initialLiquid, maxLiquid;
    public Stack<Color> colorList = new Stack<Color>();
    [SerializeField] int colorCount;
    public bool tubeComplete;

    //Layer
    private float yOffset = 0.475f;

    private gameManager managerScript;

    //Tube animation
    [SerializeField] private float scalingTempo = 0.005f;
    [SerializeField] private float scalingSpeed = 0.025f;
    [SerializeField] private float endScale = 1.3f;


    // Start is called before the first frame update
    void Start()
    {
        initialLiquid = 2;
        maxLiquid = 3;

        managerScript = GameObject.Find("Game Manager").GetComponent<gameManager>();
        tubeComplete = false;
        for(int i = 0; i < initialLiquid; i++)
        {
            addColorLayer(gameManager.colors[UnityEngine.Random.Range(0, gameManager.colors.Count())]);
        }

    }


    public void addColorLayer(Color color)
    {
        GameObject child = Instantiate(liquidPrefab, this.transform);
        child.transform.localScale = new Vector3(1f, 1f / maxLiquid, 1f);
        child.transform.localPosition = new Vector3(0f, -yOffset + (yOffset / maxLiquid) + (colorList.Count * 2 * yOffset / maxLiquid), 0f);
        colorList.Push(color);
        child.transform.GetComponent<SpriteRenderer>().color = color;
        tubeComplete = isComplete();

    }


    public void removeColorLayer()
    {
        Destroy(this.transform.GetChild(colorList.Count-1).gameObject);
        colorList.Pop();
    }


    public IEnumerator tubeAnimation(bool scalingUp)
    {
        if (scalingUp)
        {
            while (this.transform.localScale.x < endScale)
            {
                this.transform.localScale = new Vector3(this.transform.localScale.x + scalingSpeed, this.transform.localScale.y + scalingSpeed, 1);
                yield return new WaitForSeconds(scalingTempo);
            }

        }
        else
        {
            while (this.transform.localScale.x > 1)
            {
                this.transform.localScale = new Vector3(this.transform.localScale.x - scalingSpeed, this.transform.localScale.y - scalingSpeed, 1);
                yield return new WaitForSeconds(scalingTempo);
            }

        }
    }

    public void onClick()
    {
        managerScript.gameState(gameManager.actions.clickedTube, this.gameObject);
    }

    private bool isComplete()
    {
        bool isCmplt = true;
        try
        {
            if (colorList.Count < maxLiquid)
            {
                isCmplt = false;
            }
            else
            {
                foreach (Color col in colorList)
                {
                    if (col != colorList.Peek())
                    {
                        isCmplt = false;
                        break;
                    }
                }
            }
        }
        catch (InvalidOperationException)
        {
            isCmplt = false;
        }
        return isCmplt;
    }




}
