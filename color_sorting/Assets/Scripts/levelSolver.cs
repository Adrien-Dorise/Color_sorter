using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class levelSolver : MonoBehaviour
{
    private GameObject tubeParents;
    private List<GameObject> tubes;

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


    private class graph
    {
        private List<node> nodes;
        private node currentNode;
    }

    private class node
    {
        private node previousNode;
        private action previousAction;
        private List<action> nextActions;
        private List<int> actionsToExplore;
        private bool isWinnable;
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

                    if(levelSolver.isPooringPossible(pooringTube, pooredTube) && differentAction)
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
            nextActions[actionID].printAction();
            int pooredLayers = gameManager.pooringAction(nextActions[actionID].pooringTube, nextActions[actionID].pooredTube);
            nextActions[actionID].setLayerPoored(pooredLayers);

            return new node(tubesAvailable, nextActions[actionID], this);
        }

        public node rewindAction(List<GameObject> tubesAvailable)
        {    
            if(previousAction.layersPoored == 0)
            {
                Debug.LogWarning("Can't rewind action as it does not have the information on the number of layer to rewind (layersPoor = 0)");
                return this;
            }
            
            Color rewindColor = previousAction.pooredTube.GetComponent<testTube>().colorList.Peek(); 
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
