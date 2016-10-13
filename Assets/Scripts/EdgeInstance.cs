using System;
using OwlDotNetApi;
using UnityEngine;


// Class to hold edge data, namely the parent & child nodes, and a
// reference to the game object holding the edge.
public class EdgeInstance
{
    public NodeInstance mChildNode;
    public NodeInstance mParentNode;
    public GameObject mGoEdge;
    //private IOwlEdge mOwlEdge;

    public EdgeInstance(/*IOwlEdge edge,*/ NodeInstance childNode, 
        NodeInstance parentNode, GameObject goEdge)
    {
        mChildNode = childNode;
        mParentNode = parentNode;
        mGoEdge = goEdge;
        //mOwlEdge = edge;

        childNode.addParentEdge(this);
        parentNode.addChildEdge(this);
    }

}