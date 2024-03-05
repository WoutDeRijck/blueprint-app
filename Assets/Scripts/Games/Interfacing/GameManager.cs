using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;
using GameData;
using Misc;
using AL;
using Lobby;
using System;
using Platform.Interfacing.Systems;

namespace GameManagement
{

/// <summary>
///     The GameManager initializes games and does the bookkeeping of the current game.
/// </summary>
public class GameManager : PersistentSingleton<GameManager>
{   
    /// <summary>The current game.</summary>
    public Game game { set; get; }
    
    /// <summary>Lobby code.</summary>
    public string joinCode;

    // Store the ecs ids that are already played to avoid replaying the same ecs
    private List<int> minigameIDPlayed = new List<int>();
    
    /// <summary>Whether or not, and if so, which, hotspot is currently visible on the player's screen.</summary>
    public int hotspotOpened = -1;

    // <summary>Stores the current minigame object.</summary>
    public Minigame minigame;

    private Status status;

    /// <summary>Keeps track of the player's network object for syncing.</summary>
    public NetworkObject player;
    
    /// <summary>The adaptive learning algorithm instance that is being used.</summary>
    public RuleBasedAL AL;

    /// <summary>The status the player is currently in, Lobby, Game or Minigame.</summary>

    public Action<int, int, string, string, float> OnChangeStatusMinigame;
    private string studentName;

    public enum Status
    {
        inLobby,
        inGame,
        inMinigame
    }
    
    /// <summary>Used to initialize a game, given a Game object.</summary>
    public void Initialize(Game game)
    {
        this.game = game;

        AL = new RuleBasedAL(game.ecsList);

        status = Status.inLobby;
    }
    
    /// <summary>Sets the player field.</summary>
    public bool SetPlayer(NetworkObject player)
    {
        this.player = player; 
        if(player != null) 
        { 
            return true;
        }
        return false;
    }
    
    /// <summary>Set the status field.</summary>
    public void changeStatus(Status newStatus)
    {
        if (newStatus == Status.inGame)
        {
            player.GetComponent<PlayerInputHandler>().Subscribe();
            OnChangeStatusMinigame?.Invoke((int)this.player.OwnerClientId, 0, this.minigame.tag, studentName, 0);
        }
        else if (newStatus == Status.inMinigame)
        {
            player.GetComponent<PlayerInputHandler>().Unsubscribe();
        }
        status = newStatus;
    } 

    /// <summary>
    ///     This function starts a minigame, selected by Adaptive Learning algorithms.
    /// </summary>
    public void StartMinigame()
    {
        changeStatus(Status.inMinigame);

        // use Adaptive learning to get a new minigame to play
        this.minigame = AL.GetNewMinigame();

        //var all = AL.
        float all = AL.getCurrentMinigameScore();

        GameObject.Find("Main Camera").GetComponent<AudioListener>().enabled = false; //disable game camera

        studentName = GameObject.Find("AuthenticationSystem").GetComponent<AuthenticationSystem>().student.name;

        OnChangeStatusMinigame?.Invoke((int)this.player.OwnerClientId, 1, this.minigame.tag, studentName, all);

        // SceneLoader.Scene scene = GetScene(minigame.subtype);
        SceneLoader.Load(GetScene(minigame.subtype), true);
    }
   
    /// <summary>Get the minigame in a specified type of minigame.</summary>
    public T GetMinigame<T>() where T : Minigame
    {
        return (T) minigame;
    }

    /// <summary>
    /// Depending on subtype, return a random minigame scene
    /// </summary>
    public SceneLoader.Scene GetScene(Subtype subtype)
    {
        List<SceneLoader.Scene> sceneList;
        SceneLoader.Scene scene;
        switch (subtype)
        {
            case Subtype.MultipleChoice:
                sceneList = SceneLoader.GetMultipleChoiceScenes();
                scene = sceneList[UnityEngine.Random.Range(0, sceneList.Count)];
                break;
            case Subtype.Grouping:
                sceneList = SceneLoader.GetGroupingScenes();
                scene = sceneList[UnityEngine.Random.Range(0, sceneList.Count)];
                break;
            case Subtype.Ranking:
                sceneList = SceneLoader.GetRankingScenes();
                scene = sceneList[UnityEngine.Random.Range(0, sceneList.Count)];
                break;
            default:
                scene = SceneLoader.Scene.TowerDefenseNew;
                break;
        }
        return scene;
    }
}
}


