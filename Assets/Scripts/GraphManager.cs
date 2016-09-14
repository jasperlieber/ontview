using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using OwlDotNetApi;
using System.Collections.Generic;
using System;

#if UNITY_EDITOR
using UnityEditor;
#endif

public class GraphManager : MonoBehaviour
{
    public GameObject m_NodePrefab;
    public Text m_winText;
    public Text m_titleText;
    public float m_edgeAddDelaySecs;
    public int m_addElementInterval;
    public float m_lineSize;
    public GUIStyle m_style;
    public Color m_lineColor;
    public bool m_testing;

    private IOwlGraph m_OwlGraph;
    private int m_numNodes;
    private Dictionary<string, NodeInstance> m_NodeGraph;

    private OwlGraphStats m_owlGraphStats;
    //private Camera m_Camera;

    private void Awake()
    {
        //m_Camera = GetComponentInChildren<Camera>();
    }

    // Use this for initialization
    void Start()
    {
        m_winText.text = "FTL Owl Visualizer 2016-09-07\n\n";
        m_titleText.text = "";

        m_NodeGraph = new Dictionary<string, NodeInstance>();

        m_owlGraphStats = new OwlGraphStats();

        //m_Taxonomy = new Dictionary<string,>

        StartCoroutine(GameLoop());
    }

    private IEnumerator GameLoop()
    {

        yield return StartCoroutine(OpenOwl());
        yield return StartCoroutine(ProcessOwl());

        string msg = m_owlGraphStats.ToString();
        Debug.Log(msg);

        yield return StartCoroutine(DrawOwlNodes());


        if (m_testing)
            yield break;


        yield return StartCoroutine(DrawOwlEdges());
    }

    private IEnumerator OpenOwl()
    {
        string filename = Application.dataPath.ToString() 
            + "\\..\\Ontologies\\example.owl";
        //Debug.Log(filename);
        IOwlParser parser = new OwlXmlParser();
        m_OwlGraph = parser.ParseOwl(filename);
        m_numNodes = m_OwlGraph.Nodes.Count;
        m_winText.text += "There are " + m_numNodes 
            + " node(s) in this example ontology\n";

        yield return null;
    }


    private IEnumerator DrawOwlEdges()
    {
        int numEdges = m_OwlGraph.Edges.Count;

        m_winText.text += "There are " + numEdges + " edges(s).";

        IEnumerator edgeIter = m_OwlGraph.Edges.GetEnumerator();

        int cnt = 0;
        int numNull = 0;

        while (edgeIter.MoveNext())
        {
            IOwlEdge edge = (IOwlEdge)edgeIter.Current;
            IOwlNode parent = edge.ParentNode;
            IOwlNode child  = edge.ChildNode;

            NodeInstance childNode;
            NodeInstance parentNode;

            m_NodeGraph.TryGetValue(child.ToString(),  out childNode);
            m_NodeGraph.TryGetValue(parent.ToString(), out parentNode);

            if (childNode == null || parentNode == null)
            {
                numNull++;
                continue;
            }

            //Debug.Log(parent);
            //Debug.Log(child);

            //Debug.Log(childNode.m_owlNode);
            //Debug.Log(childNode.m_graphNode.transform.position.ToString());

            Vector3 childPos  = childNode .m_graphNode.transform.position;
            Vector3 parentPos = parentNode.m_graphNode.transform.position;

            DrawLine(childPos, parentPos, m_lineColor);

            cnt++;


            //Debug.Log("There are " + numNull + " edges with a null node");

            if (cnt % m_addElementInterval == 0)
            {
                //Debug.Log("cnt = " + cnt);
                yield return new WaitForSeconds(m_edgeAddDelaySecs);
            }

        }

        yield return null;
    }

    void DrawLine(Vector3 start, Vector3 end, Color color, float duration = 0.2f)
    {
        GameObject myLine = new GameObject();
        myLine.transform.position = start;
        myLine.AddComponent<LineRenderer>();
        LineRenderer lr = myLine.GetComponent<LineRenderer>();
        //lr.material = new Material(Shader.Find("Particles/Alpha Blended Premultiply"));
        lr.SetColors(color, color);
        lr.SetWidth(m_lineSize, m_lineSize);
        lr.SetPosition(0, start);
        lr.SetPosition(1, end);
         
        //GameObject.Destroy(myLine, duration);

    }




    private static void MyQuit()
    {
        Debug.Log("stop");
#if UNITY_EDITOR
        //EditorApplication.ExecuteMenuItem("Edit/Play");
        UnityEditor.EditorApplication.isPlaying = false;
#else
         Application.Quit();
#endif
    }


    // Update is called once per frame
    void Update()
    {
        if (Input.GetKey(KeyCode.Escape))
            MyQuit();
    }


