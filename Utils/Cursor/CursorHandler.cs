using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class CursorHandler : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerClickHandler, IDeselectHandler
{
    [SerializeField] private bool isButton = true;
    [SerializeField] private bool defaultCursorOnClick = true;
    [SerializeField] private CursorType cursorType;

    private Button _button;
    private bool _hasButtonComponent = false;

    protected void Awake()
    {
        cursorType = isButton ? CursorType.ButtonCursor : CursorType.TextCursor;
        // currentCursorHotspot = cursorType == CursorType.ButtonCursor ? cursorHotspotForButton : cursorHotspotForText;

        //ConfigureEvents();

        _hasButtonComponent = transform.TryGetComponent<Button>(out _button);
    }

    #region EVENTS HANDLING

    public void OnPointerEnter(PointerEventData eventData)
    {
        if (cursorType == CursorType.ButtonCursor)
        {
            if (_hasButtonComponent && !_button.interactable) return;

            CursorManager.SetButtonCursor();
        }
        else
        {
            CursorManager.SetTextCursor();
        }
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        CursorManager.SetDefaultCursor();
    }

    public void OnDeselect(BaseEventData eventData)
    {
        CursorManager.SetDefaultCursor();
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        if (defaultCursorOnClick)
            CursorManager.SetDefaultCursor();
    }

    #endregion
}

public enum CursorType
{
    ButtonCursor, TextCursor
}