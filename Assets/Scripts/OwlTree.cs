using Overby.Collections;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using UnityEngine;

// Class to hold an OWL tree of nodes based on URI of node.

public class OwlTree 
{
    public TreeNode<OwlTreeNode> m_tree;

    public OwlTree()
    {
        m_tree = new TreeNode<OwlTreeNode>(new OwlTreeNode("root", 0, 0));
    }


    public override string ToString()
    {
        return m_tree.ToString();
    }


    internal OwlTreeNode AddNode(NodeInstance graphNode)
    {
        TreeNode<OwlTreeNode> treePtr = m_tree;
        
        int depth = 0;

        string fullName = "";

        foreach (string pathSegment in graphNode.m_pathSegments)
        {
            depth++;
            bool found = false;
            TreeNode<OwlTreeNode> newPtr = null;

            fullName += pathSegment + " ";

            foreach (var kid in treePtr.Children)
            {
                OwlTreeNode kidElem = kid.Value;

                found = kidElem.matchesString(pathSegment);

                if (found)
                {
                    newPtr = kid;
                    break ;
                }
            }

            if (!found)
            {
                newPtr = treePtr.AddChild(
                    new OwlTreeNode(pathSegment, depth, treePtr.Value.mNumKids++));
                if (GraphManager.m_debug) newPtr.Value.mFullName = '<' + fullName + '>';
            }

            treePtr = newPtr;
        }

        if (treePtr.Value.mNodeInstance != null)
            Debug.Log("dupe node: " + treePtr.Value);

        treePtr.Value.mNodeInstance = graphNode;

        if (graphNode.m_owlNode != null) // && GraphManager.m_debug)
            treePtr.Value.mFullName += "(" + graphNode.m_owlNode.ID + ')';

        return treePtr.Value;
    }


    internal void UpdateLeafCount(TreeNode<OwlTreeNode> treeNode)
    {
        if (treeNode.Children.Count == 0)
        {
            treeNode.Value.nLeaves = 1;
        }
        else
        {
            int cnt = 0;

            foreach (var kid in treeNode.Children)
            {
                UpdateLeafCount(kid);
                cnt += kid.Value.nLeaves;
            }

            treeNode.Value.nLeaves = cnt;
        }
    }

}
