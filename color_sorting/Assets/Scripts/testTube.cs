using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class testTube : MonoBehaviour
{
    [SerializeField] private GameObject liquidPrefab;
    [SerializeField] private int liquidNumber;

    private float yOffset = 0.475f;


    // Start is called before the first frame update
    void Start()
    {
        for(int i = 0; i < liquidNumber; i++)
        {
            GameObject child = Instantiate(liquidPrefab, this.transform);
            child.transform.localScale = new Vector3(1f, 1f / liquidNumber, 1f);
            child.transform.position = new Vector3(0f, - yOffset + yOffset/liquidNumber + (i * 2 * yOffset / liquidNumber), 0f);
            child.transform.GetComponent<SpriteRenderer>().color = Random.ColorHSV();

        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
