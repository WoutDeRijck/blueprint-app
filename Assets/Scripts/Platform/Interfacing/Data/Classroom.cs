using System.Collections.Generic;
using UnityEngine;

namespace Platform.Interfacing.Data
{

/// <summary>
/// Class to store classroom data
/// </summary>
[System.Serializable]
public class Classroom
{
    public int id;
    public string name;
    public List<Student> students;

    /// <summary>
    /// Instantiate teacher from JSON.
    /// </summary>
    /// <param name="jsonString"></param>
    /// <returns></returns>
    public static Classroom FromJson(string jsonString)
    {
       return JsonUtility.FromJson<Classroom>(jsonString);
    }
}

}
