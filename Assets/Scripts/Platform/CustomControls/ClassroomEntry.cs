using UnityEngine;
using UnityEngine.UIElements;

namespace Platform.CustomControls
{

/// <summary>
/// UI element that displays name of classroom with an emblem next to it
/// </summary>
public class ClassroomEntry : VisualElement
{ 
    // Expose the custom control to UXML and UI Builder.
    public new class UxmlFactory : UxmlFactory<ClassroomEntry> { }
    private Label nameLabel => this.Q<Label>("NameLabel");

    private Label idLabel => this.Q<Label>("label-id");

    //public Classroom classroom =>
    public ClassroomEntry() {
        var asset = Resources.Load<VisualTreeAsset>("Framework/ClassroomEntry");
        asset.CloneTree(this);
    }

    public void SetData (string name, int id)
    {
        nameLabel.text = name;
        idLabel.text = id.ToString();
    }
}

}
