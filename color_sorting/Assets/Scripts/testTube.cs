using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.TerrainUtils;
using UnityEngine.UIElements;

public class testTube : MonoBehaviour
{
    [SerializeField] private GameObject liquidPrefab;
    [SerializeField] public int initialLiquid, maxLiquid;
    public Stack<Color> colorList = new Stack<Color>();
    [SerializeField] int colorCount;

    private float yOffset = 0.475f;

    private gameManager managerScript;


    // Start is called before the first frame update
    void Start()
    {
        initialLiquid = 4;
        maxLiquid = 6;
        managerScript = GameObject.Find("Game Manager").GetComponent<gameManager>();

        for(int i = 0; i < initialLiquid; i++)
        {
            addColorLayer(managerScript.colors[Random.Range(0,managerScript.colors.Count())]);
        }

    }


    public void addColorLayer(Color color)
    {
        GameObject child = Instantiate(liquidPrefab, this.transform);
        child.transform.localScale = new Vector3(1f, 1f / maxLiquid, 1f);
        child.transform.localPosition = new Vector3(0f, -yOffset + (yOffset / maxLiquid) + (colorList.Count * 2 * yOffset / maxLiquid), 0f);
        colorList.Push(color);
        child.transform.GetComponent<SpriteRenderer>().color = color;
    }

    public void removeColorLayer()
    {
        Destroy(this.transform.GetChild(colorList.Count-1).gameObject);
        colorList.Pop();
    }

    public void click()
    {
        managerScript.onTubeClick(this.gameObject);
    }

    // Update is called once per frame
    void Update()
    {
        colorCount = colorList.Count;
    }
}
