using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using Unity.VisualScripting;
using UnityEngine;

public class gameManager : MonoBehaviour
{

    public enum states { idleFirstAction, idleRobot, idleNoTube, idleTube, poorColor, endLevel }
    public enum actions { clickedTube, clickedRobot, clickedBackround }
    static public states currentState { get; private set; }
    public Color[] colors;
    [SerializeField] private GameObject memoryTube;
    private robot robotScript;


    //Tube animation
    [SerializeField] private float scalingTempo = 0.005f;
    [SerializeField] private float scalingSpeed = 0.025f;
    [SerializeField] private float endScale = 1.3f;

    //Pooring animation
    [SerializeField] private float pooringTime = 0.8f;
    [SerializeField] private float translationTime = 1f;
    [SerializeField] private float xOffset = 0.5f;
    [SerializeField] private float yOffset = 0.5f;



    private void Awake()
    {
        currentState = states.idleNoTube;
        colors = new Color[6] { new Color(0.071f, 0.125f, 1.000f, 1), new Color(1.000f, 0.133f, 0.121f, 1), new Color(0.019f, 1.000f, 0.329f, 1), new Color(0.604f, 0.150f, 1.000f, 1), new Color(1f, 0.966f, 0.251f, 1), new Color(0.349f, 1f, 0.925f, 1) };
    }

    // Start is called before the first frame update
    private void Start()
    {
        robotScript = GameObject.Find("Robot").GetComponent<robot>();
        memoryTube = null;
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

    private void pooring(GameObject obj)
    {
        bool stillNotMax = obj.GetComponent<testTube>().colorList.Count < obj.GetComponent<testTube>().maxLiquid;
        bool notEmpty = memoryTube.GetComponent<testTube>().colorList.Count != 0;
        int safeGuard = 0;
        StartCoroutine(pooringAnimation(memoryTube, obj));
        while (areSameColor(obj, memoryTube) && stillNotMax && notEmpty)
        {
            if (safeGuard > 10)
            {
                Debug.LogWarning("Safeguard reached while pooring!");
                break;
            }
            safeGuard++;

            //Switch colors
            obj.GetComponent<testTube>().addColorLayer(memoryTube.GetComponent<testTube>().colorList.Peek());
            memoryTube.GetComponent<testTube>().removeColorLayer();
            stillNotMax = obj.GetComponent<testTube>().colorList.Count < obj.GetComponent<testTube>().maxLiquid;
            notEmpty = memoryTube.GetComponent<testTube>().colorList.Count != 0;
        }
        currentState = states.idleNoTube;
        memoryTube = null;
    }

    private IEnumerator pooringAnimation(GameObject tube1, GameObject tube2)
    {
        int xDir = 1;
        float rotation = 40f;
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
        tube1.transform.position = initialPosition;
        tube1.transform.rotation = initialRotation;
        StartCoroutine(tubeAnimation(tube1.transform, false));
    }

    public void gameState(actions act, GameObject obj = null)
    {
        switch(currentState)
        {
            case states.idleFirstAction:
                if(act == actions.clickedRobot)
                {
                    try
                    {
                        obj.GetComponent<robot>().animateSwitch();
                        currentState = states.idleRobot;
                    }
                    catch(Exception ex)
                    {
                        //Can't find the robot script -> bad gameObject
                        Debug.Log(ex);
                    }

                }
                break;

            case states.idleRobot:
                try
                {
                    testTube tubeScript = obj.GetComponent<testTube>();
                    if(act == actions.clickedTube && robotScript.eyeColor != tubeScript.colorList.Peek()) //If clicked tube's upper color is different from the robot's color
                    {
                        robotScript.animateSwitch();
                        tubeScript.removeColorLayer();
                        tubeScript.addColorLayer(robotScript.eyeColor);
                        currentState = states.idleNoTube;
                    }
                    else
                    {
                        robotScript.animateSwitch();
                        currentState = states.idleFirstAction;
                    }
                }
                catch(Exception ex)
                {
                    currentState = states.idleFirstAction;
                    Debug.Log(ex);
                }
                break;

            case states.idleNoTube:
                try
                {
                    if(act == actions.clickedTube)
                    {
                        bool notEmpty = obj.GetComponent<testTube>().colorList.Count != 0;
                        if (!obj.GetComponent<testTube>().tubeComplete && notEmpty)
                        {
                            //Debug.Log("tube clicked");
                            StartCoroutine(tubeAnimation(obj.transform, true));
                            memoryTube = obj;
                            currentState = states.idleTube;
                        }
                    }
                }
                catch(Exception ex)
                {
                    Debug.Log(ex);
                }
                break;
            
            case states.idleTube:
                try
                {
                    if(act == actions.clickedTube)
                    {
                        bool stillNotMax = obj.GetComponent<testTube>().colorList.Count < obj.GetComponent<testTube>().maxLiquid;
                        bool notEmpty = memoryTube.GetComponent<testTube>().colorList.Count != 0;
                        if(obj == memoryTube) //Same tube selected
                        {
                            StartCoroutine(tubeAnimation(memoryTube.transform, false));
                            memoryTube = null;
                            currentState = states.idleNoTube;
                        }
                        else if(!areSameColor(obj, memoryTube)) //New tube selected but different colors
                        {
                            StartCoroutine(tubeAnimation(memoryTube.transform, false));
                            StartCoroutine(tubeAnimation(obj.transform, true));
                            memoryTube = obj;
                        }
                        else if (areSameColor(memoryTube, obj) && stillNotMax) //New tube is ok to poor additional color
                        {
                            pooring(obj);
                            currentState = states.idleNoTube;
                        }
                    }
                    if(act == actions.clickedRobot || act == actions.clickedBackround)
                    {
                        StartCoroutine(tubeAnimation(memoryTube.transform, false));
                        memoryTube = null;
                        currentState = states.idleNoTube;
                    }
                }
                catch (Exception ex)
                {
                    Debug.Log(ex);
                    StartCoroutine(tubeAnimation(memoryTube.transform, false));
                    memoryTube = null;
                    currentState = states.idleNoTube;
                }
                break;
                               
                
            case states.poorColor:
                break;

            case states.endLevel:
                break;
        }
    }

    private void Update()
    {
        Debug.Log(currentState);
    }
}
