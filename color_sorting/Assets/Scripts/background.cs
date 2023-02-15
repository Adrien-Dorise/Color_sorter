using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class background : MonoBehaviour
{
    private gameManager managerScript;

    // Start is called before the first frame update
    void Start()
    {
        managerScript = GameObject.Find("Game Manager").GetComponent<gameManager>();
    }

    public void onClick()
    {
        managerScript.gameState(gameManager.actions.clickedBackround);
    }

}