    private IEnumerator DrawOwlNodes()
    {
        //IDictionaryEnumerator nodeIter = (IDictionaryEnumerator)m_NodeGraph.GetEnumerator();
        float theta = 0;
        float radius = 20;

        int cnt = 0;  

        Vector3 center = new Vector3(0, radius+2, 0);


        foreach (KeyValuePair<string, NodeInstance> kvp in m_NodeGraph)
        {
            NodeInstance graphNode = kvp.Value;

            //string owlKey = (nodeIter.Key).ToString();
            ////OwlNode owlNode = (OwlNode)m_OwlGraph.Nodes[owlKey];
            //OwlNode owlNode = (OwlNode)nodeIter.Value;

            ////Debug.Log("owlKey  = " + owlKey);
            ////Debug.Log("owlNode = " + owlNode);
            ////yield break;
            ////Debug.Break(); 


            Vector3 pos;
            pos.x = center.x + radius * Mathf.Sin(theta * Mathf.Deg2Rad);
            pos.y = center.y + radius * Mathf.Cos(theta * Mathf.Deg2Rad);
            pos.z = center.z;

            theta += 360f / m_numNodes;
            cnt++;

            GameObject nodeInstance = Instantiate(m_NodePrefab, pos, Quaternion.identity)
                as GameObject;

            NodeManager2 nm = nodeInstance.GetComponent<NodeManager2>();

            graphNode.m_graphNode = nodeInstance;
            nm.m_owlNode = graphNode.m_owlNode; 

            //GameObject go = nodeInstance as GameObject;// nodeInstance.GetComponent<GameObject>();

            nodeInstance.GetComponent<Renderer>().material.color =
                nm.m_owlNode.IsAnonymous() ? Color.blue : Color.cyan ;



            //Vector3 pixelPos = Camera.main.WorldToScreenPoint(transform.position);
            //GUI.Label(new Rect(pixelPos.x + 7, pixelPos.y - 7, 200f, 20f),
            //    owlKey, m_style);



            if (cnt % m_addElementInterval == 0)
            {
                //Debug.Log("cnt = " + cnt);
                if (m_testing)
                    yield break;
                else
                    yield return null;// new WaitForSeconds(0.052f);
            }
        }

        yield return null;
    }


    private IEnumerator ProcessOwl()
    {
        IDictionaryEnumerator nodeIter = (IDictionaryEnumerator)m_OwlGraph.Nodes.GetEnumerator();

        int cnt = 0;




        while (nodeIter.MoveNext())
        {
            cnt++;

            string owlKey = (nodeIter.Key).ToString();
            OwlNode owlNode = (OwlNode)nodeIter.Value;

            NodeInstance graphNode = new NodeInstance();

            graphNode.m_owlNode = owlNode;
            graphNode.m_pathSegments = new ArrayList();

            //Debug.Log("owlKey  = " + owlKey);
            //Debug.Log("owlNode = " + owlNode);

            var uri = new Uri(owlKey);
            //Debug.Log("absURI = " + uri.AbsoluteUri);
            //Debug.Log("path = " + uri.PathAndQuery);
            //Debug.Log("host = " + uri.Host);

            // build up the node's path segments
            graphNode.m_pathSegments.Add(uri.Host);
            foreach (string element in uri.Segments)
            {
                //if (element == "/") continue;
                graphNode.m_pathSegments.Add(element.TrimEnd('/'));
            }

            m_owlGraphStats.addNode(graphNode);

            //string msg = "Segments: ";

            //string depthKey = "";
            //int depth = 0 ;

            //foreach (string pathSegment in graphNode.m_pathSegments)
            //{
            //    //msg += "[" + pathSegment + "]   ";

            //    depthKey += pathSegment + "/";

            //    //m_owlGraphStats.addStat(depth, depthKey);

            //    depth++;


            //}

           // Debug.Log(msg);





            //Uri uriAddress1 = new Uri(owlKey);
            //Debug.Log("The parts are {0}, {1}, {2}: " +
            //    uriAddress1.Segments[0] + ", " + uriAddress1.Segments[1]
            //     + ", " + uriAddress1.Segments[2]);

            ////string[] directories = owlKey.Split(Path.DirectorySeparatorChar);

            ////string absolutue
            ////string pathOnly = uri.LocalPath;        // "/mypage.aspx"
            ////string queryOnly = uri.Query;           // "?myvalue1=hello&myvalue2=goodbye"
            ////string pathAndQuery = uri.PathAndQuery; // "/mypage.aspx?myvalue1=hello&myvalue2=goodbye"
            //yield break;

            m_NodeGraph.Add(owlKey, graphNode);



            if (cnt % m_addElementInterval == 0)
            {
                //Debug.Log("cnt = " + cnt);
                if (m_testing)
                    yield break;
                else
                    yield return null;// new WaitForSeconds(0.052f);
            }
        }
    }


}
