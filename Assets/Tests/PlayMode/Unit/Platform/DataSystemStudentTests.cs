using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;
using Misc;
using System.Threading;
using System;
using UnityEngine.TestTools;
using System.Threading.Tasks;
using System.Collections;

using Platform.Interfacing.Data;
using Platform.Interfacing.Systems;

namespace PlatformTests
{

public class DataSystemStudentTests
{
    static GameObject d;
    static GameObject n;

    public static DataSystem ds;
    public static NetworkSystem ns;

    public static Teacher teacher;
    public static Classroom classroom;

    [UnitySetUp]
    public IEnumerator SetUp()
    {
        d = new GameObject();
        n = new GameObject();

        ns = n.AddComponent<NetworkSystem>();
        ds = d.AddComponent<DataSystem>();

        ds.RegisterTeacher("Student Test", "studenttest@test.com", (t) => { teacher = t;  });
        yield return new WaitForSeconds(5);

        ds.CreateClassroom(teacher, "Student TEST", (c) => { classroom = c; });

        yield return new WaitForSeconds(5);

        Debug.Log(classroom.name);
        Debug.Log(teacher.name);
    }

    [UnityTearDown]
    public IEnumerator TearDown()
    {
        ds.DeleteClassroom(classroom.id);
        yield return new WaitForSeconds(5);

        ds.DeleteTeacher(teacher.id);
        yield return new WaitForSeconds(5);

        GameObject.Destroy(d);
        GameObject.Destroy(n);
    }
}

[TestFixture]
public class CreatingStudentTests : DataSystemStudentTests
{
    [UnityTest]
    public IEnumerator CreateStudent_ReturnsCorrectStudent()
    {

        // Set up test inputs   
        string name = "DataSystemStudentTest";
        string email = "DataSystemStudentTest@test.com";

        // Set up a flag to track whether the callback has been invoked
        bool callbackInvoked = false;

        bool succes = false;
        bool fail = false;

        // Perform the test by calling the RegisterTeacher method
        DataSystem.Instance.RegisterStudent(classroom, name, email, (student) =>
        {
            // Perform assertiosn within the callback
            Assert.IsNotNull(student);
            Assert.AreEqual(name, name);
            Assert.AreEqual(email, email);

            // Set the flag to indicate that the callback has been invoked
            callbackInvoked = true;
            succes = true;

            DataSystem.Instance.DeleteStudent(student.id);
        }, (error) =>
        {
            callbackInvoked = true;
            fail = true;
        });

        // Wait for the callback to complete
        while (!callbackInvoked)
        {
            yield return null;
        }


        // Assert that the callback has been invoked
        Assert.IsTrue(callbackInvoked);
        Assert.IsTrue(succes);
        Assert.IsFalse(fail);
    }

    [UnityTest]
    public IEnumerator TestRegisterStudent_WithEmptyName_FailsCallback()
    {
        // Set up test inputs
        string emptyName = string.Empty;
        string email = "john.doe@example.com";

        // Set up a flag to track whether the callback has been invoked
        bool callbackInvoked = false;
        bool success = false;
        bool failure = false;

        // Perform the test by calling the RegisterTeacher method with an empty name
        DataSystem.Instance.RegisterStudent(DataSystemStudentTests.classroom, emptyName, email, (student) =>
        {
            // The callback should not be invoked in this case
            callbackInvoked = true;
            success = true;
        }, (error) =>
        {
            // The failure callback should be invoked
            callbackInvoked = true;
            failure = true;
        });

        // Wait for the callback to complete
        while (!callbackInvoked)
        {
            yield return null;
        }

        // Assert that the callback has been invoked
        Assert.IsTrue(callbackInvoked);

        // Assert that the success flag is false and the failure flag is true
        Assert.IsFalse(success);
        Assert.IsTrue(failure);
    }

