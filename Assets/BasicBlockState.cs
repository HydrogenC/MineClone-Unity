using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

public class BasicBlockState : IBlockState
{
    Block blockType;
    Facing facing;

    public string[] MaterialNames
    {
        get;
        set;
    }

    public BasicBlockState(string[] materialNames)
    {
        facing = Facing.PosY;
        MaterialNames = materialNames;
    }

    public void SetBlockType(Block type)
    {
        blockType = type;
    }

    public Block GetBlockType()
    {
        return blockType;
    }

    public Material GetFaceMaterial(int face)
    {
        return Globals.Materials[MaterialNames[face]];
    }

    public Facing GetFacicng()
    {
        return facing;
    }

    public IBlockState SetFacing(Facing facing)
    {
        this.facing = facing;
        return this;
    }

    public object Clone()
    {
        return MemberwiseClone();
    }
}
