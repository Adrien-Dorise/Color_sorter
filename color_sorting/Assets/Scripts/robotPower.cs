using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class robotPower : MonoBehaviour
{
    /*
    On some levels, the player has a last color objective. A bonus goal and a malus color goal.
    Each color gives a special power up detailed in this class
    When the players finishes on either a bonus or malus color, the effect activates for the next level
    */

    public enum powerEnum {neutral, powerup, powerdown};
    //Contains whether a level is powered up or down. 
    static List<powerEnum> poweredLevel = new List<powerEnum>();
    List<GameObject> tubes;
    GameObject tubeCanvas;

    [SerializeField] GameObject prefabSolver;
    levelSolver mainSolver;

    [SerializeField] private bool do_rollBack, do_isWinnable, do_nextMove;

    private IEnumerator Start()
    {
        yield return new WaitForEndOfFrame();
        tubeCanvas = GameObject.Find("Tube Canvas").gameObject;
        tubes = new List<GameObject>();
        GameObject tubeParents = GameObject.Find("Tube Canvas").transform.GetChild(0).gameObject;
        for(int i=0; i < tubeParents.transform.childCount; i++)
        {
            tubes.Add(tubeParents.transform.GetChild(i).gameObject);
        }
        mainSolver = GameObject.Find("Level Solver").GetComponent<levelSolver>();
    }

    private void savePoweredLevels()
    {

    }

    private void loadPoweredLevels()
    {
        //0 for neutral, 1 for powered up and 2 powered down
        if(!PlayerPrefs.HasKey(save.poweredLevel)) //If first time playing the game
        {
            string poweredSave = "";
            for(int i = 0; i < save.maxAvailableLevels; i++)
            {
                poweredSave += "0";
                poweredLevel.Add(powerEnum.neutral);
            }
        }
        else
        {
            string poweredSave = PlayerPrefs.GetString(save.poweredLevel);
        }
    }

    /// <summary>
    /// Method <c>updateMove</c> must be called by the gameManager when a change occurs in the game.
    /// It updates the current power with the state of the game, in case it is needed (such as roll back one  power).
    /// </summary>
    public void updateMove(GameObject pooringTube, GameObject pooredTube, int layerPoored)
    {
       mainSolver.advanceNode(pooringTube, pooredTube, layerPoored);
    }

    // !!! Bonus !!!
    /// <summary>
    /// Method <c>rollBackOne</c> allows the player to cancel is last moves.
    /// The game comes bakc entirely at the state before the last action
    /// </summary>
    private void rollBackOne()
    {
        mainSolver.rewindNode();
    }

    /// <summary>
    /// Method <c>addTube</c> adds an empty tube in the level.
    /// </summary>
    private void addTube()
    {
        
    }

    /// <summary>
    /// Method <c>deleteColor</c> removes one color entirely from the current level.
    /// </summary>
    private void deleteColor()
    {
        
    }


    /// <summary>
    /// Method <c>isWinnable</c> indicates if the player can still win the puzzle at its current state.
    /// </summary>
    private IEnumerator isWinnable()
    {
        GameObject tubeCanvasClone = Instantiate(tubeCanvas);
        tubeCanvasClone.name = "Tube Canvas";
        tubeCanvas.SetActive(false);
        GameObject tmpSolver = Instantiate(prefabSolver);
        tubeCanvasClone.transform.SetParent(tubeCanvas.transform.parent);
        GameObject tubeParentClone = tubeCanvasClone.transform.GetChild(0).gameObject;
        List<GameObject> tmpTubes = new List<GameObject>();
        for(int i=0; i < tubeParentClone.transform.childCount; i++)
        {
            tmpTubes.Add(tubeParentClone.transform.GetChild(i).gameObject);

            //Clone of original colorList stack from original tube to new tube
            //Be careful as a simple cloning reverse the order of the stack (as stack creation from existing stack involves pop() method), hence double initalisation.
            tmpTubes[i].transform.GetComponent<testTube>().colorList = new Stack<Color>(new Stack<Color>(tubeCanvas.transform.GetChild(0).GetChild(i).GetComponent<testTube>().colorList));
        }
        yield return (tmpSolver.transform.GetComponent<levelSolver>().searchWinnable(tmpTubes));
        Debug.Log("isWinnable: " + tmpSolver.transform.GetComponent<levelSolver>().isLevelWinnable);
        yield return new WaitForSeconds(1f);
        Destroy(tubeCanvasClone);
        Destroy(tmpSolver);
        tubeCanvas.SetActive(true);
    }

    
    /// <summary>
    /// Method <c>findNextMove</c> indicates the next possible move to the player.
    /// </summary>
    private IEnumerator findNextMove()
    {
        List<GameObject> tmpTubes = new List<GameObject>();
        yield return (mainSolver.searchWinnable(tmpTubes));
        Debug.Log("isWinnable: " + mainSolver.isLevelWinnable);
    }

    // !!! Malus !!!

    /// <summary>
    /// Method <c>addTube</c> removes a tube from the level.
    /// </summary>
    private void removeTube()
    {
        
    }

    /// <summary>
    /// Method <c>timeAttack</c> set a timer that launch a game over when finished.
    /// The player has to finish the level in the given time.
    /// </summary>
    private void timeAttack()
    {

    }

    /// <summary>
    /// Method <c>addColorLayer</c> adds a color layer in the level.
    /// Instead of generating a new level, an already generated level from higher difficulty is taken.
    /// </summary>
    private void addColorLayer()
    {

    }

    /// <summary>
    /// Method <c>shadowLevel</c> set the next level in black background instead of light
    /// </summary>
    private void shadowLevel()
    {

    }

    private void Update()
    {
        if(do_isWinnable)
        {
            do_isWinnable = false;
            StartCoroutine(isWinnable());
        }
        if(do_nextMove)
        {
            do_nextMove = false;
            StartCoroutine(findNextMove());
        }
        if(do_rollBack)
        {
            do_rollBack = false;
            rollBackOne();
        }

    }

}
