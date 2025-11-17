using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class EditModesDropdown : MonoBehaviour
{
    [SerializeField] TMP_Dropdown editModesDropdown;
    int previousIndex = 0;
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
        PopulateEditModesDropdown();
        editModesDropdown.onValueChanged.AddListener(delegate { UpdateEditorModeOnSelect(); });
        UpdateEditorModeOnSelect();
    }

    private void Update()
    {
        // Hold down ctrl for remove
        if (Input.GetKeyDown(KeyCode.LeftControl))
        {
            editModesDropdown.value = 1;
        }

        // Go to go back to the previous mode (does not track setting to remove)
        if (Input.GetKeyUp(KeyCode.LeftControl))
        {
            editModesDropdown.value = previousIndex;
        }

        // B + number to change mode
        if (!Input.GetKey(KeyCode.B)) { return; }
        int optionsCount = Math.Min(editModesDropdown.options.Count, 9);
        for (int i = 0; i < optionsCount; i++)
        {
            if (Input.GetKeyDown(numToKeyCode[i]))
            {
                editModesDropdown.value = i;
                break;
            }
        }
    }

    private void PopulateEditModesDropdown()
    {
        if (editModesDropdown == null)
        {
            editModesDropdown = GetComponent<TMP_Dropdown>();
        }

        List<TMP_Dropdown.OptionData> editStyleOptions = new();
        string[] drawStyles = Enum.GetNames(typeof(ClickMode));

        for (int i = 0; i < drawStyles.Length; i++)
        {
            TMP_Dropdown.OptionData option = new TMP_Dropdown.OptionData();
            option.text = drawStyles[i];
            editStyleOptions.Add(option);
        }

        editModesDropdown.options = editStyleOptions;
    }

    private void UpdateEditorModeOnSelect()
    {
        string stringyfiedEnumOption = editModesDropdown.options[editModesDropdown.value].text;
        if(!Enum.TryParse(stringyfiedEnumOption, out ClickMode enumOption)) { Debug.LogError("Failed to parse dropdown to enum"); return; }
        LevelEditManager.Instance.editMethod = enumOption;

        // Do not track setting the mode to remove
        if(editModesDropdown.value != 1)
        {
            previousIndex = editModesDropdown.value;
        }
    }
}
