using NUnit.Framework;
using System.Collections.Generic;
using UnityEngine;

namespace Platform.Interfacing.Data
{

/// <summary>
/// Class for student data, inherits from abstract user class. 
/// </summary>
[System.Serializable]
public class Student
{
    public int id;
    public string name;
    public string email;

    /// <summary>
    /// Instantiate student from JSON.
    /// </summary>
    /// <param name="jsonString"></param>
    /// <returns></returns>
    public static Student FromJson(string jsonString)
    {
        return JsonUtility.FromJson<Student>(jsonString);
    }
    public static bool isValid(string name, string email)
    {
        if (name.Trim().Length == 0 || email.Trim().Length == 0) return false;

        return true;
    }
}

}
