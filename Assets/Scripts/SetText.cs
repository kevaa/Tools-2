using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
[RequireComponent(typeof(Text))]
public class SetText : MonoBehaviour
{
    Text text;
    [SerializeField] Language globalLanguage;
    void Start()
    {
        text = GetComponent<Text>();
    }
    void Update()
    {
        var playText = globalLanguage.GetTerm("Play");
        var exitText = globalLanguage.GetTerm("Exit");
        text.text = playText + "\n" + exitText;
    }
}
