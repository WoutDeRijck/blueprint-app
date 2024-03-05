using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;
using Misc;
using System.Threading;
using System;
using UnityEngine.TestTools;
using System.Threading.Tasks;
using System.Collections;

using Platform.Interfacing.Systems;

namespace PlatformTests
{

public class DataSystemTeacherTests
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
public class CreatingTeacherTests : DataSystemTeacherTests
{
    [UnityTest]
    public IEnumerator CreateTeacher_ReturnsCorrectTeacher()
    {

        // Set up test inputs   
        string name = "DataSystemTeacherTest";
        string email = "DataSystemTeacherTest@test.com";

        // Set up a flag to track whether the callback has been invoked
        bool callbackInvoked = false;

        bool succes = false;
        bool fail = false;

        // Perform the test by calling the RegisterTeacher method
        DataSystemTeacherTests.ds.RegisterTeacher(name, email, (teacher) =>
        {
            // Perform assertiosn within the callback
            Assert.IsNotNull(teacher);
            Assert.AreEqual(name, teacher.name);

            // Set the flag to indicate that the callback has been invoked
            callbackInvoked = true;
            succes = true;

            DataSystemTeacherTests.ds.DeleteTeacher(teacher.id);
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
    public IEnumerator TestRegisterTeacher_WithEmptyName_FailsCallback()
    {
        // Set up test inputs
        string emptyName = string.Empty;
        string email = "john.doe@example.com";

        // Set up a flag to track whether the callback has been invoked
        bool callbackInvoked = false;
        bool success = false;
        bool failure = false;

        // Perform the test by calling the RegisterTeacher method with an empty name
        DataSystemTeacherTests.ds.RegisterTeacher(emptyName, email, (teacher) =>
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
    public IEnumerator TestRegisterTeacher_WithEmptyEmail_FailsCallback()
    {
        // Set up test inputs
        string name = "John Doe";
        string emptyEmail = string.Empty;

        // Set up a flag to track whether the callback has been invoked
        bool callbackInvoked = false;
        bool success = false;
        bool failure = false;

        // Perform the test by calling the RegisterTeacher method with an empty email
        DataSystemTeacherTests.ds.RegisterTeacher(name, emptyEmail, (teacher) =>
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
public class TeacherLoginTests : DataSystemTeacherTests
{
    int teacherId = -1;
    string validName = "Valid Teacher";
    string validEmail = "valid.teacher@test.com";

    [UnitySetUp]
    public IEnumerator Initialization()
    {
        DataSystemTeacherTests.ds.RegisterTeacher("Valid Teacher", "valid.teacher@test.com", (teacher) => teacherId = teacher.id );

        yield return new WaitForSeconds(5);
    }

    [UnityTearDown]
    public IEnumerator Breakdown()
    {
        DataSystemTeacherTests.ds.DeleteTeacher(teacherId);
        yield return new WaitForSeconds(5);
    }


    [UnityTest]
    public IEnumerator TestLoginTeacher_WithValidEmail_CallsSuccessCallback()
    {
        // Set up a flag to track whether the callbacks have been invoked
        bool successCallbackInvoked = false;
        bool failureCallbackInvoked = false;

        // Perform the test by calling the LoginTeacher method with a valid email
        ds.LoginTeacher(validEmail, (teacher) =>
        {
            // The success callback should be invoked
            successCallbackInvoked = true;

            // Perform assertions on the teacher object if needed
            Assert.IsNotNull(teacher);
            Assert.AreEqual(teacher.email, validEmail);
            Assert.AreEqual(teacher.name, validName);
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
    public IEnumerator TestLoginTeacher_WithInvalidEmail_CallsFailureCallback()
    {
        // Set up test inputs
        string invalidEmail = "invalid.email@example.com";

        // Set up a flag to track whether the callbacks have been invoked
        bool successCallbackInvoked = false;
        bool failureCallbackInvoked = false;

        // Perform the test by calling the LoginTeacher method with an invalid email
        ds.LoginTeacher(invalidEmail, (teacher) =>
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
