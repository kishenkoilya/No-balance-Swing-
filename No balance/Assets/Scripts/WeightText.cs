using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

[RequireComponent(typeof(TextMeshPro))]
public class WeightText : MonoBehaviour
{
    public TextMeshPro tmpro;
    private void Awake() {
        tmpro = GetComponent<TextMeshPro>();
    }
}
