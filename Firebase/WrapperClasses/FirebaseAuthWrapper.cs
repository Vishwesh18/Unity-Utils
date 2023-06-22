using System;
using System.Runtime.InteropServices;
using UnityEngine;

#if UNITY_WEBGL && !UNITY_EDITOR

#else
using Firebase;
using Firebase.Auth;
#endif

public static class FirebaseAuthWrapper
{
#if UNITY_WEBGL && !UNITY_EDITOR

    [DllImport("__Internal")]
    public static extern void SignInAnonymously();

    [DllImport("__Internal")]
    public static extern void OnAuthStateChanged(Action<string> onUserSignedIn, Action<string> onUserSignedOut);

#else

    private static FirebaseAuth auth;
    private static FirebaseUser user;

    public static void CheckDependencies(Action<string> __callback, Action<Exception> __fallback = null)
    {
        try
        {
            FirebaseApp.CheckAndFixDependenciesAsync().ContinueWith(task =>
            {
                DependencyStatus dependencyStatus = task.Result;
                if (dependencyStatus == Firebase.DependencyStatus.Available)
                {
                    __callback?.Invoke("Success");
                }
                else
                {

                    __fallback?.Invoke(new Exception(System.String.Format("Could not resolve all Firebase dependencies: {0}", dependencyStatus)));

                }
            });
        }
        catch (Exception ex)
        {
            __fallback?.Invoke(ex);
        }

    }


    public static void SignInAnonymously(Action<FirebaseUser> __callback, Action<Exception> __fallback = null)
    {
        try
        {
            FirebaseAuth.DefaultInstance.SignInAnonymouslyAsync().ContinueWith(task =>
            {
                if (task.IsCanceled)
                {
                    __fallback?.Invoke(new Exception("SignInAnonymouslyAsync was canceled."));
                    //Debug.LogError("SignInAnonymouslyAsync was canceled.");
                    return;
                }
                if (task.IsFaulted)
                {
                    __fallback?.Invoke(task.Exception);
                    //Debug.LogError("SignInAnonymouslyAsync encountered an error: " + task.Exception);
                    return;
                }

                //FirebaseUser newUser =   task.Result;
                //Debug.Log(task.Result);

                FirebaseUser newUser = task.Result.User;
                Debug.Log(task.Result.User);
                Debug.LogFormat("User signed in successfully: {0} ({1})",
                    newUser.DisplayName, newUser.UserId);
                __callback?.Invoke(newUser);
            });
        }
        catch (Exception ex)
        {
            __fallback?.Invoke(ex);
        }

    }

    public static void OnAuthStateChanged(Action<string> onUserSignedIn, Action<string> onUserSignedOut)
    {
        InitializeFirebaseAuth();
    }

    public static void LogOutFirebase()
    {
        FirebaseAuth.DefaultInstance.SignOut();
    }

    static void InitializeFirebaseAuth()
    {
        auth = Firebase.Auth.FirebaseAuth.DefaultInstance;
        auth.StateChanged += AuthStateChanged;
        //AuthStateChanged(this, null);
    }

    static void AuthStateChanged(object sender, System.EventArgs eventArgs)
    {
        if (auth.CurrentUser != user)
        {
            bool signedIn = user != auth.CurrentUser && auth.CurrentUser != null;
            if (!signedIn && user != null)
            {
                Debug.Log("Signed out " + user.UserId);
            }
            user = auth.CurrentUser;
            if (signedIn)
            {
                Debug.Log("Signed in " + user.UserId + " : " + user.DisplayName);
            }
        }
    }



#endif
}