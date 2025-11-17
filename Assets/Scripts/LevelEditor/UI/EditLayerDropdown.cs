using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class EditLayerDropdown : MonoBehaviour
{
    [SerializeField] TMP_Dropdown editLayerDropdown; 
    List<KeyCode> numToKeyCode = new() {
        KeyCode.Alpha1,
        KeyCode.Alpha2,
        KeyCode.Alpha3,
        KeyCode.Alpha4,
        KeyCode.Alpha5,
        KeyCode.Alpha6,
        KeyCode.Alpha7,
        KeyCode.Alpha8,
        KeyCode.Alpha9,
    };

    private void Start()
    {
        PopulateEditLayerOptions();
        editLayerDropdown.onValueChanged.AddListener(delegate { UpdateEditorLayerOnSelect(); });
        UpdateEditorLayerOnSelect();
    }

    private void Update()
    {
        if (!Input.GetKey(KeyCode.N)) { return; }

        int optionsCount = Math.Min(editLayerDropdown.options.Count, 9);

        for (int i = 0; i < optionsCount; i++)
        {
            if (Input.GetKeyDown(numToKeyCode[i]))
            {
                editLayerDropdown.value = i;
                break;
            }
        }
    }

    private void PopulateEditLayerOptions()
    {
        if(editLayerDropdown == null)
        {
            editLayerDropdown = GetComponent<TMP_Dropdown>();
        }

        List<TMP_Dropdown.OptionData> layersOptions = new();
        string[] drawStyles = Enum.GetNames(typeof(GridLayers));

        for (int i = 0; i < drawStyles.Length; i++)
        {
            TMP_Dropdown.OptionData option = new TMP_Dropdown.OptionData();
            option.text = drawStyles[i];
            layersOptions.Add(option);
        }

        editLayerDropdown.options = layersOptions;
    }

    private void UpdateEditorLayerOnSelect()
    {
        string stringyfiedEnumOption = editLayerDropdown.options[editLayerDropdown.value].text;
        if (!Enum.TryParse(stringyfiedEnumOption, out GridLayers enumOption)) { Debug.LogError("Failed to parse dropdown to enum"); return; }
        LevelEditManager.Instance.editLayer = enumOption;
    }
}
