using NUnit.Framework;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.TestTools;
using GameData;

using Platform.Interfacing.Data;
using Platform.Interfacing.Systems;

namespace PlatformTests
{

public class DataSystemMinigameTests
{
    static GameObject d;
    static GameObject n;

    public static Teacher teacher;

    public static DataSystem ds;
    public static NetworkSystem ns;

    [UnitySetUp]
    public IEnumerator SetUp()
    {
        d = new GameObject();
        n = new GameObject();

        ns = n.AddComponent<NetworkSystem>();
        ds = d.AddComponent<DataSystem>();

        DataSystem.Instance.RegisterTeacher("MG", "mg@test.com", (t) => { teacher = t; });

        yield return new WaitForSeconds(5);
    }

    [UnityTearDown]
    public IEnumerator TearDown()
    {
        GameObject.Destroy(d);
        GameObject.Destroy(n);

        DataSystem.Instance.DeleteTeacher(teacher.id);

        yield return new WaitForSeconds(5);
    }
}

    [TestFixture]
    public class CreateMinigameTests : DataSystemMinigameTests
    {
        Minigame minigame;

        Subtype subtype = Subtype.Ranking;
        Subject subject = Subject.French;
        string tag = "ABCDEF";
        string data = "test;test;test;test";
        bool shared = false;

        [UnityTest]
        public IEnumerator TestCreateClassroom_SuccessfulCreation()
        {
            // Set up a flag to track whether the callbacks have been invoked
            bool successCallbackInvoked = false;
            bool failureCallbackInvoked = false;

            // Perform the test by calling the CreateClassroom method
            DataSystem.Instance.CreateMinigame(DataSystemMinigameTests.teacher, subtype, subject, tag, data, shared, (minigame) =>
            {

                // Perform assertions on the classroom object if needed
                Assert.IsNotNull(minigame);
                Assert.AreEqual(minigame.subtype, subtype);
                Assert.AreEqual(minigame.subject, subject);
                Assert.AreEqual(minigame.tag, tag);
                Assert.AreEqual(minigame.data, data);
                Assert.AreEqual(minigame.shared, shared);
                Assert.NotNull(minigame.id);


                DataSystem.Instance.DeleteMinigame(minigame.id, () => { successCallbackInvoked = true; });
            }, (error) =>
            {
                // The failure callback should not be invoked in this case
                failureCallbackInvoked = true;
            });

            // Wait until the callbacks have been invoked
            yield return new WaitUntil(() => successCallbackInvoked || failureCallbackInvoked);

            // Assert that the success callback has been invoked and the failure callback has not been invoked
            Assert.IsTrue(successCallbackInvoked);
            Assert.IsFalse(failureCallbackInvoked);

            yield return new WaitForSeconds(5);
        }

        [UnityTest]
        public IEnumerator TestCreateClassroom_InvalidTeacher_FailureCallback()
        {

            // Set up a flag to track whether the callbacks have been invoked
            bool successCallbackInvoked = false;
            bool failureCallbackInvoked = false;

            Teacher t = new Teacher();
            t.id = -4;
            // Perform the test by calling the CreateClassroom method with an empty name
            DataSystem.Instance.CreateMinigame(t, subtype, subject, tag, data, shared, (minigame) =>
            {
                // The success callback should not be invoked in this case
                successCallbackInvoked = true;
            }, (error) =>
            {
                // The failure callback should be invoked
                failureCallbackInvoked = true;
            });

            // Wait until the callbacks have been invoked
            yield return new WaitUntil(() => successCallbackInvoked || failureCallbackInvoked);

            // Assert that the failure callback has been invoked and the success callback has not been invoked
            Assert.IsTrue(failureCallbackInvoked);
            Assert.IsFalse(successCallbackInvoked);
        }
    }

[TestFixture]
public class EditMinigameTests : DataSystemMinigameTests
{
    Minigame minigame;

    Subtype subtype = Subtype.Ranking;
    Subject subject = Subject.French;
    string tag = "ABCDEF";
    string data = "test;test;test;test";

    [UnitySetUp]
    public IEnumerator Initialization()
    {
        DataSystem.Instance.CreateMinigame(DataSystemMinigameTests.teacher, subtype, subject, tag, data, true, (m) => { minigame = m; });

        yield return new WaitForSeconds(5);
    }

