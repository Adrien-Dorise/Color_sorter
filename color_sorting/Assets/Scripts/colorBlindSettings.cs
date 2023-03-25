using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class colorBlindSettings : MonoBehaviour
{
    private gameManager managerScript; 

    // Start is called before the first frame update
    void Start()
    {
        managerScript = GameObject.Find("Game Manager").GetComponent<gameManager>();
        
        for(int i = 0; i < this.transform.childCount-1; i++)
        {
            try
            {
                this.transform.GetChild(i).GetComponent<Image>().color = gameManager.colors[i];
            }
            catch(System.Exception e)
            {
                Debug.Log("Not enough color available in game manager\n" + e);
            }
        }
    }

    


}
