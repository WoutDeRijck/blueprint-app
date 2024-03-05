using System;
using System.Collections.Generic;
using Newtonsoft.Json;
using System.Linq;

namespace GameData
{

/// <summary>
/// Representation of expected input for subtype Grouping
/// </summary>
[Serializable]
public class Grouping : Minigame
{
    public List<string> groupNames;

    public List<int> correctGroups; // List of indeces of the correct group
    public List<string> elements; // Elements of the grouping minigame

    public Grouping(Subject subject, string tag)
        : base(Subtype.Grouping, subject, tag)
    {
        this.difficulties = new List<int>();
        this.groupNames = new List<string>();
        this.correctGroups = new List<int>();
        this.elements = new List<string>();
        this.amountOfQuestions = 5;
        this.slowTime = 4.0f;
        this.fastTime = 0.5f;
    }

    [JsonConstructor]
    public Grouping(Subject subject, string tag, List<int> difficulties, List<string> groupNames, List<int> correctGroups, List<string> elements) 
        : base(Subtype.Grouping, subject, tag)
    {
        this.difficulties = difficulties ?? new List<int>();
        this.groupNames = groupNames ?? new List<string>();
        this.correctGroups = correctGroups ?? new List<int>();
        this.elements = elements ?? new List<string>();
        this.amountOfQuestions = 5;
        this.slowTime = 4.0f;
        this.fastTime = 0.5f;
    }

    /// <summary>
    /// Get the header from the CSV for Grouping questions
    /// </summary>
    /// <returns>String array of the header</returns>
    public override string[] GetCSVHeader()
    {
        return new string[]{ "Moeilijkheid (1-3)", "GroepsNaam1", "GroepsNaam2", "GroepsNaam3", "GroepsNaam4" };
    }

    /// <summary>
    /// Populate Grouping data from a formatted CSV
    /// </summary>
    public override bool ReadFromCSV(List<string[]> csv)
    {
        // Reset content
        difficulties.Clear();
        groupNames.Clear();
        correctGroups.Clear();
        elements.Clear();


        // Assign group names
        string[] header = csv[0];
        if (header.Skip(1).Take(4).Any(x => x == "")) return false;
        for (int i = 1; i < header.Length; i++) groupNames.Add(header[i]);

        foreach (string[] row in csv.Skip(1))
        {
            if (row[0] == "") row[0] = "2";
            int difficulty;
            try
            {
                difficulty = int.Parse(row[0]);
            }
            catch (FormatException ex)
            {
                UnityEngine.Debug.LogError(ex.Message);
                difficulty = 2;
            }
            if (difficulty < 0) difficulty = 0;
            if (difficulty > 3) difficulty = 3;

            // Add group elements
            for (int i = 1; i < 5; i++)
            {
                if (i < row.Length && row[i] != "")
                {
                    correctGroups.Add(i - 1);
                    elements.Add(row[i]);
                    difficulties.Add(difficulty);
                }
            }
        }
        return true;
    }

    /// <summary>
    /// Convert the Grouping data to a formatted CSV
    /// </summary>
    /// <returns>List of string arrays which represents the CSV</returns>
    public override List<string[]> ConvertToCSV()
    {
        List<string[]> csv = new List<string[]>();

        // Add the header 
        csv.Add(GetCSVHeader());

        // When nothing has ben made, just return header
        if (groupNames.Count != 4) return csv;

        // Group names
        for (int j = 1; j < 5; j++) csv[0][j] = groupNames[j-1];

        // When nothing has ben made, just return header with right group names
        if (correctGroups.Count == 0 || elements.Count == 0) return csv;

        // If there are more elements then difficulty levels, set difficulty levels to 2
        if (difficulties.Count < elements.Count)
        {
            int difference = elements.Count - difficulties.Count;
            for (int i = 0; i < difference; i++) { difficulties.Add(2); }
        }
        if (correctGroups.Count < elements.Count)
        {
            int difference = elements.Count - correctGroups.Count;
            for (int i = 0; i < difference; i++) { correctGroups.Add(0); }
        }

        for (int i = 0; i < elements.Count; i++)
        {
            // Add a row for each question
            string[] row = new string[5];

            row[0] = difficulties[i].ToString();

            for (int j = 1; j < 5; j++)
            {
                if (j - 1 == correctGroups[i])
                {
                    row[correctGroups[i] + 1] = elements[i];
                }
                else
                {
                    row[j] = "";
                }
            }
            csv.Add(row);
        }

        return csv;
    }
}
}