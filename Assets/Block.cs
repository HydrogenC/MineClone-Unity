using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

public class Block
{
    public string BlockId
    {
        get;
        set;
    }

    public bool Solid
    {
        get;
        set;
    } = true;

    public bool Renderable
    {
        get;
        set;
    } = true;

    public IBlockState DefaultBlockState
    {
        get;
        set;
    }

    public override bool Equals(object obj)
    {
        return this == (Block)obj;
    }

    public override int GetHashCode()
    {
        return BlockId.GetHashCode();
    }

    public virtual void OnBreak(BlockPos pos, IBlockState state) { }
    public virtual void OnLeftClick(BlockPos pos, IBlockState state) { }
    public virtual void OnRightClick(BlockPos pos, IBlockState state) { }

    public virtual IBlockState GetDefaultBlockState(Facing facing)
    {
        return ((IBlockState)DefaultBlockState.Clone()).SetFacing(facing);
    }

    public static bool operator ==(Block a, Block b) => a.BlockId == b.BlockId;
    public static bool operator !=(Block a, Block b) => a.BlockId != b.BlockId;
}
