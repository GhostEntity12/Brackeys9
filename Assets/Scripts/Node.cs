using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor.Experimental.GraphView;
using UnityEngine;

[Serializable]
public class Node
{
    public int xPos { get; private set; }
    public int yPos { get; private set; }

    public int movementCost { get; private set; } = 1;

    public int gCost { get; private set; }
    public int hCost { get; private set; }

    public int fCost => gCost + hCost;


    public void Init (int _xPos, int _yPos, int _movementCost)
    {
        xPos = _xPos;
        yPos = _yPos;
        movementCost = _movementCost;
    }

}
//{
//    [SerializeField, Tooltip("How many actions it takes to move into the tile")]
//    int movementCost = 1;


//    int gCost;
//    int hCost;
//    int fCost => gCost + hCost;
//    Tile[] adjacents = new Tile[4];
//    // Start is called before the first frame update
//    void Start()
//    {
        
//    }

//    // Update is called once per frame
//    void Update()
//    {
        
//    }

//    public void SetAdjacents(Tile n, Tile e, Tile s, Tile w)
//    {
//        adjacents[0] = n;
//        adjacents[1] = e;
//        adjacents[2] = s;
//        adjacents[3] = w;
//    }
//}
