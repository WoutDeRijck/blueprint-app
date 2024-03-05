using UnityEngine;
using UnityEngine.UIElements;

namespace Platform.CustomControls
{

/// <summary>
/// UI element that displays name of classroom with an emblem next to it
/// </summary>
public class StudentEntry : VisualElement
{
    // Expose the custom control to UXML and UI Builder.
    public new class UxmlFactory : UxmlFactory<StudentEntry> { }
    private Label nameLabel => this.Q<Label>("NameLabel");

    public StudentEntry()
    {
        var asset = Resources.Load<VisualTreeAsset>("Framework/StudentEntry");
        asset.CloneTree(this);
    }

    public void SetName(string name)
    {
        nameLabel.text = name;
    }
}

}
