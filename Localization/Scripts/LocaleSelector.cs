using System;
using System.Collections;
using UnityEngine;
using UnityEngine.Localization.Settings;

public class LocaleSelector : MonoBehaviour
{
    public Action<int> OnSelectedLocaleChanged;

    private void OnEnable()
    {
        OnSelectedLocaleChanged += SaveLocale;
    }

    private void OnDisable()
    {
        OnSelectedLocaleChanged -= SaveLocale;
    }

    #region PLAYER PREFS

    private string localeKey = "LocaleKey";

    public int GetSelectedLocale()
    {
        int id = PlayerPrefs.GetInt(localeKey, 0);
        return id;
    }

    private void SaveLocale(int id)
    {
        PlayerPrefs.SetInt(localeKey, id);
    }

    #endregion

    #region EXTERNAL METHODS

    public void LoadLocale()
    {
        int id = GetSelectedLocale();
        ChangeLocale(id);
    }

    public void ChangeLocale(int localeId)
    {
        StartCoroutine(SetLocale(localeId));
    }

    #endregion

    private IEnumerator SetLocale(int localeId)
    {
        yield return LocalizationSettings.InitializationOperation;
        LocalizationSettings.SelectedLocale = LocalizationSettings.AvailableLocales.Locales[localeId];

        OnSelectedLocaleChanged?.Invoke(localeId);
    }
}