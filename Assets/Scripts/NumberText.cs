﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

//use this class as a workaround for text values not updating sometimes
[RequireComponent(typeof(TextMeshPro))]
public class NumberText : MonoBehaviour
{
    private TextMeshPro tmp;
    //use this variable to compare against integer values as opposed to its text property value
    public int value;
    public string text
    {
        get { return tmp.text; }
        set { tmp.text = value; }
    }

    private void Awake()
    {
        tmp = GetComponent<TextMeshPro>();
        Debug.Assert(tmp != null);
    }

    public Vector2 GetPreferredValues()
    {
        return tmp.GetPreferredValues();
    }

    public Vector2 GetPreferredValues(float width,float height)
    {
        return tmp.GetPreferredValues(width, height);
    }

    public Vector2 GetPreferredValues(string text)
    {
        return tmp.GetPreferredValues(text);
    }

    public Vector2 GetPreferredValues(string text,float width,float height)
    {
        return tmp.GetPreferredValues(text, width, height);
    }
}
