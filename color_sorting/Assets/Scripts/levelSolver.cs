using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class levelSolver : MonoBehaviour
{
    public bool debugLog = false;
    public bool stopAtWin = true;
    private GameObject tubeParents;
    private List<GameObject> tubes;
    private int nodeID = 0;
    private List<List<string>> statesVisited; //Consist of the R value of the color
    public int count=0;
    public float waitTime;
    public bool isLevelWinnable {get; private set;}

    
    [SerializeField] bool scanNewActions = false;
    [SerializeField] bool doAction = false;
    [SerializeField] bool doRewind = false;
    [SerializeField] bool doSolveGraph = false;
    private node currentNode;


    private class node
    {
        public node previousNode {get;private set;}
        public action previousAction {get; private set;}
        public List<action> nextActions {get; private set;}
        public List<int> actionsToExplore {get; private set;}
        public bool isWinnable;
        private List<int> winnableNodes;
        public List<node> nextVisitedNodes;

        public node(List<GameObject> tubesAvailable, action prevAction, node prevNode)
        {
            actionsToExplore = new List<int>();
            nextActions = new List<action>();
            nextVisitedNodes = new List<node>();
            isWinnable = false;
            winnableNodes = new List<int>();
            previousAction = prevAction;
            previousNode = prevNode;
            findActions(tubesAvailable);
        }


        private void findActions(List<GameObject> tubesAvailable)
        {
            int pooringID=1, pooredID=1;
            bool differentAction, pooringPossible, tubesIncomplete, allPooringLayersFit, notSwitchingTube;
            foreach(GameObject pooringTube in tubesAvailable)
            {
                pooredID=1;
                foreach(GameObject pooredTube in tubesAvailable)
                {
                    if(previousAction == null)
                    {
                        differentAction = true;
                    }
                    else
                    {
                        differentAction = !(pooredTube == previousAction.pooringTube && pooringTube == previousAction.pooredTube);
                    }
                    pooringPossible = isPooringPossible(pooringTube, pooredTube);
                    tubesIncomplete = !pooringTube.GetComponent<testTube>().isComplete() && !pooredTube.GetComponent<testTube>().isComplete();
                    
                    //Check if we can poor all similar layers into new tube
                    int n_layersToPoor = 1;
                    int n_pooringLayersAvailable = pooredTube.GetComponent<testTube>().maxLiquid - pooredTube.GetComponent<testTube>().colorList.Count;
                    if(pooringTube.GetComponent<testTube>().colorList.Count > 1)
                    {
                        Color col = pooringTube.GetComponent<testTube>().colorList.ToArray()[0];
                        while(pooringTube.GetComponent<testTube>().colorList.ToArray()[n_layersToPoor] == col)
                        {
                            n_layersToPoor++;
                            if(n_layersToPoor >= pooringTube.GetComponent<testTube>().colorList.Count)
                            {
                                break;
                            }
                        }
                    }
                    allPooringLayersFit = n_layersToPoor <= n_pooringLayersAvailable;

                    //We verify that this move is not switching all layers from the pooring tube to an empty poored one, causing a useless move to be performed
                    notSwitchingTube = !(n_layersToPoor == pooringTube.GetComponent<testTube>().colorList.Count && pooredTube.GetComponent<testTube>().colorList.Count == 0);

                    if(differentAction && pooringPossible && tubesIncomplete && allPooringLayersFit && notSwitchingTube)
                    {
                        nextActions.Add(new action(pooringTube, pooredTube));
                        actionsToExplore.Add(actionsToExplore.Count);
                    }
                    pooredID++;
                }
                pooringID++;
            } 
        }


        public node performAction(List<GameObject> tubesAvailable, int actionID)
        {
            int pooredLayers = gameManager.pooringAction(nextActions[actionID].pooringTube, nextActions[actionID].pooredTube);
            nextActions[actionID].setLayerPoored(pooredLayers);
            node nextNode =  new node(tubesAvailable, nextActions[actionID], this);
            this.nextVisitedNodes.Add(nextNode);

            return nextNode;
        }

        public node rewindAction(List<GameObject> tubesAvailable)
        {    
            if(previousAction == null)
            {
                Debug.LogWarning("Can't rewind action as there is no action to rewind (maybe it is the start of the level)");
                return this;
            }
            if(previousAction.layersPoored == 0)
            {
                Debug.LogWarning("Can't rewind action as it does not have the information on the number of layer to rewind (layersPoor = 0)");
                return this;
            }
            Color rewindColor = Color.black; 
            try
            {
                rewindColor = previousAction.pooredTube.GetComponent<testTube>().colorList.Peek();
            }
            catch(Exception e)
            {
                Debug.LogWarning("Error when rewinding: " + previousAction.pooredTube.name + " -> " + previousAction.pooringTube.name);
            }
            for(int i = 0; i < previousAction.layersPoored; i++)
            {
                previousAction.pooringTube.GetComponent<testTube>().addColorLayer(rewindColor);
                previousAction.pooredTube.GetComponent<testTube>().removeColorLayer();
            }
            return previousNode;
        }
    }

    private class action
    {
        public GameObject pooringTube {get; private set;}
        public GameObject pooredTube {get; private set;}
        public int layersPoored {get; private set;}
        public void setLayerPoored(int num)
        {
            layersPoored = num;
        }

        public void printAction()
        {
            Debug.Log("Action: " + pooringTube.name + " -> " + pooredTube.name);
        }

        public action(GameObject pooring, GameObject poored)
        {
            pooringTube = pooring;
            pooredTube = poored;
            layersPoored = 0;
        }

        public action(GameObject pooring, GameObject poored, int layPoored)
        {
            pooringTube = pooring;
            pooredTube = poored;
            layersPoored = layPoored;
        }
    }


    private bool isWin(List<GameObject> availableTubes)
    {
        foreach(GameObject tube in availableTubes)
        {
            if(!tube.GetComponent<testTube>().tubeComplete && tube.GetComponent<testTube>().colorList.Count != 0)
            {
                return false;
            }
        }
        return true;
    }

    static public bool isPooringPossible(GameObject pooringTube, GameObject pooredTube)
    {
        bool stillNotMax = pooredTube.GetComponent<testTube>().colorList.Count < pooredTube.GetComponent<testTube>().maxLiquid;
        bool notEmpty = pooringTube.GetComponent<testTube>().colorList.Count != 0;
        if(pooringTube.GetComponent<testTube>().colorList.Count == 0) //Tube empty
        {
            return false;
        }
        if(pooredTube == pooringTube) //Same tube selected
        {
            return false;
        }
        else if (pooredTube.GetComponent<testTube>().tubeComplete) //completed tube selected
        {
            return false;
        }
        else if(!gameManager.areSameColor(pooredTube, pooringTube)) //New tube selected but different colors
        {
            return false;
        }
        else if (gameManager.areSameColor(pooringTube, pooredTube) && stillNotMax) //New tube is ok to poor additional color
        {
            return true;
        }
        else if (gameManager.areSameColor(pooringTube, pooredTube) && !stillNotMax) //New tube is too full to be poored
        {

            return false;
        }
        
        return false;
    }

    private bool isVisitedState(List<GameObject> tubes)
    {
        List<string> currentState = new List<string>();
        foreach(GameObject tube in tubes)
        {
            string tubeLayers = "";
            foreach(Image layerColor in tube.transform.GetChild(1).GetComponentsInChildren<Image>())
            {
                tubeLayers += layerColor.color.ToString();
            }
            currentState.Add(tubeLayers);
        }

        bool isSimilar;
        foreach(List<string> knownstate in statesVisited)
        {
            isSimilar = true;
            for(int i=0; i<knownstate.Count; i++)
            {
                if(knownstate[i] != currentState[i])
                {
                    isSimilar = false;
                    break;
                }
            }
            if(isSimilar)
            {
                //If we arrive here, it means that all layer from all tubes are similar.
                return true;
            }
        }
        statesVisited.Add(currentState);
        count = statesVisited.Count;
        return false;
    }

    IEnumerator wait(bool isFinish)
    {
        yield return new WaitForSeconds(1.5f);
        isFinish = true;
    }


    bool isWinnable = false;
    private IEnumerator resolveGraph(List<GameObject> availableTubes, node initialNode, int nodeIdx)
    {

        nodeID++;
        node tmpNode = initialNode;
        if(!isVisitedState(availableTubes))
        {
            if(isWin(availableTubes))
            {
                isWinnable = true;
                if(debugLog){Debug.Log("WIN node" + nodeIdx);}
                node winNodes = initialNode;
                currentNode.isWinnable = true;
                while(winNodes.previousNode != null)
                {
                    winNodes.isWinnable = true;
                    winNodes = winNodes.previousNode;
                }
                winNodes.isWinnable = true;
                yield return new WaitForSeconds(waitTime);
                if(stopAtWin)
                {
                    if(debugLog)
                    {
                        Debug.Log("Faster break as a win position is reached");
                    }
                    yield break;
                }
              
            }
            else
            {
                foreach(int actID in initialNode.actionsToExplore)
                {
                    if(debugLog){Debug.Log("node" + nodeIdx + ": " + (actID+1) + " / " + initialNode.actionsToExplore.Count + " actions");}
                    float initTimer = Time.time;
                    yield return new WaitForSeconds(waitTime);
                    tmpNode = initialNode.performAction(availableTubes,actID);
                    if(debugLog){initialNode.nextActions[actID].printAction();}
                    yield return StartCoroutine(resolveGraph(availableTubes, tmpNode, nodeID));
                    if(stopAtWin && isWinnable)
                    {
                        yield break;
                    }
                    
                }

            }
            if(debugLog){Debug.Log("All actions seen in node" + nodeIdx);}
        }
        else
        {
            if(debugLog){Debug.Log("State already visited for node" + nodeIdx);}
        }

        if(nodeIdx == 0)
        {
            if(debugLog)
            {
                Debug.Log("All graph complete ! Is it possible to finish the level: " + isWinnable);   
            }
        }
        else
        {
            initialNode.rewindAction(availableTubes);
        }
        yield return new WaitForSeconds(waitTime);
    }

    


    public void advanceNode(GameObject pooringTube, GameObject pooredTube, int layerPoored)
    {
        action act = new action(pooringTube, pooredTube, layerPoored);
        currentNode = new node(tubes, act, currentNode);
    }

    public void rewindNode()
    {
        currentNode = currentNode.rewindAction(tubes);
    }

    public GameObject nextWinnablePooringTube, nextWinnablePooredTube;
    public IEnumerator searchWinnable(List<GameObject> tubesForSolving)
    {
        node tmpNode = new node(tubesForSolving, null, null);
        yield return(resolveGraph(tubesForSolving, tmpNode, nodeID));
        isLevelWinnable = currentNode.isWinnable;

        if(!isLevelWinnable)
        {
            nextWinnablePooredTube = null;
            nextWinnablePooringTube = null;
        }
        else
        {
            foreach(node n in tmpNode.nextVisitedNodes)
            {
                if(n.isWinnable)
                {
                    nextWinnablePooredTube = n.previousAction.pooredTube;
                    nextWinnablePooringTube = n.previousAction.pooringTube;
                    break;
                }
            }
        }
    }

    private IEnumerator verifyLevel()
    {
        yield return StartCoroutine(resolveGraph(tubes, new node(tubes, null, null), nodeID));
        if(!currentNode.isWinnable)
        {
            SceneManager.LoadScene(SceneManager.GetActiveScene().name);
        }

    }
    
    private void Awake()
    {
        waitTime = 0.001f;
        isLevelWinnable = false;
        statesVisited = new List<List<string>>();
        tubes = new List<GameObject>();
        nodeID = 0;
    }

    // Start is called before the first frame update
    private IEnumerator Start()
    {
        yield return new WaitForEndOfFrame();      
        tubeParents = GameObject.Find("Tube Canvas").transform.GetChild(0).gameObject;
        for(int i=0; i < tubeParents.transform.childCount; i++)
        {
            tubes.Add(tubeParents.transform.GetChild(i).gameObject);
        }        
        currentNode = new node(tubes, null, null);
    }


    private void FixedUpdate()
    {
        if(doSolveGraph)
        {
            doSolveGraph = false;
            StartCoroutine(verifyLevel());
        }

        if(scanNewActions)
        {
            scanNewActions = false;
            node n = new node(tubes, null, null);
        }

        if(doAction)
        {
            doAction = false;
            currentNode = currentNode.performAction(tubes,0);
            Debug.Log(isVisitedState(tubes));
            if(isWin(tubes))
            {
                Debug.Log("win");
            }
        }

        if(doRewind)
        {
            doRewind = false;
            currentNode = currentNode.rewindAction(tubes);
        }


    }
    

}
