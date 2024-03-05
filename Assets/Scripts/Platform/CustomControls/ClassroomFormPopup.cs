using UnityEngine;
using UnityEngine.UIElements;
using System;
using System.Collections.Generic;

using Platform.Controllers;

namespace Platform.CustomControls
{

/// <summary>
/// UI element that displays an ECS
/// </summary>
public class ClassroomFormPopup : VisualElement
{
    public new class UxmlFactory : UxmlFactory<ClassroomFormPopup> { }

    public TextField nameField => this.Q<TextField>("field-name");

    private Button cancelButton => this.Q<Button>("button-cancel");
    private Button submit => this.Q<Button>("button-submit");

    public Action submitEvent;
    public Action cancelEvent;

    public ClassroomFormPopup()
    {
        var asset = Resources.Load<VisualTreeAsset>("ContentEditor/ClassroomFormPopup");
        asset.CloneTree(this);

        cancelButton.clicked += Cancel;
        submit.clicked += Submit;
    }


    public void Cancel()
    {
        this.RemoveFromHierarchy();
        this.cancelEvent?.Invoke();
    }

    public void Submit()
    {
        ClassroomDashboardController.Instance.CreateClassroom(nameField.text);
        Cancel();
    }
}

}
