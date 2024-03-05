using Misc;
using GameData;
using System;
using System.Linq;
using System.Collections.Generic;

using Platform.Interfacing.Systems;

namespace Platform.Controllers
{

public class PublicLibraryController : Singleton<PublicLibraryController>
{
    private Subject subject;
    private Subtype subtype;

    public Action libraryRefreshEvent;

    private Dictionary<uint,Minigame> libraryMinigames = new Dictionary<uint, Minigame>();

    public Dictionary<uint, Minigame> selectedMinigames { get; private set; } = new Dictionary<uint, Minigame>();

    public void GetPublicMinigames()
    {
        DataSystem.Instance.GetPublicMinigames((minigames) =>
        {
            libraryMinigames = minigames.Select((s, index) => new { s, index }).ToDictionary(x => (uint) x.index, x => x.s);
            FilterMinigames();
            libraryRefreshEvent?.Invoke();
        });
    }

    public void setSubject(Enum newSubject)
    {
        subject = (Subject) newSubject;
        FilterMinigames();

        libraryRefreshEvent?.Invoke();
    }

    public void setSubtype(Enum newSubtype)
    {
        subtype = (Subtype) newSubtype;
        FilterMinigames();

        libraryRefreshEvent?.Invoke();
    }

    private void FilterMinigames()
    {
        selectedMinigames = libraryMinigames.Where(element => (element.Value.subtype == subtype && element.Value.subject == subject)).ToDictionary(t => t.Key, t => t.Value);
    }

    public void AddMinigame(uint dataIndex)
    {
        ClassroomDashboardController.Instance.AddMinigame(libraryMinigames[dataIndex]);
    }
}

}
