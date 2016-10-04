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
    public static bool m_debug = false;

    public string m_OwlFile;
    public string m_OwlFile1;
    public string m_OwlFile2;
    public string m_OwlFile3;
    public string m_OwlFile4;
    public string m_OwlFile5;
    public GameObject m_NodePrefab;
    public EdgeManager m_EdgeManager;
    public Text m_winText;
    public Text m_titleText;
    public Toggle m_heirarchyEdgesToggle;
    public Toggle m_owlEdgesToggle;
    public float m_edgeAddDelaySecs;
    public int m_addElementInterval;
    public bool m_testing;

    private IOwlGraph m_OwlGraph;
    private int m_numNodes;

    [HideInInspector]
    public Dictionary<string, NodeInstance> m_NodeDictionary;


    private Transform m_heirTransform;
    private OwlNodeTree m_owlNodeTree;

    //private int m_drawCnt;

    //private Camera m_Camera;

    private void Awake()
    {
        //m_Camera = GetComponentInChildren<Camera>();
    }

    // Use this for initialization
    void Start()
    {
        m_winText.text = "FTL Owl Visualizer 2016-Oct-04\n\n";
        m_titleText.text = "";

        m_heirTransform = GameObject.Find("HeirLineParent").transform;

        StartCoroutine(GameLoop());
    }

    private IEnumerator GameLoop()
    {
        //m_EdgeManager = GameObject.Find("EdgeManager");

        m_NodeDictionary = new Dictionary<string, NodeInstance>();

        m_owlNodeTree = new OwlNodeTree();

        yield return StartCoroutine(OpenOwl());

        string oldText = m_winText.text;
        m_winText.text = oldText + "Processing OWL...";
        yield return StartCoroutine(ProcessOwl());
        m_winText.text = oldText;

        //string msg = m_owlNodeTree.ToString();
        //Debug.Log(msg);

        //m_drawCnt = 0;

        /*yield return StartCoroutine*/
        DrawOwlTree(m_owlNodeTree.m_keyTree.m_tree);

        if (m_testing)
            yield break;


        yield return StartCoroutine(DrawOwlEdges());
    }

    private IEnumerator OpenOwl()
    {
        string filename = m_OwlFile;//   Application.dataPath.ToString() 
            //+ "\\..\\Ontologies\\example.owl";
        //Debug.Log(filename);
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

            //string owlKey = (nodeIter.Key).ToString();
            string owlKey = ((OwlNode)nodeIter.Value).ID;
            OwlNode owlNode = (OwlNode)nodeIter.Value;

            NodeInstance graphNode = new NodeInstance();

            graphNode.m_owlNode = owlNode;
            graphNode.m_pathSegments = new ArrayList();

            //Debug.Log("owlKey  = <" + owlKey + ">");
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

            if (uri.Fragment != null)
                graphNode.m_pathSegments[graphNode.m_pathSegments.Count -1] += (uri.Fragment);

            OwlTreeNode owlTreeNode = m_owlNodeTree.addNode(graphNode);
            graphNode.m_treeNode = owlTreeNode;
            //statsElem.mNode = graphNode;
            m_NodeDictionary.Add(owlKey, graphNode);

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
        m_EdgeManager.setNodeDictionary(m_NodeDictionary);

        m_owlNodeTree.UpdateLeafCount();
        m_owlNodeTree.CalculateNodeRanges(m_owlNodeTree.m_keyTree.m_tree);
        m_owlNodeTree.CalculateDepthRadii();
        m_owlNodeTree.CalculateNodeAlphas(m_owlNodeTree.m_keyTree.m_tree);
        yield return null;
    }

    public static void MyQuit()
    {
        //Debug.Log("MyQuit() called.");
#if UNITY_EDITOR
        //EditorApplication.ExecuteMenuItem("Edit/Play");
        UnityEditor.EditorApplication.isPlaying = false;
        //throw new Exception("force-stop");
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

    private void DrawOwlTree(TreeNode<OwlTreeNode> treeNode)
    {

        //Debug.Log("visiting " + statNode.Value.ToString());
        //NodeInstance graphNode = statNode.Value.mNode;

        GameObject nodePrefab = Instantiate(m_NodePrefab, treeNode.Value.mPos,
                Quaternion.identity,
                treeNode.Children.Count > 0 ? m_heirTransform : transform) 
             as GameObject;

        NodeManager nm = nodePrefab.GetComponent<NodeManager>();
        nm.m_edgeManager = m_EdgeManager;
        nm.m_treeNode = treeNode.Value;

        if (treeNode.Value.mDepth > 0)
        {
            m_EdgeManager.DrawTaxonomyLine(treeNode.Value.mPos, treeNode.Parent.Value.mPos);
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

        foreach (var kid in treeNode.Children)
        {
            DrawOwlTree(kid);

            //else
            //    yield return DrawStatTree(kid);
        }


        //yield return null;
    }


    private IEnumerator DrawOwlEdges()
    {
        //yield return null;

        int numEdges = m_OwlGraph.Edges.Count;

        m_winText.text += "There are " + numEdges + " edges(s).";

        IEnumerator edgeIter = m_OwlGraph.Edges.GetEnumerator();

        int cnt = 0;
        int numNull = 0;

        while (edgeIter.MoveNext())
        {
            IOwlEdge edge = (IOwlEdge)edgeIter.Current;

            m_EdgeManager.addOwlEdge(edge, cnt, ref numNull);

            //EdgeInstance edgeInstance = new EdgeInstance(edge);


            if (cnt++ % m_addElementInterval == 0)
            {
                //Debug.Log("cnt = " + cnt);
                yield return null;// new WaitForSeconds(m_edgeAddDelaySecs);
            }
        }



        //m_winText.text += "\n" + numNull + " edge(s) have a null node.";

        yield return null;
    }


}
