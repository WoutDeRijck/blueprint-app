using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;
using Newtonsoft.Json;

namespace GameData
{

/// <summary>
/// Representation of expected input for subtype Ranking
/// </summary>
[Serializable]
public class Ranking : Minigame
{
    public List<string> questions;
    public List<List<string>> correctRankings;

    public Ranking(Subject subject, string tag)
        : base(Subtype.Ranking, subject, tag)
    {
        this.difficulties = new List<int>();
        this.questions = new List<string>();
        this.correctRankings = new List<List<string>>();
        this.amountOfQuestions = 5;
    }

    [JsonConstructor]
    public Ranking(Subject subject, string tag, List<int> difficulties, List<string> questions, List<List<string>> correctRankings) 
        : base(Subtype.Ranking, subject, tag)
    {
        this.questions = questions ?? new List<string>();
        if (difficulties != null)
        {
            this.difficulties = difficulties;
        }
        else
        {
            for (int i = 0; i < questions.Count; i++) { difficulties.Add(2); }

        }
        this.correctRankings = correctRankings ?? new List<List<string>>();
        this.amountOfQuestions = 5;
    }

    /// <summary>
    /// Get the header from the CSV for Ranking questions
    /// </summary>
    /// <returns>String array of the header</returns>
    public override string[] GetCSVHeader()
    {
        return new string[]{ "Moeilijkheid (1-3)", "Vraag", "Element 1", "Elemenent 2", "Elemenent 3", "Element 4" };
    }

    /// <summary>
    /// Populate Ranking data from a formatted CSV
    /// </summary>
    public override bool ReadFromCSV(List<string[]> csv)
    {
        // Reset content
        difficulties.Clear();
        questions.Clear();
        correctRankings.Clear();

        foreach (string[] row in csv.Skip(1))
        {
            // Check if elements are filled in
            if (row.Length < 4) continue;
            // Check if format of questions is good
            if (row.Skip(1).Take(5).Any(x => x == "")) continue;

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
            difficulties.Add(difficulty);

            questions.Add(row[1]);
            List<string> correctRanking = new List<string>();
            for (int i = 2; i < 6; i++) correctRanking.Add(row[i]);
            correctRankings.Add(correctRanking);
        }

        return true;
    }

    /// <summary>
    /// Convert the Ranking data to a formatted CSV
    /// </summary>
    /// <returns>List of string arrays which represents the CSV</returns>
    public override List<string[]> ConvertToCSV()
    {
        List<string[]> csv = new List<string[]>();

        // Add the header 
        csv.Add(GetCSVHeader());

        // When nothing has ben made, just return header
        if (questions.Count == 0) return csv;

        // If there are more elements then difficulty levels, set difficulty levels to 2
        if (difficulties.Count < questions.Count)
        {
            int difference = questions.Count - difficulties.Count;
            for (int i = 0; i < difference; i++) { difficulties.Add(2); }
        }

        for (int i = 0; i < questions.Count; i++)
        {
            // Add a row for each question
            string[] row = new string[6];

            row[0] = difficulties[i].ToString();
            row[1] = questions[i];
            for (int j = 2; j < 6; j++) row[j] = correctRankings[i][j - 2];
            
            csv.Add(row);
        }

        return csv;
    }
}
}