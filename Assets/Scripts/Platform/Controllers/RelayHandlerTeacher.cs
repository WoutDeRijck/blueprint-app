using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Netcode.Transports.UTP;
using Unity.Netcode;
using Unity.Services.Authentication;
using Unity.Services.Core;
using Unity.Services.Relay.Models;
using Unity.Services.Relay;
using UnityEngine;
using GameManagement;

namespace Platform.Controllers
{

public class RelayHandlerTeacher : NetworkBehaviour
{
    public string joinCode;

    public async void Start()
    {
        await UnityServices.InitializeAsync();

        AuthenticationService.Instance.SignedIn += () => {
            Debug.Log("Signed in " + AuthenticationService.Instance.PlayerId);
        };
        await AuthenticationService.Instance.SignInAnonymouslyAsync();
    }

    public async void createRelay(Action<bool> callback)
    {
        bool succes = false;
        try
        {
            Allocation allocation = await RelayService.Instance.CreateAllocationAsync(50);


            joinCode = await RelayService.Instance.GetJoinCodeAsync(allocation.AllocationId);
            GameManager.Instance.joinCode= joinCode;

            Debug.Log(joinCode);

            NetworkManager.Singleton.GetComponent<UnityTransport>().SetHostRelayData(
                allocation.RelayServer.IpV4,
                (ushort)allocation.RelayServer.Port,
                allocation.AllocationIdBytes,
                allocation.Key,
                allocation.ConnectionData
            );

            NetworkManager.Singleton.StartHost();

            succes= true;
        }
        catch (RelayServiceException e)
        {
            Debug.Log(e);
            succes = false;
        }
        finally
        {
            callback(succes);
        }

    }

}

}
