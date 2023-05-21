using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class levelSolver : MonoBehaviour
{
    private GameObject tubeParents;
    private List<GameObject> tubes;
    private int nodeID = 0;

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

    IEnumerator wait(bool isFinish)
    {
        yield return new WaitForSeconds(1.5f);
        isFinish = true;
    }


    private IEnumerator resolveGraph(List<GameObject> availableTubes, node initialNode, int nodeIdx)
    {
        nodeID++;
        node tmpNode = initialNode;
        foreach(int actID in initialNode.actionsToExplore)
        {
            Debug.Log("node" + nodeIdx);
            yield return new WaitForSeconds(0.05f);
            float initTimer = Time.time;
            tmpNode = initialNode.performAction(availableTubes,actID);
            
            if(isWin(availableTubes))
            {
                Debug.Log("WIN node" + nodeID);
                node winNodes = tmpNode;
                while(winNodes.previousNode != null)
                {
                    winNodes.isWinnable = true;
                    winNodes = winNodes.previousNode;
                }
                yield return new WaitForSeconds(0.05f);
                tmpNode = tmpNode.rewindAction(availableTubes);
                yield return new WaitForSeconds(0.05f);
                nodeID++;
            }
            else
            {
                yield return StartCoroutine(resolveGraph(availableTubes, tmpNode, nodeID));
            }
        }
        Debug.Log("All actions seen in node" + nodeIdx);
        initialNode.rewindAction(availableTubes);
        //yield return new WaitForSeconds(0.5f);
    }

    private class graph
    {
        private List<node> nodes;
        private node currentNode;
    }

    private class node
    {
        public node previousNode {get;private set;}
        public action previousAction {get; private set;}
        private List<action> nextActions;
        public List<int> actionsToExplore {get; private set;}
        public bool isWinnable;
        private List<int> winnableNodes;

        public node(List<GameObject> tubesAvailable, action prevAction, node prevNode)
        {
            actionsToExplore = new List<int>();
            nextActions = new List<action>();
            isWinnable = false;
            winnableNodes = new List<int>();
            previousAction = prevAction;
            previousNode = prevNode;
            findActions(tubesAvailable);
        }


        private void findActions(List<GameObject> tubesAvailable)
        {
            int pooringID=1, pooredID=1;
            foreach(GameObject pooringTube in tubesAvailable)
            {
                pooredID=1;
                foreach(GameObject pooredTube in tubesAvailable)
                {
                    bool differentAction;
                    if(previousAction == null)
                    {
                        differentAction = true;
                    }
                    else
                    {
                        differentAction = (pooredTube != previousAction.pooringTube && pooringTube != previousAction.pooredTube);
                    }
                    bool pooringPossible = isPooringPossible(pooringTube, pooredTube);
                    bool tubesIncomplete = !pooringTube.GetComponent<testTube>().tubeComplete && !pooredTube.GetComponent<testTube>().tubeComplete;

                    if(differentAction && pooringPossible && tubesIncomplete)
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
            nextActions[actionID].printAction();

            return new node(tubesAvailable, nextActions[actionID], this);
        }

        public node rewindAction(List<GameObject> tubesAvailable)
        {    
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

        public action(GameObject pooring,GameObject poored)
        {
            pooringTube = pooring;
            pooredTube = poored;
            layersPoored = 0;
        }
    }


    // Start is called before the first frame update
    private IEnumerator init()
    {
        yield return new WaitForEndOfFrame();
        tubes = new List<GameObject>();
        tubeParents = GameObject.Find("Tube Canvas").transform.GetChild(0).gameObject;
        for(int i=0; i < tubeParents.transform.childCount; i++)
        {
            tubes.Add(tubeParents.transform.GetChild(i).gameObject);
        }        
        currentNode = new node(tubes, null, null);

        nodeID = 0;
        StartCoroutine(resolveGraph(tubes, new node(tubes, null, null), nodeID));
        //resolveGraph(tubes, new node(tubes, null, null), 0);
    }

    [SerializeField] bool scanNewActions = false;
    [SerializeField] bool doAction = false;
    [SerializeField] bool rewind = false;
    node currentNode;


    private bool doInit = true;
    private void FixedUpdate()
    {
        if(doInit)
        {
            doInit = false;
            StartCoroutine(init());
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
            if(isWin(tubes))
            {
                Debug.Log("win");
            }
        }

        if(rewind)
        {
            rewind = false;
            currentNode = currentNode.rewindAction(tubes);
        }


    }
    

}
