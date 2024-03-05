using UnityEngine;
using UnityEngine.UIElements;
using System;
using System.Collections.Generic;
using Misc;
using System.Diagnostics;

using Platform.Controllers;

namespace Platform.CustomControls
{

/// <summary>
/// UI element that displays an ECS
/// </summary>
public class StudentFormPopup : VisualElement
{
    public new class UxmlFactory : UxmlFactory<StudentFormPopup> { }

    private VisualElement singleContainer => this.Q<VisualElement>("container-single");
    private VisualElement multipleContainer => this.Q<VisualElement>("container-multiple");

    public TextField emailField => this.Q<TextField>("field-email");
    public TextField nameField => this.Q<TextField>("field-name");

    private Button cancelButton => this.Q<Button>("button-cancel");
    private Button submitButton => this.Q<Button>("button-submit");

    private Button submitMultipleButton => this.Q<Button>("button-submit-multiple");
    private Button openButton => this.Q<Button>("button-open");
    

    public Action submitEvent;
    public Action cancelEvent;

    int selectedIndex;

    private bool csvOpened = false;
    private bool mode = true;
    private Button toggleButton => this.Q<Button>("button-toggle");

    private List<string[]> csvData = new List<string[]> { new string[] { "Naam", "Email" }, };

    public StudentFormPopup()
    {
        var asset = Resources.Load<VisualTreeAsset>("ContentEditor/StudentFormPopup");
        asset.CloneTree(this);

        toggleButton.clicked += ToggleMode;
        cancelButton.clicked += Cancel;
        openButton.clicked += Open;
        submitButton.clicked += Submit;
        submitMultipleButton.clicked += SubmitMultiple;
    }
    ~StudentFormPopup()
    {
        CSVManager.DeleteFile("student.csv");
    }

    public void SetSelectedIndex(int selectedIndex)
    {
        this.selectedIndex = selectedIndex;
    }


    private void Cancel()
    {
        this.RemoveFromHierarchy();
        this.cancelEvent?.Invoke();
    }

    private void Submit()
    {
        ClassroomDashboardController.Instance.AddStudent(selectedIndex, nameField.text, emailField.text);
        Cancel();
    }

    private void SubmitMultiple()
    {
        bool status = ClassroomDashboardController.Instance.AddStudentFromCsv(selectedIndex, csvData);
        UnityEngine.Debug.Log(status);
        if (status) 
        {
            Cancel();
        };
    }

    private void Open()
    {
        if (!csvOpened) csvOpened = true;
        
        Process excel = CSVManager.Open("student.csv", csvData);
        excel.WaitForExit();

        csvData = CSVManager.ReadCSV("student.csv");
    }

    private void ToggleMode()
    {
        if (mode)
        {
            singleContainer.style.display = DisplayStyle.None;
            multipleContainer.style.display = DisplayStyle.Flex;
            toggleButton.text = "Of maar eentje?";
        } else
        {
            singleContainer.style.display = DisplayStyle.Flex;
            multipleContainer.style.display = DisplayStyle.None;
            toggleButton.text = "Of meerdere tegelijk?";
        }
        mode = !mode;
    }
}

}
