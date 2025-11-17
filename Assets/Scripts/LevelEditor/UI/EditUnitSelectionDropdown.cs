using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UIElements;

public class EditUnitSelectionDropdown : MonoBehaviour
{
    [SerializeField] TMP_Dropdown unitSelectionDropdown;
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
        PopulateUnitSelectionOptions();
        unitSelectionDropdown.onValueChanged.AddListener(delegate { UpdateEditorSelectedUnitData(); });
        UpdateEditorSelectedUnitData();
    }
    private void Update()
    {
        if (!Input.GetKey(KeyCode.M)) { return; }

        int optionsCount = Math.Min(unitSelectionDropdown.options.Count, 9);

        for (int i = 0; i < optionsCount; i++)
        {
            if (Input.GetKeyDown(numToKeyCode[i]))
            {
                unitSelectionDropdown.value = i;
                break;
            }
        }
    }

    private void PopulateUnitSelectionOptions()
    {
        if (unitSelectionDropdown == null)
        {
            unitSelectionDropdown = GetComponent<TMP_Dropdown>();
        }

        List<TMP_Dropdown.OptionData> unitOptions = new();
        UnitData[] unitDataPrefabs = Resources.LoadAll<UnitData>(Paths.UnitDataFolderPath);

        for (int i = 0; i < unitDataPrefabs.Length; i++)
        {
            TMP_Dropdown.OptionData option = new TMP_Dropdown.OptionData();
            option.text = unitDataPrefabs[i].unitName;
            unitOptions.Add(option);
        }

        unitSelectionDropdown.options = unitOptions;
    }

    private void UpdateEditorSelectedUnitData()
    {
        EditAdditionManager.Instance.selectedUnitIndex = unitSelectionDropdown.value;
    }
}