    [UnityTest]
    public IEnumerator TestRegisterStudent_WithEmptyEmail_FailsCallback()
    {
        // Set up test inputs
        string name = "John Doe";
        string emptyEmail = string.Empty;

        // Set up a flag to track whether the callback has been invoked
        bool callbackInvoked = false;
        bool success = false;
        bool failure = false;

        // Perform the test by calling the RegisterTeacher method with an empty email
        DataSystem.Instance.RegisterStudent(DataSystemStudentTests.classroom, name, emptyEmail, (student) =>
        {
            // The callback should not be invoked in this case
            callbackInvoked = true;
            success = true;
        }, (error) =>
        {
            // The failure callback should be invoked
            callbackInvoked = true;
            failure = true;
        });;

        // Wait for the callback to complete
        while (!callbackInvoked)
        {
            yield return null;
        }

        // Assert that the callback has been invoked
        Assert.IsTrue(callbackInvoked);

        // Assert that the success flag is false and the failure flag is true
        Assert.IsFalse(success);
        Assert.IsTrue(failure);
    }


    [UnityTest]
    public IEnumerator TestRegisterStudent_WithInvalidClassroom_FailsCallback()
    {
        // Set up test inputs
        string name = "John Doe";
        string emptyEmail = string.Empty;

        // Set up a flag to track whether the callback has been invoked
        bool callbackInvoked = false;
        bool success = false;
        bool failure = false;

        Classroom invalidClassroom = new Classroom();

        // Perform the test by calling the RegisterTeacher method with an empty email
        DataSystem.Instance.RegisterStudent(invalidClassroom, name, emptyEmail, (student) =>
        {
            // The callback should not be invoked in this case
            callbackInvoked = true;
            success = true;
        }, (error) =>
        {
            // The failure callback should be invoked
            callbackInvoked = true;
            failure = true;
        });

        // Wait for the callback to complete
        while (!callbackInvoked)
        {
            yield return null;
        }

        // Assert that the callback has been invoked
        Assert.IsTrue(callbackInvoked);

        // Assert that the success flag is false and the failure flag is true
        Assert.IsFalse(success);
        Assert.IsTrue(failure);
    }

}

[TestFixture]
public class StudentLoginTests : DataSystemStudentTests
{
    public Student student;

    [UnitySetUp]
    public IEnumerator Initialize()
    {
        DataSystem.Instance.RegisterStudent(classroom, "Student Login Test", "student.login@test.com", (s) => { student = s; });
        yield return new WaitForSeconds(5);
    }

    [UnityTearDown]
    public IEnumerator Breakdown()
    {
        DataSystem.Instance.DeleteStudent(student.id);
        yield return new WaitForSeconds(5);
    }


    [UnityTest]
    public IEnumerator TestLoginStudent_WithValidCredentials_CallsSuccessCallback()
    {
        // Set up a flag to track whether the callbacks have been invoked
        bool successCallbackInvoked = false;
        bool failureCallbackInvoked = false;

        // Perform the test by calling the LoginTeacher method with a valid email
        DataSystem.Instance.LoginStudent(student.email, DataSystemStudentTests.classroom.id, (s) =>
        {
            // The success callback should be invoked
            successCallbackInvoked = true;

            // Perform assertions on the teacher object if needed
            Assert.IsNotNull(s);
            Assert.AreEqual(student.email, s.email);
            Assert.AreEqual(student.name, s.name);
            Assert.AreEqual(student.id, s.id);
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
    }

    [UnityTest]
    public IEnumerator TestLoginStudent_WithInvalidEmail_CallsFailureCallback()
    {
        // Set up test inputs
        string invalidEmail = "invalid.email@example.com";

        // Set up a flag to track whether the callbacks have been invoked
        bool successCallbackInvoked = false;
        bool failureCallbackInvoked = false;

        // Perform the test by calling the LoginTeacher method with an invalid email
        DataSystem.Instance.LoginStudent(invalidEmail, DataSystemStudentTests.classroom.id, (Student) =>
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

    [UnityTest]
    public IEnumerator TestLoginStudent_WithInvalidClassroomId_CallsFailureCallback()
    {

        // Set up a flag to track whether the callbacks have been invoked
        bool successCallbackInvoked = false;
        bool failureCallbackInvoked = false;

        // Perform the test by calling the LoginTeacher method with an invalid email
        DataSystem.Instance.LoginStudent(student.email, -4, (Student) =>
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
