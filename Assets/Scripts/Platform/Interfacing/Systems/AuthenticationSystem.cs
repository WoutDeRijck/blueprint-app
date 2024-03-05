using System;
using Misc;

using Platform.Interfacing.Data;

namespace Platform.Interfacing.Systems
{

/// <summary>
/// System handling the Authentication of user.
/// </summary>
public class AuthenticationSystem : PersistentSingleton<AuthenticationSystem>
{
    /// <summary>
    /// Contains the Teacher object when logged in as a teacher.
    /// </summary>
    public Teacher teacher { get; private set; }

    /// <summary>
    /// Contains the Student object when logged in as a student.
    /// </summary>
    public Student student { get; private set; }

    /// <summary>
    /// Role of the logged in user.
    /// </summary>
    public Role role { get; private set; } = Role.LoggedOut;

    /// <summary>
    /// Log the user out.
    /// </summary>
    /// <param name="next">Callback for what to do next.</param>
    public void Logout (Action next)
    {
        role = Role.LoggedOut;
        teacher = null;
        student = null;

        next();
    }

    /// <summary>
    /// Login the user.
    /// </summary>
    /// <param name="role">Role of the user trying to login.</param>
    /// <param name="email">Email of the user trying to login.</param>
    /// <param name="classroomId">ClassroomId of the user trying to login, if logging in as a teacher this is never used.</param>
    /// <param name="succesCb">Callback for when logging in was succesfull.</param>
    /// <param name="failCb">Callback for when logging in failed.</param>
    public void Login(Role role, string email, int classroomId, Action succesCb, Action<string> failCb)
    {
        this.role = role;
        if (role == Role.Student)
        {
            DataSystem.Instance.LoginStudent(email, classroomId, (student) => { this.student = student; succesCb(); }, failCb);
            
        }
        else if (role == Role.Teacher)
        {
            DataSystem.Instance.LoginTeacher(email, (teacher) => { this.teacher = teacher; succesCb(); }, failCb);
        }
    }
}

/// <summary>
/// Role of the User.
/// </summary>
public enum Role
{
    LoggedOut,
    Teacher,
    Student
}

}
