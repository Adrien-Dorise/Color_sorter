using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;
using static UnityEditor.PlayerSettings;

public class robot : MonoBehaviour
{
    [SerializeField] GameObject eyesObject, bodyObject;
    [SerializeField] Sprite eyesIdle, eyesHappy, eyesSad, eyesDubious, eyesCross, eyesHeart;
    private gameManager managerScript;
    public bool isIdle;
    public Color eyeColor;

    //Eye animation
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
    

    //Robot scaling
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

        idleSpeed = 0.0075f;
        scalingTempo = 0.005f;
        scalingSpeed = 0.05f;
        endScale = 10f;
        startScale = 9f;
        eyesIdleLoopRange = new int[2] {7,15};

        isIdling = false;
        areEyesIdling = false;
        areEyesAnimated = false;
        areEyesTracked = false;
        eyesIdleLoopMax = UnityEngine.Random.Range(eyesIdleLoopRange[0],eyesIdleLoopRange[1]);;
        eyesIdleLoopCurrent = 0;

        
    }

    public void initialise(Color color)
    {
        areEyesTracked = false;
        eyeColor = color;
        eyesObject.GetComponent<SpriteRenderer>().color = color;
        bodyObject.GetComponent<SpriteRenderer>().color = color;
    }

    public void switchEyeColor(Color color)
    {
        eyeColor = color;
        eyesObject.GetComponent<SpriteRenderer>().color = color;
        bodyObject.GetComponent<SpriteRenderer>().color = color;
    }


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

    private void eyeTracking(Transform transf)
    {
        Vector3 deltaPos = new Vector3(transf.position.x * xBoost, transf.position.y * yBoost, 0f) - eyesObject.transform.position;
        deltaPos = new Vector3(deltaPos.normalized.x, deltaPos.normalized.y, 0f); //Removed z component
        Debug.DrawRay(eyesObject.transform.localPosition, deltaPos, Color.red);
        eyesObject.transform.localPosition = Vector3.zero + deltaPos;
    }

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

    private IEnumerator eyeIdleTempoFunc(float tempo)
    {
            yield return new WaitForSeconds(tempo);
            areEyesIdling = false;
    }
    

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
    
    /*
    private IEnumerator eyeSarcasticTempo(float tempo)
    {
            yield return new WaitForSeconds(tempo);
            areEyesAnimated = false;
            eyesObject.GetComponent<SpriteRenderer>().sprite = eyesIdle;
    }
*/

    public IEnumerator happyEyes()
    {
        eyesObject.GetComponent<SpriteRenderer>().sprite = eyesHappy;
        yield return new WaitForSeconds(2f);
        eyesObject.GetComponent<SpriteRenderer>().sprite = eyesIdle;
    }

    public IEnumerator heartEyes()
    {
        eyesObject.GetComponent<SpriteRenderer>().sprite = eyesHeart;
        yield return new WaitForSeconds(2f);
        eyesObject.GetComponent<SpriteRenderer>().sprite = eyesIdle;
    }

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
