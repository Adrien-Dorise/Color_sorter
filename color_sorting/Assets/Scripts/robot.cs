using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;

public class robot : MonoBehaviour
{
    [SerializeField] GameObject eyesObject, bodyObject;
    [SerializeField] Sprite eyesIdle, eyesHappy, eyesSad, eyesDubious, eyesCross, eyesHeart;
    private gameManager managerScript;
    public bool isIdle;
    public Color eyeColor;

    //Eye tracking
    private bool areEyesTracked;
    private float yOffsetMax = 0.015f, xOffsetMax = 0.028f;
    [SerializeField] private float xBoost, yBoost;

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

        idleSpeed = 0.0075f;
        scalingTempo = 0.005f;
        scalingSpeed = 0.05f;
        endScale = 10f;
        startScale = 9f;

        isIdling = false;


        
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
        
        Debug.DrawRay(eyesObject.transform.localPosition, deltaPos.normalized, Color.red);
        eyesObject.transform.localPosition = Vector3.zero + (deltaPos.normalized);
    }

    private void eyeIdle()
    {
        eyesObject.transform.localPosition = Vector3.zero;
    }

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

    private void Update()
    {
        if(managerScript.memoryTube != null)
        {
            areEyesTracked = true;
            eyeTracking(managerScript.memoryTube.transform);
        }
        else
        {
            areEyesTracked = false;
            eyeIdle();
        }

        if(gameManager.currentState == gameManager.states.idleFirstAction && !isIdling)
        {
            StartCoroutine(robotIdle());
        }
    }

}
