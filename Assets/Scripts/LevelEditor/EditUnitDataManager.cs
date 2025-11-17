using Newtonsoft.Json.Linq;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEngine;

public class EditUnitDataManager : Singleton<EditUnitDataManager>
{
    List<EditorVariableDataCell> activeDataCells = new();
    UnitData dataBeingEdited = null;

    public void DisplayData(UnitData ud)
    {
        activeDataCells = new();
        dataBeingEdited = ud;

        FieldInfo[] myFieldInfo;
        Type myType = ud.GetType();
        myFieldInfo = myType.GetFields(BindingFlags.Instance | BindingFlags.Public);

        EditorUnitDataUI.Instance.ClearAllDataCells();
        for (int i = 0; i < myFieldInfo.Length; i++)
        {
            if (myFieldInfo[i].Name == nameof(ud.unitIDforClonedData)) { continue; }
            if (myFieldInfo[i].Name == nameof(ud.AssetID)) { continue; }

            var result = ud.GetType().GetField(myFieldInfo[i].Name).GetValue(ud);

            EditorVariableDataCell createdCell = EditorUnitDataUI.Instance.CreateNewDataCell(myFieldInfo[i].Name, myFieldInfo[i].FieldType.ToString(), result.ToString());
            activeDataCells.Add(createdCell);
        }
    }

    public void UpdateData()
    {
        if (activeDataCells.Count == 0) { return; }
        if (dataBeingEdited == null) { return; }

        foreach (EditorVariableDataCell dc in activeDataCells)
        {
            Type varType = Type.GetType(dc.fullTypeName);

            if(varType == typeof(int))
            {
                if (Int32.TryParse(dc.variableInputField.text, out int value)) {
                    dataBeingEdited.GetType().GetField(dc.nameLabel.text).SetValue(dataBeingEdited, value);
                }
                continue;
            }

            if (varType == typeof(float))
            {
                if (float.TryParse(dc.variableInputField.text, out float value))
                {
                    dataBeingEdited.GetType().GetField(dc.nameLabel.text).SetValue(dataBeingEdited, value);
                }
                continue;
            }

            if (varType == typeof(bool))
            {
                char[] input = dc.variableInputField.text.ToLower().ToCharArray();
                if (input.Length > 0)
                {
                    if (input[0] == 't')
                    {
                        dataBeingEdited.GetType().GetField(dc.nameLabel.text).SetValue(dataBeingEdited, true);
                    }
                    else if (input[0] == 'f')
                    {
                        dataBeingEdited.GetType().GetField(dc.nameLabel.text).SetValue(dataBeingEdited, false);
                    }
                    else
                    {
                        Debug.Log($"Could not parse boolean input for: {dc.nameLabel.text}");
                    }
                }
                continue;
            }

            if (varType == typeof(string))
            {
                dataBeingEdited.GetType().GetField(dc.nameLabel.text).SetValue(dataBeingEdited, dc.variableInputField.text);
                continue;
            }
        }
        dataBeingEdited.aquireDefaultValueOnLoad = false;
        Debug.Log("Data saved, still requires saving level for changes to persist");
        DisplayData(dataBeingEdited);
    }

    public void DiscardCustomData()
    {
        if (dataBeingEdited == null) { return; }

        UnitData[] allData = Resources.LoadAll<UnitData>(Paths.UnitDataFolderPath);
        UnitData defaultData = allData.FirstOrDefault(u => u.AssetID == dataBeingEdited.AssetID);
        if (defaultData == null) { Debug.LogError("MISSING DEFAULT DATA"); return; }
        UnitData defaultDataInstance = Instantiate(defaultData);

        string key = LevelEditManager.Instance.LevelBeingEdited.units.FirstOrDefault(u => u.Value == dataBeingEdited).Key;
        if(!GridInformant.Instance.TryGetUnit(dataBeingEdited, out Unit unitRepresentation)) { Debug.LogError("Unable to find unit in world"); }
        unitRepresentation.data = defaultDataInstance;
        LevelEditManager.Instance.LevelBeingEdited.units.Remove(key);
        LevelEditManager.Instance.LevelBeingEdited.units.Add(key, defaultDataInstance);
        dataBeingEdited = defaultDataInstance;

        DisplayData(dataBeingEdited);
        Debug.Log("Data restored to default, saving required to apply changes");
    }
}
