using UnityEngine;
using UnityEngine.Localization.Components;
using TMPro;

[RequireComponent(typeof(LocalizeStringEvent))]
public class LocalizedTextUpdater : MonoBehaviour
{
    private LocalizeStringEvent _localizeStringEvent;
    private TMP_Text _textComponent;

    private void Awake()
    {
        _localizeStringEvent = GetComponent<LocalizeStringEvent>();
        _textComponent = GetComponent<TMP_Text>();

    }

    private void OnEnable()
    {
        _localizeStringEvent.OnUpdateString.AddListener((localizedText) =>
        {
            _textComponent.text = localizedText;
        });
    }

    private void OnDestroy()
    {
        _localizeStringEvent.OnUpdateString.RemoveAllListeners();
    }
}
