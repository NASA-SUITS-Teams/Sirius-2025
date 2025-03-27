using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using System;

// Path to a data value from a specified JSON.
[Serializable]
public class TMPath
{
    [SerializeField]
    private string jsonFilePath;
    [SerializeField]
    private List<string> jsonKeyPath; 
    
    public TMPath(string jsonFilePath, List<string> jsonKeyPath)
    {
        this.jsonFilePath = jsonFilePath;
        this.jsonKeyPath = jsonKeyPath ?? new List<string>();
    }

    public override string ToString()
    {
        return $"JSON File: {jsonFilePath}\nJSON Key Path: {string.Join(" -> ", jsonKeyPath)}";
    }

    public override bool Equals(object obj)
    {
        if (obj is TMPath other)
        {
            return jsonFilePath == other.jsonFilePath &&
                   jsonKeyPath.SequenceEqual(other.jsonKeyPath);
        }
        return false;
    }

    public override int GetHashCode()
    {
        int hash = jsonFilePath?.GetHashCode() ?? 0;
        foreach (var key in jsonKeyPath)
        {
            hash = (hash * 397) ^ key.GetHashCode(); // Hash combination
        }
        return hash;
    }

    public string GetJSONFilePath()
    {
        return jsonFilePath;
    }

    public List<string> GetJSONKeyPath()
    {
        return jsonKeyPath;
    }
}
