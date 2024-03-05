using NUnit.Framework;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.TestTools;

using Platform.Interfacing.Data;
using Platform.Interfacing.Systems;

namespace PlatformTests
{

public class DataSystemClassroomTests
{
    static GameObject d;
    static GameObject n;

    public static DataSystem ds;
    public static NetworkSystem ns;

    [OneTimeSetUp]
    public void SetUp()
    {
        d = new GameObject();
        n = new GameObject();

        ns = n.AddComponent<NetworkSystem>();
        ds = d.AddComponent<DataSystem>();
    }

    [OneTimeTearDown]
    public void TearDown()
    {
        GameObject.Destroy(d);
        GameObject.Destroy(n);
    }
}

[TestFixture]
public class CreateClassroomTests : DataSystemClassroomTests
{
    private Teacher teacherValid;
    private string teacherName = "CR";
    private string teacherEmail = "cr@test.com";
    private string validName = "Math Class";

    [UnitySetUp]
    public IEnumerator Init()
    {
        // Set up the teacher for testing
        DataSystem.Instance.RegisterTeacher(teacherName, teacherEmail, (teacher) => { teacherValid = teacher; });

        yield return new WaitForSeconds(5);
    }

    [UnityTearDown]
    public IEnumerator Breakdown()
    {
        // Clean up the teacher after testing
        DataSystem.Instance.DeleteTeacher(teacherValid.id);

        yield return new WaitForSeconds(5);
    }

    [UnityTest]
    public IEnumerator TestCreateClassroom_SuccessfulCreation()
    {
        // Set up a flag to track whether the callbacks have been invoked
        bool successCallbackInvoked = false;
        bool failureCallbackInvoked = false;

        // Perform the test by calling the CreateClassroom method
        DataSystem.Instance.CreateClassroom(teacherValid, validName, (classroom) =>
        {
            // The success callback should be invoked
            successCallbackInvoked = true;

            // Perform assertions on the classroom object if needed
            Assert.IsNotNull(classroom);
            Assert.AreEqual(classroom.name, validName);

            DataSystem.Instance.DeleteClassroom(classroom.id);
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
    public IEnumerator TestCreateClassroom_EmptyName_FailureCallback()
    {
        // Set up test inputs
        string emptyName = string.Empty;

        // Set up a flag to track whether the callbacks have been invoked
        bool successCallbackInvoked = false;
        bool failureCallbackInvoked = false;

        // Perform the test by calling the CreateClassroom method with an empty name
        DataSystem.Instance.CreateClassroom(teacherValid, emptyName, (classroom) =>
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
public class FetchClassroomTests : DataSystemClassroomTests
{
    Teacher teacher;
    Classroom classroom;
    Student student;

    Teacher teacher2;

    [UnitySetUp]
    public IEnumerator Initialize()
    {
        DataSystem.Instance.RegisterTeacher("CR", "cr@test.com", (t) => { teacher = t;  });
        DataSystem.Instance.RegisterTeacher("CR2", "cr2@test.com", (t) => { teacher2 = t; });
        yield return new WaitForSeconds(5);

        DataSystem.Instance.CreateClassroom(teacher, "CR FETCH TEST", (c) => classroom = c);
        yield return new WaitForSeconds(5);

        DataSystem.Instance.RegisterStudent(classroom, "Student CR", "student.cr@test.com", (s) => { student = s; });
        yield return new WaitForSeconds(5);
    }

    [UnityTearDown]
    public IEnumerator Breakdown()
    {
        DataSystem.Instance.DeleteStudent(student.id);
        yield return new WaitForSeconds(5);

        DataSystem.Instance.DeleteClassroom(classroom.id);
        yield return new WaitForSeconds(5);

        DataSystem.Instance.DeleteTeacher(teacher.id);
        DataSystem.Instance.DeleteTeacher(teacher2.id);
        yield return new WaitForSeconds(5);
    }

    [UnityTest]
    public IEnumerator TestFetchClassroom_Succes()
    {
        // Set up a flag to track whether the callbacks have been invoked
        bool successCallbackInvoked = false;
        bool failureCallbackInvoked = false;

        // Perform the test by calling the CreateClassroom method
        DataSystem.Instance.GetClassroomsByTeacher(teacher, (classroomList) =>
        {
            // The success callback should be invoked
            successCallbackInvoked = true;

            Assert.AreEqual(1, classroomList.classrooms.Count);
            Assert.AreEqual(classroom.id, classroomList.classrooms[0].id);
            Assert.AreEqual(student.id, classroomList.classrooms[0].students[0].id);

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
    public IEnumerator TestFetchClassroom_WrongTeacher_FailureCallback()
    {
        // Set up test inputs
        string emptyName = string.Empty;

        // Set up a flag to track whether the callbacks have been invoked
        bool successCallbackInvoked = false;
        bool failureCallbackInvoked = false;

        // Perform the test by calling the CreateClassroom method with an empty name
        DataSystem.Instance.GetClassroomsByTeacher(teacher2, (classroomList) =>
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

}
