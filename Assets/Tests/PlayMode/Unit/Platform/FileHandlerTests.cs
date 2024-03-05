using System.Collections.Generic;
using NUnit.Framework;
using System.IO;
using System.Reflection;
using Newtonsoft.Json;
using System;
using GameData;
using Misc;

namespace PlatformTests
{

/// <summary>
/// This class (unit) tests the functionality of the FileHandler
/// </summary>
public class FileHandlerTests
{
    private const string testFilename = "testFile.json";
    /// <summary>
    /// Get a sample object to write to disk
    /// </summary>
    private MultipleChoice GetMPC()
    {
        Subject subject = Subject.Mathematics;
        string tag = "Multiplication 0..10";
        List<int> difficulties = new List<int>() { 2, 3 };
        List<string> questions = new List<string>() { "2x2?", "6/2?" };
        List<string> correctAnswers = new List<string>() { "4", "3" };
        List<List<string>> wrongAnswers = new List<List<string>>
        {
            {new List<string>(){"1", "2", "3"}},
            {new List<string>(){"1", "2", "4"}}
        };
        return new MultipleChoice(subject, tag, difficulties, questions, correctAnswers, wrongAnswers);
    }

    /// <summary>
    /// Helper function to use GetPath, even though it's a private method
    /// </summary>
    private static string GetPath(string filename)
    {
        return (string)typeof(FileHandler).GetMethod("GetPath", BindingFlags.NonPublic | BindingFlags.Static).Invoke(null, new object[] { filename });
    }

    /// <summary>
    /// Helper function to use ReadFile, even though it's a private method
    /// </summary>
    private static string ReadFile(string path)
    {
        return (string)typeof(FileHandler).GetMethod("ReadFile", BindingFlags.NonPublic | BindingFlags.Static).Invoke(null, new object[] { path });
    }

    [Test]
    public void SaveToJSONFile_SavesObjectToFile()
    {
        // Arrange
        MultipleChoice mpc = GetMPC();

        // Act
        FileHandler.SaveToJSONFile(mpc, testFilename);

        // Assert
        string path = GetPath(testFilename);
        Assert.IsTrue(File.Exists(path));
    }

    [Test]
    public void SaveToJSONFile_WritesCorrectJSONDataToFile()
    {
        // Arrange
        MultipleChoice mpc = GetMPC();

        // Act
        FileHandler.SaveToJSONFile(mpc, testFilename);

        // Assert
        string content = ReadFile(GetPath(testFilename));
        MultipleChoice deserializedObject = JsonConvert.DeserializeObject<MultipleChoice>(content);
        Assert.AreEqual(mpc.questions, deserializedObject.questions);
        Assert.AreEqual(mpc.wrongAnswers, deserializedObject.wrongAnswers);
    }

    [Test]
    public void SaveToJSONFile_OverwritesExistingFile()
    {
        // Arrange
        MultipleChoice mpc1 = GetMPC();
        MultipleChoice mpc2 = GetMPC();
        mpc2.questions[0] = "TEST?";

        FileHandler.SaveToJSONFile(mpc1, testFilename);

        // Act
        FileHandler.SaveToJSONFile(mpc2, testFilename);

        // Assert
        string content = ReadFile(GetPath(testFilename));
        MultipleChoice savedObject = JsonConvert.DeserializeObject<MultipleChoice>(content);
        Assert.AreEqual("TEST?", savedObject.questions[0]);
    }

    [Test]
    public void SaveToJSONFile_ThrowsExceptionWhenFilenameIsNull()
    {
        // Arrange
        MultipleChoice mpc = GetMPC();
        string filename = null;

        // Act & Assert
        Assert.Throws<ArgumentNullException>(() => FileHandler.SaveToJSONFile(mpc, filename));
    }

    [Test]
    public void SaveToJSONFile_ThrowsExceptionWhenObjectIsNull()
    {
        // Arrange
        MultipleChoice mpc = null;

        // Act & Assert
        Assert.Throws<ArgumentNullException>(() => FileHandler.SaveToJSONFile(mpc, testFilename));
    }

