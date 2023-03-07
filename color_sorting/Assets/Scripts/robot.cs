using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;
using static UnityEditor.PlayerSettings;

/// <summary>
/// Class <c>robot</c> to attach to the robot gameObject of each scenes.
/// This class manage the robot animation, more specifically, its eyes.
/// </summary>
public class robot : MonoBehaviour
{
    //References
    [SerializeField] GameObject eyesObject, bodyObject; //To set in editor: References to robot components
    [SerializeField] Sprite eyesIdle, eyesHappy, eyesSad, eyesDubious, eyesCross, eyesHeart; //To set in editor: Sprites used to animate the eyes.
    private gameManager managerScript;
    
    //States
    public bool isIdle;
    public Color eyeColor;

    //Eye animation (to set in Start())
    private bool areEyesTracked, areEyesIdling, areEyesAnimated;
    private float yOffsetMax, xOffsetMax;
    [SerializeField] private float xBoost, yBoost, eyesSpeed;
    private List<float> eyesIdleTempos;
    private List<Vector3> eyesPositions;
    [SerializeField] private float eyeTempo;
    [SerializeField] private Vector3 eyePos;
    private IEnumerator eyeRoutine;
    private int eyesIdleLoopMax, eyesIdleLoopCurrent;
    private int[] eyesIdleLoopRange;
    

    //Robot scaling (to set in Start())
    bool isIdling;
    [SerializeField] private float scalingTempo;
    [SerializeField] private float scalingSpeed;
    [SerializeField] private float endScale;
    [SerializeField] private float startScale;
    [SerializeField] private float idleSpeed;



    // Start is called before the first frame update
    void Start()
    {
        managerScript = GameObject.Find("Game Manager").GetComponent<gameManager>();    
        
        //Eyes animation
        xBoost = 5.5f;
        yBoost = 2f;
        yOffsetMax = 0.015f;
        xOffsetMax = 0.028f;
        eyesSpeed = 0.2f;
        eyesIdleTempos = new List<float> { 3.5f , 3.5f, 3f, 2.5f, 2.5f, 2f, 1.5f, 1f, 0.5f };
        eyesPositions = new List<Vector3> { Vector3.zero,
                                            new Vector3(xOffsetMax, yOffsetMax,0),
                                            new Vector3(-xOffsetMax, -yOffsetMax,0),
                                            new Vector3(-xOffsetMax, yOffsetMax,0),
                                            //new Vector3(xOffsetMax, -yOffsetMax,0),
                                            new Vector3(xOffsetMax, 0,0),
                                            new Vector3(0, yOffsetMax,0),
                                            new Vector3(xOffsetMax, yOffsetMax,0),
                                            new Vector3(xOffsetMax, yOffsetMax,0)};


        //Scaling animation
        idleSpeed = 0.0075f;
        scalingTempo = 0.005f;
        scalingSpeed = 0.05f;
        endScale = 10f;
        startScale = 9f;
        eyesIdleLoopRange = new int[2] {7,15};

        //Initialisation
        isIdling = false;
        areEyesIdling = false;
        areEyesAnimated = false;
        areEyesTracked = false;
        eyesIdleLoopMax = UnityEngine.Random.Range(eyesIdleLoopRange[0],eyesIdleLoopRange[1]);;
        eyesIdleLoopCurrent = 0;
    }

    /// <summary>
    /// Method <c>initialise</c> setup the robot color
    /// </summary>
    /// <param name="color"> color used to init the robot </param>
    public void initialise(Color color)
    {
        areEyesTracked = false;
        eyeColor = color;
        eyesObject.GetComponent<SpriteRenderer>().color = color;
        bodyObject.GetComponent<SpriteRenderer>().color = color;
    }

    /// <summary>
    /// Method <c>switchEyeColor</c> Change the eyes of the robot
    /// </summary>
    /// <param name="color"> new eye's color </param>
    public void switchEyeColor(Color color)
    {
        eyeColor = color;
        eyesObject.GetComponent<SpriteRenderer>().color = color;
        bodyObject.GetComponent<SpriteRenderer>().color = color;
    }

