using Overby.Collections;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using UnityEngine;

public class TreeElem
{
    private readonly string mSpotName;  // name of spot in tree
    public int mNumRefs;                // number of references in owl to this node
    public NodeInstance mNode;          // only set if a leaf - null otherwise
    public int mDepth;                  // depth of this node in tree
    public int mNumKids;                // number of kids of this node

    public TreeElem()
    {
        mSpotName = null;
        mNumRefs = 1;
        mNode = null;
        mDepth = 0;
        mNumKids = 0;

    }

    public TreeElem(string spotName, int depth) : this()
    {
        mSpotName = spotName;
        mDepth = depth;
    }

    public override string ToString()
    {
        string msg = "";
        for (int jj = 0; jj < mDepth; jj++)
            msg += '-';
        msg += " '" + mSpotName + "' (" + mNumRefs + ", " + mNumKids + ")";

        return msg;
    }


    public override bool Equals(object obj)
    {
        TreeElem li = obj as TreeElem;
        return (mSpotName == li.mSpotName);  // ID is a property of MyListItem
    }

    public override int GetHashCode()
    {
        return mSpotName.GetHashCode();
    }

    //public override string ToString()
    //{
    //    return toString();
    //}

    public static bool operator ==(TreeElem a, TreeElem b)
    {
        // If both are null, or both are same instance, return true.
        if (System.Object.ReferenceEquals(a, b))
        {
            return true;
        }

        // If one is null, but not both, return false.
        if (((object)a == null) || ((object)b == null))
        {
            return false;
        }

        // Return true if the fields match:
        return a.mSpotName == b.mSpotName;
    }

    public static bool operator !=(TreeElem a, TreeElem b)
    {
        return !(a == b);
    }

    internal bool matchesString(string pathSegment)
    {
        return (mSpotName == pathSegment);
    }
}

public class OwlTree 
{
    public TreeNode<TreeElem> m_tree;

    public OwlTree()
    {
        m_tree = new TreeNode<TreeElem>(new TreeElem("root",0));
    }


    public override string ToString()
    {
        return m_tree.ToString();
    }


    internal void AddNode(NodeInstance graphNode)
    {
        TreeNode<TreeElem> treePtr = m_tree;
        Debug.Log("Adding " + graphNode.m_owlNode.ID);

        int depth = 0;

        m_tree.Value.mNumRefs++;

        foreach (string pathSegment in graphNode.m_pathSegments)
        {
            depth++;
            bool found = false;
            TreeNode<TreeElem> newPtr = null;

            ReadOnlyCollection<TreeNode<TreeElem>> kids = treePtr.Children;

            foreach (TreeNode<TreeElem> kid in kids)
            {
                TreeElem kidElem = kid.Value;

                found = kidElem.matchesString(pathSegment);

                if (found)
                {
                    Debug.Log("found " + pathSegment);
                    kidElem.mNumRefs++;
                    newPtr = kid;
                    break;
                }
            }

            if (!found)
            {
                newPtr = treePtr.AddChild(new TreeElem(pathSegment,depth));
                Debug.Log("adding '" + pathSegment + "'");
                treePtr.Value.mNumKids++;
            }

            treePtr = newPtr;
        }
        treePtr.Value.mNode = graphNode;
        //Debug.Log("done");
    }
}
