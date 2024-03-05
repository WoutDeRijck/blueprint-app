using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;
using System.IO;
using System;
using GameData;
using Misc;

using Platform.CustomControls;

namespace Platform.Controllers
{

/// <summary>
/// This class implements functionality to make a new Educational Content Sheet (ECS)
/// </summary>
public class ContentEditor : MonoBehaviour
{
    public Minigame minigame { get; set; }

    private EcsEditorPopup view;

    private bool canBeSaved = false;

    private ChatGPT chatGPT;

    private void Start()
    {
        chatGPT = GetComponent<ChatGPT>();
    }
    /// <summary>
    /// Add ECS functionality to ECS UI
    /// </summary>
    public void SetupECS(EcsEditorPopup view)
    {
        this.view = view;
        if (this.view == null) return;

        view.saveEvent += this.OnSave;
        view.openEvent += this.OnOpen;
        view.gptEvent += this.CallGPT;
    }

    /// <summary>
    /// Connect open-button to open csv-editor
    /// </summary>
    public void OnOpen()
    {
        if (!SetMinigame()) return;
        List<string[]> csv = minigame != null ? minigame.ConvertToCSV() : null;
        try
        {
            CSVManager.Open("VragenSet" + minigame.subtype + ".csv", csv);
            canBeSaved = true;
        }
        catch (IOException ex)
        {
            UnityEngine.Debug.Log("There is an Excel open already! \n" + ex);
        }
    }

    /// <summary>
    /// Connect save-button to reconstruct data from csv
    /// </summary>
    public void OnSave()
    {
        if (!SetMinigame() || !canBeSaved) return;

        List<string[]> csv = CSVManager.ReadCSV("VragenSet" + minigame.subtype + ".csv");
        CSVToMinigame(csv);

        minigame.SetData();
        minigame.shared = view.shared;

        CSVManager.DeleteFile("VragenSet" + minigame.subtype + ".csv");
        canBeSaved = false;

    }

    private async void CallGPT(string prompt)
    {
        if (!SetMinigame()) return;

        view.gptReady.text = "Laden...";
        this.minigame = await chatGPT.GenerateQuestions(this.minigame, prompt);
        view.gptReady.text = "Klaar";
    }

    /// <summary>
    /// Reconstruct Minigame object from data in csv depending on subtype
    /// </summary>
    private void CSVToMinigame(List<string[]> csv)
    {
        if (minigame == null) return;
        bool successful = minigame.ReadFromCSV(csv);

        if (!successful) throw new FormatException("The minigame could not be reconstructed from the CSV");
    }

    /// <summary>
    /// Get the data from the ECS fields and make a new Minigame object
    /// </summary>
    private bool SetMinigame()
    {
        if (view == null) return false;

        Minigame newMinigame = view.GetMinigame();
        if (newMinigame == null) return false;

        if (minigame != null && minigame.subtype == newMinigame.subtype)
        {
            minigame.tag = newMinigame.tag;
            minigame.subject = newMinigame.subject;
            return true;
        }

        minigame = newMinigame;
        if (minigame == null) return false;

        return true;
    }
}

}
