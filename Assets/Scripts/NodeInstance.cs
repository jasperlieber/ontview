using UnityEngine;
using System.Collections;
using OwlDotNetApi;
using Overby.Collections;
using System;
using System.Collections.Generic;

public class NodeInstance
{
    public OwlNode m_owlNode;
    //public GameObject m_graphNode;
    public ArrayList m_pathSegments; // should be moved to TreeNode class
    internal OwlTreeNode m_treeNode;

    public List<EdgeInstance> m_childEdges = new List<EdgeInstance>();
    public List<EdgeInstance> m_parentEdges = new List<EdgeInstance>();

    public void addParentEdge(EdgeInstance edgeInstance)
    {
        m_parentEdges.Add(edgeInstance);
    }

    public void addChildEdge(EdgeInstance edgeInstance)
    {
        m_childEdges.Add(edgeInstance);
    }
}