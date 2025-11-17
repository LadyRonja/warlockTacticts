using Newtonsoft.Json;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Windows;

public class GridGenerator : Singleton<GridGenerator>
{
    public GameObject tilePrefab;
    public GameObject unitPrefab;

    [SerializeField] TextAsset levelDataFile;

    private void Start()
    {
        if (tilePrefab == null || unitPrefab == null)
        {
            Debug.LogError("GridGenerator Requires proper initialization");
        }

        if(SceneManager.GetActiveScene().name != "_GameScene") { return; }     

        GenerateFromJson(levelDataFile.text, out LevelData loadedLevel, out _);

        GridInformant.Instance.SetActiveGrid(loadedLevel);
        ActiveLevelManager.Instance.SetActiveGrid(loadedLevel);
        
    }

    public void Initialize(GameObject tilePrefab, GameObject unitPrefab)
    {
        this.tilePrefab = tilePrefab;
        this.unitPrefab = unitPrefab;
    }

    public void GenerateFromJson(string level, out LevelData loadedLevel, out List<GameObject> levelObjects)
    {
        LevelData levelToLoad = (LevelData)JsonConvert.DeserializeObject<LevelData>(level);
        levelObjects = new();

        foreach (GridTile t in levelToLoad.tiles.Values)
        {
            levelObjects.Add(CreateTile(t.q, t.r, t, levelToLoad.layoutData));
        }

        List<(string, UnitData)> loadedDefaultDataReplacements = new();
        foreach (UnitData ud in levelToLoad.units.Values)
        {
            (int q, int r, _) = GridTile.GetCoordsFromCoordString(levelToLoad.units.First(u => u.Value == ud).Key);

            UnitData dataToLoad = Instantiate(ud);

            if (ud.aquireDefaultValueOnLoad)
            {
                UnitData[] allData = Resources.LoadAll<UnitData>(Paths.UnitDataFolderPath);
                UnitData defaultData = allData.FirstOrDefault(u => u.AssetID == ud.AssetID);
                if (defaultData == null) { Debug.LogError("MISSING DEFAULT DATA"); goto NoDefaultData; }

                dataToLoad = Instantiate(defaultData);
            }

            NoDefaultData:
            Vector2 tilePos = GridLayoutRules.GetPositionForFlatTopTile(levelToLoad.layoutData, q, r);
            GameObject unitObj = Instantiate(unitPrefab, tilePos, Quaternion.identity);
            Unit unitScr = unitObj.GetComponent<Unit>();
            unitScr.data = dataToLoad;
            unitScr.unitID = Guid.NewGuid().ToString();
            dataToLoad.unitIDforClonedData = unitScr.unitID;
            unitObj.name = unitScr.unitID;

            levelObjects.Add(unitObj);


            string key = GridTile.GetStringFromCoords(q, r);
            loadedDefaultDataReplacements.Add((key, dataToLoad));
        }

        for (int i = 0; i < loadedDefaultDataReplacements.Count; i++)
        {
            (string key, UnitData data) = loadedDefaultDataReplacements[i];
            levelToLoad.units.Remove(key);
            levelToLoad.units.Add(key, data);
        }

        loadedLevel = levelToLoad;
    }

    public GameObject CreateTile(int q, int r, GridTile tile, GridLayoutRules.LayoutData layoutRules)
    {
        Vector2 tilePos = GridLayoutRules.GetPositionForFlatTopTile(layoutRules, q, r);
        GameObject newTileObj = Instantiate(tilePrefab, tilePos, Quaternion.identity, this.transform);
        newTileObj.transform.name = tile.coords;

        return newTileObj;
    }

    public GameObject CreateUnit(int q, int r, UnitData data, GridLayoutRules.LayoutData layoutRules)
    {
        string coords = GridTile.GetStringFromCoords(q, r);

        Vector2 worldPos = GridLayoutRules.GetPositionForFlatTopTile(layoutRules, q, r);
        GameObject unitObj = Instantiate(unitPrefab, worldPos, Quaternion.identity);
        Unit unitScr = unitObj.GetComponent<Unit>();
        unitScr.data = data;
        unitScr.unitID = Guid.NewGuid().ToString();
        data.unitIDforClonedData = unitScr.unitID;
        unitObj.transform.name = unitScr.unitID;

        return unitObj;
    }
}
