﻿using System;
using System.Collections;
using System.Collections.Generic;
//using System.Diagnostics;
using UnityEngine;
using Overby.Collections;

public class OwlNodeTree
{
    public OwlTree m_keyTree;

    public OwlNodeTree()
    {
        m_keyTree = new OwlTree();
    }

    internal OwlTreeNode addNode(NodeInstance graphNode)
    {
        return m_keyTree.AddNode(graphNode);
    }

    public override string ToString()
    {
        return m_keyTree.ToString();
    }

    internal void UpdateLeafCount()
    {
        m_keyTree.UpdateLeafCount(m_keyTree.m_tree);
    }

    // By range, I mean how much of the interval of [0,1] this
    // node should be allocated for its number of children & depth in a tree.
    // This range is then converted to an angle of a circle for the
    // circular display of the graph.
    internal void CalculateNodeRanges(TreeNode<OwlTreeNode> treePtr)
    {
        treePtr.Value.mRange = treePtr.Value.mDepth == 0 ? 1 
            : (float)treePtr.Value.nLeaves / treePtr.Parent.Value.nLeaves 
                * treePtr.Parent.Value.mRange;

        foreach (var child in treePtr.Children)
        {
            CalculateNodeRanges(child);
        }
    }

    // By alpha, I mean where in the range of [0,1] should this node
    // be positioned based its "range" (see above) and the position of
    // its parent.
    internal void CalculateNodeAlphas(TreeNode<OwlTreeNode> treePtr)
    {
        //float mRadialIncrement = 5;

        float alpha = treePtr.Value.mDepth == 0 ? 0
            : treePtr.Value.mAlpha;

        foreach (var child in treePtr.Children)
        {
            //int numAtThisDepth = mDepthCounts[child.Value.mDepth];
            //float radius = numAtThisDepth * 1.1f / 2 / (float)Math.PI;// (float)child.Value.mDepth * mRadialIncrement;

            //Debug.Log(child.Value.mDepth);
            //float minRangeAtThisDepth = mDepthMinRange[child.Value.mDepth];

            float radius = mDepthRadii[child.Value.mDepth];// 1f / (2f * (float)Math.PI * minRangeAtThisDepth * 1.1f) * child.Value.mDepth;// * mRadialIncrement;

            //float radius = child.Value.mDepth * 20;

            child.Value.mAlpha = alpha;
            alpha += child.Value.mRange;

            float mid = child.Value.mRange * .5f;

            float theta = (alpha-mid) * 2 * (float)Math.PI;
            child.Value.mPos = new Vector3(
                (float)Math.Cos(theta) * radius,
                (float)Math.Sin(theta) * radius);

            CalculateNodeAlphas(child);
        }
    }


    // Nodes are put in circles of various radii.  Which circle is based on the 
    // depth of the node in its taxonomy.  The radii of the circle is based on 
    // how many nodes exist at that level, with some fudging for max values.

    //public List<int> mDepthCounts;
    private List<float> mDepthMinRange;
    private List<float> mDepthRadii;
    private float m_maxRadius = 30;

    internal void CalculateDepthRadii()
    {
        //mDepthCounts = new List<int>();
        mDepthMinRange = new List<float>();
        //mDepthCounts.Add(1);
        mDepthMinRange.Add(1f);
        GetMinRangesOfKids(0, m_keyTree.m_tree);

        //string vals = "minDepthRanges: ";
        //foreach(float ff in mDepthMinRange)
        //    vals += ff + " ";
        //Debug.Log(vals);// String.Join(" ", mDepthCounts));


        mDepthRadii = new List<float>();
        float prevRadius = 0;
        foreach (float rangeAtDepth in mDepthMinRange)
        {
            float neededRadius = Math.Min(1f / (4f * rangeAtDepth * (float)Math.PI), 
                m_maxRadius);

            float selectedRadius = Math.Max(neededRadius, prevRadius);

            prevRadius = selectedRadius + 3;

            mDepthRadii.Add(selectedRadius);

        }

        //vals = "depth radii: ";
        //foreach (float ff in mDepthRadii)
        //    vals += ff + " ";
        //Debug.Log(vals);// String.Join(" ", mDepthCounts));
        //Debug.Break();
    }

    private void GetMinRangesOfKids(int curDepth, TreeNode<OwlTreeNode> treeNode)
    {
        if (mDepthMinRange.Count <= curDepth)
            mDepthMinRange.Add(treeNode.Value.mRange);
        else
        {
            if (treeNode.Value.mRange < mDepthMinRange[curDepth])
            {
                mDepthMinRange[curDepth] = treeNode.Value.mRange;
            }
        }

        //if (mDepthCounts.Count <= curDepth)
        //{
        //    mDepthCounts.Add(kidCnt);
        //    mDepthMinRange.Add(1f);
        //}
        //else mDepthCounts[curDepth] += kidCnt;

        foreach (var kid in treeNode.Children)
        {
            //Debug.Log(kid);
            GetMinRangesOfKids(curDepth + 1, kid);
        }
    }
}