using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;

public class gameManager : MonoBehaviour
{

    public enum states { wait, idleFirstAction, idleRobot, idleNoTube, idleTube, poorColor, endLevel, mainMenu }
    public enum actions { clickedTube, clickedRobot, clickedBackround, pooring }
    static public states currentState { get; private set; }
    static public Color[] colors;
    public GameObject memoryTube { get; private set; }
    private GameObject selectedTubeObject;
    private GameObject tubesGroupObject;
    [SerializeField] GameObject pooredLiquidPrefab;
    private robot robotScript;

    private audio audioManager;




    //Pooring animation
    [SerializeField] private float pooringTime = 0.8f;
    [SerializeField] private float translationTime = 1f;
    private float xOffset = 0.75f;
    private float yOffset = 0.5f;


    static public int availableLevels { get; private set; }
    static public string currentScene { get; set; }


    private void Awake()
    {
        currentScene = "MainMenu";
        if(SceneManager.GetActiveScene() == SceneManager.GetSceneByName("MainMenu"))
        {
            currentState = states.mainMenu;
        }
        else
        {
            currentState = states.idleFirstAction;
        }
        colors = new Color[6] { new Color(0.071f, 0.125f, 1.000f, 1), new Color(1.000f, 0.133f, 0.121f, 1), new Color(0.019f, 1.000f, 0.329f, 1), new Color(0.604f, 0.150f, 1.000f, 1), new Color(1f, 0.966f, 0.251f, 1), new Color(0.349f, 1f, 0.925f, 1) };


        if (!PlayerPrefs.HasKey("Available Levels")) //First play ! 
        {
            int colorInt = UnityEngine.Random.Range(0, 2);
            PlayerPrefs.SetInt("Available Levels", 1);
            PlayerPrefs.SetInt("Robot color Level0", colorInt);
            PlayerPrefs.SetInt("Robot color Level1", colorInt);
        }
        availableLevels = PlayerPrefs.GetInt("Available Levels");

    }

