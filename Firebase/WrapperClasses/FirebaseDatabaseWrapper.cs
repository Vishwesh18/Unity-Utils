using System;
using System.Runtime.InteropServices;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Firebase.Extensions;

#if UNITY_WEBGL && !UNITY_EDITOR

#else
using Firebase;
using Firebase.Database;
using System.Threading.Tasks;
#endif


public class FirebaseDatabaseWrapper
{


#if UNITY_WEBGL && !UNITY_EDITOR

    [DllImport("__Internal")]
    public static extern void GetJSON(string path, Action<string> callback);

    [DllImport("__Internal")]
    public static extern void GetDataSnapshot(string path, Action<string> callback, Action emptyFallback);

    [DllImport("__Internal")]
    public static extern void PostJSON(string path, string value, Action<string> callback);

    [DllImport("__Internal")]
    public static extern void PushJSON(string path, string value, Action<string> callback);

    [DllImport("__Internal")]
    public static extern void UpdateJSON(string path, string value, Action<string> callback);

     [DllImport("__Internal")]
    public static extern void DeleteJSON(string path, Action<string> callback);

#else

    private static DatabaseReference _databaseRef = FirebaseDatabase.DefaultInstance.RootReference;

    private static Dictionary<string, ListenOnValueChangeData> ListenOnValueChangeDict = new Dictionary<string, ListenOnValueChangeData>();
    private static Dictionary<string, ListenOnChildChangeData> ListenOnChildAddedDict = new Dictionary<string, ListenOnChildChangeData>();
    private static Dictionary<string, ListenOnValueChangeWithContraintsData> ListenOnValueChangeWithContraintsDict = new Dictionary<string, ListenOnValueChangeWithContraintsData>();





    public static void GetJSON(string path, Action<string> callback)
    {
        Debug.Log("Path : " + path);

        //var taskScheduler = TaskScheduler.FromCurrentSynchronizationContext();
        _databaseRef.Child(path).GetValueAsync().ContinueWithOnMainThread(task =>
       {
           Debug.Log("Getting Async : ");

           if (task.IsFaulted)
           {
               // Handle the error...
               Debug.Log("Network Issue : ");

               callback.Invoke("Network Issue");
           }
           else if (task.IsCompleted)
           {
               DataSnapshot snapshot = task.Result;
               // Do something with snapshot...

               Debug.Log("Snapshot : " + snapshot.GetRawJsonValue());

               if (snapshot == null)
               {
                   callback.Invoke(null);
                   return;
               }

               callback.Invoke(snapshot.GetRawJsonValue());
           }
       });
    }

    public static void GetValue<T>(string path, Action<T> callback)
    {
        //Debug.Log("Path : " + path);

        var taskScheduler = TaskScheduler.FromCurrentSynchronizationContext();
        _databaseRef.Child(path).GetValueAsync().ContinueWith(task =>
        {
            Debug.Log("Getting Async : ");

            if (task.IsFaulted)
            {
                // Handle the error...
                Debug.Log("Network Issue : ");

                //callback.Invoke("Network Issue");
                callback.Invoke((T)(object)null);
                if (typeof(T) == typeof(string))
                    callback?.Invoke((T)(object)"");
                if (typeof(T) == typeof(int))
                    callback?.Invoke((T)(object)-1);

            }
            else if (task.IsCompleted)
            {
                DataSnapshot snapshot = task.Result;
                // Do something with snapshot...

                Debug.Log("Snapshot : " + snapshot.GetRawJsonValue());

                if (snapshot == null)
                {
                    callback.Invoke((T)(object)null);
                    return;
                }

                callback.Invoke((T)snapshot.GetValue(false));
            }
        }, taskScheduler);
    }

    public static void GetDataSnapshot(string path, Action<string> callback, Action emptyFallback = null)
    {
        var taskScheduler = TaskScheduler.FromCurrentSynchronizationContext();
        _databaseRef.Child(path).GetValueAsync().ContinueWith(task =>
        {
            if (task.IsFaulted)
            {
                // Handle the error...
                callback.Invoke("Network Issue");
            }
            else if (task.IsCompleted)
            {
                DataSnapshot snapshot = task.Result;
                // Do something with snapshot...

                if (snapshot == null)
                {
                    callback.Invoke("No Data Found");
                    return;
                }

                if (snapshot.ChildrenCount == 0)
                {
                    emptyFallback.Invoke();
                    return;
                }

                IEnumerable<DataSnapshot> games = snapshot.Children;

                foreach (DataSnapshot dataSnapshot in games)
                {
                    callback.Invoke(dataSnapshot.GetRawJsonValue());
                }
            }
        }, taskScheduler);
    }

