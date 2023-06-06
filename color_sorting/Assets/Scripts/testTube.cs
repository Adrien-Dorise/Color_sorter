using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;


/// <summary>
/// Class <c>testTube</c> to attach to tube gameObject of a level scene.
/// This class rules tube behaviour. It includes adding/removing color layers, as well as clicked behaviour.
/// </summary>
public class testTube : MonoBehaviour
{
    //States
    [SerializeField] private GameObject liquidPrefab; //To set in editor: Reference to the liquid prefab to use when adding color layers
    [SerializeField] public int initialLiquid, maxLiquid; //Initialised by setup script: Initial sate of the tube
    private gameManager managerScript; //Reference to game manager object
    public Stack<Color> colorList = new Stack<Color>(); //Reference the current colors in the tube
    [SerializeField] int colorCount; //Number of color layer in the tube
    public bool tubeComplete; //State if the tube is full and cannot be touched anymore

    //Layer
    private float yOffset; //To set manually: Y offset between the middle and the bottom of a tube sprite


    //Tube animation
    [SerializeField] private float scalingTempo; //Time taking to up or down scale a tube
    [SerializeField] private float scalingSpeed; //Speed at which a u=tube is up or down scaled
    [SerializeField] private float endScale; //Max scale threshold when animating
    private bool isMoving; //Set true when the tube have to move from one position to another


    private void Awake()
    {
        scalingTempo = 0.005f;
        scalingSpeed = 0.025f;
        endScale = 1.3f;
        tubeComplete = false;
        isMoving = false;

        //Layer offset
        yOffset = 980f;

        foreach(Image sprite in transform.GetChild(0).GetComponentsInChildren<Image>())
        {
            sprite.enabled = false;
        }
    }
    void Start()
    {
        managerScript = GameObject.Find("Game Manager").GetComponent<gameManager>();
    }



    /// <summary>
    /// Method <c>initialise</c> setup the correct number of color layer inside the test tube.
    /// This class rules tube behaviour. It includes adding/removing color layers, as well as clicked behaviour.
    /// </summary>
    /// <param name="max"> information on the maximum possible number of layer in this tube </param>
    /// <param name="colors"> list containing color information of each layer to init</param> 
    public void initialise(int max, List<Color> colors)
    {
        initialLiquid = colors.Count;
        maxLiquid = max;

        while (colorList.Count > 0)
        {
            removeColorLayer();
        }

        foreach(Color color in colors)
        {
            addColorLayer(color);
        }
    }


    /// <summary>
    /// Method <c>addColorLayer</c> push a new layer on top of already active layers.
    /// This is done by adding a variable to the colorList global stack
    /// </summary>
    /// <param name="color"> state the color of the new layer </param>
    /// <return> gives the information if the tube is now complete
    public bool addColorLayer(Color color)
    {
        GameObject child = Instantiate(liquidPrefab, this.transform.GetChild(1));
        child.transform.localScale = new Vector3(1f, 1f / maxLiquid, 1f);
        child.transform.localPosition = new Vector3(0f, -yOffset + (yOffset / maxLiquid) + (colorList.Count * 2 * yOffset / maxLiquid), 0f);
        colorList.Push(color);
        child.transform.GetComponent<Image>().color = color;
        return isComplete();

    }


    /// <summary>
    /// Method <c>removeColorLayer</c> remove the top layer inside the tube.
    /// This is done by removing the top variable of the colorList global stack.
    /// </summary>
    public void removeColorLayer()
    {
        this.tubeComplete = false;
        try
        {
            Destroy(this.transform.GetChild(1).GetChild(colorList.Count-1).gameObject);
            colorList.Pop();
        }
        catch(Exception e) { Debug.LogException(e); }
    }


    /// <summary>
    /// Method <c>tubeScaling</c> animate the tube by up or down scaling it.
    /// Note that this is an IEnumerator to be called with StartCoroutine
    /// </summary>
    /// <param name="scaling up">: true to scale up, false to scale down </param>
    public IEnumerator tubeScaling(bool scalingUp)
    {
        if (scalingUp)
        {
            if(!save.debugLevel)
            {
                while (this.transform.localScale.x < endScale)
                {
                    this.transform.localScale = new Vector3(this.transform.localScale.x + scalingSpeed, this.transform.localScale.y + scalingSpeed, 1);
                    yield return new WaitForSeconds(scalingTempo);
                }
            }
            else
            {
                this.transform.localScale = new Vector3(endScale, endScale, 1);
            }

        }
        else
        {
            if(!save.debugLevel)
            {
                while (this.transform.localScale.x > 1)
                {
                    this.transform.localScale = new Vector3(this.transform.localScale.x - scalingSpeed, this.transform.localScale.y - scalingSpeed, 1);
                    yield return new WaitForSeconds(scalingTempo);
                }
            }
            else
            {
                this.transform.localScale = new Vector3(1, 1, 1);
            }

        }
    }

    /// <summary>
    /// Method <c>moveTube<c> move the tube to a given destination while rotating it.
    /// The displacement speed is contrained by time
    /// <param name="destination">: destination location </param>
    /// <param name="rotation">: total rotation to perform </param>
    /// <param name="time">: time given to perform the animation </param>
    public IEnumerator moveTube(Vector3 destination, float rotation, float time)
    {
        float translationOffset = 0.01f;
        float rotationOffset = 0.01f;
        float translationSpeed = (Mathf.Abs((destination - this.transform.localPosition).magnitude) / time);
        float rotationSpeed = (Mathf.Abs(rotation - this.transform.localEulerAngles.z) / time);
        double startTime = Time.realtimeSinceStartupAsDouble;

        while(Mathf.Abs((destination - this.transform.localPosition).magnitude) >= translationOffset || (Mathf.Abs(rotation - this.transform.localEulerAngles.z) >= rotationOffset && Mathf.Abs(rotation - this.transform.localEulerAngles.z) <= 360.0f - rotationOffset))
        {

            this.transform.localPosition = Vector3.MoveTowards(this.transform.localPosition, destination,translationSpeed * Time.fixedDeltaTime);
            this.transform.localRotation = Quaternion.RotateTowards(this.transform.localRotation, Quaternion.Euler(0f,0f,rotation), rotationSpeed * Time.fixedDeltaTime);

            if(Time.realtimeSinceStartupAsDouble - startTime >= time*1.1) //SafeGuard
            {
                Debug.LogWarning("moveTube animation taking too long: Force break");
                break;
            }
            yield return new WaitForFixedUpdate();
        }
    }


    /// <summary>
    /// Method <c>onClick</c> set the tube behaviour when clicked by player.
    /// For now, it refers to the gameManager state machine.
    /// </summary>
    public void onClick()
    {
        managerScript.gameState(gameManager.actions.clickedTube, this.gameObject);
    }

    
    /// <summary>
    /// Method <c>isComplete</c> set the behaviour when the colorList global stack count has reached the maxLayer threshold.
    /// For now: the tube cannot be used again by player when full.
    /// </summary>
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
        this.tubeComplete = isCmplt;
        return isCmplt;
    }

    private void FixedUpdate()
    {
        if(isMoving)
        {

        }
    }


}
