using UnityEngine;
using System.Collections;
using OwlDotNetApi;
using Overby.Collections;

public class NodeInstance
{
    [HideInInspector]
    public OwlNode m_owlNode;

    [HideInInspector]
    public GameObject m_graphNode;

    [HideInInspector]
    public ArrayList m_pathSegments;

    [HideInInspector]
    internal TreeElem m_statNode;

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