    [Test]
    public void ReadFromJSONFile_WithValidFile_ReturnsDeserializedObject()
    {
        // Arrange
        MultipleChoice expected = GetMPC();
        FileHandler.SaveToJSONFile(expected, testFilename);

        // Act
        MultipleChoice actual = FileHandler.ReadFromJSONFile<MultipleChoice>(testFilename);

        // Assert
        Assert.AreEqual(expected.questions, actual.questions);
        Assert.AreEqual(expected.wrongAnswers, actual.wrongAnswers);
    }

    [Test]
    public void ReadFromJSONFile_WithInvalidFile_ThrowsArgumentException()
    {
        // Arrange
        FileHandler.SaveToJSONFile(GetMPC(), testFilename);
        File.Delete(GetPath(testFilename));

        // Act & Assert
        Assert.Throws<ArgumentException>(() => FileHandler.ReadFromJSONFile<MultipleChoice>(testFilename));
    }

    [Test]
    public void SaveToJSONString_NullObject_ThrowsArgumentNullException()
    {
        // Arrange
        object obj = null;

        // Act + Assert
        Assert.Throws<ArgumentNullException>(() => FileHandler.SaveToJSONString(obj));
    }

    [Test]
    public void SaveToJSONString_ValidObject_ReturnsNonNullString()
    {
        // Arrange
        MultipleChoice mpc = GetMPC();

        // Act
        string jsonString = FileHandler.SaveToJSONString(mpc);

        // Assert
        Assert.IsNotNull(jsonString);
    }

    [Test]
    public void SaveToJSONString_WithDifferentTypes_ReturnsValidJSON()
    {
        // Arrange
        MultipleChoice mpc = GetMPC();

        // Act
        string result = FileHandler.SaveToJSONString(mpc);

        // Assert
        Assert.AreEqual(FileHandler.ReadFromJSONString<MultipleChoice>(result).questions, mpc.questions);
        Assert.AreEqual(FileHandler.ReadFromJSONString<MultipleChoice>(result).wrongAnswers, mpc.wrongAnswers);
    }

    [Test]
    public void ReadFromJSONString_ValidContent_ReturnsObject()
    {
        // Arrange
        MultipleChoice mpc = GetMPC();
        string content = FileHandler.SaveToJSONString<MultipleChoice>(mpc);

        // Act
        MultipleChoice obj = FileHandler.ReadFromJSONString<MultipleChoice>(content);

        // Assert
        Assert.IsNotNull(obj);
        Assert.AreEqual(mpc.questions, obj.questions);
        Assert.AreEqual(mpc.wrongAnswers, obj.wrongAnswers);
    }

    [Test]
    public void ReadFromJSONString_MissingProperty_ReturnsObjectWithDefaultValues()
    {
        // Arrange
        MultipleChoice mpc = GetMPC();
        mpc.questions = null;
        string content = FileHandler.SaveToJSONString<MultipleChoice>(mpc);

        // Act
        MultipleChoice obj = FileHandler.ReadFromJSONString<MultipleChoice>(content);

        // Assert
        Assert.IsNotNull(obj);
        Assert.AreEqual(new List<string>(), obj.questions);
        Assert.AreEqual(mpc.wrongAnswers, obj.wrongAnswers);
    }

    [Test]
    public void ReadFromJSONString_NullContent_ThrowsArgumentException()
    {
        // Arrange
        string content = null;

        // Act & Assert
        Assert.Throws<ArgumentException>(() => FileHandler.ReadFromJSONString<MultipleChoice>(content));
    }

    [Test]
    public void ReadFromJSONString_EmptyContent_ThrowsArgumentException()
    {
        // Arrange
        string content = "";

        // Act & Assert
        Assert.Throws<ArgumentException>(() => FileHandler.ReadFromJSONString<MultipleChoice>(content));
    }

    /// <summary>
    /// Helper class to easily make invalid content
    /// </summary>
    public class Person
    {
        public string Name { get; set; }
        public int Age { get; set; }
    }

    [Test]
    public void ReadFromJSONString_InvalidContent_ThrowsJsonReaderException()
    {
        // Arrange
        string content = "{\"name\":\"John\",\"age\":}";

        // Act & Assert
        Assert.Throws<JsonReaderException>(() => FileHandler.ReadFromJSONString<Person>(content));
    }
}

}
