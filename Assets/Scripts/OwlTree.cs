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
    public string mFullName { set; get; }
    public int nLeaves { set; get; }    // number of references in owl to this node
    public NodeInstance mNode;          // only set if a leaf - null otherwise
    public int mDepth;                  // depth of this node in tree
    public int mNumKids { set; get; }   // number of kids of this node
    public Vector3 mPos { set; get; }   // position for the node
    public float mAlpha { set; get; }   // radial start for this node
    public float mRange { set; get; }   // radial range for this node
    public int mKidNum;                 // child number of this node

    public TreeElem()
    {
        mSpotName = null;
        nLeaves = 0;
        mNode = null;
        mDepth = 0;
        mNumKids = 0;
        mPos = new Vector3();
        mKidNum = 0;
        mRange = 0;
        mAlpha = 0;
    }

    public TreeElem(string spotName, int depth, int kidCnt) : this()
    {
        mSpotName = spotName;
        mDepth = depth;
        mKidNum = kidCnt;
    }

    public override string ToString()
    {
        string msg = mSpotName + '\n';
        for (int jj = 0; jj < mDepth; jj++)
            msg += '-';
        msg += " (Depth = " + mDepth + " nLeaves=" + nLeaves
            + " mKidNum=" + mKidNum + " mNumKids=" + mNumKids
            + " mPos=" + mPos 
            + " mRange=" + mRange + " mAlpha=" + mAlpha + ")\n" 
            + mFullName;

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
        m_tree = new TreeNode<TreeElem>(new TreeElem("root", 0, 0));
    }


    public override string ToString()
    {
        return m_tree.ToString();
    }


    internal TreeElem AddNode(NodeInstance graphNode)
    {
        TreeNode<TreeElem> treePtr = m_tree;
        
        int depth = 0;

        string fullName = "";

        foreach (string pathSegment in graphNode.m_pathSegments)
        {
            depth++;
            bool found = false;
            TreeNode<TreeElem> newPtr = null;

            fullName += pathSegment + " | ";

            foreach (var kid in treePtr.Children)
            {
                TreeElem kidElem = kid.Value;

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
                    new TreeElem(pathSegment, depth, treePtr.Value.mNumKids++));
                newPtr.Value.mFullName = fullName;
            }

            treePtr = newPtr;
        }
        //treePtr.Value.mNode = graphNode;

        return treePtr.Value;
    }


    internal void UpdateLeafCount(TreeNode<TreeElem> treeNode)
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