    /// <summary>
    /// Method <c>robotIdle</c> Manage the behaviour of the robot when idling.
    /// The idling behaviour occurs at the start of a level when the robot has to be selected to start the level.
    /// For now, it consists of scaling up and down the robot.
    /// </summary>
    public IEnumerator robotIdle()
    {
        isIdling = true;
        if (this.transform.localScale.x <= endScale)
        {
            while (this.transform.localScale.x < endScale && gameManager.currentState == gameManager.states.idleFirstAction)
            {
                this.transform.localScale = new Vector3(this.transform.localScale.x + idleSpeed, this.transform.localScale.y + idleSpeed, 1);
                yield return new WaitForSeconds(scalingTempo);
            }

        }
        else
        {
            while (this.transform.localScale.x > startScale && gameManager.currentState == gameManager.states.idleFirstAction)
            {
                this.transform.localScale = new Vector3(this.transform.localScale.x - idleSpeed, this.transform.localScale.y - idleSpeed, 1);
                yield return new WaitForSeconds(scalingTempo);
            }

        }
        isIdling = false;
    }


    /// <summary>
    /// Method <c>robotSelected</c> manage robot behaviour when clicked.
    /// For now, we scale up or down the robot whether it is already selected
    /// </summary>
    /// <param name="scalingUp"> true to scale up the robot, false to scale down </param>
    public IEnumerator robotSelected(bool scalingUp)
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
        startScale = 9f;
            while (this.transform.localScale.x > startScale)
            {
                this.transform.localScale = new Vector3(this.transform.localScale.x - scalingSpeed, this.transform.localScale.y - scalingSpeed, 1);
                yield return new WaitForSeconds(scalingTempo);
            }

        }
    }

    /// <summary>
    /// Method <c>eyeTracking</c> set the robot eyes to look at a specific direction.
    /// It is mainly used to look at a selected tube
    /// </summary>
    /// <param name="transf"> transform component containing the position information of the object to look </param>
    private void eyeTracking(Transform transf)
    {
        Vector3 deltaPos = new Vector3(transf.position.x * xBoost, transf.position.y * yBoost, 0f) - eyesObject.transform.position;
        deltaPos = new Vector3(deltaPos.normalized.x, deltaPos.normalized.y, 0f); //Removed z component
        Debug.DrawRay(eyesObject.transform.localPosition, deltaPos, Color.red);
        eyesObject.transform.localPosition = Vector3.zero + deltaPos;
    }


    /// <summary>
    /// Method <c>eyeIdle</c> manage the eyes' idle behaviour. 
    /// Idle is when the eyes aren't tracking a specific object with eyeTracking() func.
    /// It sets up random animation to lifen the robot. 
    /// There are two possible state:
    /// <list type="bullet">
    /// <item>
    /// <description>1. Random eyes positioning: The eyes look at specific location described in eyePos variable. 
    /// The eyes stay at this position for a given amount of time before switching to another position.</description>
    /// </item>
    /// <item>
    /// <description>2. Specific eye animation: At random intervals, the eys perform a few seconds animation (wink, laugh...)</description>
    /// </item>
    /// </list>
    /// Regardless, the eyes translate is always perform to the target position when this function is called
    /// </summary>
    private void eyeIdle()
    {
        if(!areEyesIdling && !areEyesAnimated) //Starting new Idle position (after deselecting tube or last idle animation terminated)
        {
            areEyesIdling = true;
            eyesIdleLoopCurrent += 1;
            eyeTempo = eyesIdleTempos[UnityEngine.Random.Range(0, eyesIdleTempos.Count)];
            eyePos = eyesPositions[UnityEngine.Random.Range(0, eyesPositions.Count)];
            eyeRoutine = eyeIdleTempoFunc(eyeTempo);
            StartCoroutine(eyeRoutine);            
        }
        
        if(!areEyesAnimated && eyesIdleLoopCurrent >= eyesIdleLoopMax) //Starting specific eye animation (ex: dubious, sarcastic...)
        {
            StopCoroutine(eyeRoutine);
            areEyesIdling = false;
            areEyesAnimated = true;
            eyesIdleLoopCurrent = 0;
            eyesIdleLoopMax = UnityEngine.Random.Range(eyesIdleLoopRange[0],eyesIdleLoopRange[1]);
            switch(UnityEngine.Random.Range(0,2))
            {
                case 0:
                    dubiousEyeAnimation();
                    break;

                case 1:
                    sarcasticEyeAnimation();
                    break;
            }

        }

        //Debug.Log( eyesObject.transform.localPosition + " move to: " + eyePos);
        if(Mathf.Abs((eyePos - eyesObject.transform.localPosition).magnitude) >= 0.005f) //Moving eyes accordingly to eyePos variable
        {
            eyesObject.transform.Translate((eyePos - eyesObject.transform.localPosition).normalized * eyesSpeed * Time.fixedDeltaTime);
        }

    }

    /// <summary>
    /// Method <c>eyeIdleTempoFunc</c> is the tempo used before switching for a new position.
    /// The switching flag is symbolised to areEyesIdling bool
    /// </summary>
    /// <param name="tempo"> time before switching eyeidle bool to false </param>
    private IEnumerator eyeIdleTempoFunc(float tempo)
    {
            yield return new WaitForSeconds(tempo);
            areEyesIdling = false;
    }
    

    /// <summary>
    /// Method <c>dubiousEyeAnimation</c> manage the perplex robot animation.
    /// It is linked to the eyeDubiousTempo IEnumerator to manage its tempo.
    /// </summary>
    private void dubiousEyeAnimation()
    {
        eyesObject.GetComponent<SpriteRenderer>().sprite = eyesDubious;
        eyePos = Vector3.zero;
        eyeRoutine = eyeDubiousTempo(2f);
        StartCoroutine(eyeRoutine);
    }

    private IEnumerator eyeDubiousTempo(float tempo)
    {
            yield return new WaitForSeconds(tempo);
            areEyesAnimated = false;
            eyesObject.GetComponent<SpriteRenderer>().sprite = eyesIdle;
    }


    /// <summary>
    /// Method <c>sarcasticEyeAnimation</c> manage the laughing robot animation.
    /// It is linked to the eyeSarcasticTempo IEnumerator to manage its tempo.
    /// </summary>
    private void sarcasticEyeAnimation()
    {
        eyesObject.GetComponent<SpriteRenderer>().sprite = eyesHappy;
        eyePos = Vector3.zero;
        //eyeRoutine = eyeSarcasticTempo(2f);
        StartCoroutine(eyeSarcasticTempo(0.1f, 0, 1));
    }

    //Animation function for fast moving eyes.
    //Tempo is teh time the eys stay in fixed position
    //State is the number of time this animation has been playing
    //Direction is the next direction of moving eyes (1 or -1)
    private IEnumerator eyeSarcasticTempo(float tempo, int state, int direction)
    {
        if(areEyesAnimated == true) //We verify that we did not enter another state while doing this animation
        {
            if(state <= 11)
            {
                yield return new WaitForSeconds(tempo);
                eyePos = new Vector3(0f, direction * (yOffsetMax/3),0f);
                state += 1;
                StartCoroutine(eyeSarcasticTempo(tempo, state,direction*=-1));
            }
            else
            {
                eyePos = Vector3.zero;
                areEyesAnimated = false;
                eyesObject.GetComponent<SpriteRenderer>().sprite = eyesIdle;
            }
        }
    }
    

    
    /// <summary>
    /// Method <c>happyEyes</c> switch idle sprite to happy sprite and reverse.
    /// </summary>
    public IEnumerator happyEyes()
    {
        eyesObject.GetComponent<SpriteRenderer>().sprite = eyesHappy;
        yield return new WaitForSeconds(2f);
        eyesObject.GetComponent<SpriteRenderer>().sprite = eyesIdle;
    }


    /// <summary>
    /// Method <c>heartEyes</c> switch idle sprite to heart sprite and reverse.
    /// </summary>
    public IEnumerator heartEyes()
    {
        eyesObject.GetComponent<SpriteRenderer>().sprite = eyesHeart;
        yield return new WaitForSeconds(2f);
        eyesObject.GetComponent<SpriteRenderer>().sprite = eyesIdle;
    }


    
    /// <summary>
    /// Method <c>onClick</c> call the gameManager state machine when robot is selected.
    /// </summary>
    public void onClick()
    {
        managerScript.gameState(gameManager.actions.clickedRobot, this.gameObject);
    }

    private void FixedUpdate()
    {
        if(managerScript.memoryTube != null)
        {
            if(areEyesIdling)
            {
                try { StopCoroutine(eyeRoutine); }
                catch (Exception e) { Debug.Log(e); }
            }
            areEyesTracked = true;
            areEyesIdling = false;
            areEyesAnimated = false;
            eyeTracking(managerScript.memoryTube.transform);
        }
        else
        {
            if(areEyesTracked) //Case where we just finished to track a test tube
            {
                eyesIdleLoopMax = UnityEngine.Random.Range(eyesIdleLoopRange[0],eyesIdleLoopRange[1]); 
                areEyesTracked = false;
                areEyesAnimated = false;
            }
            eyeIdle();
        }

        if(gameManager.currentState == gameManager.states.idleFirstAction && !isIdling)
        {
            StartCoroutine(robotIdle());
        }
    }



}