    public static void GetDataSnapshot(string path, Action<DataSnapshot> callback, Action emptyFallback = null)
    {
        var taskScheduler = TaskScheduler.FromCurrentSynchronizationContext();
        _databaseRef.Child(path).GetValueAsync().ContinueWith(task =>
        {
            if (task.IsFaulted)
            {
                // Handle the error...
                callback.Invoke(null);
            }
            else if (task.IsCompleted)
            {
                DataSnapshot snapshot = task.Result;
                // Do something with snapshot...

                if (snapshot == null)
                {
                    callback.Invoke(null);
                    return;
                }

                if (snapshot.ChildrenCount == 0)
                {
                    callback.Invoke(null);
                    return;
                }

                callback.Invoke(snapshot);
            }
        }, taskScheduler);
    }


    public static void GetDataSnapshotWithConstraints(string path, string orderBy, int limit, Action<DataSnapshot> callback)
    {
        var taskScheduler = TaskScheduler.FromCurrentSynchronizationContext();
        _databaseRef.Child(path).OrderByChild(orderBy).LimitToLast(limit).GetValueAsync().ContinueWith(task =>
        {
            if (task.IsFaulted)
            {
                // Handle the error...
                callback.Invoke(null);
            }
            else if (task.IsCompleted)
            {
                DataSnapshot snapshot = task.Result;
                // Do something with snapshot...

                if (snapshot == null)
                {
                    callback.Invoke(null);
                    return;
                }

                if (snapshot.ChildrenCount == 0)
                {
                    callback.Invoke(snapshot);
                    return;
                }

                callback.Invoke(snapshot);
            }
        }, taskScheduler);
    }


    public static void PostJSON(string path, string json, Action<string> callback)
    {
        //var taskScheduler = TaskScheduler.FromCurrentSynchronizationContext();
        DateTime date = DateTime.Now;
        FirebaseDatabase.DefaultInstance.GetReference(path).SetRawJsonValueAsync(json).ContinueWithOnMainThread((Task task) =>
        {

            Debug.Log("Post");
            //Debug.Log(DateTime.Now.Subtract(date).Milliseconds);
            if (task.IsFaulted)
            {
                callback?.Invoke(null);
            }
            else if (task.IsCompleted)
            {
                string successString = "Success: " + json + " was posted to " + path;
                callback?.Invoke(successString);
            }
        });
    }

    public static void PostValue<T>(string path, T value, Action<string> callback)
    {

        var taskScheduler = TaskScheduler.FromCurrentSynchronizationContext();
        FirebaseDatabase.DefaultInstance.GetReference(path).SetValueAsync(value).ContinueWith((Task task) =>
        {
            if (task.IsFaulted)
            {
                callback?.Invoke(null);
            }
            else if (task.IsCompleted)
            {
                string successString = "Success: " + value + " was posted to " + path;
                Debug.Log(successString);
                callback?.Invoke(successString);
            }
        }, taskScheduler);
    }

    public static void UpdateJSON(string path, Dictionary<string,object> value, Action<string> callback)
    {
        var taskScheduler = TaskScheduler.FromCurrentSynchronizationContext();
        FirebaseDatabase.DefaultInstance.GetReference(path).UpdateChildrenAsync(value).ContinueWith((Task task) =>
        {
            if (task.IsFaulted)
            {
                callback?.Invoke(null);
            }
            else if (task.IsCompleted)
            {
                string successString = "Success: " + value + " was UPDATED to " + path;
                callback?.Invoke(successString);
            }
        }, taskScheduler);
    }

    public static void DeleteJSON(string path, Action<string> callback)
    {
        //var taskScheduler = TaskScheduler.FromCurrentSynchronizationContext();
        _databaseRef.Child(path).RemoveValueAsync().ContinueWithOnMainThread(task =>
            {
                if (task.IsFaulted)
                {
                    // Handle the error...
                    Debug.LogError("Error : " + task.ToString());
                    callback.Invoke(null);
                }
                else if (task.IsCompleted)
                {
                    callback.Invoke("Successfully Deleted!");
                    return;
                }
            });
    }

