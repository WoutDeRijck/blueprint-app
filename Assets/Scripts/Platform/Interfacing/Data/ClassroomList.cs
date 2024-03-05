using System.Collections.Generic;
using UnityEngine;

namespace Platform.Interfacing.Data
{

/// <summary>
/// Class to store classroom data
/// </summary>
[System.Serializable]
public class ClassroomList
{ 
    public List<Classroom> classrooms;

    /// <summary>
    /// Instantiate teacher from JSON.
    /// </summary>
    /// <param name="jsonString"></param>
    /// <returns></returns>
    public static ClassroomList FromJson(string jsonString)
    {
        return JsonUtility.FromJson<ClassroomList>("{\"classrooms\":" + jsonString + "}");
    }
}

}
