using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SelectorHighlightManager : Singleton<SelectorHighlightManager>
{
    Sprite hlUnitSprite;
    Sprite hlTerrainSprite;
    string hlUnitSpritePath = @"HighLighterSprites\unitHL";
    string hlTerrainSpritePath = @"HighLighterSprites\terrainHL";

    const float terrainGFXScale = 1f;
    const int terrainGFXLayer = 5;
    const float unitGFXScale = 15f; 
    const int unitGFXLayer = 15;

    Dictionary<(string, GridLayers), GameObject> objects = new(); 


    protected override void Awake()
    {
        base.Awake();

        Initialization();
    }

    private void Initialization()
    {
        hlUnitSprite = Resources.Load<Sprite>(hlUnitSpritePath);
        hlTerrainSprite = Resources.Load<Sprite>(hlTerrainSpritePath);
    }

    /// <summary>
    /// NOTE: Does not set highlight sprite, layer, or scale
    /// </summary>
    private (GameObject o, SpriteRenderer s) CreateHighlightObject(int q, int r)
    {
        string coords = GridTile.GetStringFromCoords(q, r);
        GameObject obj = new GameObject(coords);

        GameObject child = new GameObject("GFX");
        child.transform.parent = obj.transform;
        SpriteRenderer sr = child.AddComponent<SpriteRenderer>();

        Vector2 pos = GridLayoutRules.GetPositionForFlatTopTile(LevelEditManager.Instance.LevelBeingEdited.layoutData, q, r);
        obj.transform.position = pos;

        return (obj, sr);
    }

    public void SetHighlightTerrain(int q, int r)
    {
        if(objects.TryGetValue((GridTile.GetStringFromCoords(q, r), GridLayers.TERRAIN), out _)) { return; }

        (GameObject obj, SpriteRenderer sr) = CreateHighlightObject(q, r);

        objects.Add((GridTile.GetStringFromCoords(q, r), GridLayers.TERRAIN), obj);
        sr.sprite = hlTerrainSprite;
        sr.sortingOrder = terrainGFXLayer;
        sr.transform.localScale = Vector2.one * terrainGFXScale;
    }

    public void SetHighlightUnit(int q, int r)
    {
        if (objects.TryGetValue((GridTile.GetStringFromCoords(q, r), GridLayers.UNIT), out _)) { return; }

        (GameObject obj, SpriteRenderer sr) = CreateHighlightObject(q, r);

        objects.Add((GridTile.GetStringFromCoords(q, r), GridLayers.UNIT), obj);
        sr.sprite = hlUnitSprite;
        sr.sortingOrder = unitGFXLayer;
        sr.transform.localScale = Vector2.one * unitGFXScale;
    }

    public void DeselectAll()
    {
        foreach (GameObject obj in objects.Values)
        {
            Destroy(obj);
        }
        objects.Clear();
    }
}
