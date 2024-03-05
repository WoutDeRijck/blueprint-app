using System.Collections.Generic;
using NUnit.Framework;
using System.Reflection;
using GameData;

/// <summary>
/// This class (unit) tests the functionality of the Ranking data class
/// </summary>
public class RankingDataTests
{
    [Test]
    public void GetCSVHeader_ReturnsHeader()
    {
        // Arrange
        Ranking ranking = new Ranking(Subject.Mathematics, "");

        // Act & Assert
        Assert.AreEqual(new string[] { "Moeilijkheid (1-3)", "Vraag", "Element 1", "Elemenent 2", "Elemenent 3", "Element 4" }, ranking.GetCSVHeader());
    }

    [Test]
    public void ReadFromCSV_WrongFormattedCSV_ReconstructsPartialECS()
    {
        // Arrange
        Ranking ranking = new Ranking(Subject.Mathematics, "");

        List<string[]> csv = new List<string[]>() {
            new string[] { "Moeilijkheid (1-3)", "Vraag", "Element 1", "Elemenent 2", "Elemenent 3", "Element 4" },
            new string[] { "3", "Rank ascending", "1", "2", "3", "4", "Too many arguments", "Should not be included"},
            new string[] { "", "", "A", "B", "C"}
        };

        // Act
        ranking.ReadFromCSV(csv);

        // Assert
        Assert.AreEqual("Rank ascending", ranking.questions[0]);
        Assert.AreEqual(3, ranking.difficulties[0]);
        Assert.AreEqual(new List<string>() { "1", "2", "3", "4"}, ranking.correctRankings[0]);
    }

    [Test]
    public void ConvertToCSV_ValidECS_EditableCSV()
    {
        // Arrange
        Ranking ranking = new Ranking
           (
               Subject.Mathematics,
               "Test",
               new List<int>() { 3 },
               new List<string>() { "Rank ascending" },
               new List<List<string>>()
                   {
                        new List<string>(){ "1", "2", "3", "4"},
                   }
           );

        List<string[]> expected = new List<string[]>() {
            new string[] { "Moeilijkheid (1-3)", "Vraag", "Element 1", "Elemenent 2", "Elemenent 3", "Element 4" },
            new string[] { "3", "Rank ascending", "1", "2", "3", "4"}
        };

        // Act
        List<string[]> result = ranking.ConvertToCSV();

        // Assert
        Assert.AreEqual(expected, result);
    }
}
