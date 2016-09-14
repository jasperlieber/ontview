using UnityEngine;
using System.Collections;
using OwlDotNetApi;

public class NodeInstance
{
    [HideInInspector]
    public OwlNode m_owlNode;

    [HideInInspector]
    public GameObject m_graphNode;

    [HideInInspector]
    public ArrayList m_pathSegments;

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