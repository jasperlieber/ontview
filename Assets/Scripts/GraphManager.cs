using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using OwlDotNetApi;
using System.Collections.Generic;
using System;
using Overby.Collections;

#if UNITY_EDITOR
using UnityEditor;
#endif

public class GraphManager : MonoBehaviour
{
    public string m_OwlFile;
    public string m_OwlFile1;
    public string m_OwlFile2;
    public string m_OwlFile3;
    public string m_OwlFile4;
    public GameObject m_NodePrefab;
    public Text m_winText;
    public Text m_titleText;
    public Toggle m_heirarchyEdgesToggle;
    public Toggle m_owlEdgesToggle;
    public float m_edgeAddDelaySecs;
    public int m_addElementInterval;
    public float m_lineSize;
    public GUIStyle m_style;
    public Color m_lineColor;
    public Color m_TaxonomylineColor;
    public bool m_testing;

    private IOwlGraph m_OwlGraph;
    private int m_numNodes;
    private Dictionary<string, NodeInstance> m_NodeGraph;

    private GameObject m_HeirLineParent;
    private GameObject m_OwlLineParent;

    private OwlGraphStats m_owlGraphStats;

    //private int m_drawCnt;

    //private Camera m_Camera;

    private void Awake()
    {
        //m_Camera = GetComponentInChildren<Camera>();
    }

    // Use this for initialization
    void Start()
    {
        m_winText.text = "FTL Owl Visualizer 2016-09-21\n\n";
        m_titleText.text = "";


        m_HeirLineParent = GameObject.Find("HeirLineParent");
        m_OwlLineParent = GameObject.Find("OwlLineParent");

        m_NodeGraph = new Dictionary<string, NodeInstance>();

        m_owlGraphStats = new OwlGraphStats();

        StartCoroutine(GameLoop());

        //m_LineParent.SetActive(true);

        //MyQuit();
    }

    private IEnumerator GameLoop()
    {
        yield return StartCoroutine(OpenOwl());

        string oldText = m_winText.text;
        m_winText.text = oldText + "Processing OWL...";
        yield return StartCoroutine(ProcessOwl());
        m_winText.text = oldText;

        string msg = m_owlGraphStats.ToString();
        Debug.Log(msg);

        //m_drawCnt = 0;

        /*yield return StartCoroutine*/
        DrawStatTree(m_owlGraphStats.m_keyTree.m_tree);

        if (m_testing)
            yield break;


        yield return StartCoroutine(DrawOwlEdges());
    }

    private IEnumerator OpenOwl()
    {
        string filename = m_OwlFile;//   Application.dataPath.ToString() 
            //+ "\\..\\Ontologies\\example.owl";
        Debug.Log(filename);
        IOwlParser parser = new OwlXmlParser();
        m_OwlGraph = parser.ParseOwl(filename);
        m_numNodes = m_OwlGraph.Nodes.Count;
        m_winText.text += "There are " + m_numNodes 
            + " node(s) in the ontology '" + m_OwlFile + "'\n";

        string oldText = m_winText.text;


        //yield return null;

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
                graphNode.m_pathSegments.Add(element);//.TrimEnd('/'));
            }

            TreeElem statsElem = m_owlGraphStats.addNode(graphNode);
            graphNode.m_statNode = statsElem;
            statsElem.mNode = graphNode;
            m_NodeGraph.Add(owlKey, graphNode);

            if (cnt % m_addElementInterval == 0)
            {
                string[] dots = { ".", "..", "..." };
                for (int jj = 0; jj < 3; jj++)
                    m_winText.text = oldText + "Opening OWL file" + dots[cnt % 3];

                //Debug.Log("cnt = " + cnt);
                if (m_testing)
                    yield break;
                else
                    yield return null;// new WaitForSeconds(0.052f);
            }

        }

        m_winText.text = oldText;
        yield return null;
    }


    private IEnumerator ProcessOwl()
    {
        m_owlGraphStats.UpdateLeafCount();
        m_owlGraphStats.CalculateNodeRanges(m_owlGraphStats.m_keyTree.m_tree);
        m_owlGraphStats.CalculateNodeAlphas(m_owlGraphStats.m_keyTree.m_tree);
        m_owlGraphStats.CalculateDepthCounts();
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

            Vector3 childPos = childNode.m_statNode.mPos;
            Vector3 parentPos = parentNode.m_statNode.mPos;

            DrawLine(childPos, parentPos, m_lineColor, m_OwlLineParent);

            cnt++;

            //Debug.Log("There are " + numNull + " edges with a null node");

            if (cnt % m_addElementInterval == 0)
            {
                //Debug.Log("cnt = " + cnt);
                yield return null;// new WaitForSeconds(m_edgeAddDelaySecs);
            }
        }



        //m_winText.text += "\n" + numNull + " edge(s) have a null node.";

        yield return null;
    }

    void DrawLine(Vector3 start, Vector3 end, Color color, GameObject parent) // , float duration = 0.2f )
    {
        GameObject myLine = new GameObject();
        myLine.transform.SetParent(parent.transform);
        myLine.transform.position = start;
        LineRenderer lr = myLine.AddComponent<LineRenderer>();
        lr.material = new Material(Shader.Find("Particles/Additive"));
        //lr.material = new Material(Shader.Find("Particles/Alpha Blended Premultiply"));
        //Debug.Log(color);
        lr.SetColors(color, color);
        //lr.SetColors(Color.green, color);
        lr.SetWidth(m_lineSize, m_lineSize);
        lr.SetPosition(0, start);
        lr.SetPosition(1, end);
         
        //GameObject.Destroy(myLine, duration);

    }


    public static void MyQuit()
    {
        //Debug.Log("MyQuit() called.");
#if UNITY_EDITOR
        //EditorApplication.ExecuteMenuItem("Edit/Play");
        UnityEditor.EditorApplication.isPlaying = false;
        throw new Exception("force-stop");
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

    private void DrawStatTree(TreeNode<TreeElem> statNode)
    {

        //Debug.Log("visiting " + statNode.Value.ToString());
        //NodeInstance graphNode = statNode.Value.mNode;

        GameObject nodeInstance = Instantiate(m_NodePrefab, statNode.Value.mPos,
                Quaternion.identity)
             as GameObject;

        NodeManager2 nm = nodeInstance.GetComponent<NodeManager2>();
        nm.m_statsElem = statNode.Value;

        if (statNode.Value.mDepth > 0)
        {
            DrawLine(statNode.Value.mPos, statNode.Parent.Value.mPos, 
                m_TaxonomylineColor, m_HeirLineParent);
        }

        //m_drawCnt++;

        //if (m_drawCnt % m_addElementInterval == 0)
        //{
        //    //Debug.Log("cnt = " + cnt);
        //    if (m_testing)
        //        yield break;
        //    else
        //        yield return null; // StartCoroutine(DrawStatTree(kid));
        //}

        foreach (var kid in statNode.Children)
        {
            DrawStatTree(kid);

            //else
            //    yield return DrawStatTree(kid);
        }


        //yield return null;
    }


}
