using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IFieldObject
{
    public bool IsSameColor(int color);
    public void ActivateEffect();
    public int GetWeight();
}