    public static void PushJSON(string path, string json, Action<string> callback, Action<string> fallback)
    {
        DateTime date = DateTime.Now;
        var taskScheduler = TaskScheduler.FromCurrentSynchronizationContext();
        DatabaseReference pushref = FirebaseDatabase.DefaultInstance.GetReference(path).Push();
        pushref.SetRawJsonValueAsync(json).ContinueWith((Task task) =>
        {
            Debug.Log("Push");
            Debug.Log(DateTime.Now.Subtract(date).Milliseconds);
            if (task.IsFaulted)
            {
                fallback?.Invoke("Somthing went wrong in push JSON");
            }
            else if (task.IsCompleted)
            {

                callback?.Invoke(pushref.Key);
            }
        }, taskScheduler);
    }


    public static void PerformTransaction(string __path, int __addingValue, Action<int> __callBack, Action<string> __fallBack)
    {
        try
        {

            // Get a reference to the data to be updated
            DatabaseReference scoreRef = _databaseRef.Child(__path);

            // Start the transaction
            scoreRef.RunTransaction(mutableData =>
            {
                if (mutableData.Value != null && mutableData.Value is long currentValue)
                {
                    // Add the __addingValue to the current value
                    mutableData.Value = currentValue + __addingValue;
                }
                else
                {
                    mutableData.Value = __addingValue;
                }

                // Return the updated value
                return TransactionResult.Success(mutableData);

            }).ContinueWithOnMainThread(task =>
            {
                if (task.Exception != null)
                {
                    __fallBack?.Invoke(task.Exception.Message);
                }
                else if (task.IsCompleted)
                {
                    if (task.Result.Exists)
                    {
                        // Get the updated value from the transaction result
                        int updatedValue = (int)(long)task.Result.Value;
                        __callBack?.Invoke(updatedValue);
                        Debug.Log("Transaction completed successfully!");
                    }
                    else
                    {
                        // Data doesn't exist at the specified path
                        __fallBack?.Invoke("Data doesn't exist at the specified path.");
                    }
                }
            });
        }
        catch (Exception ex)
        {
            __fallBack?.Invoke(ex.Message);
        }
    }


    #region LISTEN/UNLISTEN METHODS

    public static void AddListenerForValueChanged(string path, Action<object, ValueChangedEventArgs> callback)
    {
        if (ListenOnValueChangeDict.ContainsKey(path))
        {
            ListenOnValueChangeData data = ListenOnValueChangeDict[path];
            data.pathRef.ValueChanged -= data.callback.Invoke;
            ListenOnValueChangeDict.Remove(path);
            Debug.LogWarning("Lister already exit for same path");
        }

        DatabaseReference pathRef = FirebaseDatabase.DefaultInstance.GetReference(path);

        pathRef.ValueChanged += callback.Invoke;

        ListenOnValueChangeDict.Add(path, new ListenOnValueChangeData(pathRef, callback));

    }
    public static void RemoveListenerForValueChanged(string path)
    {
        if (!ListenOnValueChangeDict.ContainsKey(path))
        {
            Debug.LogWarning("Lister not avalible!");
            return;
        }
        ListenOnValueChangeData data = ListenOnValueChangeDict[path];
        data.pathRef.ValueChanged -= data.callback.Invoke;
        ListenOnValueChangeDict.Remove(path);
    }

    public static void AddListenerForValueChangedWithConstraints(string path, string orderBy, int limit, Action<object, ValueChangedEventArgs> callback)
    {

        if (ListenOnValueChangeWithContraintsDict.ContainsKey(path))
        {
            ListenOnValueChangeWithContraintsData data = ListenOnValueChangeWithContraintsDict[path];
            data.query.ValueChanged -= data.callback.Invoke;
            ListenOnValueChangeWithContraintsDict.Remove(path);
            Debug.LogWarning("Lister already exit for same path");
        }

        Query query = FirebaseDatabase.DefaultInstance.GetReference(path).OrderByChild(orderBy).LimitToLast(limit);

        query.ValueChanged += callback.Invoke;

        ListenOnValueChangeWithContraintsDict.Add(path, new ListenOnValueChangeWithContraintsData(query, callback));

    }
    public static void RemoveListenerForContraints(string path)
    {
        if (!ListenOnValueChangeWithContraintsDict.ContainsKey(path))
        {
            Debug.LogWarning("Lister not avalible!");
            return;
        }

        ListenOnValueChangeWithContraintsDict[path].query.ValueChanged -= ListenOnValueChangeWithContraintsDict[path].callback.Invoke;
    }




