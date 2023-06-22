using UnityEngine;
using System.Collections;

/*This script is used to display the fps*/
public class FpsShow : MonoBehaviour
{
    float deltaTime = 0.0f;
    public Color textColor;
    public TextAnchor textAnchor;

    public float offsetX;
    public float offsetY;

    void Update()
    {
        deltaTime += (Time.deltaTime - deltaTime) * 0.1f;
    }

    void OnGUI()
    {
        int w = Screen.width, h = Screen.height;

        GUIStyle style = new GUIStyle();

        //Rect rect = new Rect(0, 0, w, h * 2 / 100);
        Rect rect = new Rect(0 + offsetX, 0 + offsetY, w, h);
        style.alignment = textAnchor;
        style.fontSize = h * 2 / 100;
        style.normal.textColor = textColor;
        float msec = deltaTime * 1000.0f;
        float fps = 1.0f / deltaTime;
        string text = string.Format("{0:0.0} ms ({1:0.} fps)", msec, fps);
        GUI.Label(rect, text, style);
    }
}
