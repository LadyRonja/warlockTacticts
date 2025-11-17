using System.Collections;
using System.Collections.Generic;
using System.IO;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class EditorFileHandlingUI : Singleton<MonoBehaviour>
{
    [SerializeField] TMP_InputField saveNameField;
    [SerializeField] Button saveButton;
    [Space]
    [SerializeField] TMP_Dropdown loadableLevels;
    [SerializeField] Button loadButton;

    protected override void Awake()
    {
        base.Awake();

        if(saveNameField == null)
        {
            saveNameField = GetComponentInChildren<TMP_InputField>();
        }
        if(saveButton == null)
        {
            saveButton = GetComponentsInChildren<Button>()[0];
        }
        if(loadableLevels== null)
        {
            loadableLevels = GetComponentInChildren<TMP_Dropdown>();
        }
        if (loadButton == null)
        {
            loadButton = GetComponentsInChildren<Button>()[1];
        }

        saveButton.onClick.AddListener(delegate { AttemptSaveLevel(); });

        PopulateLoadableLevelsOptions();
        loadButton.onClick.AddListener(delegate { AttemptLoadLevel(); });
    }

    public void AttemptSaveLevel()
    {
        if(saveNameField == null) { Debug.LogError("saveNameField missing, unable to save"); return; }
        if(saveNameField.text == null || saveNameField.text.Length == 0) { Debug.Log("No text entered in saveNameField"); return; }

        LevelEditManager.Instance.SaveActiveLevel(saveNameField.text);
        saveNameField.text = "";
        PopulateLoadableLevelsOptions();
    }

    private void PopulateLoadableLevelsOptions()
    {
        loadableLevels.options = new();

        List<TMP_Dropdown.OptionData> levelOptions = new();
        TextAsset[] levels = Resources.LoadAll<TextAsset>(Paths.LevelDataFolderPath);

        for (int i = 0; i < levels.Length; i++)
        {
            TMP_Dropdown.OptionData option = new TMP_Dropdown.OptionData();
            option.text = levels[i].name;
            levelOptions.Add(option);
        }

        loadableLevels.options = levelOptions;
    }

    public void AttemptLoadLevel()
    {
        if(loadableLevels == null) { Debug.LogError("loadableLevels missing reference"); return; }

        string assetPath = Path.Combine(Paths.LevelDataFolderPath, loadableLevels.options[loadableLevels.value].text);
        TextAsset levelAsset = Resources.Load<TextAsset>(assetPath);
        LevelEditManager.Instance.LoadLevelFromData(levelAsset.text);

        saveNameField.text = loadableLevels.options[loadableLevels.value].text;
    }
}
