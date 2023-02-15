using System;
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

    private float yOffset = 0.475f;

    private gameManager managerScript;


    // Start is called before the first frame update
    void Start()
    {
        initialLiquid = 2;
        maxLiquid = 3;

        managerScript = GameObject.Find("Game Manager").GetComponent<gameManager>();
        tubeComplete = false;
        for(int i = 0; i < initialLiquid; i++)
        {
            addColorLayer(managerScript.colors[UnityEngine.Random.Range(0,managerScript.colors.Count())]);
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
        tubeComplete = isComplete();
    }

    public void click()
    {
        if(!tubeComplete)
        {
            managerScript.onTubeClick(this.gameObject);
        }
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

        if (isCmplt)
        {
            completed();
        }
        return isCmplt;

    }

    private void completed()
    {
        Debug.Log("Tube complete !!!");
        this.GetComponent<Image>().raycastTarget = false;
    }

    // Update is called once per frame
    void Update()
    {
        colorCount = colorList.Count;
    }
}
