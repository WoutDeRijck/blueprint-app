using System;
using UnityEngine;
using UnityEngine.UIElements;

namespace Platform.Views.Login
{

public class RoleToggle : VisualElement
{
    // Expose the custom control to UXML and UI Builder.
    public new class UxmlFactory : UxmlFactory<RoleToggle> { }
    private VisualElement studentContainer => this.Q<VisualElement>("container-student");
    private VisualElement teacherContainer => this.Q<VisualElement>("container-teacher");

    private Button studentButton => this.Q<Button>("button-student");
    private Button teacherButton => this.Q<Button>("button-teacher");

    public Action studentSelected;
    public Action teacherSelected;

    public string selected { get; private set; }

    public RoleToggle()
    {
        var asset = Resources.Load<VisualTreeAsset>("Framework/RoleToggle");
        asset.CloneTree(this);

        selected = "student";

        studentButton.clicked += SelectStudent;
        teacherButton.clicked += SelectTeacher;
    }

    private void SelectStudent()
    {
        selected = "student";
        studentContainer.EnableInClassList("highlighted", true);
        studentButton.EnableInClassList("highlighted-text", true);

        teacherContainer.EnableInClassList("highlighted", false);
        teacherButton.EnableInClassList("highlighted-text", false);
        teacherContainer.EnableInClassList("normal", true);
        teacherButton.EnableInClassList("normal-text", true);

        studentSelected?.Invoke();
    }

    private void SelectTeacher()
    {
        selected = "teacher";
        teacherContainer.EnableInClassList("highlighted", true);
        teacherButton.EnableInClassList("highlighted-text", true);

        studentContainer.EnableInClassList("highlighted", false);
        studentButton.EnableInClassList("highlighted-text", false);
        studentContainer.EnableInClassList("normal", true);
        studentButton.EnableInClassList("normal-text", true);

        teacherSelected?.Invoke();
    }

    public void Enable()
    {
        studentButton.SetEnabled(true);
        teacherButton.SetEnabled(true);
    }
    public void Disable()
    {
        studentButton.SetEnabled(false);
        teacherButton.SetEnabled(false);
    }
}

}
