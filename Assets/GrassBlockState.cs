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

    public Rect GetFaceUVRect(int face)
    {
        if (face == 0)
        {
            if (!Textures.ContainsKey("grass_generated"))
            {/*
                Material mat = Material.Instantiate(Textures["grass_top"]);
                mat.color = new Color(134f / 255, 183f / 255, 131f / 255);
            */}

            return Textures["grass_top"];
        }
        else if (face == 5)
        {
            return Textures["dirt"];
        }
        else
        {
            return Textures["grass_side"];
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
