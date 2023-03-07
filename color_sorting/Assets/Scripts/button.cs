using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Class <c>button</c> 
/// </summary>
public class button : MonoBehaviour
{

    [SerializeField] selection selectScript;
    // Start is called before the first frame update
    void Start()
    {
        selectScript = GameObject.Find("Selection Canvas").GetComponent<selection>();
    }

    public void onClick()
    {
        selectScript.textLevel = GetComponentInChildren<Text>().text;
        //Debug.Log(GetComponentInChildren<Text>().text);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
