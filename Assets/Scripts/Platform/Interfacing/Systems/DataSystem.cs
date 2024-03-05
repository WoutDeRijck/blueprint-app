using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GameData;
using Misc;
using Unity.Services.Lobbies.Models;
using System.Threading;

using Platform.Interfacing.Data;

namespace Platform.Interfacing.Systems
{

public class DataSystem : PersistentSingleton<DataSystem>
{
    /// <summary>
    /// Register teacher on backend.
    /// </summary>
    /// <param name="name">Name of the teacher to register.</param>
    /// <param name="email">Email of the teacher to register</param>
    /// <param name="successCallback">Callback for when the registration was succesful.</param>
    /// <param name="failCallback">Callback for when the registration was not succesfull, if null the error gets logged.</param>
    public void RegisterTeacher(string name, string email, Action<Teacher> successCallback, Action<string> failCallback = null)
    {
        if (failCallback == null) failCallback = (error) => { Debug.LogError(error); };

        Dictionary<string, string> parameters = new Dictionary<string, string>() { };
        parameters.Add("name", name);
        parameters.Add("email", email);

        StartCoroutine(NetworkSystem.Instance.PostCoroutine("teacher/create", parameters, (error) => failCallback(error), (response) => {
            Teacher teacher = Teacher.FromJson(response);
            successCallback(teacher);
        }
        ));
    }

    /// <summary>
    /// Fetch teacher from backend to login.
    /// </summary>
    /// <param name="email">Email of the teacher to login as.</param>
    /// <param name="successCallback">Called when logging in was succesful.</param>
    /// <param name="failCallback">Called when logging in was not succesful.</param>
    public void LoginTeacher(string email, Action<Teacher> successCallback, Action<string> failCallback = null)
    {
        if (failCallback == null) failCallback = (error) => { Debug.LogError(error); };

        Dictionary<string, string> parameters = new Dictionary<string, string>() { };
        parameters.Add("email", email);

        StartCoroutine(NetworkSystem.Instance.PostCoroutine("teacher/login", parameters, (error) =>
        {
            failCallback(error);
        }, (response) =>
        {
            Teacher teacher = Teacher.FromJson(response);
            successCallback(teacher);
        }
        ));
    }

    /// <summary>
    /// Delete a teacher from the backend. (Only use in testing!)
    /// </summary>
    /// <param name="id"></param>
    public void DeleteTeacher(int id)
    {
        Dictionary<string, string> parameters = new Dictionary<string, string>() { };
        parameters.Add("id", id.ToString());

        StartCoroutine(NetworkSystem.Instance.PostCoroutine("teacher/delete", parameters, (error) => {
            Debug.Log(error);
        }, (response) => {}));
    }

    //public void GetTeacherById(string id, Action<Teacher> successCallback)
    //{
    //    Dictionary<string, string> parameters = new Dictionary<string, string>() { };
    //    parameters.Add("mode", "id");
    //    StartCoroutine(NetworkController.Instance.PostCoroutine("teacher/fetch/id", parameters, (error) => Debug.LogError(error), (response) => {
    //        Teacher teacher = Teacher.FromJson(response);
    //        successCallback(teacher);
    //    }
    //    ));
    //}

    /// <summary>
    /// Register student on backend.
    /// </summary>
    /// <param name="classroom">Classroom to enroll the student in.</param>
    /// <param name="name">Name of the student to register.</param>
    /// <param name="email">Email of the student to register.</param>
    /// <param name="successCallback">Callback for when the registration was succesful.</param>
    /// <param name="failCallback">Callback for when the registration was not succesful, if left default error is logged.</param>
    public void RegisterStudent(Classroom classroom, string name, string email, Action<Student> successCallback, Action<string> failCallback = null)
    {
        if (failCallback == null) failCallback = (error) => { Debug.LogError(error); };

        Dictionary<string, string> parameters = new Dictionary<string, string>() { };
        parameters.Add("name", name);
        parameters.Add("email", email);
        parameters.Add("classroomId", classroom.id.ToString());
        StartCoroutine(NetworkSystem.Instance.PostCoroutine("student/create", parameters, (error) => failCallback(error), (response) => {
            Student student = Student.FromJson(response);
            successCallback(student);
        }
        ));
    }