    // Start is called before the first frame update
    private void Start()
    {
        selectedTubeObject = GameObject.Find("Selected Tube");
        tubesGroupObject = GameObject.Find("Tubes");
        robotScript = GameObject.Find("Robot").GetComponent<robot>();
        audioManager = GameObject.Find("Audio Manager").GetComponent<audio>();
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



    

    private void pooring(GameObject obj)
    {
        bool stillNotMax = obj.GetComponent<testTube>().colorList.Count < obj.GetComponent<testTube>().maxLiquid;
        bool notEmpty = memoryTube.GetComponent<testTube>().colorList.Count != 0;
        int safeGuard = 0;
        robotScript.switchEyeColor(memoryTube.GetComponent<testTube>().colorList.Peek());
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

        //Sorting order update
        tube1.transform.SetParent(selectedTubeObject.transform.GetChild(0)); //We change the parent canvas to display the moving tube up front
        for (int i = 0; i < tube1.transform.childCount; i++)
        {
            tube1.transform.GetChild(i).GetComponent<SpriteRenderer>().sortingOrder += 10;
        }

        //Animate
        tube1.transform.localPosition = new Vector3(tube2.transform.localPosition.x + xOffset * xDir, tube2.transform.localPosition.y + yOffset, 0f);
        tube1.transform.Rotate(new Vector3(0, 0, rotation));

        //Add poored liquid
        GameObject tempLiquid = GameObject.Instantiate(pooredLiquidPrefab,tube1.transform.position, new Quaternion(0,0,0,0));
        tempLiquid.transform.localScale = new Vector3(-xDir, 1, 1);
        foreach(SpriteRenderer sprite in tempLiquid.GetComponentsInChildren<SpriteRenderer>())
        {
            try
            {
                sprite.color = tube1.GetComponent<testTube>().colorList.Peek();
            }
            catch(Exception ex)
            {
                Debug.LogWarning(ex);
            }
        }

        audioManager.pooringSound();        
        yield return new WaitForSeconds(pooringTime);

        //Return to initial position
        Destroy(tempLiquid);
        tube1.transform.position = initialPosition;
        tube1.transform.rotation = initialRotation;

        
        tube1.transform.SetParent(tubesGroupObject.transform);
        for (int i = 0; i < tube1.transform.childCount; i++)
        {
            tube1.transform.GetChild(i).GetComponent<SpriteRenderer>().sortingOrder -= 10;
        }
        StartCoroutine(tube1.GetComponent<testTube>().tubeScaling(false));
    }

    private IEnumerator waitState(float time, states newState)
    {
        yield return new WaitForSeconds(time);
        currentState = newState;
        if(newState == states.idleNoTube)
        {
            memoryTube = null;
        }
    }

    public void gameState(actions act, GameObject obj = null)
    {
        switch(currentState)
        {
            case states.wait:
                if(act == actions.pooring)
                {
                    StartCoroutine(waitState(pooringTime, states.idleNoTube));
                }
                break;

            case states.idleFirstAction:
                if(act == actions.clickedRobot)
                {
                    try
                    {
                        StartCoroutine(obj.GetComponent<robot>().robotSelected(true));
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
                    if(act == actions.clickedTube) 
                    {
                        testTube tubeScript = obj.GetComponent<testTube>();
                        if(tubeScript.colorList.Count <= 0) //Chose empty tube
                        {
                            StartCoroutine(robotScript.GetComponent<robot>().robotSelected(false));
                            currentState = states.idleFirstAction;
                        }
                        else if (robotScript.eyeColor != tubeScript.colorList.Peek()) //If clicked tube's upper color is different from the robot's color
                        {
                            StartCoroutine(robotScript.GetComponent<robot>().robotSelected(false));
                            bool notEmpty = tubeScript.colorList.Count != 0;
                            int safeGuard = 0, layerRemoved = 0;
                            Color previousColor = tubeScript.colorList.Peek();
                            /*
                            This can change all similar layer color to upper layer.
                            Don' know how to integrate it in the game right now
                            while (notEmpty)
                            {
                                if (safeGuard > 10)
                                {
                                    Debug.LogWarning("Safeguard reached while pooring!");
                                    break;
                                }
                                safeGuard++;

                                if (tubeScript.colorList.Peek() != previousColor)
                                {
                                    break;
                                }
                                previousColor = tubeScript.colorList.Peek();  

                                //Switch colors
                                tubeScript.removeColorLayer();
                                notEmpty = tubeScript.colorList.Count != 0;
                                layerRemoved++;
                            }
                            for (int i = 0; i < layerRemoved; i++)
                            {
                                tubeScript.addColorLayer(robotScript.eyeColor);
                            }
                            */

                            //One layer change method
                            tubeScript.removeColorLayer();
                            tubeScript.addColorLayer(robotScript.eyeColor);

                            currentState = states.idleNoTube;
                        }
                        else
                        {
                            StartCoroutine(robotScript.GetComponent<robot>().robotSelected(false));
                            currentState = states.idleFirstAction;
                        }
                    }
                    else
                    {
                        StartCoroutine(robotScript.GetComponent<robot>().robotSelected(false));
                        currentState = states.idleFirstAction;
                    }
                }
                catch(Exception ex)
                {
                    StartCoroutine(robotScript.GetComponent<robot>().robotSelected(false));
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
                            StartCoroutine(obj.GetComponent<testTube>().tubeScaling(true));
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
                            StartCoroutine(memoryTube.GetComponent<testTube>().tubeScaling(false));
                            memoryTube = null;
                            currentState = states.idleNoTube;
                        }
                        else if (obj.GetComponent<testTube>().tubeComplete) //completed tube selected
                        {
                            StartCoroutine(memoryTube.GetComponent<testTube>().tubeScaling(false));
                            memoryTube = null;
                            currentState = states.idleNoTube;
                        }
                        else if(!areSameColor(obj, memoryTube)) //New tube selected but different colors
                        {
                            StartCoroutine(memoryTube.GetComponent<testTube>().tubeScaling(false));
                            StartCoroutine(obj.GetComponent<testTube>().tubeScaling(true));
                            memoryTube = obj;
                        }
                        else if (areSameColor(memoryTube, obj) && stillNotMax) //New tube is ok to poor additional color
                        {
                            pooring(obj);
                            currentState = states.wait;
                            gameState(actions.pooring);
                        }
                        else if (areSameColor(memoryTube, obj) && !stillNotMax) //New tube is too full to be poored
                        {
                            StartCoroutine(memoryTube.GetComponent<testTube>().tubeScaling(false));
                            StartCoroutine(obj.GetComponent<testTube>().tubeScaling(true));
                            memoryTube = obj;
                        }
                    }
                    if(act == actions.clickedRobot || act == actions.clickedBackround)
                    {
                        StartCoroutine(memoryTube.GetComponent<testTube>().tubeScaling(false));
                        memoryTube = null;
                        currentState = states.idleNoTube;
                    }
                }
                catch (Exception ex)
                {
                    Debug.Log(ex);
                    StartCoroutine(memoryTube.GetComponent<testTube>().tubeScaling(false));
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
