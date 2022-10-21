using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IFieldObject
{
    public int collumn {get; set;}
    public int row {get; set;}
    public bool IsSameColor(int color);
    public void ActivateEffect();
}
