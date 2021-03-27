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

    public Material[] OverrideMaterials
    {
        get;
        set;
    } = new Material[6] { null, null, null, null, null, null };

    public string[] BaseMaterialNames
    {
        get;
        set;
    }

    public static bool operator ==(Block a, Block b) => a.BlockId == b.BlockId;
    public static bool operator !=(Block a, Block b) => a.BlockId != b.BlockId;
}
