using System;
using UnityEngine;
using UnityEngine.UIElements;
using GameData;
using System.Collections.Generic;
using Platform.Controllers;

namespace Platform.CustomControls
{

/// <summary>
/// Makes a popup displaying the public library.
/// </summary>
public class PublicLibraryPopup : VisualElement
{
    public new class UxmlFactory : UxmlFactory<PublicLibraryPopup> { }

    private EnumField subjectField => this.Q<EnumField>("subject-field");
    private EnumField typeField => this.Q<EnumField>("type-field");

    private Button cancelButton => this.Q<Button>("button-cancel");

    private VisualElement ecsContainer => this.Q<VisualElement>("ecs-container");

    public PublicLibraryPopup()
    {
        var asset = Resources.Load<VisualTreeAsset>("ContentEditor/PublicLibraryPopup");
        asset.CloneTree(this);

        PublicLibraryController.Instance.libraryRefreshEvent += RefreshPublicLibrary;

        subjectField.RegisterCallback<ChangeEvent<Enum>>(subjectChanged);
        typeField.RegisterCallback<ChangeEvent<Enum>>(typeChanged);

        cancelButton.clicked += Cancel;

        PublicLibraryController.Instance.GetPublicMinigames();
        subjectField.value = Subject.Mathematics;
        typeField.value = Subtype.MultipleChoice;
        PublicLibraryController.Instance.setSubject(subjectField.value);
        PublicLibraryController.Instance.setSubtype(typeField.value);
        }

    private void RefreshPublicLibrary()
    {
        ecsContainer.Clear();

        Dictionary<uint, Minigame> minigames = PublicLibraryController.Instance.selectedMinigames;
        List<uint> minigameDataKeys = new List<uint>(minigames.Keys);

        // The "makeItem" function is called when the
        // ListView needs more items to render.
        Func<VisualElement> makeItem = () => new LibraryEntry();

        // As the user scrolls through the list, the ListView object
        // recycles elements created by the "makeItem" function,
        // and invoke the "bindItem" callback to associate
        // the element with the matching data item (specified as an index in the list).
        Action<VisualElement, int> bindItem = (e, i) => (e as LibraryEntry).SetData(minigameDataKeys[i], minigames[minigameDataKeys[i]]);

        ListView listView = new ListView(new List<Minigame>(minigames.Values), 64, makeItem, bindItem);
        listView.style.flexGrow = 1.0f;
        listView.selectionType = SelectionType.None;

        ecsContainer.Add(listView);
    }

    private void subjectChanged(ChangeEvent<Enum> evt)
    {
        PublicLibraryController.Instance.setSubject((Subject)evt.newValue);
    }

    private void typeChanged(ChangeEvent<Enum> evt)
    {
        PublicLibraryController.Instance.setSubtype((Subtype)evt.newValue);
    }

    private void Cancel()
    {
        this.RemoveFromHierarchy();
    }
}

}