    public static void AddListenerForChildAdded(string path, Action<object, ChildChangedEventArgs> callback)
    {
        if (ListenOnChildAddedDict.ContainsKey(path))
        {
            ListenOnChildChangeData data = ListenOnChildAddedDict[path];
            data.pathRef.ChildAdded -= data.callback.Invoke;
            ListenOnChildAddedDict.Remove(path);
            Debug.LogWarning("Lister already exit for same path");
        }

        DatabaseReference pathRef = FirebaseDatabase.DefaultInstance.GetReference(path);

        //pathRef.ChildAdded -= callback.Invoke;
        pathRef.ChildAdded += callback.Invoke;

        ListenOnChildAddedDict.Add(path, new ListenOnChildChangeData(pathRef, callback));
    }

    public static void RemoveListenerForChildAdded(string path)
    {

        if (!ListenOnChildAddedDict.ContainsKey(path))
        {
            Debug.LogWarning("Lister not avalible!");
            return;
        }

        ListenOnChildChangeData data = ListenOnChildAddedDict[path];
        data.pathRef.ChildAdded -= data.callback.Invoke;
        ListenOnChildAddedDict.Remove(path);

    }





    #endregion

    #region OLD METHODS

    //public static void AddListenerForChildAdded(string path, Action<object, ChildChangedEventArgs> callback)
    //{
    //    //DatabaseReference currentRef = FirebaseDatabase.DefaultInstance
    //    //              .GetReference(path);
    //    //currentRef.ChildAdded += callback.Invoke;
    //    _leaderboardRef = FirebaseDatabase.DefaultInstance.GetReference(path);
    //    _leaderboardRef.ChildAdded -= callback.Invoke;
    //    _leaderboardRef.ChildAdded += callback.Invoke;

    //    listenCallback = callback;

    //    //  FirebaseDatabase.DefaultInstance
    //    //.GetReference(path).ChildAdded += callback.Invoke;
    //}

    //public static void RemoveListenerForChildAdded(string path, Action<object, ChildChangedEventArgs> callback)
    //{
    //    //DatabaseReference currentRef = FirebaseDatabase.DefaultInstance
    //    //              .GetReference(path);
    //    //currentRef.ChildAdded -= callback.Invoke;

    //    //Debug.Log(_leaderboardRef.)
    //    if (_leaderboardRef != null && listenCallback != null)
    //    {
    //        _leaderboardRef.ChildAdded -= listenCallback.Invoke;
    //        //FirebaseDatabase.DefaultInstance.GetReference(path).ChildAdded -= listenCallback.Invoke;
    //        Debug.Log("Unlisten INVOKED");
    //    }

    //    //FirebaseDatabase.DefaultInstance
    //    //      .GetReference(path).ChildAdded -= callback.Invoke;
    //    //;
    //}
    #endregion
#endif
}


public struct ListenOnValueChangeData
{
    public DatabaseReference pathRef;
    public Action<object, ValueChangedEventArgs> callback;

    public ListenOnValueChangeData(DatabaseReference __pathRef, Action<object, ValueChangedEventArgs> __callback)
    {
        pathRef = __pathRef;
        callback = __callback;
    }
}
public struct ListenOnChildChangeData
{
    public DatabaseReference pathRef;
    public Action<object, ChildChangedEventArgs> callback;

    public ListenOnChildChangeData(DatabaseReference __pathRef, Action<object, ChildChangedEventArgs> __callback)
    {
        pathRef = __pathRef;
        callback = __callback;
    }
}
public struct ListenOnValueChangeWithContraintsData
{
    public Query query;
    public Action<object, ValueChangedEventArgs> callback;

    public ListenOnValueChangeWithContraintsData(Query __pathRef, Action<object, ValueChangedEventArgs> __callback)
    {
        query = __pathRef;
        callback = __callback;
    }
}