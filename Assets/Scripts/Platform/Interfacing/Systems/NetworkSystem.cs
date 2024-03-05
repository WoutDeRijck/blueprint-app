using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using Misc;

namespace Platform.Interfacing.Systems
{

public class NetworkSystem : PersistentSingleton<NetworkSystem>
{
    /// <summary>
    /// Coroutine that executes a HTTP POST request asynchronously given a route and callbacks for succes and error
    /// </summary>
    /// <param name="route">Route for POST request</param>
    /// <param name="parameters">Parameters for POST request as dictionary</param>
    /// <param name="errorCallback">Callback for error handling</param>
    /// <param name="succesCallback">Callback for when request was succesful</param>
    public IEnumerator PostCoroutine(string route, Dictionary<string, string> parameters, Action<string> errorCallback, Action<string> succesCallback)
    {
        WWWForm form = new WWWForm();


        foreach (KeyValuePair<string,string> field in parameters)
        {
            form.AddField(field.Key, field.Value);
        }
        

        using (UnityWebRequest www = UnityWebRequest.Post("http://193.190.127.174:8000/" + route, form))
        {
            yield return www.SendWebRequest();

            if (www.result != UnityWebRequest.Result.Success)
            {
                errorCallback(www.error);
            }
            else
            {
                succesCallback(www.downloadHandler.text);
            }
        }
    }
}

}
