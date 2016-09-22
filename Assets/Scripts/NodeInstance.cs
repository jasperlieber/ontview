using UnityEngine;
using System.Collections;
using OwlDotNetApi;
using Overby.Collections;
using System;

public class NodeInstance
{
    public OwlNode m_owlNode;
    //public GameObject m_graphNode;
    public ArrayList m_pathSegments; // should be moved to TreeNode class
    internal TreeElem m_statNode;

    internal void addOwlEdge(GameObject goEdge)
    {
        //throw new NotImplementedException();
    }

    /*
      
    want to put something liket this onto the graph node
    
     void OnMouseEnter()
 {
     startcolor = renderer.material.color;
     renderer.material.color = Color.yellow;
 }
 void OnMouseExit()
 {
     renderer.material.color = startcolor;
 }     

    */

}