using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GameData;
using Misc;
using System.Linq;
using Unity.Services.Lobbies.Models;
using GameManagement;


using Platform.CustomControls;
using Platform.Interfacing.Data;
using Platform.Interfacing.Systems;

namespace Platform.Controllers
{

/// <summary>
/// Logic of the Classroom Dashboard.
/// </summary>
public class ClassroomDashboardController : Singleton<ClassroomDashboardController>
{
    public List<Classroom> classrooms { get; private set; } = new List<Classroom>();
    public Dictionary<uint, Minigame> minigames { get; private set; } = new Dictionary<uint, Minigame>();

    public Action classroomRefreshEvent;
    public Action minigameRefreshEvent;

    [SerializeField]
    private GameObject contentEditorPrefab;
    private ContentEditor contentEditor;

    public Dictionary<uint,Minigame> selectedEcs { get; private set; }  = new Dictionary<uint, Minigame>();

    public void GetClassrooms()
    {
        DataSystem.Instance.GetClassroomsByTeacher(AuthenticationSystem.Instance.teacher, (list) => { classrooms = list.classrooms; classroomRefreshEvent.Invoke(); });
    }

    public void CreateClassroom(string name)
    {
        DataSystem.Instance.CreateClassroom(AuthenticationSystem.Instance.teacher, name, (classroom) => { GetClassrooms(); });
    }

    public List<Student> GetStudents(int classroomIndex)
    {
        List<Student> students = classrooms[classroomIndex].students;
        return students;
    }

    public void AddStudent(int classroomIndex, string name, string email)
    {
        DataSystem.Instance.RegisterStudent(classrooms[classroomIndex], name, email, (student) => { GetClassrooms(); });
    }

    public void GetEcs()
    {
        DataSystem.Instance.GetMinigamesByTeacher(AuthenticationSystem.Instance.teacher, (List<Minigame> minigames) => {
            this.minigames = minigames.Select((s, index) => new { s, index }).ToDictionary(x => (uint)x.index, x => x.s);
            minigameRefreshEvent?.Invoke();
        } );
    }

    public void CreateEcsEditor (EcsEditorPopup view)
    {
        GameObject editorObject = Instantiate(contentEditorPrefab);
        this.contentEditor =  editorObject.GetComponent<ContentEditor>();
        contentEditor.SetupECS(view);
    }

    public void CloseEcsEditor()
    {
        Destroy(this.contentEditor.gameObject);
        contentEditor = null;
    }

    public void SaveEcs()
    {
        Minigame savedMinigame = contentEditor.minigame;

        DataSystem.Instance.CreateMinigame(AuthenticationSystem.Instance.teacher, savedMinigame.subtype, savedMinigame.subject, savedMinigame.tag, savedMinigame.data, savedMinigame.shared,(minigame) => {
            GetEcs();
        });
    }

    public void AddToSelection(uint dataIndex)
    {
        selectedEcs.Add(dataIndex, minigames[dataIndex]);
    }

    public void RemoveFromSelection(uint dataIndex)
    {
        selectedEcs.Remove(dataIndex);
    }

    public void AddMinigame(Minigame minigame) {
        selectedEcs.Clear();
        DataSystem.Instance.CreateMinigame(AuthenticationSystem.Instance.teacher, minigame.subtype, minigame.subject, minigame.tag, minigame.data, false,(minigame) =>
        {
            GetEcs();
        });
    }

    public void RemoveMinigame(uint dataIndex)
    {
        DataSystem.Instance.DeleteMinigame(minigames[dataIndex].id, () => { GetEcs(); });
    }

    public void EditMinigame(uint dataIndex, string data)
    {
        Minigame minigame = minigames[dataIndex];
        
        if (minigame.data == data) return;
        DataSystem.Instance.EditMinigame(minigame.id, data, (newData) =>
        {
            minigames[dataIndex].data = newData;
        });
    }

    public bool AddStudentFromCsv(int classroomIndex, List<string[]> csv)
    {
        // Check headers
        if (csv[0][0] != "Naam" || csv[0][1] != "Email") return false;
        
        // Check if entries are correctly formatted
        for (int i = 1; i < csv.Count; i++)
        {
            string[] studentData = csv[i];
            string name = studentData[0];
            string email = studentData[1];

            if (!Student.isValid(name, email)) return false;
        }

        // Create them on backend
        for (int i = 1; i < csv.Count; i++)
        {
            string[] studentData = csv[i];
            string name = studentData[0];
            string email = studentData[1];

            int index = i;
            DataSystem.Instance.RegisterStudent(classrooms[classroomIndex], name, email, (student) => {
                if (index == csv.Count - 1) GetClassrooms();
            });
        }
        return true;
    }

    public void StartGame()
    {
        if (selectedEcs.Count <= 0) return;
        List<Minigame> minigames = new List<Minigame>();
        foreach (Minigame minigame in selectedEcs.Values)
        {
            minigames.Add(minigame);
        }

        Game game = new Game(SceneLoader.Scene.TowerDefenseNew, minigames);
        GameManager.Instance.Initialize(game);

        GameObject.Find("RelayHandlerTeacher").GetComponent<RelayHandlerTeacher>().createRelay((succes) =>
        {
            if (succes == true)
            {
                //TODO load scene here, (now not possible becasue of network spawner error)
            }
        });
        SceneLoader.Load(SceneLoader.Scene.Lobby);
        
    }
}

}
