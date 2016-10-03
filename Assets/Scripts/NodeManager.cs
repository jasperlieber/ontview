using UnityEngine;
using System.Collections;
using OwlDotNetApi;
using UnityEngine.UI;
using System;

public class NodeManager : MonoBehaviour
{
    //public GUIStyle m_style;
    public float m_yLabelOffset;
    public float m_xLabelOffset;
    public GameObject m_NodeLabelPrefab;

    private Text m_nodeLabel;
    private Renderer m_renderer;
    private bool m_showLabel = false;
    private bool m_highlighted = false; 
     //private GameObject m_nodeLabelGO;

     [HideInInspector]
    public OwlNode m_owlNode;

    [HideInInspector]
    public OwlTreeNode m_statsElem;

    [HideInInspector]
    public EdgeManager m_edgeManager;

    // Use this for initialization
    void Start()
    {

        //Debug.Log(m_yLabelOffset);
        
        //Debug.Log("m_statsElem.ToString() = " + m_statsElem.ToString());

        //m_nodeLabelGO = Instantiate(m_NodeLabelPrefab, 
        //        Vector3.zero,
        //        Quaternion.identity)
        //    as GameObject;


        m_nodeLabel = GetComponentInChildren<Text>();
        m_nodeLabel.text = m_statsElem.ToString(); //"test";// m_owlNode.ToString();

        transform.SetParent(GameObject.Find("Canvas").transform);

        m_renderer = GetComponent<Renderer>();

        m_nodeLabel.enabled = false;

        m_renderer.material.color =
            m_statsElem.mNodeInstance == null 
                ? Color.magenta 
                : (m_statsElem.mNodeInstance.m_owlNode.IsAnonymous() 
                    ? Color.blue 
                    : Color.cyan);


        m_startcolor = m_renderer.material.color;



        //Debug.Log("tt text = " + m_nodeLabel.text + ", startColor = " + 
        //    m_startcolor + ", pos = " + m_renderer.bounds.center.ToString());
    }

    // Update is called once per frame
    void Update()
    {

        if (m_showLabel && Input.GetMouseButtonDown(0))
        {
            m_highlighted = !m_highlighted;

            brightenEdges(m_highlighted);

            Debug.Log("Pressed left click and showLabel = " + m_showLabel);
        }
    }

    private Color m_startcolor;

    void OnMouseEnter()
    {
        //brightenEdges(true);
        m_showLabel = true;
    }
    void OnMouseExit()
    {
        //brightenEdges(false);
        //m_renderer.material.color = m_startcolor;
        m_showLabel = false;
    }


    void OnGUI()
    {
        m_nodeLabel.enabled = m_showLabel;
        //m_nodeLabelGO.SetActive(m_showLabel);
        if (m_showLabel)
        {

            //Rect rr = GUIRectWithObject(m_renderer.bounds);

            //Debug.Log(rr);

            //Vector3 pixelPos = new Vector3(); rr.x + rr.width + 155,
            //    rr.center.y, 0);

            Vector3 pixelPos = Vector3.zero;// new Vector3();
            pixelPos.x = 160;
            pixelPos.y = 70;

            //Vector3 pixelPos = Camera.main.WorldToScreenPoint(transform.position);
            //Debug.Log(pixelPos.ToString());


            m_nodeLabel.rectTransform.position = pixelPos;
            //new Vector3(
            //    pixelPos.x + m_xLabelOffset, pixelPos.y + m_yLabelOffset, 0);
        }
    }

    private void brightenEdges(bool onOff)
    {
        m_renderer.material.color = onOff ? Color.yellow : m_startcolor;

        if (m_statsElem.mNodeInstance == null)
            return;

        Color c1 = onOff ? Color.yellow : m_edgeManager.m_lineColor;

        foreach (EdgeInstance edge in m_statsElem.mNodeInstance.m_childEdges)
        {
            LineRenderer lr = edge.mGoEdge.GetComponent<LineRenderer>();
            lr.SetColors(c1, c1);
        }
        foreach (EdgeInstance edge in m_statsElem.mNodeInstance.m_parentEdges)
        {
            LineRenderer lr = edge.mGoEdge.GetComponent<LineRenderer>();
            lr.SetColors(c1, c1);
        }
    }

    //public static Rect GUIRectWithObject(Bounds bb)
    //{
    //    Vector3 cen = bb.center;
    //    Vector3 ext = bb.extents;
    //    Vector2[] extentPoints = new Vector2[8]
    //     {
    //           WorldToGUIPoint(new Vector3(cen.x-ext.x, cen.y-ext.y, cen.z-ext.z)),
    //           WorldToGUIPoint(new Vector3(cen.x+ext.x, cen.y-ext.y, cen.z-ext.z)),
    //           WorldToGUIPoint(new Vector3(cen.x-ext.x, cen.y-ext.y, cen.z+ext.z)),
    //           WorldToGUIPoint(new Vector3(cen.x+ext.x, cen.y-ext.y, cen.z+ext.z)),
    //           WorldToGUIPoint(new Vector3(cen.x-ext.x, cen.y+ext.y, cen.z-ext.z)),
    //           WorldToGUIPoint(new Vector3(cen.x+ext.x, cen.y+ext.y, cen.z-ext.z)),
    //           WorldToGUIPoint(new Vector3(cen.x-ext.x, cen.y+ext.y, cen.z+ext.z)),
    //           WorldToGUIPoint(new Vector3(cen.x+ext.x, cen.y+ext.y, cen.z+ext.z))
    //     };
    //    Vector2 min = extentPoints[0];
    //    Vector2 max = extentPoints[0];
    //    foreach (Vector2 v in extentPoints)
    //    {
    //        min = Vector2.Min(min, v);
    //        max = Vector2.Max(max, v);
    //    }
    //    return new Rect(min.x, min.y, max.x - min.x, max.y - min.y);
    //}

    //public static Vector2 WorldToGUIPoint(Vector3 world)
    //{
    //    Vector2 screenPoint = Camera.main.WorldToScreenPoint(world);
    //    //screenPoint.y = (float)Screen.height - screenPoint.y;
    //    return screenPoint;
    //}

}
