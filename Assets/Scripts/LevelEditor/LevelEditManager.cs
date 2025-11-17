using Newtonsoft.Json;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public enum ClickMode { ADD, REMOVE, SELECT };
public enum GridLayers { TERRAIN, UNIT};

public delegate void EditMode();

public class LevelEditManager : Singleton<LevelEditManager>
{
    [HideInInspector] public ClickMode editMethod = ClickMode.ADD;
    [HideInInspector] public GridLayers editLayer = GridLayers.TERRAIN;
    Dictionary<(ClickMode, GridLayers), EditMode> editModes = new();

    [HideInInspector] public EditAdditionManager additionManager;
    [HideInInspector] public EditRemovalManager removalManager;
    [HideInInspector] public EditSelectionManager selectionManager;

    LevelData levelBeingEdited = null;
    public LevelData LevelBeingEdited { get => levelBeingEdited; private set => levelBeingEdited = value; }

    [SerializeField] GameObject tilePrefab;
    [SerializeField] GameObject unitPrefab;
    List<GameObject> levelObjects = new();
    [Space]
    [SerializeField] GraphicRaycaster graphicsRayCaster;
    [SerializeField] EventSystem eventSystem;

    string lastPosClicked = "";

    protected override void Awake()
    {
        base.Awake();

        SetUpEditModes();
        SetUpFreshStart();
        SetUpRaycaster();
        GridGenerator.Instance.Initialize(tilePrefab, unitPrefab);

    }

    private void Update()
    {
        if (Input.GetMouseButtonDown((int)MouseButton.Left))
        {
            AttemptExecuteEdit();
        }
        else if (Input.GetMouseButton((int)MouseButton.Left))
        {
            string newCoords = ClickToCoords();
            if (string.Equals(lastPosClicked, newCoords)) { return; }

            AttemptExecuteEdit();
        }
        else if (Input.GetMouseButtonUp((int)MouseButton.Left))
        {
            lastPosClicked = "";
        }

        if (Input.GetKeyDown(KeyCode.Z))
        {
            CommandManager.Instance.Undo();
        }
    }

    private void AttemptExecuteEdit()
    {
        if (ClickIsAtEdge(Input.mousePosition)) { lastPosClicked = ""; return; }
        if (ClickHitUI(Input.mousePosition)) { lastPosClicked = ""; return; }

        if (editModes.TryGetValue((editMethod, editLayer), out EditMode currentMode))
        {
            lastPosClicked = ClickToCoords();
            currentMode();
        }
    }

    private void SetUpEditModes()
    {
        additionManager = EditAdditionManager.Instance;
        additionManager.editor = this;
        removalManager = EditRemovalManager.Instance;
        removalManager.editor = this;
        selectionManager = EditSelectionManager.Instance;
        selectionManager.editor = this;

        editModes.Clear();
        editModes.Add((ClickMode.ADD, GridLayers.TERRAIN), () => additionManager.AddGround());
        editModes.Add((ClickMode.REMOVE, GridLayers.TERRAIN), () => removalManager.RemoveGround());
        editModes.Add((ClickMode.SELECT, GridLayers.TERRAIN), () => selectionManager.SelectTerrain());

        editModes.Add((ClickMode.ADD, GridLayers.UNIT), () => additionManager.AddUnit());
        editModes.Add((ClickMode.REMOVE, GridLayers.UNIT), () => removalManager.RemoveUnit());
        editModes.Add((ClickMode.SELECT, GridLayers.UNIT), () => selectionManager.SelectUnit());
    }

    private void SetUpFreshStart()
    {
        levelBeingEdited = new();

        Vector2 startPos = transform.position;
        GameObject temp = Instantiate(tilePrefab, startPos, Quaternion.identity, this.transform);
        SpriteRenderer sr = temp.GetComponent<SpriteRenderer>();
        float tileWidth = sr.bounds.size.x;
        float tileHeight = sr.bounds.size.y;
        GridLayoutRules.LayoutData layoutData = new GridLayoutRules.LayoutData(startPos, tileWidth, tileHeight);
        Destroy(temp);

        levelBeingEdited = new();
        levelBeingEdited.layoutData = layoutData;

        levelObjects = new();

        GridInformant.Instance.SetActiveGrid(levelBeingEdited);
        ActiveLevelManager.Instance.SetActiveGrid(levelBeingEdited);
    }

    private void SetUpRaycaster()
    {
        if (graphicsRayCaster == null)
        {
            Debug.Log("Attempting to find graphics raycaster");
            graphicsRayCaster = FindAnyObjectByType<GraphicRaycaster>();
            if (graphicsRayCaster != null)  { Debug.Log("Graphics raycaster found"); }
            else                            { Debug.LogError("Unable to find graphics raycaster"); }
        }
        if (eventSystem == null)
        {
            Debug.Log("Attempting to find eventSystem");
            eventSystem = FindAnyObjectByType<EventSystem>();
            if (eventSystem != null)    { Debug.Log("EventSystem found"); }
            else                        { Debug.LogError("Unable to find eventSystem"); }
        }
    }

