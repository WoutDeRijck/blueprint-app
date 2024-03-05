using System;
using System.Collections.Generic;
using UnityEngine.UIElements;
using GameData;
using Misc;

using Platform.Controllers;
using Platform.CustomControls;
using Platform.Interfacing.Data;
using Platform.Interfacing.Systems;

namespace Platform.Views
{

/// <summary>
/// View for the dashboard base, handles logging out and 
/// </summary>
public class DashboardView : Singleton<DashboardView>
{
    private VisualElement root;
    private VisualElement content;

    private VisualElement ecsContainer;
    private VisualElement classroomContainer;
    private VisualElement studentContainer;

    private int selectedClassroomIndex = -1;

    private Button logOutButton;
    private Button createEcsButton;
    private Button createStudentButton;
    private Button createClassroomButton;
    private Button startGameButton;
    private Button openLibButton;
    private RadioButton gameModeRadio;

    // Start is called before the first frame update
    void OnEnable()
    {
        // Initializing the UI components
        root = GetComponent<UIDocument>().rootVisualElement;
        content = root.Q<VisualElement>("content");
        logOutButton = root.Q<Button>("button-log-out");

        ecsContainer = root.Q<VisualElement>("container-ecs");
        classroomContainer = root.Q<VisualElement>("container-classroom");
        studentContainer = root.Q<VisualElement>("container-student");

        createEcsButton = root.Q<Button>("button-create-ecs");
        createStudentButton = root.Q<Button>("button-create-student");
        createClassroomButton = root.Q<Button>("button-create-classroom");
        startGameButton = root.Q<Button>("button-start-game");
        openLibButton = root.Q<Button>("button-open-lib");
        gameModeRadio = root.Q<RadioButton>("game-radio");
        gameModeRadio.value = true;

        // Register callbacks
        logOutButton.clicked += Logout;
        createEcsButton.clicked += CreateEcs;
        createStudentButton.clicked += CreateStudent;
        createClassroomButton.clicked += CreateClassroom;
        startGameButton.clicked += StartGame;
        openLibButton.clicked += OpenLibrary;
    }
    private void Start()
    {
        ClassroomDashboardController.Instance.minigameRefreshEvent += RefreshEcs;
        ClassroomDashboardController.Instance.classroomRefreshEvent += RefreshClassrooms;

        ClassroomDashboardController.Instance.GetEcs();
        ClassroomDashboardController.Instance.GetClassrooms();
    }

    /// <summary>
    /// Handles the change of view when logging out and signaling the Authentication System to logout.
    /// </summary>
    private void Logout()
    {
        AuthenticationSystem.Instance.Logout(() => SceneLoader.Load(SceneLoader.Scene.LoginScene));
    }

    /// <summary>
    /// Fetch all the ECSs and add them to the ECS list
    /// </summary>
    private void RefreshEcs()
    {
        ecsContainer.Clear();

        // Create a list of data
        Dictionary<uint, Minigame> minigames = ClassroomDashboardController.Instance.minigames;
        List<uint> minigameDataKeys = new List<uint>(minigames.Keys);

        // The "makeItem" function is called when the
        // ListView needs more items to render.
        Func<VisualElement> makeItem = () => new EcsEntry();

        // As the user scrolls through the list, the ListView object
        // recycles elements created by the "makeItem" function,
        // and invoke the "bindItem" callback to associate
        // the element with the matching data item (specified as an index in the list).
        Action<VisualElement, int> bindItem = (e, i) =>
        {
            uint index = minigameDataKeys[i];
            Minigame minigame = minigames[index];
            bool selected = (ClassroomDashboardController.Instance.selectedEcs.ContainsKey(index));
            (e as EcsEntry).SetData(index, minigame, selected);
        };

        // Provide the list view with an explicit height for every row
        // so it can calculate how many items to actually display
        const int itemHeight = 64;
        var listView = new ListView(minigameDataKeys, itemHeight, makeItem, bindItem);
        listView.selectionType = SelectionType.Multiple;
        listView.style.flexGrow = 1.0f;

        ecsContainer.Add(listView);
    }

    private void CreateEcs()
    {
        EcsEditorPopup view = new EcsEditorPopup();
        content.Add(view);
        ClassroomDashboardController.Instance.CreateEcsEditor(view);
    }

    private void RefreshClassrooms()
    {
        classroomContainer.Clear();

        // Create a list of data
        List<Classroom> items = ClassroomDashboardController.Instance.classrooms;

        // The "makeItem" function is called when the
        // ListView needs more items to render.
        Func<VisualElement> makeItem = () => new ClassroomEntry();

        // As the user scrolls through the list, the ListView object
        // recycles elements created by the "makeItem" function,
        // and invoke the "bindItem" callback to associate
        // the element with the matching data item (specified as an index in the list).
        Action<VisualElement, int> bindItem = (e, i) => (e as ClassroomEntry).SetData(items[i].name, items[i].id);

        // Provide the list view with an explicit height for every row
        // so it can calculate how many items to actually display
        ListView listView = new ListView(items, 64, makeItem, bindItem);
        listView.selectionType = SelectionType.Multiple;
        listView.style.flexGrow = 1.0f;
        listView.selectionChanged += (objects) => SelectClassroom(listView.selectedIndex);

        classroomContainer.Add(listView);

        if (selectedClassroomIndex >= 0 && selectedClassroomIndex < items.Count)
        {
            listView.SetSelection(selectedClassroomIndex);
        }
    }

    private void SelectClassroom(int index)
    {
        selectedClassroomIndex = index;
        studentContainer.Clear();

        List<Student> items = ClassroomDashboardController.Instance.GetStudents(index);

        // The "makeItem" function is called when the
        // ListView needs more items to render.
        Func<VisualElement> makeItem = () => new StudentEntry();

        // As the user scrolls through the list, the ListView object
        // recycles elements created by the "makeItem" function,
        // and invoke the "bindItem" callback to associate
        // the element with the matching data item (specified as an index in the list).
        Action<VisualElement, int> bindItem = (e, i) => (e as StudentEntry).SetName(items[i].name);

        ListView listView = new ListView(items, 64, makeItem, bindItem);
        listView.selectionType = SelectionType.Multiple;
        listView.style.flexGrow = 1.0f;

        studentContainer.Add(listView);
    }

    private void CreateStudent()
    {
        if (selectedClassroomIndex >= 0)
        {
            StudentFormPopup popup = new StudentFormPopup();
            popup.SetSelectedIndex(selectedClassroomIndex);

            content.Add(popup);
        }
    }

    private void CreateClassroom()
    {
        ClassroomFormPopup popup = new ClassroomFormPopup();
        content.Add(popup);
    }

    private void StartGame()
    {
        ClassroomDashboardController.Instance.StartGame();
    }

    private void OpenLibrary()
    {
        PublicLibraryPopup popup = new PublicLibraryPopup();
        content.Add(popup);
    }
}

}
