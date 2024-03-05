using UnityEngine;
using UnityEngine.UIElements;
using System;
using System.Collections.Generic;
using GameData;

using Platform.Controllers;

namespace Platform.CustomControls
{

/// <summary>
/// UI element that displays an ECS
/// </summary>
public class EcsEditorPopup : VisualElement
{
    public new class UxmlFactory : UxmlFactory<EcsEditorPopup> { }
    private DropdownField subject => this.Q<DropdownField>("Subject");
    private DropdownField type => this.Q<DropdownField>("Type");
    private TextField tag => this.Q<TextField>("Tag");

    public Action saveEvent;
    public Action openEvent;
    public Action cancelEvent;
    public Action<string> gptEvent;

    private Button cancelButton => this.Q<Button>("button-cancel");
    private Button saveButton => this.Q<Button>("button-save");
    private Button openButton => this.Q<Button>("button-open");
    private Toggle gptToggle => this.Q<Toggle>("toggle-gpt");
    private Toggle publicToggle => this.Q<Toggle>("toggle-public");

    private TextField gptField => this.Q<TextField>("field-gpt");
    private Button gptButton => this.Q<Button>("button-gpt");
    public Label gptReady => this.Q<Label>("ready-gpt");

    public bool shared = false;

    public EcsEditorPopup()
    {
        var asset = Resources.Load<VisualTreeAsset>("ContentEditor/EcsEditorPopup");
        asset.CloneTree(this);

        SetOptions();

        cancelButton.clicked += Cancel;
        saveButton.clicked += Save;
        openButton.clicked += Open;
        gptButton.clicked += GPT;

        gptToggle.RegisterCallback<ChangeEvent<bool>>(OnGptToggle);
        publicToggle.RegisterCallback<ChangeEvent<bool>>(OnPublicToggle);

        gptField.EnableInClassList("heightZero", true);
        gptButton.EnableInClassList("heightZero", true);
    }

    /// <summary>
    /// Set the options of the dropdown fields of an EcsEditorPopup
    /// </summary>
    private void SetOptions()
    {
        subject.choices.AddRange(new List<string>() { "Wiskunde", "Nederlands", "Lezen", "Wereld Orientatie", "Religie", "Zedenleer", "Frans" });
        type.choices.AddRange(new List<string>() { "Meerkeuze", "Groepering", "Rangschikken"});
    }


    /// <summary>
    /// Get a minigame object depending on data in ECS fields
    /// </summary>
    /// <returns>Minigame object or null if not all fields are filled in</returns>
    public Minigame GetMinigame()
    {
        Subject? subject = GetSubject();
        string tag = GetTag();
        if (subject == null) return null;

        Minigame result = null;
        switch (type.value)
        {
            case "Meerkeuze":
                result = new MultipleChoice(subject.Value, tag);
                break;
            case "Groepering":
                result = new Grouping(subject.Value, tag);
                break;
            case "Rangschikken":
                result = new Ranking(subject.Value, tag);
                break;
            default:
                break;
        }
        return result;
    }

    /// <summary>
    /// Get subject depending on selected option
    /// </summary>
    private Subject? GetSubject()
    {
        Subject? result = null;
        switch (subject.value)
        {
            case "Wiskunde":
                result = Subject.Mathematics;
                break;
            case "Nederlands":
                result = Subject.WritingDutch;
                break;
            case "Lezen":
                result = Subject.ReadingDutch;
                break;
            case "Wereld Orientatie":
                result = Subject.WorldOrientation;
                break;
            case "Religie":
                result = Subject.Religion;
                break;
            case "Zedenleer":
                result = Subject.Ethics;
                break;
            case "Frans":
                result = Subject.French;
                break;
            default: 
                break;
        }
        return result;
    }

    /// <summary>
    /// Get the tag from textfield
    /// </summary>
    private string GetTag()
    {
        return tag.value;
    }

    /// <summary>
    /// Set the UI depending on the minigame subject
    /// </summary>
    public void SetSubject(Subject subject)
    {
        switch (subject)
        {
            case Subject.Mathematics:
                this.subject.value = "Wiskunde";
                break;
            case Subject.WritingDutch:
                this.subject.value = "Nederlands";
                break;
            case Subject.ReadingDutch:
                this.subject.value = "Lezen";
                break;
            case Subject.WorldOrientation:
                this.subject.value = "Wereld Orientatie";
                break;
            case Subject.Religion:
                this.subject.value = "Religie";
                break;
            case Subject.Ethics:
                this.subject.value = "Zedenleer";
                break;
            case Subject.French:
                this.subject.value = "Frans";
                break;
            default:
                break;
        }
    }

    /// <summary>
    /// Set the UI depending on the minigame subtype
    /// </summary>
    public void SetSubtype(Subtype subtype)
    {
        switch (subtype)
        {
            case Subtype.MultipleChoice:
                type.value = "Meerkeuze";
                break;
            case Subtype.Grouping:
                type.value = "Groepering";
                break;
            case Subtype.Ranking:
                type.value = "Rangschikken";
                break;
            default:
                break;
        }
    }

    public void Open()
    {
        this.openEvent?.Invoke();
    }

    public void Save()
    {
        this.saveEvent?.Invoke();
        ClassroomDashboardController.Instance.SaveEcs();
        Cancel();
    }


    public void Cancel()
    {
        this.RemoveFromHierarchy();
        this.cancelEvent?.Invoke();
        ClassroomDashboardController.Instance.CloseEcsEditor();
    }

    public void OnGptToggle(ChangeEvent<bool> evt)
    {
        if (evt.newValue)
        {
            gptField.EnableInClassList("heightZero", false);
            gptButton.EnableInClassList("heightZero", false);
        }
        else
        {
            gptField.EnableInClassList("heightZero", true);
            gptButton.EnableInClassList("heightZero", true);
        }
    }

    public void OnPublicToggle(ChangeEvent<bool> evt)
    {
        if (evt.newValue)
        {
            shared = true;
        }
        else
        {
            shared = false;
        }
    }

    public void GPT()
    {
        gptEvent?.Invoke(this.gptField.text);
    }
}

}
