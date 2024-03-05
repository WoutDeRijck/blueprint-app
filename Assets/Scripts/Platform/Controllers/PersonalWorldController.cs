using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Misc;

using Platform.Interfacing.Systems;

namespace Platform.Controllers
{

public class PersonalWorldController : Singleton<PersonalWorldController>
{
    public void Submit(string code)
    {
        //GeneralInfo gi = GeneralInfo.GetInstance();
        //gi.setEnterLobby(code);
        GameObject.Find("RelayHandler").GetComponent<RelayHandler>().joinRelay(code, (succes) =>
        {
            if (succes == true)
            {
                SceneLoader.Load(SceneLoader.Scene.Lobby);
            }
        }); 
    }

    public void Logout()
    {
        AuthenticationSystem.Instance.Logout(() => { SceneLoader.Load(SceneLoader.Scene.LoginScene); });
    }

}

}
