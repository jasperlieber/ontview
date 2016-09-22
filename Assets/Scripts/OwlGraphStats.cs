using System;
using System.Collections;
using System.Collections.Generic;
//using System.Diagnostics;
using UnityEngine;
using Overby.Collections;

public class OwlGraphStats
{
    public OwlTree m_keyTree;

    public OwlGraphStats()
    {
        m_keyTree = new OwlTree();
    }

    internal TreeElem addNode(NodeInstance graphNode)
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

    internal void CalculateNodeRanges(TreeNode<TreeElem> treePtr)
    {
        treePtr.Value.mRange = treePtr.Value.mDepth == 0 ? 1 
            : (float)treePtr.Value.nLeaves / treePtr.Parent.Value.nLeaves 
                * treePtr.Parent.Value.mRange;

        foreach (var child in treePtr.Children)
        {
            CalculateNodeRanges(child);
        }
    }

    internal void CalculateNodeAlphas(TreeNode<TreeElem> treePtr)
    {
        float mRadialIncrement = 5;

        float alpha = treePtr.Value.mDepth == 0 ? 0
            : treePtr.Value.mAlpha;

        foreach (var child in treePtr.Children)
        {
            //int numAtThisDepth = mDepthCounts[child.Value.mDepth];
            //float radius = numAtThisDepth * 1.1f / 2 / (float)Math.PI;// (float)child.Value.mDepth * mRadialIncrement;

            float radius = (float)child.Value.mDepth * mRadialIncrement;
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

    public List<int> mDepthCounts;
    public List<float> mDepthMinAlphas;

    internal void CalculateDepthCounts()
    {
        mDepthCounts = new List<int>();
        mDepthMinAlphas = new List<float>();
        mDepthCounts.Add(1);
        mDepthMinAlphas.Add(1f);
        CalculateDepthCounts(1, m_keyTree.m_tree);

        string vals = null;
        foreach(int ii in mDepthMinAlphas)
            vals += ii + " ";
        Debug.Log(vals);// String.Join(" ", mDepthCounts));

    }

    private void CalculateDepthCounts(int curDepth, TreeNode<TreeElem> treeNode)
    {
        int kidCnt = treeNode.Children.Count;
        if (kidCnt == 0) return;

        if (mDepthCounts.Count <= curDepth)
        {
            mDepthCounts.Add(kidCnt);
            mDepthMinAlphas.Add(1f);
        }
        else mDepthCounts[curDepth] += kidCnt;

        foreach (var kid in treeNode.Children)
        {
            Debug.Log(kid);
            CalculateDepthCounts(curDepth + 1, kid);
            if (kid.Value.mAlpha < mDepthMinAlphas[curDepth])
                mDepthMinAlphas[curDepth] = kid.Value.mAlpha;
        }
    }
}