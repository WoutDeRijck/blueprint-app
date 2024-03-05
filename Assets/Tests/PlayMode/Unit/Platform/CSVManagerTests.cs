using System.Collections.Generic;
using NUnit.Framework;
using System.IO;
using UnityEngine;
using Misc;

namespace PlatformTests
{

/// <summary>
/// This class (unit) tests the functionality of the CSVManager
/// </summary>
public class CSVManagerTests
{
    private const string TEST_FILENAME = "test.csv";

    [SetUp]
    public void SetUp()
    {
        // Create test CSV file
        List<string[]> testData = new List<string[]>()
        {
            new string[] {"1", "John", "Doe"},
            new string[] {"2", "Jane", "Doe"},
            new string[] {"3", "", "", "TEST"}
        };
        CSVManager.WriteCSV(TEST_FILENAME, testData);
    }

    [TearDown]
    public void TearDown()
    {
        // Delete test CSV file
        CSVManager.DeleteFile(TEST_FILENAME);
    }

    [Test]
    public void ReadCSV_ReturnsCorrectData()
    {
        // Arrange
        List<string[]> expectedData = new List<string[]>()
        {
            new string[] {"1", "John", "Doe"},
            new string[] {"2", "Jane", "Doe"},
            new string[] {"3", "", "", "TEST"}
        };

        // Act
        List<string[]> actualData = CSVManager.ReadCSV(TEST_FILENAME);

        // Assert
        Assert.AreEqual(expectedData.Count, actualData.Count);
        for (int i = 0; i < expectedData.Count; i++)
        {
            CollectionAssert.AreEqual(expectedData[i], actualData[i]);
        }
    }

    [Test]
    public void ReadCSV_ThrowsExceptionForInvalidFilename()
    {
        // Arrange
        string invalidFilename = "nonexistent.csv";

        // Act and Assert
        Assert.Throws<FileNotFoundException>(() => CSVManager.ReadCSV(invalidFilename));
    }

    [Test]
    public void ReadCSV_ReturnsEmptyListForEmptyCsv()
    {
        // Arrange
        const string filename = "empty.csv";
        File.WriteAllText(GetPath(filename), "");

        // Act
        List<string[]> actualData = CSVManager.ReadCSV(filename);

        // Assert
        Assert.IsEmpty(actualData);
    }

    [Test]
    public void ReadCSVToString_ThrowsExceptionForInvalidFilename()
    {
        // Arrange
        string nonExistentFile = "non_existent.csv";

        // Act and Assert
        Assert.Throws<FileNotFoundException>(() => CSVManager.ReadCSVToString(nonExistentFile));
    }

    [Test]
    public void ReadCSVToString_ReturnsCorrectString()
    {
        // Arrange
        string expectedString = "1;John;Doe\r\n2;Jane;Doe\r\n3;;;TEST\r\n";

        // Act
        string actualString = CSVManager.ReadCSVToString(TEST_FILENAME);

        // Assert
        Assert.AreEqual(expectedString, actualString);
    }

    [Test]
    public void ReadCSVToString_ReturnsEmptyStringWhenFileIsEmpty()
    {
        // Arrange
        string emptyFile = "empty.csv";
        CSVManager.WriteCSV(emptyFile, new List<string[]>());

        // Act
        string actualString = CSVManager.ReadCSVToString(emptyFile);

        // Assert
        Assert.AreEqual("", actualString);

        // Clean up
        CSVManager.DeleteFile(emptyFile);
    }

    [Test]
    public void StringToCSVList_ReturnsEmptyListWhenInputIsEmptyString()
    {
        // Arrange
        string testData = "";
        List<string[]> expectedData = new List<string[]>();

        // Act
        List<string[]> actualData = CSVManager.StringToCSVList(testData);

        // Assert
        Assert.AreEqual(expectedData.Count, actualData.Count);
    }

    [Test]
    public void StringToCSVList_ReturnsEmptyListWhenInputIsOnlyNewLine()
    {
        // Arrange
        string testData = "\n";
        List<string[]> expectedData = new List<string[]>();

        // Act
        List<string[]> actualData = CSVManager.StringToCSVList(testData);

        // Assert
        Assert.AreEqual(expectedData.Count, actualData.Count);
    }