    //public void GetStudentById(Student student, Action<Student> successCallback)
    //{
    //    Dictionary<string, string> parameters = new Dictionary<string, string>() { };
    //    parameters.Add("id", student.id.ToString());
    //    StartCoroutine(NetworkController.Instance.PostCoroutine("student/fetch/id", parameters, (error) => Debug.LogError(error), (response) => {
    //        Student student = Student.FromJson(response);
    //        successCallback(student);
    //    }
    //    ));
    //}

    /// <summary>
    /// Fetch student from backend to login.
    /// </summary>
    /// <param name="email">Email of the student.</param>
    /// <param name="classroomId">Id of the classroom the student belongs to.</param>
    /// <param name="successCallback">Callback for when logging in is succesful.</param>
    /// <param name="failCallback">Callback for when logging in fails.</param>
    /// /// <param name="failCallback">Callback for when the logging in was not succesful, if left default error is logged.</param>
    public void LoginStudent(string email, int classroomId, Action<Student> successCallback, Action<string> failCallback = null)
    {
        if (failCallback == null) failCallback = (error) => { Debug.LogError(error); };

        Dictionary<string, string> parameters = new Dictionary<string, string>() { };
        parameters.Add("email", email);
        parameters.Add("classroomId", classroomId.ToString());
        StartCoroutine(NetworkSystem.Instance.PostCoroutine("student/login", parameters, (error) => {
            failCallback(error);
        }, (response) => {
            Student student = Student.FromJson(response);
            successCallback(student);
        }
        ));
    }

    public void DeleteStudent(int id)
    {
        Dictionary<string, string> parameters = new Dictionary<string, string>() { };
        parameters.Add("id", id.ToString());

        StartCoroutine(NetworkSystem.Instance.PostCoroutine("student/delete", parameters, (error) => {
            Debug.Log(error);
        }, (response) => { }));
    }

    /// <summary>
    /// Create a classroom on the database.
    /// </summary>
    /// <param name="teacher">Teacher for which the classroom needs to be created.</param>
    /// <param name="name">Name of the classroom.</param>
    /// <param name="successCallback">Callback that is called after creation with as argument the newly made classroom.</param>
    /// /// /// <param name="failCallback">Callback for when the creation was not succesful, if left default error is logged.</param>
    public void CreateClassroom(Teacher teacher, string name, Action<Classroom> successCallback, Action<string> failCallback = null)
    {
        if (failCallback == null) failCallback = (error) => { Debug.LogError(error); };

        Dictionary<string, string> parameters = new Dictionary<string, string>() { };
        parameters.Add("name", name);
        parameters.Add("teacherId", teacher.id.ToString());
        StartCoroutine(NetworkSystem.Instance.PostCoroutine("classroom/create", parameters, (error) => failCallback(error), (response) => {
            Classroom classroom= Classroom.FromJson(response);
            successCallback(classroom);
        }
        ));
    }

    public void DeleteClassroom(int id)
    {
        Dictionary<string, string> parameters = new Dictionary<string, string>() { };
        parameters.Add("id", id.ToString());

        StartCoroutine(NetworkSystem.Instance.PostCoroutine("classroom/delete", parameters, (error) => {
            Debug.Log(error);
        }, (response) => { }));
    }


    //public void GetClassroomsById(int id, Action<ClassroomList> successCallback)
    //{
    //    Dictionary<string, string> parameters = new Dictionary<string, string>() { };
    //    parameters.Add("id", id.ToString());
    //    StartCoroutine(NetworkController.Instance.PostCoroutine("classroom/fetch/id", parameters, (error) => Debug.LogError(error), (response) => {
    //        ClassroomList classroomList = ClassroomList.FromJson(response);
    //        successCallback(classroomList);
    //    }
    //    ));
    //}

