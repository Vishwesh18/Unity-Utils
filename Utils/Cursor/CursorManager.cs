using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Jambav.Utilities;

public class CursorManager : Singleton<CursorManager>
{
    //Hotspot (0,0) is at LEFT-TOP
    private static Vector2 cursorHotspotForDefault = new Vector2(12, 4);
    private static Vector2 cursorHotspotForButton = new Vector2(12, 4);
    private static Vector2 cursorHotspotForText = Vector2.zero;

    private Vector2 currentCursorHotspot;

    #region SET METHODS

    public static void SetButtonCursor()
    {
        Cursor.SetCursor(ResourceManager.sharedInstance.buttonCursor, cursorHotspotForButton, CursorMode.Auto);
    }

    public static void SetTextCursor()
    {
        Cursor.SetCursor(ResourceManager.sharedInstance.textCursor, cursorHotspotForText, CursorMode.Auto);
    }

    public static void SetDefaultCursor()
    {
        Cursor.SetCursor(null, cursorHotspotForDefault, CursorMode.Auto);
    }

    #endregion
}
