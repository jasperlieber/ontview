using System;
using OwlDotNetApi;
using UnityEngine;

public class EdgeInstance
{
    private NodeInstance mChildNode;
    public GameObject mGoEdge;
    private IOwlEdge mOwlEdge;
    private NodeInstance mParentNode;

    public EdgeInstance(IOwlEdge edge, NodeInstance childNode, 
        NodeInstance parentNode, GameObject goEdge)
    {
        mChildNode = childNode;
        mParentNode = parentNode;
        mGoEdge = goEdge;
        mOwlEdge = edge;

        childNode.addParentEdge(this);
        parentNode.addChildEdge(this);
    }

}