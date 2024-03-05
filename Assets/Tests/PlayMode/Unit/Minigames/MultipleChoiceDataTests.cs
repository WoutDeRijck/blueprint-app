using System.Collections.Generic;
using NUnit.Framework;
using System.Reflection;
using GameData;

/// <summary>
/// This class (unit) tests the functionality of the MultipleChoice data class
/// </summary>
public class MultipleChoiceDataTests
{
    [Test]
    public void GetCSVHeader_ReturnsHeader()
    {
        // Arrange
        MultipleChoice mpc = new MultipleChoice(Subject.Mathematics, "");

        // Act & Assert
        Assert.AreEqual(new string[] { "Moeilijkheid (1-3)", "Vraag", "Juist antwoord", "Fout antwoord", "Fout antwoord", "Fout antwoord" }, mpc.GetCSVHeader());
    }

    [Test]
    public void ReadFromCSV_WrongFormattedCSV_ReconstructsPartialECS()
    {
        // Arrange
        MultipleChoice mpc = new MultipleChoice(Subject.Mathematics, "Test");

        List<string[]> csv = new List<string[]>() {
            new string[] { "Moeilijkheid (1-3)", "Vraag", "Juist antwoord", "Fout antwoord", "Fout antwoord", "Fout antwoord" },
            new string[] { "3", "What is 2x2?", "4", "1", "2", "3", "Too many arguments", "Should not be included"},
            new string[] { "", "", "A", "B", "C"}
        };

        // Act
        mpc.ReadFromCSV(csv);

        // Assert
        Assert.AreEqual("What is 2x2?", mpc.questions[0]);
        Assert.AreEqual(3, mpc.difficulties[0]);
        Assert.AreEqual("4", mpc.correctAnswers[0]);
        Assert.AreEqual("1", mpc.wrongAnswers[0][0]);
        Assert.AreEqual("2", mpc.wrongAnswers[0][1]);
        Assert.AreEqual("3", mpc.wrongAnswers[0][2]);
    }

    [Test]
    public void ConvertToCSV_ValidECS_EditableCSV()
    {
        // Arrange
        MultipleChoice mpc = new MultipleChoice
           (
               Subject.Mathematics,
               "Test",
               new List<int>() { 3 },
               new List<string>() { "What is 2x2?" },
               new List<string>() { "4" },
               new List<List<string>>()
                   {
                        new List<string>(){ "1", "2", "3"},
                   }
           );

        List<string[]> expected = new List<string[]>()
        {
            new string[]{ "Moeilijkheid (1-3)", "Vraag", "Juist antwoord", "Fout antwoord", "Fout antwoord", "Fout antwoord" },
            new string[]{ "3", "What is 2x2?", "4", "1", "2", "3"},
        };

        // Act
        List<string[]> result = mpc.ConvertToCSV();

        // Assert
        Assert.AreEqual(expected, result);
    }
}
