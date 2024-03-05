using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;
using UnityEngine.SceneManagement;
using Misc;

using Platform.Interfacing.Systems;

namespace Platform.Views.Login
{

public class LoginUIHandler : MonoBehaviour
{
    private VisualElement root;
    private Button submitButton;
    private Button registerButton;
    private TextField emailField;
    private TextField nameField;
    private Label registerLabel;
    private RoleToggle toggle;
    private TextField classroomField;
    private bool newUser;

    private string selected;


    private void OnEnable()
    {
        Screen.SetResolution(1920, 1080, true);
        // Initialize necesarry references to UI elements
        root = GetComponent<UIDocument>().rootVisualElement;
        submitButton = root.Q<Button>("SubmitButton");
        registerButton = root.Q<Button>("Register");
        emailField = root.Q<TextField>("EmailField");
        nameField = root.Q<TextField>("NameField");
        classroomField = root.Q<TextField>("field-classroom");
        registerLabel = root.Q<Label>("RegisterLabel");

        toggle = root.Q<RoleToggle>("toggle-role");

        // Register callback for clicking on button event
        submitButton.RegisterCallback<ClickEvent>(ProcessSubmitCallback);
        registerButton.RegisterCallback<ClickEvent>(ProcessRegister);

        // Register callback for keyboard submit
        root.RegisterCallback<KeyDownEvent>(ProcessKeyDownCallback, TrickleDown.TrickleDown);

        // Register callbacks for selection of role
        toggle.teacherSelected += teacherUI;
        toggle.studentSelected += studentUI;

        registerButton.SetEnabled(false);

        // Initialize the selected state;
        selected = "student";
    }

    private void ProcessSubmitCallback(ClickEvent evt)
    {
        string email = emailField.text;
        string name = nameField.text;
        int classroomId;
        int.TryParse(classroomField.text, out classroomId);

        Debug.Log("Submit! Email: "+email);

        if (selected == "teacher")
        {
            if (newUser)
            {
                DataSystem.Instance.RegisterTeacher(name, email, (teacher) => {
                    if (teacher != null)
                    {
                        AuthenticationSystem.Instance.Login(Role.Teacher, email, 0, () => { SceneLoader.Load(SceneLoader.Scene.ClassroomDashboard); }, (error) => { Debug.LogError(error); });      
                    }
                });
            }
            else
            {
                DataSystem.Instance.LoginTeacher(email, (teacher) => {
                    if (teacher != null)
                    {
                        AuthenticationSystem.Instance.Login(Role.Teacher, email, 0, () => { SceneLoader.Load(SceneLoader.Scene.ClassroomDashboard); }, (error) => { Debug.LogError(error); });
                    }
                }, (error) => { Debug.Log(error); });
            }
        }
        else if (selected == "student")
        {
            DataSystem.Instance.LoginStudent(email, classroomId, (student) => {
                if (student != null)
                {
                    AuthenticationSystem.Instance.Login(Role.Student, email, classroomId, () => { SceneLoader.Load(SceneLoader.Scene.PersonalWorld); }, (error) => { Debug.LogError(error); });
                }
                
            }, (error) => { Debug.Log(error); });
        }    

    }

    private void ProcessRegister(ClickEvent evt)
    {
        if (!newUser) {
            newUser = true;

            toggle.Disable();

            nameField.EnableInClassList("hideShrink", false);

            nameField.EnableInClassList("showGrow", true);

            nameField.EnableInClassList("heightZero", false);

            registerLabel.text = "Al een account?";
            registerButton.text = "Login";
            submitButton.text = "Registreer";
            
        } else {
            newUser = false;

            toggle.Enable();

            nameField.EnableInClassList("showGrow", false);

            nameField.EnableInClassList("hideShrink", true);

            nameField.EnableInClassList("heightZero", true);

            registerLabel.text = "Nog geen account?";
            registerButton.text = "Registreer";
            submitButton.text = "Log in!";
        }
    }

    private void ProcessKeyDownCallback(KeyDownEvent evt)
    {
        // Check if player wants to submit
        if (evt.keyCode == KeyCode.Return)
        {
            submitButton.SendEvent(new ClickEvent { target = submitButton });
        }
    }

    private void teacherUI()
    {
        registerButton.text = "registreer";
        registerButton.SetEnabled(true);
        selected = "teacher";

        classroomField.EnableInClassList("showGrow", false);

        classroomField.EnableInClassList("hideShrink", true);

        classroomField.EnableInClassList("heightZero", true);
        Debug.Log(selected);
    }

    private void studentUI()
    {
        registerButton.text = "vraag het aan je leerkracht!";
        registerButton.SetEnabled(false);
        selected = "student";

        classroomField.EnableInClassList("hideShrink", false);

        classroomField.EnableInClassList("showGrow", true);

        classroomField.EnableInClassList("heightZero", false);
        Debug.Log(selected);
    }
}

}
