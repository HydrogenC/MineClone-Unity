using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using static Globals;

public class GrassBlockState : IBlockState
{
    public object Clone()
    {
        return MemberwiseClone();
    }

    public Block GetBlockType()
    {
        return BlockTypes[2];
    }

    public Material GetFaceMaterial(int face)
    {
        if (face == 0)
        {
            if (!Materials.ContainsKey("grass_generated"))
            {
                Material mat = Material.Instantiate(Materials["grass_top"]);
                mat.color = new Color(134f / 255, 183f / 255, 131f / 255);
                Materials["grass_generated"] = mat;
            }

            return Materials["grass_generated"];
        }
        else if (face == 5)
        {
            return Materials["dirt"];
        }
        else
        {
            return Materials["grass_side"];
        }
    }

    public Facing GetFacicng()
    {
        return Facing.PosY;
    }

    public IBlockState SetFacing(Facing facing)
    {
        return this;
    }
}
