using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class robot : MonoBehaviour
{
    [SerializeField] GameObject eyesObject, bodyObject;
    [SerializeField] Sprite eyesIdle, eyesHappy, eyesSad, eyesDubious, eyesCross, eyesHeart;
    private gameManager managerScript;
    public bool isIdle;
    public Color eyeColor;

    // Start is called before the first frame update
    void Start()
    {
        managerScript = GameObject.Find("Game Manager").GetComponent<gameManager>();    
    }


    public void animateSwitch()
    {
        
    }

    public void onClick()
    {
        managerScript.gameState(gameManager.actions.clickedRobot);
    }



}
