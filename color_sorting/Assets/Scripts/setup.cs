using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class setup : MonoBehaviour
{
    //Init objects
    [SerializeField] private GameObject tubeParent;
    [SerializeField] private GameObject tubePrefab;
    [SerializeField] private GameObject robot;

    //Scene parameters
    [SerializeField] private int numberOfTube = 1;
    [SerializeField] private int numberOfInitLayers = 2;
    [SerializeField] private int numberOfMaxLayers = 4;

    //Tube positions
    [SerializeField] private float yTop, yBottom;
    [SerializeField] private float xLeft, xRight;

    // Start is called before the first frame update
    void Start()
    {
        List<Color> randomCol = new List<Color>();
        for(int i = 0; i < numberOfInitLayers; i++)
        {
            randomCol.Add(gameManager.colors[UnityEngine.Random.Range(0, gameManager.colors.Count())]);
        }

        for (int i = 0; i < numberOfTube; i++)
        {
            GameObject tube = Instantiate(tubePrefab, tubeParent.transform);
            tube.transform.localPosition = new Vector3(-1 + i * 1, 0, 0);
            tube.GetComponent<testTube>().initialise(numberOfMaxLayers, numberOfInitLayers, randomCol);
        }

        robot.GetComponent<robot>().initialise(randomCol[0]);
        //gameManager.colors[UnityEngine.Random.Range(0, gameManager.colors.Count())];
    }


}
