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
    [SerializeField] private float scalingTempo = 0.005f;
    [SerializeField] private float scalingSpeed = 0.025f;
    [SerializeField] private float endScale = 9.5f;

    // Start is called before the first frame update
    void Start()
    {
        managerScript = GameObject.Find("Game Manager").GetComponent<gameManager>();    
        
        xBoost = 5.5f;
        yBoost = 2f;
        yOffsetMax = 0.015f;
        xOffsetMax = 0.028f;

        scalingTempo = 0.005f;
        scalingSpeed = 0.05f;
        endScale = 10f;

        
    }

    public void initialise(Color color)
    {
        areEyesTracked = false;
        eyeColor = color;
        eyesObject.GetComponent<SpriteRenderer>().color = color;
        bodyObject.GetComponent<SpriteRenderer>().color = color;
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
            while (this.transform.localScale.x > 9)
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
    }

}
