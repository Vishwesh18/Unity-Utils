using System;
using System.Collections;
using System.Collections.Generic;
using Firebase.Functions;
using UnityEngine;

public class FirebaseCloudFunctionWrapper
{
    public static void FirebaseCallable(string funtionName, Action<string> __callback, Action<Exception> __fallBack = null)
    {

        FirebaseFunctions functions = FirebaseFunctions.DefaultInstance;
        HttpsCallableReference reference = functions.GetHttpsCallable(funtionName);
        try
        {
            reference.CallAsync().ContinueWith((response) =>
            {

                __callback?.Invoke(JsonUtility.ToJson(response.Result.Data));

            });
        }
        catch (Exception err)
        {
            __fallBack?.Invoke(err);
        }

    }


    public static void FirebaseHttpsCallable(string funtionName, string data, Action<string> __callback, Action<Exception> __fallBack = null)
    {
        Debug.Log(data);
        FirebaseFunctions functions = FirebaseFunctions.DefaultInstance;
        HttpsCallableReference reference = functions.GetHttpsCallable(funtionName);
        try
        {
            reference.CallAsync(data).ContinueWith((response) =>
            {
                //Debug.Log(response);
                //Debug.Log(response.Result);
                Debug.Log(response.Result.Data.ToString());
                //Debug.Log(response.Result.Data);

                __callback?.Invoke(response.Result.Data.ToString());

            });
        }
        catch (Exception err)
        {
            __fallBack?.Invoke(err);
        }

    }

    public static void FirebaseApiCall(string __url, Action<string> __callback, Action<Exception> __fallBack = null)
    {

        FirebaseFunctions functions = FirebaseFunctions.DefaultInstance;

        HttpsCallableReference reference = functions.GetHttpsCallableFromURL(new Uri(__url));
        try
        {
            reference.CallAsync().ContinueWith((response) =>
            {

                __callback?.Invoke(response.Result.Data.ToString());

            });
        }
        catch (Exception err)
        {
            __fallBack?.Invoke(err);
        }
    }

}