    [UnityTearDown]
    public IEnumerator BreakDown()
    {
        DataSystem.Instance.DeleteMinigame(minigame.id, () => { });
        yield return new WaitForSeconds(5);
    }

    [UnityTest]
    public IEnumerator TestEditMinigame_SuccessfulEdit()
    {
        // Set up a flag to track whether the callbacks have been invoked
        bool successCallbackInvoked = false;
        bool failureCallbackInvoked = false;

        string newData = "A;B;C;D;";
        // Perform the test by calling the CreateClassroom method
        DataSystem.Instance.EditMinigame(minigame.id, newData, (returnData) =>
        {
            Assert.AreEqual(newData, returnData);
            DataSystem.Instance.DeleteMinigame(minigame.id, () => { successCallbackInvoked = true; });
        }, (error) =>
        {
            // The failure callback should not be invoked in this case
            failureCallbackInvoked = true;
        });

        // Wait until the callbacks have been invoked
        yield return new WaitUntil(() => successCallbackInvoked || failureCallbackInvoked);

        // Assert that the success callback has been invoked and the failure callback has not been invoked
        Assert.IsTrue(successCallbackInvoked);
        Assert.IsFalse(failureCallbackInvoked);

        yield return new WaitForSeconds(5);
    }

    [UnityTest]
    public IEnumerator TestEditMinigame_InvalidTeacher_FailureCallback()
    {

        // Set up a flag to track whether the callbacks have been invoked
        bool successCallbackInvoked = false;
        bool failureCallbackInvoked = false;

        string newData = "A;B;C;D;";
        // Perform the test by calling the CreateClassroom method
        DataSystem.Instance.EditMinigame(-4, newData, (returnData) =>
        {
            successCallbackInvoked = true;
        }, (error) =>
        {
            // The failure callback should not be invoked in this case
            failureCallbackInvoked = true;
        });

        // Wait until the callbacks have been invoked
        yield return new WaitUntil(() => successCallbackInvoked || failureCallbackInvoked);

        // Assert that the success callback has been invoked and the failure callback has not been invoked
        Assert.IsFalse(successCallbackInvoked);
        Assert.IsTrue(failureCallbackInvoked);

        yield return new WaitForSeconds(5);
    }
}

[TestFixture]
public class FetchMinigameTests : DataSystemMinigameTests
{
    Minigame minigame;

    Subtype subtype = Subtype.Ranking;
    Subject subject = Subject.French;
    string tag = "ABCDEF";
    string data = "test;test;test;test";

    [UnitySetUp]
    public IEnumerator Initialization()
    {
        DataSystem.Instance.CreateMinigame(DataSystemMinigameTests.teacher, subtype, subject, tag, data, true, (m) => { minigame = m; });

        yield return new WaitForSeconds(5);
    }

    [UnityTearDown]
    public IEnumerator BreakDown()
    {
        DataSystem.Instance.DeleteMinigame(minigame.id, () => { });
        yield return new WaitForSeconds(5);
    }

    [UnityTest]
    public IEnumerator TestFetchMinigames_SuccessfulFetch()
    {
        // Set up a flag to track whether the callbacks have been invoked
        bool successCallbackInvoked = false;
        bool failureCallbackInvoked = false;

        // Perform the test by calling the CreateClassroom method
        DataSystem.Instance.GetMinigamesByTeacher(DataSystemMinigameTests.teacher, (minigames) =>
        {
            Assert.AreEqual(minigames[0].id, minigame.id);
            successCallbackInvoked = true;
        }, (error) =>
        {
            // The failure callback should not be invoked in this case
            failureCallbackInvoked = true;
        });

        // Wait until the callbacks have been invoked
        yield return new WaitUntil(() => successCallbackInvoked || failureCallbackInvoked);

        // Assert that the success callback has been invoked and the failure callback has not been invoked
        Assert.IsTrue(successCallbackInvoked);
        Assert.IsFalse(failureCallbackInvoked);

        yield return new WaitForSeconds(5);
    }

