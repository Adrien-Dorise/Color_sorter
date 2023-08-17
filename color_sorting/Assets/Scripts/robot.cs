using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

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
    private enum eyesStates { idle, tracked, animated }
    public enum eyesActions {newIdle, animate, endAnimate, nothing}
    public enum avalaibleAnim {idle, happy, sad, dubious, sarcastic, heart, solving, goodSolution, badSolution}
    private avalaibleAnim currentAnim;
    private eyesStates currentState;
    [SerializeField] private float yOffsetMax, xOffsetMax;
    [SerializeField] private float xBoost, yBoost, eyesSpeed;
    private List<float> eyesIdleTempos;
    private List<Vector3> eyesPositions;
    [SerializeField] private float eyeTempo;
    [SerializeField] private Vector3 eyePos;
    private IEnumerator eyeRoutine;
    private int eyesIdleLoopMax, eyesIdleLoopCurrent;
    private int[] eyesIdleLoopRange;
    

    //Robot scaling (to set in Start())
    public bool isIdling;
    [SerializeField] private float scalingTempo;
    [SerializeField] private float scalingSpeed;
    [SerializeField] private float endScale;
    [SerializeField] private float startScale;
    [SerializeField] private float idleSpeed;



    private void Awake()
    {
        managerScript = GameObject.Find("Game Manager").GetComponent<gameManager>();    
    }

    void Start()
    {
        
        //Eyes animation
        xBoost = 175f; //xBoost and yBoost are for tracked state
        yBoost = 30f;
        yOffsetMax = 30f;
        xOffsetMax = 50f;
        eyesSpeed = 360f;
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
        idleSpeed = 0.00075f;
        scalingTempo = 0.005f;
        scalingSpeed = 0.005f;
        eyesIdleLoopRange = new int[2] {7,15};
        startScale = this.transform.localScale.x;
        endScale = startScale+0.10f;;

        //Initialisation
        isIdling = false;
        currentState = eyesStates.idle;
        currentAnim = avalaibleAnim.idle;
        eyesIdleLoopMax = UnityEngine.Random.Range(eyesIdleLoopRange[0],eyesIdleLoopRange[1]);;
        eyesIdleLoopCurrent = 0;
        eyesStateMachine(eyesActions.newIdle);
    }

    /// <summary>
    /// Method <c>initialise</c> setup the robot color
    /// </summary>
    /// <param name="color"> color used to init the robot </param>
    public void initialise(Color color)
    {
        eyeColor = color;
        eyesObject.GetComponent<Image>().color = color;
        bodyObject.GetComponent<Image>().color = color;
    }

    /// <summary>
    /// Method <c>switchEyeColor</c> Change the eyes of the robot
    /// </summary>
    /// <param name="color"> new eye's color </param>
    public void switchEyeColor(Color color)
    {
        eyeColor = color;
        eyesObject.GetComponent<Image>().color = color;
        bodyObject.GetComponent<Image>().color = color;
    }

    /// <summary>
    /// Method <c>robotIdle</c> Manage the behaviour of the robot when idling.
    /// The idling behaviour occurs at the start of a level when the robot has to be selected to start the level.
    /// For now, it consists of scaling up and down the robot.
    /// </summary>
    public IEnumerator robotIdle()
    {
        if (this.transform.localScale.x <= endScale)
        {
            while (this.transform.localScale.x < endScale && isIdling)
            {
                this.transform.localScale = new Vector3(this.transform.localScale.x + idleSpeed, this.transform.localScale.y + idleSpeed, 1);
                yield return new WaitForSeconds(scalingTempo);
            }

        }
        else
        {
            while (this.transform.localScale.x > startScale && isIdling)
            {
                this.transform.localScale = new Vector3(this.transform.localScale.x - idleSpeed, this.transform.localScale.y - idleSpeed, 1);
                yield return new WaitForSeconds(scalingTempo);
            }

        }
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
        Vector3 deltaPos = new Vector3(transf.position.x, transf.position.y, 0f) - eyesObject.transform.position;
        eyePos = new Vector3(deltaPos.normalized.x * xBoost, deltaPos.normalized.y * yBoost, 0f); //Removed z component
        Debug.DrawRay(eyesObject.transform.position, deltaPos*3, Color.red);
        if(Mathf.Abs(eyePos.x) > xOffsetMax)
        {
            eyePos.x = xOffsetMax * Mathf.Sign(eyePos.x);
        }
        if(Mathf.Abs(eyePos.y) > yOffsetMax)
        {
            eyePos.y = yOffsetMax * Mathf.Sign(eyePos.y);
        }
    }


    /// <summary>
    /// Method <c>eyesIdleFunc</c> is the tempo used before switching for a new position.
    /// The switching flag is symbolised to areEyesIdling bool
    /// </summary>
    /// <param name="tempo"> time before switching eyeidle bool to false </param>
    private IEnumerator eyesIdleFunc(float tempo)
    {
        //Starting new Idle position (after deselecting tube or last idle animation terminated)
        eyesIdleLoopCurrent += 1;
        eyeTempo = eyesIdleTempos[UnityEngine.Random.Range(0, eyesIdleTempos.Count)];
        eyePos = eyesPositions[UnityEngine.Random.Range(0, eyesPositions.Count)];
        yield return new WaitForSeconds(tempo);
        eyesStateMachine(eyesActions.newIdle);
    }
    

    /// <summary>
    /// Method <c>dubiousEyeAnimation</c> manage the perplex robot animation.
    /// </summary>
    private IEnumerator eyeDubious(float tempo)
    {
        eyesObject.GetComponent<Image>().sprite = eyesDubious;
        if(tempo>0)
        {
            yield return new WaitForSeconds(tempo);
            eyesStateMachine(eyesActions.endAnimate);
        }
    }


    //Animation function for fast moving eyes.
    //Tempo is teh time the eys stay in fixed position
    //State is the number of time this animation has been playing
    //Direction is the next direction of moving eyes (1 or -1)
    private IEnumerator eyeSarcastic(float tempo, int state, int direction)
    {
        
        eyesObject.GetComponent<Image>().sprite = eyesHappy;
        if(state <= 11)
        {
            yield return new WaitForSeconds(tempo);
            eyePos = new Vector3(0f, direction * (yOffsetMax/3),0f);
            state += 1;
            eyeRoutine = eyeSarcastic(tempo, state,direction*=-1);
            StartCoroutine(eyeRoutine);
        }
        else
        {
            eyesStateMachine(eyesActions.endAnimate);
        }
    }
    

    
    /// <summary>
    /// Method <c>happyEyes</c> switch idle sprite to happy sprite and reverse.
    /// </summary>
    public IEnumerator eyeHappy(float tempo)
    {
        eyesObject.GetComponent<Image>().sprite = eyesHappy;
        if(tempo>0)
        {
            yield return new WaitForSeconds(tempo);
            eyesStateMachine(eyesActions.endAnimate);
        }
    }


    /// <summary>
    /// Method <c>eyeHeart</c> switch idle sprite to heart sprite and reverse.
    /// </summary>
    public IEnumerator eyeHeart(float tempo)
    {
        eyesObject.GetComponent<Image>().sprite = eyesHeart;
        if(tempo>0)
        {
            yield return new WaitForSeconds(tempo);
            eyesStateMachine(eyesActions.endAnimate);
        }
    }

    public IEnumerator eyeCross(float tempo)
    {
        eyesObject.GetComponent<Image>().sprite = eyesCross;
        if(tempo>0)
        {
            yield return new WaitForSeconds(tempo);
            eyesStateMachine(eyesActions.endAnimate);
        }
    }
    
    /// <summary>
    /// Method <c>onClick</c> call the gameManager state machine when robot is selected.
    /// </summary>
    public void onClick()
    {
        managerScript.gameState(gameManager.actions.clickedRobot, this.gameObject);
    }



    public void eyesStateMachine(eyesActions action, avalaibleAnim anim = avalaibleAnim.idle)
    {
        switch(currentState)
        {
            case eyesStates.idle:
                if(action == eyesActions.nothing) 
                {
                    if(managerScript.memoryTube != null)//We verify if an object needs to be tracked
                    {
                        StopCoroutine(eyeRoutine);
                        currentState = eyesStates.tracked;
                        eyesObject.GetComponent<Image>().sprite = eyesIdle;
                    }
                }
                else if(action == eyesActions.animate) //Start animation
                {
                    StopCoroutine(eyeRoutine);
                    currentState = eyesStates.animated;
                    eyesObject.GetComponent<Image>().sprite = eyesIdle;
                    eyesStateMachine(eyesActions.animate, anim);
                }
                else if(action == eyesActions.newIdle) // We finished the tempo for the current eyes state and need to start a new one
                {
                    if(eyesIdleLoopCurrent >= eyesIdleLoopMax) //Starting specific eye animation (ex: dubious, sarcastic...)
                    {
                        eyesIdleLoopCurrent = 0;
                        eyesIdleLoopMax = UnityEngine.Random.Range(eyesIdleLoopRange[0],eyesIdleLoopRange[1]);
                        switch(UnityEngine.Random.Range(0,2))
                        {
                            case 0:
                                eyesStateMachine(eyesActions.animate,avalaibleAnim.dubious);
                                break;

                            case 1:
                                eyesStateMachine(eyesActions.animate,avalaibleAnim.sarcastic);
                                break;
                        }
                    }
                    else //New idle position
                    {
                        eyeRoutine = eyesIdleFunc(eyeTempo);
                        StartCoroutine(eyeRoutine);
                    }
                }
                break;

            case eyesStates.tracked:
                if(action == eyesActions.animate) //Switch to a new animation 
                {
                    currentState = eyesStates.animated;
                    eyesObject.GetComponent<Image>().sprite = eyesIdle;
                    eyesStateMachine(eyesActions.animate, anim);
                }
                else if(action == eyesActions.nothing) //Continue to track 
                {
                    if(managerScript.memoryTube == null)
                    {
                        eyesIdleLoopMax = UnityEngine.Random.Range(eyesIdleLoopRange[0],eyesIdleLoopRange[1]);
                        currentState = eyesStates.idle;
                        eyesStateMachine(eyesActions.newIdle);
                    }
                    else 
                    {
                        eyeTracking(managerScript.memoryTube.transform);
                    }
                }
                break;

            case eyesStates.animated:
                if(action == eyesActions.endAnimate) //Animation finished
                {
                    currentAnim = avalaibleAnim.idle;
                    StopCoroutine(eyeRoutine);
                    eyesObject.GetComponent<Image>().sprite = eyesIdle;
                    if(managerScript.memoryTube != null)
                    {
                        currentState = eyesStates.tracked;
                    }
                    else
                    {
                        eyesIdleLoopMax = UnityEngine.Random.Range(eyesIdleLoopRange[0],eyesIdleLoopRange[1]);
                        currentState = eyesStates.idle;
                        eyesStateMachine(eyesActions.newIdle);
                    }
                }
                else if(action == eyesActions.nothing) 
                {
                    if(managerScript.memoryTube != null && (currentAnim == avalaibleAnim.sarcastic || currentAnim == avalaibleAnim.dubious)) //Tracking can be prioritary on specific animations
                    {
                        currentAnim = avalaibleAnim.idle;
                        StopCoroutine(eyeRoutine);
                        eyesObject.GetComponent<Image>().sprite = eyesIdle;
                        currentState = eyesStates.tracked;
                    }
                }
                else if(action == eyesActions.animate)
                {
                    eyePos = Vector3.zero;
                    StopCoroutine(eyeRoutine);
                    switch(anim)
                    {
                        case avalaibleAnim.happy:
                            currentAnim = anim;
                            eyeRoutine = eyeHappy(1f);
                            StartCoroutine(eyeRoutine);
                            break;

                        case avalaibleAnim.heart:
                            currentAnim = anim;
                            eyeRoutine = eyeHeart(3f);
                            StartCoroutine(eyeRoutine);
                            break;

                        case avalaibleAnim.dubious:
                            currentAnim = anim;
                            eyeRoutine = eyeDubious(2f);
                            StartCoroutine(eyeRoutine);
                            break;

                        case avalaibleAnim.sarcastic:
                            currentAnim = anim;
                            eyeRoutine = eyeSarcastic(0.1f, 0, 1);
                            StartCoroutine(eyeRoutine);
                            break;

                        case avalaibleAnim.solving:
                            currentAnim = anim;
                            eyeRoutine = eyeDubious(0f);
                            StartCoroutine(eyeRoutine);
                            break;

                        case avalaibleAnim.goodSolution:
                            currentAnim = anim;
                            eyeRoutine = eyeHeart(0f);
                            StartCoroutine(eyeRoutine);
                            break;
                        
                        case avalaibleAnim.badSolution:
                            currentAnim = anim;
                            eyeRoutine = eyeCross(0f);
                            StartCoroutine(eyeRoutine);
                            break;
                    }
                }
                break;

        }
    }

    private void FixedUpdate()
    {
        eyesObject.transform.localPosition = Vector3.MoveTowards(eyesObject.transform.localPosition, eyePos,eyesSpeed * Time.fixedDeltaTime);
        eyesStateMachine(eyesActions.nothing);
    }



}
