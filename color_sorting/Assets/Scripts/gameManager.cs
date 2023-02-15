using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class gameManager : MonoBehaviour
{

    private enum gameState { changeColor, takeTube, switchColor, waiting }
    public Color[] colors;
    [SerializeField]  private GameObject memoryTube;

    //Tube animation
    [SerializeField] private float scalingTempo = 0.005f;
    [SerializeField] private float scalingSpeed = 0.025f;
    [SerializeField] private float endScale = 1.3f;

    //Pooring animation
    [SerializeField] private float pooringTime = 0.8f;
    [SerializeField] private float xOffset = 0.5f;
    [SerializeField] private float yOffset = 0.5f;



    private void Awake()
    {
        colors = new Color[] { Random.ColorHSV(), Random.ColorHSV(), Random.ColorHSV() };
    }

    // Start is called before the first frame update
    void Start()
    {
        memoryTube = null;
    }

    public void onBackClick()
    {
        //Debug.Log("Background click");
        if(memoryTube != null)
        {
            StartCoroutine(tubeAnimation(memoryTube.transform, false));
            memoryTube= null;
        }
    }

    public void onTubeClick(GameObject clickedTube)
    {

        if(clickedTube.GetComponent<testTube>().colorList.Count == 0 && memoryTube == null) //Clicked tube is empty and no tube are selected
        {
            memoryTube = null;
        }

        else if(memoryTube == null) //Case where no tube is clicked beforehand
        {
            //Debug.Log("tube clicked");
            StartCoroutine(tubeAnimation(clickedTube.transform, true));
            memoryTube = clickedTube;
        }
    
        else if(memoryTube == clickedTube) //Case where the same tube is clicked
        {
            //Debug.Log("Tube declicked");
            StartCoroutine(tubeAnimation(clickedTube.transform, false));
            memoryTube = null;
        }

        else //New tube clicked
        {
            //Debug.Log("Other tube clicked");
            bool stillNotMax = clickedTube.GetComponent<testTube>().colorList.Count < clickedTube.GetComponent<testTube>().maxLiquid;
            bool notEmpty = memoryTube.GetComponent<testTube>().colorList.Count != 0;
            if (areSameColor(clickedTube, memoryTube) && stillNotMax) //Ok to switch colors
            {
                //Debug.Log("Same color");
                StartCoroutine(pooring(memoryTube, clickedTube));
                while(areSameColor(clickedTube,memoryTube) && stillNotMax && notEmpty)
                {
                    //Switch colors
                    clickedTube.GetComponent<testTube>().addColorLayer(memoryTube.GetComponent<testTube>().colorList.Peek());
                    memoryTube.GetComponent<testTube>().removeColorLayer();
                    stillNotMax = clickedTube.GetComponent<testTube>().colorList.Count < clickedTube.GetComponent<testTube>().maxLiquid;
                    notEmpty = memoryTube.GetComponent<testTube>().colorList.Count != 0;
                }
                memoryTube = null;
            }
            else //Not OK to switch color
            {
                //Debug.Log("Different color");
                StartCoroutine(tubeAnimation(memoryTube.transform, false));
                StartCoroutine(tubeAnimation(clickedTube.transform, true));
                memoryTube = clickedTube;
            }
        }
    
    }

    private bool areSameColor(GameObject tube1, GameObject tube2)
    {
        if(tube1.GetComponent<testTube>().colorList.Count == 0 || tube2.GetComponent<testTube>().colorList.Count == 0)
        {
            return true;
        }
        else
        {
            return tube1.GetComponent<testTube>().colorList.Peek() == tube2.GetComponent<testTube>().colorList.Peek();
        }
    }



    private IEnumerator tubeAnimation(Transform objectTransf, bool scalingUp)
    {
        if(scalingUp)
        {
            while(objectTransf.localScale.x < endScale)
            {
                objectTransf.localScale = new Vector3(objectTransf.localScale.x + scalingSpeed, objectTransf.localScale.y + scalingSpeed, 1);
                yield return new WaitForSeconds(scalingTempo);
            }

        }
        else
        {
            while(objectTransf.localScale.x > 1)
            {
                objectTransf.localScale = new Vector3(objectTransf.localScale.x - scalingSpeed, objectTransf.localScale.y - scalingSpeed, 1);
                yield return new WaitForSeconds(scalingTempo);
            }

        }
    }

    private IEnumerator pooring(GameObject tube1, GameObject tube2)
    {
        int xDir = 1;
        float rotation = 30f;
        Vector3 initialPosition = tube1.transform.position;
        Quaternion initialRotation = tube1.transform.rotation;
        if(tube1.transform.localPosition.x < tube2.transform.localPosition.x) //pooring tube is at right
        {
            xDir = -1;
            rotation *= -1;
        }

        //Animate
        tube1.transform.localPosition = new Vector3(tube2.transform.localPosition.x + xOffset * xDir, tube2.transform.localPosition.y + yOffset, 0f);
        tube1.transform.Rotate(new Vector3(0, 0, rotation));
        
        yield return new WaitForSeconds(pooringTime);

        //Return to initial position
        Debug.Log("Retur");
        tube1.transform.position = initialPosition;
        tube1.transform.rotation = initialRotation;
        StartCoroutine(tubeAnimation(tube1.transform, false));
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
