using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class EditorVariableDataCell : MonoBehaviour
{
    public TMP_Text nameLabel;
    public TMP_Text typeLabel;
    public TMP_InputField variableInputField;
    [HideInInspector] public string fullTypeName = "";
}
