using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CellData
{
  [field: SerializeField]
  public string id;
  [field: SerializeField]
  public CellRenderers renderers;

  //The actual data, will store wall info on that cell, maybe info about what items are in it, etc.
  [field: SerializeField]
  public int value;

  [field: SerializeField]
  public Sprite sprite;
}


//this will eventually include a sprite renderer, maybe mesh renderers?
public class CellRenderers
{
  [field: SerializeField]
  public TextMesh textMesh;
  [field: SerializeField]
  public SpriteRenderer spriteRenderer;
}

public class MyGrid
{
  int width;
  int height;
  float cellSize;
  Transform parent;
  LayerMask layerMask;
  CellData[,] gridArray;
  Vector3 originPosition;

  public MyGrid(int width, int height, float cellSize, Vector3 originPosition, Transform parent, LayerMask layerMask)
  {
    this.width = width;
    this.height = height;
    this.cellSize = cellSize;
    this.originPosition = originPosition;
    this.parent = parent;
    this.layerMask = layerMask;

    gridArray = new CellData[width, height];
    // debugTextArray = new TextMesh[width, height];

    for (int x = 0; x < gridArray.GetLength(0); x++)
    {
      for (int y = 0; y < gridArray.GetLength(1); y++)
      {
        gridArray[x, y] = new CellData { id = (x + "_" + y).ToString(), value = x * width + y, renderers = new CellRenderers() };
        //TODO: the grid appearing is actually useful, but needs to bea ble to turn off easily, figure it out baby
        // gridArray[x, y].renderers.textMesh = CreateWorldText(gridArray[x, y], null, GetWorldPosition(x, y) + new Vector3(cellSize, 0, cellSize) * 0.5f, 350, Color.white, TextAnchor.MiddleCenter, 0);
        gridArray[x, y].renderers.spriteRenderer = CreateSpriteRenderer(gridArray[x, y], parent, GetWorldPosition(x, y) + new Vector3(cellSize, 0, cellSize) * 0.5f, 0);

        Debug.DrawLine(GetWorldPosition(x, y), GetWorldPosition(x, y + 1), Color.white, 100f);
        Debug.DrawLine(GetWorldPosition(x, y), GetWorldPosition(x + 1, y), Color.white, 100f);
      }
    }

    Debug.DrawLine(GetWorldPosition(0, height), GetWorldPosition(width, height), Color.white, 100f);
    Debug.DrawLine(GetWorldPosition(width, 0), GetWorldPosition(width, height), Color.white, 100f);
  }

  private Vector3 GetWorldPosition(int x, int y)
  {
    return new Vector3(x, 0, y) * cellSize + originPosition;
  }

  private Vector2 GetXY(Vector2 screenPos)
  {
    Ray ray = Camera.main.ScreenPointToRay(screenPos);
    RaycastHit hit;

    Physics.Raycast(
      origin: ray.origin,
      direction: ray.direction,
      hitInfo: out hit,
      maxDistance: Mathf.Infinity,
      layerMask: this.layerMask.value
    );

    if (hit.collider == null)
    {
      return new Vector2(-1, -1);
    }

    Vector3 offset = hit.point - originPosition;
    Vector2 vec2offset = new Vector2(offset.x, offset.z);
    float x = vec2offset.x / (cellSize);
    float y = vec2offset.y / (cellSize);

    return new Vector2(Mathf.FloorToInt(x), Mathf.FloorToInt(y));
  }

  SpriteRenderer CreateSpriteRenderer(CellData data, Transform parent = null, Vector3 localPos = default(Vector3), int sortingOrder = 0)
  {
    GameObject gameObject = new GameObject("world_sprite", typeof(SpriteRenderer));
    Transform transform = gameObject.transform;
    transform.SetParent(parent, false);

    //scale x and y by 100
    transform.localScale = new Vector3(200, 200, 1);
    //rotate about y axis 45 degrees
    transform.localEulerAngles = new Vector3(0, 45, 0);
    //make sure the bottom is above the gorund
    transform.localPosition = new Vector3(localPos.x, localPos.y + 0.5f, localPos.z);

    SpriteRenderer spriteRenderer = gameObject.GetComponent<SpriteRenderer>();
    // transform.localPosition = localPos;
    spriteRenderer.sortingOrder = sortingOrder;

    return spriteRenderer;
  }

