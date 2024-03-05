using System.Collections.Generic;
using NUnit.Framework;
using System.Reflection;
using GameData;

using Platform.Controllers;
using Platform.CustomControls;

namespace PlatformTests
{

/// <summary>
/// This class (unit) tests the functionality of the ContentEditor
/// </summary>
public class ContentEditorTests
{
    [Test]
    public void SetupECS_WhenViewIsNotNull_ShouldSubscribeToEvents()
    {
        // Arrange
        var contentEditor = new ContentEditor();
        var view = new EcsEditorPopup();
        var saveEventFired = false;
        var openEventFired = false;
        string gptEventFired = "";

        view.saveEvent += () => saveEventFired = true;
        view.openEvent += () => openEventFired = true;
        view.gptEvent += (s) => gptEventFired = "Test";

        // Act
        contentEditor.SetupECS(view);

        // Assert
        Assert.IsNotNull(view.saveEvent);
        Assert.IsNotNull(view.openEvent);
        Assert.IsNotNull(view.gptEvent);

        view.saveEvent.Invoke();
        view.openEvent.Invoke();
        view.gptEvent.Invoke("Test");

        Assert.IsTrue(saveEventFired);
        Assert.IsTrue(openEventFired);
        Assert.AreEqual(gptEventFired, "Test");
    }

    [Test]
    public void SetupECS_WhenViewIsNull_ShouldNotThrowException()
    {
        // Arrange
        var contentEditor = new ContentEditor();

        // Act
        TestDelegate act = () => contentEditor.SetupECS(null);

        // Assert
        Assert.DoesNotThrow(act);
    }

    [Test]
    public void CSVToMinigame_WhenMinigameNull_ReturnsNull()
    {
        // Arrange
        var contentEditor = new ContentEditor();
        contentEditor.minigame = null;

        MethodInfo method = typeof(ContentEditor).GetMethod("CSVToMinigame", BindingFlags.NonPublic | BindingFlags.Instance);

        List<string[]> csv = new List<string[]>() {
            new string[] { "Moeilijkheid (1-3)", "Vraag", "Juist antwoord", "Fout antwoord", "Fout antwoord", "Fout antwoord" },
            new string[] { "3", "What is 2x2?", "4", "1", "2", "3"}
        };

        // Act
        TestDelegate act = () => method.Invoke(contentEditor, new object[] { csv });

        // Assert
        Assert.That(act, Throws.Nothing);
    }

    [Test]
    public void CSVToMinigame_FormatIsGood_DataIsPopulated()
    {
        // Arrange
        var contentEditor = new ContentEditor();
        contentEditor.minigame = new MultipleChoice(
            Subject.Mathematics,
            "Test");

        MethodInfo method = typeof(ContentEditor).GetMethod("CSVToMinigame", BindingFlags.NonPublic | BindingFlags.Instance);

        List<string[]> csv = new List<string[]>() {
            new string[] { "Moeilijkheid (1-3)", "Vraag", "Juist antwoord", "Fout antwoord", "Fout antwoord", "Fout antwoord" },
            new string[] { "3", "What is 2x2?", "4", "1", "2", "3"}
        };

        // Act
        method.Invoke(contentEditor, new object[] { csv });
        contentEditor.minigame.SetData();

        // Assert
        Assert.AreEqual(contentEditor.minigame.data, "Moeilijkheid (1-3);Vraag;Juist antwoord;Fout antwoord;Fout antwoord;Fout antwoord\n3;What is 2x2?;4;1;2;3\n");
        Assert.AreEqual(((MultipleChoice)contentEditor.minigame).questions[0], "What is 2x2?");
        Assert.AreEqual(((MultipleChoice)contentEditor.minigame).difficulties[0], 3);
        Assert.AreEqual(((MultipleChoice)contentEditor.minigame).correctAnswers[0], "4");
        Assert.AreEqual(((MultipleChoice)contentEditor.minigame).wrongAnswers[0][0], "1");
        Assert.AreEqual(((MultipleChoice)contentEditor.minigame).wrongAnswers[0][1], "2");
        Assert.AreEqual(((MultipleChoice)contentEditor.minigame).wrongAnswers[0][2], "3");
    }

    [Test]
    public void CSVToMinigame_FormatIsWrong_NotAllDataIsRecovered()
    {
        // Arrange
        var contentEditor = new ContentEditor();
        contentEditor.minigame = new MultipleChoice(
            Subject.Mathematics,
            "Test");

        MethodInfo method = typeof(ContentEditor).GetMethod("CSVToMinigame", BindingFlags.NonPublic | BindingFlags.Instance);

        List<string[]> csv = new List<string[]>() {
            new string[] { "Moeilijkheid (1-3)", "Vraag", "Juist antwoord", "Fout antwoord", "Fout antwoord", "Fout antwoord" },
            new string[] { "3", "What is 2x2?", "4", "1", "2", "3", "Too many arguments", "Should not be included"},
            new string[] { "", "", "A", "B", "C"}
        };

        // Act
        method.Invoke(contentEditor, new object[] { csv });
        contentEditor.minigame.SetData();

        // Assert
        Assert.AreEqual(contentEditor.minigame.data, "Moeilijkheid (1-3);Vraag;Juist antwoord;Fout antwoord;Fout antwoord;Fout antwoord\n3;What is 2x2?;4;1;2;3\n");
        Assert.AreEqual(((MultipleChoice)contentEditor.minigame).questions[0], "What is 2x2?");
        Assert.AreEqual(((MultipleChoice)contentEditor.minigame).difficulties[0], 3);
        Assert.AreEqual(((MultipleChoice)contentEditor.minigame).correctAnswers[0], "4");
        Assert.AreEqual(((MultipleChoice)contentEditor.minigame).wrongAnswers[0][0], "1");
        Assert.AreEqual(((MultipleChoice)contentEditor.minigame).wrongAnswers[0][1], "2");
        Assert.AreEqual(((MultipleChoice)contentEditor.minigame).wrongAnswers[0][2], "3");
    }
}

}
