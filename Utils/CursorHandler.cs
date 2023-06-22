using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;


public class CursorHandler : Singleton<CursorHandler>, IPointerEnterHandler, IPointerExitHandler, IDeselectHandler
{
    [SerializeField] private bool isButton = true;
    [SerializeField] private CursorType cursorType;

    private Vector2 cursorHotspot = new Vector2(6, 2);

    private Button _button;
    private bool _hasButtonComponent = false;

    protected override void Awake()
    {
        base.Awake();
        cursorType = isButton ? CursorType.ButtonCursor : CursorType.TextCursor;
        cursorHotspot = cursorType == CursorType.ButtonCursor ? new Vector2(6, 2) : Vector2.zero;

        //ConfigureEvents();

        _hasButtonComponent = transform.TryGetComponent<Button>(out _button);
    }

    #region SET METHODS

    public void SetButtonCursor()
    {
        Cursor.SetCursor(ResourceManager.Instance.buttonCursor, cursorHotspot, CursorMode.Auto);
    }

    public void SetTextCursor()
    {
        Cursor.SetCursor(ResourceManager.Instance.textCursor, cursorHotspot, CursorMode.Auto);
    }

    public static void SetDefaultCursor()
    {
        Cursor.SetCursor(null, Vector2.zero, CursorMode.Auto);
    }

    #endregion

    #region EVENTS HANDLING

    public void OnPointerEnter(PointerEventData eventData)
    {
        if (cursorType == CursorType.ButtonCursor)
        {
            if (_hasButtonComponent && !_button.interactable) return;

            SetButtonCursor();
        }
        else
        {
            SetTextCursor();
        }
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        SetDefaultCursor();
    }

    public void OnDeselect(BaseEventData eventData)
    {
        SetDefaultCursor();
    }

    #endregion
}

public enum CursorType
{
    ButtonCursor, TextCursor
}