  private TextMesh CreateWorldText(CellData data, Transform parent = null, Vector3 localPos = default(Vector3), int fontSize = 40, Color color = default(Color), TextAnchor textAnchor = TextAnchor.MiddleCenter, int sortingOrder = 0)
  {
    if (parent == null) parent = this.parent;

    GameObject gameObject = new GameObject("World_Text", typeof(TextMesh));

    Transform transform = gameObject.transform;
    transform.SetParent(parent, false);
    transform.localPosition = localPos;

    //make it flat on the xy plane
    transform.localEulerAngles = new Vector3(90, 0, 0);

    TextMesh textMesh = gameObject.GetComponent<TextMesh>();

    textMesh.anchor = textAnchor;
    textMesh.alignment = TextAlignment.Center;
    textMesh.text = data.value.ToString();
    textMesh.fontSize = fontSize;
    textMesh.color = color;
    textMesh.GetComponent<MeshRenderer>().sortingOrder = sortingOrder;

    return textMesh;
  }

  public void SetValue(int x, int y, CellData data)
  {
    if (x >= 0 && y >= 0 && x < width && y < height)
    {
      data.renderers = data.renderers == null ? this.gridArray[x, y].renderers : data.renderers;
      this.gridArray[x, y] = data;
      UpdateGridRendering();
    }
  }

  public void SetValue(Vector3 worldPos, CellData value)
  {
    Vector2 xy = GetXY(worldPos);

    if (xy.x < 0 && xy.y < 0) return;

    SetValue((int)xy.x, (int)xy.y, value);
  }

  public void SetValue(Vector3 worldPos, string fieldName, object value)
  {
    Vector2 xy = GetXY(worldPos);

    if (xy.x < 0 && xy.y < 0) return;

    CellData data = GetValue((int)xy.x, (int)xy.y);

    data?.GetType()?.GetField(fieldName)?.SetValue(data, value);

    SetValue((int)xy.x, (int)xy.y, data);
  }

  public void UpdateGridRendering()
  {
    for (int x = 0; x < gridArray.GetLength(0); x++)
    {
      for (int y = 0; y < gridArray.GetLength(1); y++)
      {
        //for now it just keeps numbers going, will eventually update sprites too
        if (gridArray[x, y].renderers.textMesh != null)
        {
          Debug.Log("updating text");
          gridArray[x, y].renderers.textMesh.text = gridArray[x, y].value.ToString();
        }

        if (gridArray[x, y].renderers.spriteRenderer != null)
        {
          Debug.Log("updating sprite");
          gridArray[x, y].renderers.spriteRenderer.sprite = gridArray[x, y].sprite;

          //make sure the sprite renderes lowest y value is still above its layer
          Vector3 currentPos = gridArray[x, y].renderers.spriteRenderer.transform.position;
          Bounds bounds = gridArray[x, y].renderers.spriteRenderer.bounds;

          if (currentPos.y <= originPosition.y + bounds.size.y / 2)
          {
            currentPos.y = originPosition.y + bounds.size.y / 2;
          }

          gridArray[x, y].renderers.spriteRenderer.transform.position = currentPos;
        }
      }
    }
  }

  public CellData GetValue(int x, int y)
  {
    if (x >= 0 && y >= 0 && x < width && y < height)
    {
      return this.gridArray[x, y];
    }
    else
    {
      return null;
    }
  }

  public CellData GetValue(Vector2 screenPos)
  {
    Vector2 xy = GetXY(screenPos);
    return GetValue((int)xy.x, (int)xy.y);
  }
}

