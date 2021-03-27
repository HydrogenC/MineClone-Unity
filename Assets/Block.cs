using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

public class Block
{
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
}