    /// <summary>
    /// Get all the classrooms and its student from the backend.
    /// </summary>
    /// <param name="teacher">Teacher for which classroom data needs to be fetched.</param>
    /// <param name="successCallback">Callback called after the data has been fetched with the ClassroomList as argument.</param>
    /// <param name="failCallback">Callback for when the fetch was not succesful or none were found, if left default error is logged.</param>
    public void GetClassroomsByTeacher(Teacher teacher, Action<ClassroomList> successCallback, Action<string> failCallback = null)
    {
        if (failCallback == null) failCallback = (error) => { Debug.LogError(error); };

        Dictionary<string, string> parameters = new Dictionary<string, string>() { };
        parameters.Add("id", teacher.id.ToString());

        StartCoroutine(NetworkSystem.Instance.PostCoroutine("classroom/fetch/teacherId", parameters, (error) => failCallback(error), (response) => {
            ClassroomList classroomList = ClassroomList.FromJson(response);
            successCallback(classroomList);
        }
        ));
    }

    /// <summary>
    /// Get all the minigames of the teacher from the backend.
    /// </summary>
    /// <param name="teacher">Teacher for which the minigames need to be fetched.</param>
    /// <param name="successCallback">Callback called when the minigames are fetched with as argument a list of Minigame objects.</param>
    /// <param name="failCallback">Callback for when the fetch was not succesful or none were found, if left default error is logged.</param>
    public void GetMinigamesByTeacher(Teacher teacher, Action<List<Minigame>> successCallback, Action<string> failCallback=null)
    {
        if (failCallback == null) failCallback = (error) => { Debug.LogError(error); };

        Dictionary<string, string> parameters = new Dictionary<string, string>() { };
        parameters.Add("teacherId", teacher.id.ToString());
        StartCoroutine(NetworkSystem.Instance.PostCoroutine("minigame/fetch/teacherId", parameters, (error) => failCallback(error), (response) =>
        {
            List<Minigame> minigameList = Minigame.MultipleFromJson(response);
            successCallback(minigameList);
        }
        ));
    }

    //public void GetEcsById(int id, Action<Minigame> successCallback)
    //{
    //    Dictionary<string, string> parameters = new Dictionary<string, string>() { };
    //    parameters.Add("id", id.ToString());
    //    StartCoroutine(NetworkController.Instance.PostCoroutine("minigame/fetch/id", parameters, (error) => Debug.LogError(error), (response) =>
    //    {
    //        Minigame minigameList = Minigame.FromJson(response);
    //        successCallback(minigameList);
    //    }
    //    ));
    //}

    /// <summary>
    /// Create a new minigame.
    /// </summary>
    /// <param name="teacher">Teacher for which the minigame needs to be created.</param>
    /// <param name="subtype">Subtype of the new minigame.</param>
    /// <param name="subject">Subject of the new minigame.</param>
    /// <param name="tag">Tag of the new minigame.</param>
    /// <param name="data">Data of the new minigame. (CSV in string format)</param>
    /// <param name="shared">Boolean indicating if the minigame is to be shared in the public library.</param>
    /// <param name="successCallback">Callback called for when the minigame is created with as argument the new minigame.</param>
    /// <param name="failCallback">Callback for when the creation was not succesful or none were found, if left default error is logged.</param>
    public void CreateMinigame(Teacher teacher, Subtype subtype, Subject subject, string tag, string data, bool shared, Action<Minigame> successCallback, Action<string> failCallback = null)
    {
        if (failCallback == null) failCallback = (error) => { Debug.LogError(error); };

        Dictionary<string, string> parameters = new Dictionary<string, string>() { };
        parameters.Add("teacherId", teacher.id.ToString());
        parameters.Add("subject", ((int)subject).ToString());
        parameters.Add("subtype", ((int) subtype).ToString());
        parameters.Add("tag", tag);
        parameters.Add("data", data);
        parameters.Add("shared", shared.ToString());

        StartCoroutine(NetworkSystem.Instance.PostCoroutine("minigame/create", parameters, (error) => failCallback(error), (response) =>
        {
            Minigame minigame = Minigame.FromJson(response);
            successCallback(minigame);
        }
        ));
    }

