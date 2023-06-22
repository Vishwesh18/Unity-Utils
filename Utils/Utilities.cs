using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEditor;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;

public class Utilities : Singleton<Utilities>
{
    public float screenHeight;
    public float screenWidth;
    public RectTransform canvas;

    protected override void Awake()
    {
        base.Awake();
        Application.targetFrameRate = 60;
        Input.multiTouchEnabled = false;
        screenWidth = Screen.width;
        screenHeight = Screen.height;
    }

    #region GENERAL MATH LOGICS

    public static float GetAngle(Vector2 p1, Vector2 p2)
    {
        float angle = Mathf.Atan2(p2.y - p1.y, p2.x - p1.x) * 180 / Mathf.PI;
        return angle;
    }

    public static float RoundToNearestAngle(float angle, float snapAngle = 45f)
    {
        float snappedAngle = Mathf.Round(angle / snapAngle) * snapAngle;

        return snappedAngle;
    }

    public static int RoundToNearestNumber(int number, int roundTo = 10)
    {
        int x = number / roundTo;
        int y = ((number / roundTo) - x) * roundTo;
        if (roundTo - y < 5)
        {
            number += (roundTo - y);
        }
        else
        {
            number -= y;
        }

        return number;
    }

    public static float XisWhatPercentageOfY(float x, float y)
    {
        return (x / y) * 100;
    }

    public static float Round(float value, int digits)
    {
        float mult = Mathf.Pow(10.0f, (float)digits);
        return Mathf.Round(value * mult) / mult;
    }

    #endregion

    #region UPDATE LAYOUT SIZE

    public void UpdateContentLayoutSize(RectTransform contentHolder, bool isVertical = true)
    {
        //return;
        //Canvas.ForceUpdateCanvases();
        LayoutRebuilder.ForceRebuildLayoutImmediate(contentHolder);
        LayoutRebuilder.ForceRebuildLayoutImmediate(contentHolder);
        //LayoutRebuilder.MarkLayoutForRebuild(contentHolder);

        if (isVertical)
        {
            contentHolder.GetComponent<VerticalLayoutGroup>().enabled = false;
            contentHolder.GetComponent<VerticalLayoutGroup>().enabled = true;
        }
        else
        {
            contentHolder.GetComponent<HorizontalLayoutGroup>().enabled = false;
            contentHolder.GetComponent<HorizontalLayoutGroup>().enabled = true;
        }
    }

    #endregion

    #region RANDOMIZE/GETRANDOM 

    public static void RandomizeArray<T>(T[] items)
    {
        for (int i = 0; i < items.Length; i++)
        {
            T temp = items[i];
            int j = UnityEngine.Random.Range(0, items.Length);
            items[i] = items[j];
            items[j] = temp;
        }
    }

    public static void RandomizeList<T>(List<T> items)
    {
        for (int i = 0; i < items.Count; i++)
        {
            T temp = items[i];
            int j = UnityEngine.Random.Range(0, items.Count);
            items[i] = items[j];
            items[j] = temp;
        }
    }


    public static char GetRandomLetter()
    {
        int num = UnityEngine.Random.Range(0, 26); // Zero to 25
        char let = (char)('a' + num);
        return let;
    }

    #endregion

    #region COLOR

    public static Color HexToColor(string hex, float alpha = 1)
    {
        byte r = byte.Parse(hex.Substring(0, 2), System.Globalization.NumberStyles.HexNumber);
        byte g = byte.Parse(hex.Substring(2, 2), System.Globalization.NumberStyles.HexNumber);
        byte b = byte.Parse(hex.Substring(4, 2), System.Globalization.NumberStyles.HexNumber);

        Color color = new Color(r / 255f, g / 255f, b / 255f);
        color.a = alpha;

        return color;
    }


    public static Color GetRandomColor()
    {
        Color rand = new Color(
          UnityEngine.Random.Range(0f, 1f),
          UnityEngine.Random.Range(0f, 1f),
          UnityEngine.Random.Range(0f, 1f)
        );

        return rand;
    }

    #endregion

    public static void CopyTextToClipboard(string text)
    {
        WebGLClipboard.SetClipboardText(text);
        PopupNotificationUIHandler.Instance.ShowSuccessToast("Copied to Clipboard");
    }

    public static string GetListAsCSVString(List<string> stringList, int startIndex = 0)
    {
        StringBuilder alternatives = new StringBuilder();

        for (int i = startIndex; i < stringList.Count; i++)
        {
            if (i > startIndex + 1)
            {
                alternatives.Append(", ");
            }
            alternatives.Append(stringList[i]);
        }

        return alternatives.ToString();
    }

    #region IMAGE/BYTES/BASE64 CONVERSION

    public static void SetImageBasedOnRatio(Texture2D texture, Image image)
    {
        Sprite sprite = Sprite.Create(texture, new Rect(0, 0, texture.width, texture.height), Vector2.one / 2, 100, 0, SpriteMeshType.FullRect);

        float widthToHeightRatio = (float)texture.width / (float)texture.height;

        image.GetComponent<AspectRatioFitter>().aspectRatio = widthToHeightRatio;

        image.sprite = sprite;
    }

    public static void SetImageBasedOnRatio(Texture2D texture, RawImage image)
    {
        float widthToHeightRatio = (float)texture.width / (float)texture.height;

        image.GetComponent<AspectRatioFitter>().aspectRatio = widthToHeightRatio;

        image.texture = texture;
    }

    public static string ByteArrayToString(byte[] ba)
    {
        return BitConverter.ToString(ba).Replace("-", "");
    }


    public static string Texture2DToString(Texture2D tex)
    {
        byte[] bytes = tex.EncodeToPNG();

        string enc = Convert.ToBase64String(bytes);

        return enc;
    }

    public static byte[] StringToByteArray(String base64String)
    {
        byte[] bytes = Convert.FromBase64String(base64String);

        return bytes;
    }

    #endregion

    #region IMAGE DOWNLOAD

    public void DownloadImageFromURL(string url, Action<float> progressCallback, Action<Texture2D> downloadCallback)
    {
        Debug.Log("Download URL Called : " + url);
        StartCoroutine(DownloadCoroutine(url, progressCallback, downloadCallback));
    }

    public void DownloadImageFromURL(string url, Action<Texture2D> downloadCallback)
    {
        Debug.Log("Download URL Called : " + url);
        StartCoroutine(DownloadCoroutine(url, null, downloadCallback));
    }

    private IEnumerator DownloadCoroutine(string url, Action<float> progressCallback, Action<Texture2D> downloadCallback)
    {
        UnityWebRequest www = UnityWebRequestTexture.GetTexture(url);

        //Attach DownloadHandler to track download progress.
        DownloadHandlerTexture dH = new DownloadHandlerTexture();
        www.downloadHandler = dH;

        //Send Request and wait
        //yield return www.SendWebRequest();
        www.SendWebRequest();

        while (!www.isDone)
        {
            //Debug.Log(www.downloadProgress * 100);
            progressCallback?.Invoke(www.downloadProgress);
            yield return null;
        }

        if (www.result == UnityWebRequest.Result.ProtocolError || www.result == UnityWebRequest.Result.ConnectionError)
        {
            Debug.Log("Error while Receiving: " + www.error);
        }
        else
        {
            Debug.Log("Success");
            //Load Image
            Texture2D texture = DownloadHandlerTexture.GetContent(www);
            downloadCallback.Invoke(texture);
        }
    }

    #endregion
}