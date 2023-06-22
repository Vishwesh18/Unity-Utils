using System;
using System.Collections.Generic;
using Firebase.Auth;
using Firebase.Database;
using Leguar.TotalJSON;
using UnityEngine;

public class FirebaseManager : Singleton<FirebaseManager>
{
    public void ExamplePostMethod(Action<string> callback)
    {
        string path = CreatePath(FirebasePathKeys.exampleKey);
        FirebaseDatabaseWrapper.PostValue<string>(path, (resultString) =>
        {
            callback?.Invoke(resultString);
        });
    }

    #region UTILITIES METHODS

    // private string CreatePathWithGameID(params string[] __nodes)
    // {
    //     var finalString = FirebasePathKeys.activeGamesKey + "/" + DataHandler.GetGameCode() + "/";
    //     //var finalString = FirebasePathKeys.activeGamesKey + "/" + _gameCode + "/";
    //     foreach (var item in __nodes)
    //     {
    //         finalString += item + "/";
    //     }
    //     return finalString;
    // }

    private string CreatePath(params string[] __nodes)
    {
        var finalString = "";
        foreach (var item in __nodes)
        {
            finalString += item + "/";
        }
        return finalString;
    }

    #endregion

}