    public void LoadLevelFromData(string levelJSON)
    {
        UnloadActiveLevel();

        GridGenerator.Instance.GenerateFromJson(levelJSON, out levelBeingEdited, out levelObjects);

        CommandManager.Instance.ClearHistory();
        GridInformant.Instance.SetActiveGrid(levelBeingEdited);
    }

    private void UnloadActiveLevel()
    {
        foreach (GameObject go in levelObjects)
        {
            Destroy(go);
        }

        SelectorHighlightManager.Instance.DeselectAll();
        CommandManager.Instance.ClearHistory();
        levelBeingEdited = null;
    }

    public void SaveActiveLevel(string asName)
    {
        string stringyfiedLevelData = JsonConvert.SerializeObject(levelBeingEdited);

        File.WriteAllText(Application.dataPath + "/Resources/Levels/" + asName + ".txt", stringyfiedLevelData);

#if UNITY_EDITOR
        AssetDatabase.Refresh();
#endif

        Debug.Log("Level Saved!");
    }

    private bool ClickIsAtEdge(Vector2 screenPos)
    {
        float cutOffPercentage = 0.85f;

        float rightCutOff = Screen.width * cutOffPercentage;
        float leftCutOff = Screen.width * (1f - cutOffPercentage);
        float upCutOff = Screen.height * cutOffPercentage;
        float downCutOff = Screen.height * (1f - cutOffPercentage);

        #region Debug
        /*
        Debug.Log($"Cutoff right: {rightCutOff}");
        Debug.Log($"Cutoff left: {leftCutOff}");
        Debug.Log($"Cutoff up: {upCutOff}");
        Debug.Log($"Cutoff down : {downCutOff}");

        Debug.Log($"screenPos.x: {screenPos.x}");
        Debug.Log($"screenPos.y: {screenPos.y}");
        */
        #endregion

        if (screenPos.x < leftCutOff || screenPos.x > rightCutOff) { return true; }
        if (screenPos.y < downCutOff || screenPos.y > upCutOff) { return true; }

        return false;
    }

    private bool ClickHitUI(Vector2 mousePos)
    {
        if (graphicsRayCaster != null && eventSystem != null)
        {
            PointerEventData ped = new PointerEventData(eventSystem);
            ped.position = Input.mousePosition;
            List<RaycastResult> results = new();
            graphicsRayCaster.Raycast(ped, results);

            if (results.Count != 0) { return true; }
        }
        return false;
    }

    private string ClickToCoords()
    {
        Vector2 worldPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        (int _q, int _r, _) = GridLayoutRules.GetTileCoordsFromPositionFlatTop(LevelBeingEdited.layoutData, worldPos);
        string output = GridTile.GetStringFromCoords(_q, _r);
        return output;
    } 

    public void CreateTile(int q, int r, GridTile tile)
    {
        RemoveTile(q, r);
        levelBeingEdited.tiles.Add(tile.coords, tile);
        levelObjects.Add(GridGenerator.Instance.CreateTile(q, r, tile, levelBeingEdited.layoutData));
    }

    public void RemoveTile(int q, int r)
    {
        string tileName = GridTile.GetStringFromCoords(q, r);
        GameObject obj = GameObject.Find(tileName);

        if (levelObjects.Contains(obj))
        {
            levelObjects.Remove(obj);
        }

        Destroy(obj);

        if (levelBeingEdited.tiles.ContainsKey(tileName))
        {
            levelBeingEdited.tiles.Remove(tileName);
        }
    }

    public void CreateUnit(int q, int r, UnitData data)
    {
        RemoveUnit(q, r);

        string coords = GridTile.GetStringFromCoords(q, r);
        levelBeingEdited.units.Add(coords, data);

        levelObjects.Add(GridGenerator.Instance.CreateUnit(q, r, data, levelBeingEdited.layoutData));
    }

    public void RemoveUnit(int q, int r)
    {
        if (!GridInformant.Instance.TryGetUnit(q, r, out Unit unit))
        {
            return;
        }

        GameObject obj = unit.gameObject;

        if (levelObjects.Contains(obj))
        {
            levelObjects.Remove(obj);
        }

        string dataCoords = GridTile.GetStringFromCoords(q, r);

        if (levelBeingEdited.units.ContainsKey(dataCoords))
        {
            levelBeingEdited.units.Remove(dataCoords);
        }

        Destroy(obj);
    }

}
