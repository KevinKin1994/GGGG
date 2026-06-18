using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.UI;

public class MapCreator : MonoBehaviour
{
    [Header("Brush")]
    public Image selectedPainter;
    public List<Sprite> brushSprites = new List<Sprite>();

    [Header("Grid")]
    public GameObject grid;
    public RectTransform gridRoot;
    public int mapSideLength = 3;
    public Vector2 gridSize = new Vector2(100f, 100f);
    public Vector4 transparentPadding = new Vector4(4f, 9f, 6f, 3f);

    [Header("Save")]
    public string saveFileName = "map_creator_map.json";

    [Header("Buttons")]
    public Button saveButton;
    public Button loadButton;
    public Button clearButton;

    private readonly Dictionary<Vector2Int, Image> gridImages = new Dictionary<Vector2Int, Image>();
    private readonly Dictionary<string, Sprite> spriteLookup = new Dictionary<string, Sprite>();
    private RectTransform canvasRectTransform;
    private Canvas canvas;
    private Camera uiCamera;

    private Map map;

    void Start()
    {
        if (gridRoot == null)
        {
            gridRoot = transform as RectTransform;
            map = gridRoot.gameObject.AddComponent<Map>();
        }

        InitializeCanvasContext();

        RegisterBrushSprites();
        BindButtons();
    }

    private void Update()
    {
        Update_Painter();
    }

    private void Update_Painter()
    {
        if (Input.GetMouseButton(0))
        {
            PaintAtMousePosition();
        }
    }

    public void PaintAtMousePosition()
    {
        if (grid == null)
        {
            return;
        }

        if (!MapTranslator.TryScreenToMapPosition(
                Input.mousePosition,
                canvasRectTransform,
                uiCamera,
                mapSideLength,
                gridSize,
                transparentPadding,
                out Vector2Int mapPosition))
        {
            return;
        }

        Paint(mapPosition);
    }

    public void SaveMap()
    {
        MapSaveData saveData = new MapSaveData
        {
            mapSideLength = mapSideLength,
            gridSize = gridSize,
            transparentPadding = transparentPadding,
            grids = new List<GridSaveData>()
        };

        foreach (KeyValuePair<Vector2Int, Image> pair in gridImages)
        {
            Sprite sprite = pair.Value == null ? null : pair.Value.sprite;
            saveData.grids.Add(new GridSaveData
            {
                q = pair.Key.x,
                r = pair.Key.y,
                spriteName = sprite == null ? string.Empty : sprite.name
            });
        }

        File.WriteAllText(GetSavePath(), JsonUtility.ToJson(saveData, true));
        Debug.Log($"Map saved: {GetSavePath()}");
    }

    public void LoadMap()
    {
        string path = GetSavePath();
        if (!File.Exists(path))
        {
            Debug.LogWarning($"Map save file not found: {path}");
            return;
        }

        MapSaveData saveData = JsonUtility.FromJson<MapSaveData>(File.ReadAllText(path));
        if (saveData == null)
        {
            Debug.LogWarning($"Map save file is invalid: {path}");
            return;
        }

        ClearMap();

        mapSideLength = Mathf.Max(1, saveData.mapSideLength);
        if (saveData.gridSize.x > 0f && saveData.gridSize.y > 0f)
        {
            gridSize = saveData.gridSize;
        }
        else if (saveData.gridHeight > 0f)
        {
            gridSize = new Vector2(saveData.gridHeight, saveData.gridHeight);
        }

        if (saveData.transparentPadding != Vector4.zero)
        {
            transparentPadding = saveData.transparentPadding;
        }

        if (saveData.grids == null)
        {
            return;
        }

        foreach (GridSaveData gridData in saveData.grids)
        {
            Sprite sprite = FindSprite(gridData.spriteName);
            Paint(new Vector2Int(gridData.q, gridData.r), sprite);
        }

        SortGridLayers();
        Debug.Log($"Map loaded: {path}");
    }

    public void ClearMap()
    {
        foreach (Image image in gridImages.Values)
        {
            if (image != null)
            {
                MapGrid mapGrid = image.GetComponent<MapGrid>();
                if (MapSystem.Instance != null)
                {
                    map.UnregisterGrid(mapGrid);
                }

                Destroy(image.gameObject);
            }
        }

        if (MapSystem.Instance != null)
        {
            map.ClearGrids();
        }

        gridImages.Clear();
    }

    public void SetSelectedPainter(Image painter)
    {
        selectedPainter = painter;
        RegisterSprite(selectedPainter == null ? null : selectedPainter.sprite);
    }

    public void SetSelectedSprite(Sprite sprite)
    {
        if (selectedPainter != null)
        {
            selectedPainter.sprite = sprite;
        }

        RegisterSprite(sprite);
    }

    public void SelectBrushByIndex(int index)
    {
        if (index < 0 || index >= brushSprites.Count)
        {
            return;
        }

        SetSelectedSprite(brushSprites[index]);
    }

    private void Paint(Vector2Int mapPosition)
    {
        Paint(mapPosition, selectedPainter == null ? null : selectedPainter.sprite);
    }

