using System.Collections;
using System.Collections.Generic;
using UnityEngine;


/// <summary>
/// Class <c>background</c> is to attach to the background gameObject of each scenes.
/// This class manages the button behaviour when players clicks background elements
/// </summary>
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
        managerScript.gameState(gameManager.actions.clickedBackground);
    }

}
