using System.Collections.Generic;
using NUnit.Framework;
using System.Reflection;
using GameData;

/// <summary>
/// This class (unit) tests the functionality of the Minigame data class
/// </summary>
public class MinigameDataTests
{
    [Test]
    public void GetMinigameFromData_MultipleChoice()
    {
        // Arrange
        Minigame minigame = new Minigame(Subtype.MultipleChoice, Subject.Mathematics, "Test");
        minigame.data = "Moeilijkheid (1-3);Vraag;Juist antwoord;Fout antwoord;Fout antwoord;Fout antwoord\n3;What is 2x2?;4;1;2;3\n";

        MultipleChoice expected = new MultipleChoice
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

        // Act
        Minigame result = minigame.GetMinigameFromData();

        // Assert
        Assert.IsInstanceOf(typeof(MultipleChoice), result);
        Assert.AreEqual(expected.questions, ((MultipleChoice) result).questions);
        Assert.AreEqual(expected.difficulties, ((MultipleChoice)result).difficulties);
        Assert.AreEqual(expected.correctAnswers, ((MultipleChoice)result).correctAnswers);
        Assert.AreEqual(expected.wrongAnswers, ((MultipleChoice)result).wrongAnswers);
    }

    [Test]
    public void GetMinigameFromData_Grouping()
    {
        // Arrange
        Minigame minigame = new Minigame(Subtype.Grouping, Subject.Mathematics, "Test");
        minigame.data = "Moeilijkheid (1-3);Water;Ground;Sky;Old\n2;Fish;Ape;Bird;Dinosaur\n";

        Grouping expected = new Grouping
           (
               Subject.Mathematics,
               "Test",
               new List<int>() { 2, 2, 2, 2 },
               new List<string>() { "Water", "Ground", "Sky", "Old" },
               new List<int>() { 0, 1, 2, 3 },
               new List<string>(){ "Fish", "Ape", "Bird", "Dinosaur"}
           );

        // Act
        Minigame result = minigame.GetMinigameFromData();

        // Assert
        Assert.IsInstanceOf(typeof(Grouping), result);
        Assert.AreEqual(expected.groupNames, ((Grouping) result).groupNames);
        Assert.AreEqual(expected.difficulties, ((Grouping)result).difficulties);
        Assert.AreEqual(expected.correctGroups, ((Grouping)result).correctGroups);
        Assert.AreEqual(expected.elements, ((Grouping)result).elements);
    }

    [Test]
    public void GetMinigameFromData_Ranking()
    {
        // Arrange
        Minigame minigame = new Minigame(Subtype.Ranking, Subject.Mathematics, "Test");
        minigame.data = "Moeilijkheid (1-3);Vraag;Element 1;Elemenent 2;Elemenent 3;Element 4\n1;Sort Ascending;1;2;3;4\n";

        Ranking expected = new Ranking
           (
               Subject.Mathematics,
               "Test",
               new List<int>() { 1 },
               new List<string>() { "Sort Ascending" },
               new List<List<string>>() {
                   new List<string>(){"1", "2", "3", "4"}
               }
           );

        // Act
        Minigame result = minigame.GetMinigameFromData();

        // Assert
        Assert.IsInstanceOf(typeof(Ranking), result);
        Assert.AreEqual(expected.difficulties, ((Ranking)result).difficulties);
        Assert.AreEqual(expected.questions, ((Ranking)result).questions);
        Assert.AreEqual(expected.correctRankings, ((Ranking)result).correctRankings);
    }

    [Test]
    public void SetData_Minigame_EmptyData()
    {
        // Arrange
        Minigame minigame = new Minigame(Subtype.MultipleChoice, Subject.Mathematics, "Test");

        // Act
        minigame.SetData();

        // Assert
        Assert.AreEqual("", minigame.data);
    }

    [Test]
    public void SetData_MultipleChoice_DataSet()
    {
        // Arrange
        MultipleChoice minigame = new MultipleChoice
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

        // Act
        minigame.SetData();

        // Assert
        Assert.AreEqual("Moeilijkheid (1-3);Vraag;Juist antwoord;Fout antwoord;Fout antwoord;Fout antwoord\n3;What is 2x2?;4;1;2;3\n", minigame.data);
    }
}
