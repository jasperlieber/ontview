using Overby.Collections;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using UnityEngine;


public class OwlTreeNode
{
    private readonly string mSpotName;      // name of spot in tree
    public string mFullName { set; get; }   // full name (concat'd uri pieces)
    public int nLeaves { set; get; }        // number of leaves below this node
    public NodeInstance mNodeInstance { set; get; } // only set if a leaf - null otherwise
    public int mDepth;                      // depth of this node in tree
    public int mNumKids { set; get; }       // number of kids of this node
    public Vector3 mPos { set; get; }       // position for the node
    public float mAlpha { set; get; }       // radial start for this node
    public float mRange { set; get; }       // radial range for this node
    public int mKidNum;                     // child number of this node
    public bool mClicked { set; get; }      // has the node been clicked?

    public OwlTreeNode()
    {
        mSpotName = null;
        nLeaves = 0;
        mNodeInstance = null;
        mDepth = 0;
        mNumKids = 0;
        mPos = new Vector3();
        mKidNum = 0;
        mRange = 0;
        mAlpha = 0;
        mClicked = false;
    }

    public OwlTreeNode(string spotName, int depth, int kidCnt) : this()
    {
        mSpotName = spotName;
        mDepth = depth;
        mKidNum = kidCnt;
    }

    public override string ToString()
    {
        string msg = mSpotName;

        if (GraphManager.m_debug)
        {
            msg += '\n';
            for (int jj = 0; jj < mDepth; jj++)
                msg += '-';
            msg += " (Depth = " + mDepth
                + " mRange=" + mRange
                + " mAlpha=" + mAlpha
                + " nLeaves=" + nLeaves
                + " mKidNum=" + mKidNum
                + " mNumKids=" + mNumKids + ")";

        }

        msg += '\n' + mFullName;

        return msg;
    }


    public override bool Equals(object obj)
    {
        OwlTreeNode li = obj as OwlTreeNode;
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

    public static bool operator ==(OwlTreeNode a, OwlTreeNode b)
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

    public static bool operator !=(OwlTreeNode a, OwlTreeNode b)
    {
        return !(a == b);
    }

    internal bool matchesString(string pathSegment)
    {
        return (mSpotName == pathSegment);
    }

}