    [Test]
    public void StringToCSVList_ReturnsSingleRowWhenInputHasSingleRow()
    {
        // Arrange
        string testData = "1;John;Doe";
        List<string[]> expectedData = new List<string[]>()
        {
            new string[] {"1", "John", "Doe"},
        };

        // Act
        List<string[]> actualData = CSVManager.StringToCSVList(testData);

        // Assert
        Assert.AreEqual(expectedData.Count, actualData.Count);
        CollectionAssert.AreEqual(expectedData[0], actualData[0]);
    }

    [Test]
    public void StringToCSVList_ReturnsMultipleRowsWhenInputHasMultipleRows()
    {
        // Arrange
        string testData = "1;John;Doe\n2;Jane;Doe\n3;Bob;Smith\n";
        List<string[]> expectedData = new List<string[]>()
        {
            new string[] {"1", "John", "Doe"},
            new string[] {"2", "Jane", "Doe"},
            new string[] {"3", "Bob", "Smith"}
        };

        // Act
        List<string[]> actualData = CSVManager.StringToCSVList(testData);

        // Assert
        Assert.AreEqual(expectedData.Count, actualData.Count);
        for (int i = 0; i < expectedData.Count; i++)
        {
            CollectionAssert.AreEqual(expectedData[i], actualData[i]);
        }
    }

    [Test]
    public void WriteCSV_WritesCorrectData()
    {
        // Arrange
        List<string[]> testData = new List<string[]>()
        {
            new string[] {"1", "John", "Doe"},
            new string[] {"2", "Jane", "Doe"}
        };
        string testFileName = "test.csv";

        // Act
        CSVManager.WriteCSV(testFileName, testData);
        List<string[]> actualData = CSVManager.ReadCSV(testFileName);

        // Assert
        Assert.AreEqual(testData.Count, actualData.Count);
        for (int i = 0; i < testData.Count; i++)
        {
            CollectionAssert.AreEqual(testData[i], actualData[i]);
        }
    }

    [Test]
    public void WriteCSV_CreatesFileIfNotExist()
    {
        // Arrange
        List<string[]> testData = new List<string[]>()
        {
            new string[] {"1", "John", "Doe"},
            new string[] {"2", "Jane", "Doe"}
        };
        string testFileName = "nonexistent.csv";

        // Act
        CSVManager.WriteCSV(testFileName, testData);
        List<string[]> actualData = CSVManager.ReadCSV(testFileName);

        // Assert
        Assert.AreEqual(testData.Count, actualData.Count);
        for (int i = 0; i < testData.Count; i++)
        {
            CollectionAssert.AreEqual(testData[i], actualData[i]);
        }

        // Clean up
        CSVManager.DeleteFile(testFileName);
    }

    [Test]
    public void WriteCSV_OverwritesExistingFile()
    {
        // Arrange
        List<string[]> initialData = new List<string[]>()
        {
            new string[] {"1", "John", "Doe"},
            new string[] {"2", "Jane", "Doe"}
        };
        List<string[]> testData = new List<string[]>()
        {
            new string[] {"3", "Bob", "Smith"},
            new string[] {"4", "Alice", "Jones"}
        };
        string testFileName = "existing.csv";
        CSVManager.WriteCSV(testFileName, initialData);

        // Act
        CSVManager.WriteCSV(testFileName, testData);
        List<string[]> actualData = CSVManager.ReadCSV(testFileName);

        // Assert
        Assert.AreEqual(testData.Count, actualData.Count);
        for (int i = 0; i < testData.Count; i++)
        {
            CollectionAssert.AreEqual(testData[i], actualData[i]);
        }

        // Clean up
        CSVManager.DeleteFile(testFileName);
    }

    [Test]
    public void WriteCSV_WritesEmptyFile()
    {
        // Arrange
        List<string[]> testData = new List<string[]>();
        string testFileName = "empty.csv";

        // Act
        CSVManager.WriteCSV(testFileName, testData);
        List<string[]> actualData = CSVManager.ReadCSV(testFileName);

        // Assert
        Assert.AreEqual(testData.Count, actualData.Count);

        // Clean up
        CSVManager.DeleteFile(testFileName);
    }

    /// <summary>
    /// Creates path from filename, from the persistent data path
    /// </summary>
    /// <returns> path </returns>
    private static string GetPath(string filename)
    {
        return Application.persistentDataPath + "/" + filename;
    }
}

}