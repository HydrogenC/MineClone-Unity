using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

public class EmptyBlockState : IBlockState
{
    public object Clone()
    {
        return this;
    }

    public Block GetBlockType()
    {
        return Globals.BlockTypes[0];
    }

    public Rect GetFaceUVRect(int face)
    {
        throw new NotImplementedException();
    }

    public Facing GetFacicng()
    {
        throw new NotImplementedException();
    }

    public IBlockState SetFacing(Facing facing)
    {
        return this;
    }
}
