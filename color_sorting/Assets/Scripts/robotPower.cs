using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

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

    [SerializeField] private bool do_rollBack, do_isWinnable, do_nextMove, do_deleteColor;

    private GameObject nextPooringTube, nextPooredTube;
    private bool isStateWinnable;
    private gameManager managerScript;

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
        managerScript = GameObject.Find("Game Manager").GetComponent<gameManager>();
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

    
    public void powerButton(string selection)
    {
        managerScript.gameState(gameManager.actions.usePower,null,selection);

    }

    /// <summary>
    /// powerButtonRoutine is called by the state machine. 
    /// It performs the power action when a specific button is pressed. The power selected is laucnhed by calling the corresponding method
    /// </summary>
    /// <param name="selection">Power linked to the pressed button </param>
    public IEnumerator powerButtonRoutine(string selection)
    {
        managerScript.gameState(gameManager.actions.usePower);
        
        if(selection == "rollBack")
        {
            rollBackOne();
        }
        
        else if(selection == "isWin")
        {
            yield return StartCoroutine(isWinnable());
        }
            
        else if(selection == "nextMove")
        {
            yield return StartCoroutine(findNextMove());
        }

        else if (selection == "deleteColor")
        {
            deleteColor();
        }
        managerScript.gameState(gameManager.actions.finishAction);
    }


    private IEnumerator activateSolver()
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
        
        isStateWinnable = tmpSolver.transform.GetComponent<levelSolver>().isLevelWinnable;
        nextPooredTube = tmpSolver.transform.GetComponent<levelSolver>().nextWinnablePooredTube;
        nextPooringTube = tmpSolver.transform.GetComponent<levelSolver>().nextWinnablePooringTube;
        yield return new WaitForSeconds(1f);
        Destroy(tubeCanvasClone);
        Destroy(tmpSolver);
        tubeCanvas.SetActive(true);
    }

    // !!! Bonus !!!
    /// <summary>
    /// Method <c>rollBackOne</c> allows the player to cancel is last moves.
    /// The game comes bakc entirely at the state before the last action
    /// </summary>
    private void rollBackOne()
    {
        mainSolver.rewindNode();
        managerScript.updateCompletedTubes();
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
        Color colorToDelete=Color.black, colorReference=Color.black;
        bool isColorToDeleteFound = false, isColorReferenceFound=false, allDone=false;
        
        //Finding colors to swap
        isColorToDeleteFound = false;
        foreach(GameObject tube in tubes)
        {
            if(!tube.GetComponent<testTube>().isComplete() && tube.GetComponent<testTube>().colorList.Count > 0)
            {
                
                foreach(Color col in tube.GetComponent<testTube>().colorList.ToArray())
                {
                    if(!isColorToDeleteFound)
                    {
                        colorToDelete = col;
                        isColorToDeleteFound = true;    
                    }
                    if(isColorToDeleteFound && col != colorToDelete && !isColorReferenceFound)
                    {
                        colorReference = col;
                        isColorReferenceFound = true;
                    }
                    if(isColorToDeleteFound && isColorReferenceFound && col != colorToDelete && col != colorReference) //We make sure that at least three colors are present in the level (Avoid having only one color left)
                    {
                        allDone = true;
                        break;
                    }
                }
                if(allDone)
                {
                    break;
                }
            }
        }

        //Process swapping
        if(allDone)
        {
            List<Color> newColors = new List<Color>();
            foreach(GameObject tube in tubes)
            {
                newColors.Clear();
                int n_layers = tube.GetComponent<testTube>().colorList.Count;
                for(int i=0; i<n_layers; i++)
                {
                    newColors.Add(tube.GetComponent<testTube>().colorList.Pop());
                    if(newColors[^1] == colorToDelete)
                    {
                        newColors[^1] = colorReference;
                    }
                }

                for(int i=0; i<n_layers; i++)
                {
                    tube.GetComponent<testTube>().colorList.Push(newColors[(n_layers-1)-i]);
                    tube.transform.GetChild(1).GetChild(i).GetComponent<Image>().color = newColors[(n_layers-1)-i];
                }
            }
            managerScript.updateCompletedTubes();
            mainSolver.reInitialise();
        }
        else
        {
            Debug.Log("Impossible to swap color as level setup does not permit it");
        }
    }


    /// <summary>
    /// Method <c>isWinnable</c> indicates if the player can still win the puzzle at its current state.
    /// </summary>
    private IEnumerator isWinnable()
    {
        yield return StartCoroutine(activateSolver());
        Debug.Log("Is state winnable? " + isStateWinnable);
    }

    
    /// <summary>
    /// Method <c>findNextMove</c> indicates the next possible move to the player.
    /// </summary>
    private IEnumerator findNextMove()
    {
        yield return StartCoroutine(activateSolver());
        Debug.Log( "pooring winnable: tube " +  nextPooringTube.name + "poored winnable: tube " +  nextPooredTube.name );
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
        if(do_deleteColor)
        {
            do_deleteColor = false;
            deleteColor();
        }

    }

}
