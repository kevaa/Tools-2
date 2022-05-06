using UnityEngine;
using System.Collections.Generic;

[CreateAssetMenu(fileName = "New Language", menuName = "Language", order = 1)]
public class Language : ScriptableObject
{
    public string language;
    public List<string> keys = new List<string>();
    public List<string> values = new List<string>();
    public string GetTerm(string key)
    {
        Debug.Log(string.Join(" ", keys));
        Debug.Log(string.Join(" ", values));
        var ind = keys.IndexOf(key);
        return ind == -1 ? "" : values[ind];
    }

    public bool ContainsKey(string key)
    {
        return keys.Contains(key);
    }

    void Add(string key, string value)
    {
        keys.Add(key);
        values.Add(value);
    }

    public void Set(string key, string value)
    {
        var ind = keys.IndexOf(key);
        if (ind == -1)
        {
            Add(key, value);
        }
        else
        {
            values[ind] = value;
        }
    }
    public void Remove(string key)
    {
        var ind = keys.IndexOf(key);
        keys.RemoveAt(ind);
        values.RemoveAt(ind);
    }

    public void ResetTerms()
    {
        keys = new List<string>();
        values = new List<string>();
    }
}