    /// <summary>
    /// Get all public minigames from backend.
    /// </summary>
    /// <param name="callback">Callback for when minigames are fetched with as argument these fetched public minigames.</param>
    public void GetPublicMinigames(Action<List<Minigame>> callback)
    {
        Dictionary<string, string> parameters = new Dictionary<string, string>() { };
        StartCoroutine(NetworkSystem.Instance.PostCoroutine("minigame/public", parameters, (error) => Debug.LogError(error), (response) =>
        {
            List<Minigame> minigames = Minigame.MultipleFromJson(response);
            callback(minigames);
        }
        ));
    }

    /// <summary>
    /// Edit the minigames data on the backend.
    /// </summary>
    /// <param name="id">Id of the minigame to edit</param>
    /// <param name="data">New data of the minigame.</param>
    /// <param name="succesCallback">Callback for when the minigame is edited containing the new data checked by the server.</param>
    public void EditMinigame(int id, string data, Action<string> succesCallback, Action<string> failCallback=null)
    {
        if (failCallback == null) failCallback = (error) => { Debug.LogError(error); };

        Dictionary<string, string> parameters = new Dictionary<string, string>() { };
        parameters.Add("id", id.ToString());
        parameters.Add("data", data);

        StartCoroutine(NetworkSystem.Instance.PostCoroutine("minigame/edit", parameters, (error) => failCallback(error), (response) =>
        {
            Minigame minigame = Minigame.FromJson(response);
            succesCallback(minigame.data);
        }
        ));
    }

    /// <summary>
    /// Delete a minigame from the backend.
    /// </summary>
    /// <param name="id">Id of the minigame to delete.</param>
    /// <param name="callback">Callback for what to do after succesful deleteion.</param>
    public void DeleteMinigame(int id, Action callback)
    {
        Dictionary<string, string> parameters = new Dictionary<string, string>() { };
        parameters.Add("id", id.ToString());

        StartCoroutine(NetworkSystem.Instance.PostCoroutine("minigame/delete", parameters, (error) => Debug.LogError(error), (response) =>
        {
            callback();
        }
        ));
    }

    /// <summary>
    /// Get AL inter game data.
    /// </summary>
    /// <param name="studentID">Id of the student.</param>
    /// <param name="ECSID">Id of the minigame</param>
    /// <param name="successCallback">Callback for when succesful.</param>
    /// <param name="errorCallback">Callback for when it failed.</param>
    public void GetALData(int studentID, int ECSID, Action<string> successCallback, Action<string> errorCallback) 
    {
        Dictionary<string, string> parameters = new Dictionary<string, string>() {};
        parameters.Add("studentId", studentID.ToString());
        parameters.Add("minigameId", ECSID.ToString());
        StartCoroutine(NetworkSystem.Instance.PostCoroutine("al/get", parameters, (error) => errorCallback(error), 
            (response) => successCallback(response)));
    }

    /// <summary>
    /// Set AL inter game data or create new one if it doesn't exist.
    /// </summary>
    /// <param name="studentID">Id of the student.</param>
    /// <param name="ECSID">Id of the minigame.</param>
    /// <param name="jsonData">Json string with new data.</param>
    public void SetALData(int studentID, int ECSID, string jsonData) 
    {
        Dictionary<string, string> parameters = new Dictionary<string, string>() { };
        parameters.Add("studentId", studentID.ToString());
        parameters.Add("minigameId", ECSID.ToString());
        parameters.Add("data", jsonData);
        StartCoroutine(NetworkSystem.Instance.PostCoroutine("al/set", parameters, (error) => Debug.LogError(error), 
            (response) => Debug.Log("Set ECS data.")));
    }
}

}