    [UnityTest]
    public IEnumerator TestFetchMinigames_InvalidTeacher_FailureCallback()
    {

        // Set up a flag to track whether the callbacks have been invoked
        bool successCallbackInvoked = false;
        bool failureCallbackInvoked = false;


        Teacher t = new Teacher();
        t.id = -4;

        // Perform the test by calling the CreateClassroom method
        DataSystem.Instance.GetMinigamesByTeacher(t, (minigames) =>
        { 
            successCallbackInvoked = true;
        }, (error) =>
        {
            // The failure callback should not be invoked in this case
            failureCallbackInvoked = true;
        });

        // Wait until the callbacks have been invoked
        yield return new WaitUntil(() => successCallbackInvoked || failureCallbackInvoked);

        // Assert that the success callback has been invoked and the failure callback has not been invoked
        Assert.IsFalse(successCallbackInvoked);
        Assert.IsTrue(failureCallbackInvoked);

        yield return new WaitForSeconds(5);
    }
}

//[TestFixture]
//public class FetchClassroomTests : DataSystemClassroomTests
//{
//    Teacher teacher;
//    Classroom classroom;
//    Student student;

//    Teacher teacher2;

//    [UnitySetUp]
//    public IEnumerator Initialize()
//    {
//        DataSystem.Instance.RegisterTeacher("CR", "cr@test.com", (t) => { teacher = t;  });
//        DataSystem.Instance.RegisterTeacher("CR2", "cr2@test.com", (t) => { teacher2 = t; });
//        yield return new WaitForSeconds(5);

//        DataSystem.Instance.CreateClassroom(teacher, "CR FETCH TEST", (c) => classroom = c);
//        yield return new WaitForSeconds(5);

//        DataSystem.Instance.RegisterStudent(classroom, "Student CR", "student.cr@test.com", (s) => { student = s; });
//        yield return new WaitForSeconds(5);
//    }

//    [UnityTearDown]
//    public IEnumerator Breakdown()
//    {
//        DataSystem.Instance.DeleteStudent(student.id);
//        yield return new WaitForSeconds(5);

//        DataSystem.Instance.DeleteClassroom(classroom.id);
//        yield return new WaitForSeconds(5);

//        DataSystem.Instance.DeleteTeacher(teacher.id);
//        DataSystem.Instance.DeleteTeacher(teacher2.id);
//        yield return new WaitForSeconds(5);
//    }

//    [UnityTest]
//    public IEnumerator TestFetchClassroom_Succes()
//    {
//        // Set up a flag to track whether the callbacks have been invoked
//        bool successCallbackInvoked = false;
//        bool failureCallbackInvoked = false;

//        // Perform the test by calling the CreateClassroom method
//        DataSystem.Instance.GetClassroomsByTeacher(teacher, (classroomList) =>
//        {
//            // The success callback should be invoked
//            successCallbackInvoked = true;

//            Assert.AreEqual(1, classroomList.classrooms.Count);
//            Assert.AreEqual(classroom.id, classroomList.classrooms[0].id);
//            Assert.AreEqual(student.id, classroomList.classrooms[0].students[0].id);

//        }, (error) =>
//        {
//            // The failure callback should not be invoked in this case
//            failureCallbackInvoked = true;
//        });

//        // Wait until the callbacks have been invoked
//        yield return new WaitUntil(() => successCallbackInvoked || failureCallbackInvoked);

//        // Assert that the success callback has been invoked and the failure callback has not been invoked
//        Assert.IsTrue(successCallbackInvoked);
//        Assert.IsFalse(failureCallbackInvoked);

//        yield return new WaitForSeconds(5);
//    }

//    [UnityTest]
//    public IEnumerator TestFetchClassroom_WrongTeacher_FailureCallback()
//    {
//        // Set up test inputs
//        string emptyName = string.Empty;

//        // Set up a flag to track whether the callbacks have been invoked
//        bool successCallbackInvoked = false;
//        bool failureCallbackInvoked = false;

//        // Perform the test by calling the CreateClassroom method with an empty name
//        DataSystem.Instance.GetClassroomsByTeacher(teacher2, (classroomList) =>
//        {
//            // The success callback should not be invoked in this case
//            successCallbackInvoked = true;
//        }, (error) =>
//        {
//            // The failure callback should be invoked
//            failureCallbackInvoked = true;
//        });

//        // Wait until the callbacks have been invoked
//        yield return new WaitUntil(() => successCallbackInvoked || failureCallbackInvoked);

//        // Assert that the failure callback has been invoked and the success callback has not been invoked
//        Assert.IsTrue(failureCallbackInvoked);
//        Assert.IsFalse(successCallbackInvoked);
//    }
//}

}
