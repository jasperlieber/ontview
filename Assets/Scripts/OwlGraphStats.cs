using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;
using Overby.Collections;

public class OwlGraphStats
{
    

    // ordered list of depth keys.  index is depth to a dictionary 
    // containing depth keys & count at that depth.
    //private List<Dictionary<string,int>> m_graphDepths;

    private OwlTree m_keyTree;


    public OwlGraphStats()
    {
        //m_graphDepths = new List<Dictionary<string, int>>();
        m_keyTree = new OwlTree();
        m_keyTree.m_tree.Value.mNumRefs = 0;
    }

    internal void addNode(NodeInstance graphNode)
    {
        m_keyTree.AddNode(graphNode);
    }

    public override string ToString()
    {
        return m_keyTree.ToString();
    }


    //public void addStat(int depth, string depthKey)
    //{
    //    //UnityEngine.Debug.Log("addStat(" + depth + ", " + depthKey + ")");
    //    if (m_graphDepths.Count < depth+1)
    //    {
    //        // need a new depth dictionary item at this depth, with count of one
    //        Dictionary<string, int> depthKeyNote = new Dictionary<string, int>();
    //        depthKeyNote.Add(depthKey, 1);
    //        m_graphDepths.Add(depthKeyNote);
    //    }
    //    else
    //    {
    //        if (!m_graphDepths[depth].ContainsKey(depthKey))
    //        {
    //            m_graphDepths[depth].Add(depthKey, 1);
    //        }
    //        else
    //        {
    //            m_graphDepths[depth][depthKey]++;
    //        }
    //    }
    //    //UnityEngine.Debug.Log(toString());
    //}

    //public string toString()
    //{
    //    string msg = "Depth values:\n";
    //    for(int depth = 0; depth < m_graphDepths.Count; depth++)
    //    {
    //        msg += "Depth " + depth + " has " + m_graphDepths[depth].Count 
    //            + " elements:\n";

    //        foreach (KeyValuePair<string, int> kvp in m_graphDepths[depth])
    //        {
    //            msg += "  " + kvp.Key + " count is " + kvp.Value + "\n";
    //        }

    //    }

    //    return msg;
    //}
}