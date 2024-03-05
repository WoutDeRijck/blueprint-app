using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;

namespace GameData
{

    /// <summary>
    /// Representation of expected input for subtype Multiple Choice (MPC)
    /// </summary>
    [Serializable]
    public class MultipleChoice : Minigame
    {
        public List<string> questions;
        public List<string> correctAnswers;
        public List<List<string>> wrongAnswers;

        public MultipleChoice(Subject subject, string tag)
        : base(Subtype.MultipleChoice, subject, tag)
        {
            this.difficulties = new List<int>();
            this.questions = new List<string>();
            this.correctAnswers = new List<string>();
            this.wrongAnswers = new List<List<string>>();
            this.amountOfQuestions = 5;
        }

        [JsonConstructor]
        public MultipleChoice(Subject subject, string tag, List<int> difficulties, List<string> questions, List<string> correctAnswers, List<List<string>> wrongAnswers)
            : base(Subtype.MultipleChoice, subject, tag)
        {
            // Initialize when property is null
            this.questions = questions ?? new List<string>();
            if (difficulties != null)
            {
                this.difficulties = difficulties;
            }
            else
            {
                this.difficulties = new List<int>();
                for (int i = 0; i < this.questions.Count; i++) { this.difficulties.Add(2); }

            }
            this.wrongAnswers = wrongAnswers ?? new List<List<string>>();
            this.correctAnswers = correctAnswers ?? new List<string>();
            this.amountOfQuestions = 5;
        }

        /// <summary>
        /// Get the header from the CSV for MultipleChoice questions
        /// </summary>
        /// <returns>String array of the header</returns>
        public override string[] GetCSVHeader()
        {
            return new string[] { "Moeilijkheid (1-3)", "Vraag", "Juist antwoord", "Fout antwoord", "Fout antwoord", "Fout antwoord" };
        }

        /// <summary>
        /// Populate MultipleChoice data from a formatted CSV
        /// </summary>
        public override bool ReadFromCSV(List<string[]> csv)
        {
            // Reset content
            difficulties.Clear();
            questions.Clear();
            correctAnswers.Clear();
            wrongAnswers.Clear();

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
                difficulties.Add(difficulty);

                // Check if format of questions is good
                if (row.Skip(1).Take(5).Any(x => x == "")) continue;

                questions.Add(row[1]);
                correctAnswers.Add(row[2]);

                List<string> wrongAnswers = new List<string>();
                for (int i = 3; i < row.Length; i++) wrongAnswers.Add(row[i]);
                this.wrongAnswers.Add(wrongAnswers);
            }

            this.amountOfQuestions = 5;

            return true;
        }

        /// <summary>
        /// Convert the MultipleChoice data to a formatted CSV
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
            // If there are more elements then difficulty levels, set difficulty levels to 2
            if (questions.Count < correctAnswers.Count)
            {
                int difference = correctAnswers.Count - questions.Count;
                for (int i = 0; i < difference; i++) { correctAnswers.RemoveAt(correctAnswers.Count - 1); }
            }

            for (int i = 0; i < questions.Count; i++)
            {
                // Add a row for each question
                string[] row = new string[6];

                row[0] = difficulties[i].ToString();
                row[1] = questions[i];
                row[2] = correctAnswers[i];
                for (int j = 3; j < 6; j++) row[j] = wrongAnswers[i][j - 3];

                csv.Add(row);
            }

            return csv;
        }
    }
}