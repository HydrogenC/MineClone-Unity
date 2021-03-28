using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

public enum Facing
{
    PosX,
    PosZ,
    NegX,
    NegZ,
    PosY,
    NegY
}

public interface IBlockState : ICloneable
{
    public Block GetBlockType();

    public Material GetFaceMaterial(int face);

    public Facing GetFacicng();
    public IBlockState SetFacing(Facing facing);
}