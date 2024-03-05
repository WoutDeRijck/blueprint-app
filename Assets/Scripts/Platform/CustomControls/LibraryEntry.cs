using UnityEngine;
using UnityEngine.UIElements;
using GameData;
using Misc;
using System.Linq;
using System.Collections.Generic;
using System;

using Platform.Controllers;

namespace Platform.CustomControls
{

/// <summary>
/// Ui element that displays the ECS data (Minigame object)
/// </summary>
public class LibraryEntry : VisualElement
{
    // Expose the custom control to UXML and UI Builder.
    public new class UxmlFactory : UxmlFactory<LibraryEntry> { }

    public Label subjectLabel => this.Q<Label>("label-subject");
    public Label subtypeLabel => this.Q<Label>("label-subtype");
    public Label tagLabel => this.Q<Label>("label-tag");

    public Button addButton => this.Q<Button>("button-add");
    public Button inspectButton => this.Q<Button>("button-inspect");

    public uint dataIndex;

    public LibraryEntry() 
    {
        var asset = Resources.Load<VisualTreeAsset>("Framework/LibraryEntry");
        asset.CloneTree(this);

        addButton.RegisterCallback<ClickEvent>(AddToLibrary);
        inspectButton.RegisterCallback<ClickEvent>(InspectMinigame);
    }

    public void SetData(uint dataIndex, Minigame minigame)
    {
        this.dataIndex = dataIndex;

        subjectLabel.text = EnumDescriptor.GetDescription(minigame.subject);
        subtypeLabel.text = EnumDescriptor.GetDescription(minigame.subtype);
        tagLabel.text = minigame.tag;
    }

    public void AddToLibrary(ClickEvent evt) 
    {
        PublicLibraryController.Instance.AddMinigame(dataIndex);
    }

    public void InspectMinigame(ClickEvent evt)
    {
        List<string[]>  csv = CSVManager.StringToCSVList(PublicLibraryController.Instance.selectedMinigames[dataIndex].data);
        
        CSVManager.Open("test.csv", csv);
    }
}

}
