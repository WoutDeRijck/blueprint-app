using UnityEngine;
using UnityEngine.UIElements;
using GameData;
using System;
using Misc;
using System.Diagnostics;
using System.Collections.Generic;
using System.Linq;

using Platform.Controllers;

namespace Platform.CustomControls
{

/// <summary>
/// Ui element that displays the ECS data (Minigame object)
/// </summary>
public class EcsEntry : VisualElement
{
    // Expose the custom control to UXML and UI Builder.
    public new class UxmlFactory : UxmlFactory<EcsEntry> { }

    public Label subjectLabel => this.Q<Label>("label-subject");
    public Label subtypeLabel => this.Q<Label>("label-subtype");
    public Label tagLabel => this.Q<Label>("label-tag");

    public Button editButton => this.Q<Button>("button-edit");
    public Button deleteButton => this.Q<Button>("button-delete");

    public uint dataIndex;
    private Toggle selectedToggle => this.Q<Toggle>("toggle-selected");

    public EcsEntry() 
    {
        var asset = Resources.Load<VisualTreeAsset>("Framework/EcsEntry");
        asset.CloneTree(this);

        selectedToggle.RegisterCallback<ChangeEvent<bool>>(EditSelection);
        editButton.RegisterCallback<ClickEvent>(Edit);
        deleteButton.RegisterCallback<ClickEvent>(Delete);
    }

    public void SetData(uint index, Minigame minigame, bool selected)
    {
        this.dataIndex = index;
        subjectLabel.text = EnumDescriptor.GetDescription(minigame.subject);
        subtypeLabel.text = EnumDescriptor.GetDescription(minigame.subtype);
        tagLabel.text = minigame.tag;

        selectedToggle.SetValueWithoutNotify(selected);
    }

    private void EditSelection(ChangeEvent<bool> evt)
    {
        bool newValue = evt.newValue;

        if (newValue)
        {
            ClassroomDashboardController.Instance.AddToSelection(dataIndex);
        } else
        {
            ClassroomDashboardController.Instance.RemoveFromSelection(dataIndex);
        }
    }

    private void Edit(ClickEvent evt)
    {
        string data = ClassroomDashboardController.Instance.minigames[dataIndex].data;

        Process p = CSVManager.Open("temp.csv", CSVManager.StringToCSVList(data));

        p.WaitForExit();

        string newData = CSVManager.ReadCSVToString("temp.csv");

        ClassroomDashboardController.Instance.EditMinigame(dataIndex, newData);

        CSVManager.DeleteFile("temp.csv");
    }

    private void Delete(ClickEvent evt)
    {
        ClassroomDashboardController.Instance.RemoveMinigame(dataIndex);
    }

}

}
