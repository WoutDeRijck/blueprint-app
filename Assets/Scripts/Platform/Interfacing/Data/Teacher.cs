using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Platform.Interfacing.Data
{

/// <summary>
/// Class for teacher data, inherits from abstract user class. 
/// </summary>
[System.Serializable]
public class Teacher
{
    public int id;
    public string name;
    public string email;


    public Teacher() {
        id = 1;
        name = "John Doe";
        email = "john@";
    }

    /// <summary>
    /// Instantiate teacher from JSON.
    /// </summary>
    /// <param name="jsonString"></param>
    /// <returns></returns>
    public static Teacher FromJson(string jsonString)
    {
        Teacher t = JsonUtility.FromJson<Teacher>(jsonString);
        return t;
    }

}

}
