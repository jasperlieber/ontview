using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using OwlDotNetApi;
using System;

public class EdgeManager : MonoBehaviour
{
    public float m_lineSize;
    //public GUIStyle m_style;
    public Color m_lineColor;
    public Color m_TaxonomylineColor;

    private Dictionary<string, NodeInstance> m_NodeDictionary;

    private GameObject m_OwlLineParent;
    private GameObject m_HeirLineParent;

    // Use this for initialization
    void Start()
    {
        m_HeirLineParent = GameObject.Find("HeirLineParent");
        m_OwlLineParent = GameObject.Find("OwlLineParent");
    }

    internal void addOwlEdge(IOwlEdge edge, int cnt, ref int numNull)
    {
        IOwlNode parent = edge.ParentNode;
        IOwlNode child = edge.ChildNode;

        NodeInstance childNode;
        NodeInstance parentNode;

        m_NodeDictionary.TryGetValue(child.ToString(), out childNode);
        m_NodeDictionary.TryGetValue(parent.ToString(), out parentNode);

        if (childNode == null || parentNode == null)
        {
            if (childNode == null)
            {
                //Debug.Log("no child node found for edge " + edge.ID);
            }
            if (parentNode == null)
            {
                //Debug.Log("no parent node found for edge " + edge.ID);
            }
            numNull++;
            return;
        }

        Vector3 childPos = childNode.m_treeNode.mPos;
        Vector3 parentPos = parentNode.m_treeNode.mPos;

        GameObject goEdge = DrawLine(childPos, parentPos, m_lineColor, m_OwlLineParent);

        /*EdgeInstance myEdge =*/ new EdgeInstance(/*edge,*/ childNode, parentNode, goEdge);
    }


    internal GameObject DrawTaxonomyLine(Vector3 start, Vector3 end)
    {
        return DrawLine(start, end, m_TaxonomylineColor, m_HeirLineParent);
    }

    public GameObject DrawLine(Vector3 start, Vector3 end, Color color, GameObject parent) // , float duration = 0.2f )
    {
        GameObject myLine = new GameObject();
        myLine.transform.SetParent(parent.transform);
        myLine.transform.position = start;
        LineRenderer lr = myLine.AddComponent<LineRenderer>();
        lr.material = new Material(Shader.Find("Particles/Additive"));
        //lr.material = new Material(Shader.Find("Particles/Alpha Blended Premultiply"));
        //Debug.Log(color);
        lr.SetColors(color, color);
        //lr.SetColors(Color.cyan, Color.green);
        //lr.SetColors(Color.green, color);
        lr.SetWidth(m_lineSize, m_lineSize);
        lr.SetPosition(0, start);
        lr.SetPosition(1, end);

        return myLine;

        //GameObject.Destroy(myLine, duration);

    }

    internal void setNodeDictionary(Dictionary<string, NodeInstance> nodeDictionary)
    {
        m_NodeDictionary = nodeDictionary;
    }


    // Update is called once per frame
    void Update()
    {
        if (Input.GetKey(KeyCode.Escape))
            GraphManager.MyQuit();
    }

}