    private void Paint(Vector2Int mapPosition, Sprite sprite)
    {
        if (gridImages.TryGetValue(mapPosition, out Image existingImage))
        {
            ApplySprite(existingImage, sprite);
            SortGridLayers();
            return;
        }

        GameObject gridObject = Instantiate(grid, gridRoot == null ? transform : gridRoot);
        gridObject.name = $"Grid_{mapPosition.x}_{mapPosition.y}";

        RectTransform rectTransform = gridObject.transform as RectTransform;
        if (rectTransform != null)
        {
            rectTransform.anchorMin = new Vector2(0.5f, 0.5f);
            rectTransform.anchorMax = new Vector2(0.5f, 0.5f);
            rectTransform.pivot = new Vector2(0.5f, 0.5f);
            rectTransform.anchoredPosition = MapTranslator.MapToLocalPosition(
                mapPosition,
                gridSize,
                transparentPadding);
            rectTransform.sizeDelta = gridSize;
        }

        MapGrid mapGrid = gridObject.GetComponent<MapGrid>();
        if (mapGrid == null)
        {
            mapGrid = gridObject.AddComponent<MapGrid>();
        }

        mapGrid.SetPosition(mapPosition.x, mapPosition.y);
        if (MapSystem.Instance != null)
        {
            map.RegisterGrid(mapGrid);
        }

        Image image = gridObject.GetComponent<Image>();
        ApplySprite(image, sprite);
        gridImages[mapPosition] = image;
        SortGridLayers();
    }

    private void ApplySprite(Image image, Sprite sprite)
    {
        if (image == null || sprite == null)
        {
            return;
        }

        image.sprite = sprite;
        RegisterSprite(sprite);
    }

    private void SortGridLayers()
    {
        List<Image> images = new List<Image>(gridImages.Values);
        images.RemoveAll(image => image == null);
        images.Sort(CompareGridLayer);

        for (int i = 0; i < images.Count; i++)
        {
            images[i].transform.SetSiblingIndex(i);
        }
    }

    private int CompareGridLayer(Image a, Image b)
    {
        RectTransform rectA = a.transform as RectTransform;
        RectTransform rectB = b.transform as RectTransform;
        Vector2 positionA = rectA == null ? Vector2.zero : rectA.anchoredPosition;
        Vector2 positionB = rectB == null ? Vector2.zero : rectB.anchoredPosition;

        int yCompare = positionB.y.CompareTo(positionA.y);
        if (yCompare != 0)
        {
            return yCompare;
        }

        return positionA.x.CompareTo(positionB.x);
    }

    private string GetSavePath()
    {
        return Path.Combine(Application.persistentDataPath, saveFileName);
    }

    private void InitializeCanvasContext()
    {
        canvas = GetComponentInParent<Canvas>();
        canvasRectTransform = canvas == null ? transform as RectTransform : canvas.transform as RectTransform;
        uiCamera = canvas != null && canvas.renderMode != RenderMode.ScreenSpaceOverlay ? canvas.worldCamera : null;
    }

    public bool TryMouseToMapPosition(out Vector2Int mapPosition)
    {
        if (canvasRectTransform == null)
        {
            InitializeCanvasContext();
        }

        return MapTranslator.TryScreenToMapPosition(
            Input.mousePosition,
            canvasRectTransform,
            uiCamera,
            mapSideLength,
            gridSize,
            transparentPadding,
            out mapPosition);
    }

    private void BindButtons()
    {
        if (saveButton != null)
        {
            saveButton.onClick.AddListener(SaveMap);
        }

        if (loadButton != null)
        {
            loadButton.onClick.AddListener(LoadMap);
        }

        if (clearButton != null)
        {
            clearButton.onClick.AddListener(ClearMap);
        }
    }

    private void RegisterSprite(Sprite sprite)
    {
        if (sprite == null || string.IsNullOrEmpty(sprite.name))
        {
            return;
        }

        spriteLookup[sprite.name] = sprite;
    }

    private void RegisterBrushSprites()
    {
        foreach (Sprite sprite in brushSprites)
        {
            RegisterSprite(sprite);
        }

        RegisterSprite(selectedPainter == null ? null : selectedPainter.sprite);
    }

    private Sprite FindSprite(string spriteName)
    {
        if (string.IsNullOrEmpty(spriteName))
        {
            return selectedPainter == null ? null : selectedPainter.sprite;
        }

        if (spriteLookup.TryGetValue(spriteName, out Sprite sprite))
        {
            return sprite;
        }

        RegisterBrushSprites();

        Sprite[] sprites = Resources.LoadAll<Sprite>(string.Empty);
        foreach (Sprite resourceSprite in sprites)
        {
            RegisterSprite(resourceSprite);
        }

        return spriteLookup.TryGetValue(spriteName, out sprite)
            ? sprite
            : selectedPainter == null ? null : selectedPainter.sprite;
    }

    [System.Serializable]
    private class MapSaveData
    {
        public int mapSideLength;
        public float gridHeight;
        public Vector2 gridSize;
        public Vector4 transparentPadding;
        public List<GridSaveData> grids;
    }

    [System.Serializable]
    private class GridSaveData
    {
        public int q;
        public int r;
        public string spriteName;
    }
}
