using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;


/// <summary>
/// This class is only used in the tutorial (Scene: level1)
/// </summary>
public class tutorial : MonoBehaviour
{
    private int currentIdx;
    [SerializeField] private List<Button> tubeButton;
    bool isInit;

    // Start is called before the first frame update
    void Start()
    {
        this.transform.GetChild(currentIdx).gameObject.SetActive(true);
        currentIdx = 0;
        isInit = false;
    }

    private void Update()
    {
        if(!isInit)
        {
            isInit = true;
            int idx = 0;
            tubeButton = new List<Button>();
            foreach(Button butt in GameObject.Find("Tubes").GetComponentsInChildren<Button>())
            {
                tubeButton.Add(butt);
            }
            tubeButton[0].onClick.AddListener(() => advanceTutorial(0));
            tubeButton[1].onClick.AddListener(() => advanceTutorial(1));
            tubeButton[2].onClick.AddListener(() => advanceTutorial(2));
            tubeButton[3].onClick.AddListener(() => advanceTutorial(3));
        }   
    }

    /// <summary>
    /// This function continue the tutorial by switching the displayed arrow 
    /// </summary>
    /// <param name="buttonIdx">button index called for the tutorial, it corresponds to the four tubes in the scene</param>
    private void advanceTutorial(int buttonIdx)
    {
        StartCoroutine(advanceTutorialRoutine(buttonIdx));
    }
    
    private IEnumerator advanceTutorialRoutine(int buttonIdx)
    {
        switch(buttonIdx)
        {
            case 0:
                if (currentIdx == 2 || currentIdx == 4 || currentIdx == 6)
                {
                    this.transform.GetChild(currentIdx).gameObject.SetActive(false);
                    currentIdx = 100;
                }
                if(currentIdx == 0)
                {
                    this.transform.GetChild(currentIdx).gameObject.SetActive(false);
                    currentIdx++;
                    this.transform.GetChild(currentIdx).gameObject.SetActive(true);
                }
                if(currentIdx == 5 || currentIdx == 7)
                {
                    this.transform.GetChild(currentIdx).gameObject.SetActive(false);
                    currentIdx++;
                    yield return new WaitForSeconds(1f);
                    this.transform.GetChild(currentIdx).gameObject.SetActive(true);
                }
                break;
            
            case 1:
                if (currentIdx == 0 || currentIdx == 2 || currentIdx == 4)
                {
                    this.transform.GetChild(currentIdx).gameObject.SetActive(false);
                    currentIdx = 100;
                }
                if(currentIdx == 6 || currentIdx == 8)
                {
                    this.transform.GetChild(currentIdx).gameObject.SetActive(false);
                    currentIdx++;
                    this.transform.GetChild(currentIdx).gameObject.SetActive(true);
                }
                break;

            case 2:
                if (currentIdx == 0 || currentIdx == 6 || currentIdx == 8)
                {
                    this.transform.GetChild(currentIdx).gameObject.SetActive(false);
                    currentIdx = 100;
                }
                if(currentIdx == 2 || currentIdx == 4)
                {
                    this.transform.GetChild(currentIdx).gameObject.SetActive(false);
                    currentIdx++;
                    this.transform.GetChild(currentIdx).gameObject.SetActive(true);
                }
                if(currentIdx == 9)
                {
                    this.transform.GetChild(currentIdx).gameObject.SetActive(false);
                    currentIdx++;
                }
                break;

            case 3:
                if(currentIdx == 1 || currentIdx == 3)
                {
                    this.transform.GetChild(currentIdx).gameObject.SetActive(false);
                    currentIdx++;
                    yield return new WaitForSeconds(1);
                    this.transform.GetChild(currentIdx).gameObject.SetActive(true);
                }
                break;

            default:
                break;
        }
    }
}
