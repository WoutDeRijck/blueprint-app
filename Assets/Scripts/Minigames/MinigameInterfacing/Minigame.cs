    using System;
using System.Collections.Generic;
using UnityEngine;
using Newtonsoft.Json;
using Misc;
using System.ComponentModel;

namespace GameData
{

public enum Subtype
{
    [Description("Meerkeuze")]
    MultipleChoice,
    [Description("Groeperen")]
    Grouping,
    [Description("Sorteren")]
    Ranking
}

public enum Subject
{
    [Description("Wiskunde")]
    Mathematics,
    [Description("Nederlands")]
    WritingDutch,
    [Description("Lezen")]
    ReadingDutch,
    [Description("Wereld Orientatie")]
    WorldOrientation,
    [Description("Religie")]
    Religion,
    [Description("Zedenleer")]
    Ethics,
    [Description("Frans")]
    French
}


/// <summary>
/// Representation of all necessary data to play a minigame
/// Parent class for different types of input depending on subtype
/// </summary>
[Serializable]
public class Minigame
{
    public int id;
    public Subtype subtype;
    public Subject subject;
    public string tag;
    public bool shared;

    // Difficulty per element per group
    [JsonIgnore]
    public List<int> difficulties = new List<int>();

    // Used in the minigame for Adaptive Learning
    [JsonIgnore]
    public double speed = 0.30;

    // Used to store in database and reconstruct from database
    public string data;

    [JsonIgnore]
    public int amountOfQuestions;

    [JsonIgnore]
    public float slowTime = 7.0f; // default, configure per minigame type!
    [JsonIgnore]
    public float fastTime = 2.0f; // default, configure per minigame type!

    public Minigame(Subtype subtype, Subject subject, string tag)
    {
        this.subtype = subtype;
        this.subject = subject;
        this.tag = tag;
    }

    /// <summary>
    /// Get the format depending of the subtype of minigame
    /// </summary>
    public virtual string[] GetCSVHeader() 
    {
        return new string[0];
    }
    
    /// <summary>
    /// Populate data depending on subtype
    /// </summary>
    /// <returns>A bool that indicates that population was succesful</returns>
    public virtual bool ReadFromCSV(List<string[]> csv) { return false; }

    /// <summary>
    /// Convert the data of the minigame to a CSV format
    /// </summary>
    public virtual List<string[]> ConvertToCSV() { return new List<string[]>(); }
    
    /// <summary>
    /// Set data of the minigame
    /// </summary>
    public void SetData() 
    {
        List<string[]> csv = ConvertToCSV();
        string csvData = "";

        foreach (string[] row in csv)
        {
            csvData += string.Join(";", row) + "\n";
        }

        this.data = csvData;
    }

    /// <summary>
    /// Get a downcasted minigame object depending on the subject and data
    /// </summary>
    public Minigame GetMinigameFromData()
    {
        // when data wasn't set, we can't do anything
        if (data == null) {return this;}

        Minigame minigame = this;
        List<string[]> csv = CSVManager.StringToCSVList(this.data);
        switch (subtype)
        {
            case Subtype.MultipleChoice:
                minigame = new MultipleChoice(this.subject, this.tag);
                minigame.ReadFromCSV(csv);
                break;
            case Subtype.Grouping:
                minigame = new Grouping(this.subject, this.tag);
                minigame.ReadFromCSV(csv);
                break;
            case Subtype.Ranking:
                minigame = new Ranking(this.subject, this.tag);
                minigame.ReadFromCSV(csv);
                break;
            default: 
                break;
        }
        return minigame;
    }

    /// <summary>
    ///     Returns a list of question indeces that are of the inputted difficulty level.
    ///     Can be useful for AL algorithms.
    /// </summary>
    /// <returns>
    ///     List of integers (corresponding to quesion indexes)
    /// </returns>
    public List<int> GetDifficultyIndeces(int diff) {
        List<int> lst = new List<int>();

        for (int i = 0; i < difficulties.Count; i++) {
            if (difficulties[i] == diff) {
                lst.Add(i);
            }
        }

        return lst;
    }

    /// <summary>
    /// Get Minigame object from a json string
    /// </summary>
    /// <returns>The Minigame object</returns>
    public static Minigame FromJson(string jsonString)
    {
        if (jsonString != null)
        {
            return JsonConvert.DeserializeObject<Minigame>(jsonString);
        } else
        {
            return null;
        }
    }

    /// <summary>
    /// Get multiple Minigame objects from a json string
    /// </summary>
    /// <returns>List of the Minigame objects</returns>
    public static List<Minigame> MultipleFromJson(string jsonString) 
    {
        if (jsonString != null)
        {
            return JsonConvert.DeserializeObject<List<Minigame>>(jsonString);
        } else
        {
            Debug.LogError("Empty object");
            return null;
        }
    }

}
}
