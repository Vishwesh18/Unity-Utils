using System;
using System.Runtime.InteropServices;

#if UNITY_WEBGL && !UNITY_EDITOR

#else
using Firebase;
using Firebase.Storage;
using UnityEngine;
using System.Threading.Tasks;
#endif

public class FirebaseStorageWrapper

{
#if UNITY_WEBGL && !UNITY_EDITOR

    [DllImport("__Internal")]
    public static extern void UploadFile(string path, string data, Action<string> callback);

    [DllImport("__Internal")]
    public static extern void DownloadFile(string path, Action<string> callback);

    [DllImport("__Internal")]
    public static extern void DownloadFileOld(string path, Action<string> callback);


#else

    private static StorageReference _storageRef = FirebaseStorage.DefaultInstance.RootReference;

    public static void UploadFile(string path, string data, Action<string> callback)
    {
        Debug.Log("Firebase Upload Called");
        byte[] bytes = Utilities.StringToByteArray(data);

        StorageReference pathRef = _storageRef.Child(path);

        var metaData = new MetadataChange();
        metaData.ContentType = "image/jpg";

        var taskScheduler = TaskScheduler.FromCurrentSynchronizationContext();
        pathRef.PutBytesAsync(bytes, metaData).ContinueWith((Task<StorageMetadata> task) =>
         {
             if (task.IsFaulted || task.IsCanceled)
             {
                 Debug.Log(task.Exception.ToString());
                 // Uh-oh, an error occurred!
             }
             else
             {
                 // Metadata contains file metadata such as size, content-type, and md5hash.
                 StorageMetadata metadata = task.Result;
                 string md5Hash = metadata.Md5Hash;
                 Debug.Log("Finished uploading...");
                 Debug.Log("md5 hash = " + md5Hash);
                 GetDownloadURL(path, callback);
             }
         }, taskScheduler);
    }

    public static void GetDownloadURL(string path, Action<string> callback)
    {
        StorageReference pathRef = _storageRef.Child(path);

        var taskScheduler = TaskScheduler.FromCurrentSynchronizationContext();
        pathRef.GetDownloadUrlAsync().ContinueWith(task =>
        {
            if (!task.IsFaulted && !task.IsCanceled)
            {
                Debug.Log("Download URL: " + task.Result);
                callback.Invoke(task.Result.ToString());
                // ... now download the file via WWW or UnityWebRequest.
            }
            else
            {
                Debug.Log("Download FAILED!");
            }
        }, taskScheduler);
    }

    public static void DownloadFile(string path, Action<string> callback)
    {
        StorageReference pathRef = _storageRef.Child(path);

        const long maxAllowedSize = 1 * 1024 * 1024;

        var taskScheduler = TaskScheduler.FromCurrentSynchronizationContext();
        //pathRef.GetBytesAsync(maxAllowedSize).ContinueWith(task =>
        //{
        //    if (task.IsFaulted || task.IsCanceled)
        //    {
        //        Debug.LogException(task.Exception);
        //        // Uh-oh, an error occurred!
        //    }
        //    else
        //    {
        //        byte[] fileContents = task.Result;
        //        Debug.Log("Finished downloading!");
        //        string data = Utilities.ByteArrayToString(fileContents);

        //        //FirebaseTestScript.Instance.DownloadSet(fileContents);
        //        callback.Invoke(data);
        //    }
        //}, taskScheduler);

        pathRef.GetDownloadUrlAsync().ContinueWith(task =>
        {
            if (!task.IsFaulted && !task.IsCanceled)
            {
                Debug.Log("Download URL: " + task.Result);
                //callback.Invoke(task.Result.ToString());
                callback.Invoke(task.Result.ToString());
                // ... now download the file via WWW or UnityWebRequest.
            }
            else
            {
                Debug.Log("Download FAILED!");
            }
        }, taskScheduler);
    }

    public static void DownloadFileOld(string path, Action<string> callback)
    {

    }

#endif
}
