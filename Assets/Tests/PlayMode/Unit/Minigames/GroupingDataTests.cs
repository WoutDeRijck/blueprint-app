using System.Collections.Generic;
using NUnit.Framework;
using System.Reflection;
using System.Diagnostics;
using GameData;

/// <summary>
/// This class (unit) tests the functionality of the Grouping data class
/// </summary>
public class GroupingDataTests
{
    [Test]
    public void GetCSVHeader_ReturnsHeader()
    {
        // Arrange
        Grouping grouping = new Grouping(Subject.Mathematics, "");

        // Act & Assert
        Assert.AreEqual(new string[] { "Moeilijkheid (1-3)", "GroepsNaam1", "GroepsNaam2", "GroepsNaam3", "GroepsNaam4" }, grouping.GetCSVHeader());
    }

    [Test]
    public void ReadFromCSV_WrongFormattedCSV_ReconstructsPartialECS()
    {
        // Arrange
        Grouping grouping = new Grouping(Subject.Mathematics, "Test");

        List<string[]> csv = new List<string[]>() {
            new string[] { "Moeilijkheid (1-3)", "Water", "Ground", "Sky", "Old" },
            new string[] { "3", "Fish", "Ape", "Bird", "Dinosaur", "Too many arguments", "Should not be included"},
            new string[] { "", "", "A", "B", "C"}
        };

        // Act
        grouping.ReadFromCSV(csv);

        // Assert
        Assert.AreEqual(new List<int>() { 3, 3, 3, 3, 2, 2, 2}, grouping.difficulties);
        Assert.AreEqual(new List<string>() { "Water", "Ground", "Sky", "Old" }, grouping.groupNames);
        Assert.AreEqual(new List<int>() { 0, 1, 2, 3, 1, 2, 3 }, grouping.correctGroups);
        Assert.AreEqual(new List<string>() { "Fish", "Ape", "Bird", "Dinosaur", "A", "B", "C" }, grouping.elements);
    }

    [Test]
    public void ConvertToCSV_ValidECS_EditableCSV()
    {
        // Arrange
        Grouping grouping = new Grouping
           (
               Subject.Mathematics,
               "Test",
               new List<int>() { 2, 2, 3, 3 },
               new List<string>() { "Water", "Ground", "Sky", "Old" },
               new List<int>() { 0, 1, 2, 3 },
               new List<string>() { "Fish", "Ape", "Bird", "Dinosaur" }
           );

        List<string[]> expected = new List<string[]>()
        {
            new string[]{ "Moeilijkheid (1-3)", "Water", "Ground", "Sky", "Old" },
            new string[]{ "2", "Fish", "", "", ""},
            new string[]{ "2", "", "Ape", "", ""},
            new string[]{ "3", "", "", "Bird", ""},
            new string[]{ "3", "", "", "", "Dinosaur"},
        };

        // Act
        List<string[]> result = grouping.ConvertToCSV();

        // Assert
        Assert.AreEqual(expected, result);
    }
}
