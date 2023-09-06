using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Localization.Settings;
using UnityEngine.Localization.Components;
using UnityEngine.Localization;
using UnityEngine.Localization.Tables;
using UnityEngine.Localization.Metadata;
using TMPro;

//Use as Singleton, if you would like to.
public class LocalizationManager : MonoBehaviour
{
    [SerializeField] private LocaleSelector localeSelector;

    [Header("UI")]
    [SerializeField] private TMP_Dropdown languageDropdown;
    [SerializeField] private TextMeshProUGUI selectedLanguageText;

    [Header("OPTIONS")]
    [SerializeField] private bool loadFromLocal;
    [SerializeField] private bool canSwitchLanguagesThroughKeyboard = false;


    [Header("Locale Info")]
    [SerializeField] private int selectedLocaleId;
    [SerializeField] private int availableLocalesCount;


    IEnumerator Start()
    {
        localeSelector.OnSelectedLocaleChanged += OnLocaleChanged;
        // Wait for the localization system to initialize
        yield return LocalizationSettings.InitializationOperation;

        Initialize();
        InitializeDropdown();
        UpdateUI(selectedLocaleId);

        if (loadFromLocal)
        {
            localeSelector.LoadLocale();
        }

    }

    private void Update()
    {
        if (!canSwitchLanguagesThroughKeyboard) return;

        if (Input.GetKeyUp(KeyCode.A))
        {
            PrevLanguage();
        }
        if (Input.GetKeyUp(KeyCode.D))
        {
            NextLanguage();
        }
    }


    private void Initialize()
    {
        availableLocalesCount = LocalizationSettings.AvailableLocales.Locales.Count;

        selectedLocaleId = 0;
        for (int i = 0; i < availableLocalesCount; ++i)
        {
            var locale = LocalizationSettings.AvailableLocales.Locales[i];
            if (LocalizationSettings.SelectedLocale == locale)
                selectedLocaleId = i;
        }
    }

    private void OnLocaleChanged(int newLocaleId)
    {
        selectedLocaleId = newLocaleId;
        UpdateUI(selectedLocaleId);
    }

    #region DROPDOWN

    private void InitializeDropdown()
    {
        if (languageDropdown == null)
        {
            Debug.LogWarning("NOT INITIALIZED : No dropdown is assigned in UI section");
            return;
        }
        // Generate list of available Locales
        var options = new List<TMP_Dropdown.OptionData>();
        for (int i = 0; i < availableLocalesCount; ++i)
        {
            var locale = LocalizationSettings.AvailableLocales.Locales[i];
            options.Add(new TMP_Dropdown.OptionData(locale.name));
        }
        languageDropdown.options = options;

        //languageDropdown.value = selectedLocaleId;
        //languageDropdown.value = localeSelector.GetSelectedLocale();
        languageDropdown.onValueChanged.AddListener(OnDropdownChanged);
    }

    private void OnDropdownChanged(int value)
    {
        SelectLanguage(value);
    }

    #endregion

    private void UpdateUI(int selectedLocaleId)
    {
        var locale = LocalizationSettings.AvailableLocales.Locales[selectedLocaleId];

        if (languageDropdown == null)
            Debug.LogWarning("Assign -languageDropdown- in UI section");
        else
            languageDropdown.value = selectedLocaleId;

        if (selectedLanguageText == null)
            Debug.LogWarning("Assign -selectedLanguageText- in UI section");
        else
            selectedLanguageText.text = locale.name;
    }


    #region EXTERNAL METHODS

    public void SelectLanguage(int value)
    {
        selectedLocaleId = value;
        localeSelector.ChangeLocale(value);
    }

    public void PrevLanguage()
    {
        selectedLocaleId = (selectedLocaleId - 1 + availableLocalesCount) % availableLocalesCount;
        localeSelector.ChangeLocale(selectedLocaleId);
    }

    public void NextLanguage()
    {
        selectedLocaleId = (selectedLocaleId + 1) % availableLocalesCount;
        localeSelector.ChangeLocale(selectedLocaleId);
    }

    public Locale GetCurrentLocale()
    {
        Locale currentLocale = LocalizationSettings.SelectedLocale;

        return currentLocale;
    }

    public string GetLocalizedText(string tableName, string entryKey)
    {
        // Get the table based on its name
        StringTable table = LocalizationSettings.StringDatabase.GetTable(tableName);

        if (table != null)
        {
            return GetLocalizedString(table, entryKey);
        }

        return null;
    }

    #endregion

    static string GetLocalizedString(StringTable table, string entryName)
    {
        var entry = table.GetEntry(entryName);

        // We can also extract Metadata here
        var comment = entry.GetMetadata<Comment>();
        if (comment != null)
        {
            Debug.Log($"Found metadata comment for {entryName} - {comment.CommentText}");
        }

        return entry.GetLocalizedString(); // We can pass in optional arguments for Smart Format or String.Format here.
    }
}

public static class LocalisationExtensions
{
    public static void SetStringReference(this LocalizeStringEvent localizedStringEvent, string tableName, string entryKey)
    {
        localizedStringEvent.StringReference.SetReference(tableName, entryKey);
        localizedStringEvent.StringReference.RefreshString();
    }
}