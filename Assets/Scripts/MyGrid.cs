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
}


//this will eventually include a sprite renderer, maybe mesh renderers?
public class CellRenderers
{
  [field: SerializeField]
  public TextMesh textMesh;
}

public class MyGrid
{
  int width;
  int height;
  float cellSize;
  Transform parent;
  CellData[,] gridArray;
  Vector3 originPosition;

  public MyGrid(int width, int height, float cellSize, Vector3 originPosition, Transform parent)
  {
    this.width = width;
    this.height = height;
    this.cellSize = cellSize;
    this.originPosition = originPosition;
    this.parent = parent;

    gridArray = new CellData[width, height];
    // debugTextArray = new TextMesh[width, height];

    for (int x = 0; x < gridArray.GetLength(0); x++)
    {
      for (int y = 0; y < gridArray.GetLength(1); y++)
      {
        gridArray[x, y] = new CellData { id = (x + "_" + y).ToString(), value = x * width + y, renderers = new CellRenderers() };
        gridArray[x, y].renderers.textMesh = CreateWorldText(gridArray[x, y], null, GetWorldPosition(x, y) + new Vector3(cellSize, 0, cellSize) * 0.5f, 350, Color.white, TextAnchor.MiddleCenter, 0);
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
    Physics.Raycast(ray.origin, ray.direction, out hit, Mathf.Infinity);

    if (hit.collider == null)
    {
      Debug.Log("no hit");
      return new Vector2(-1, -1);
    }

    Vector3 offset = hit.point - originPosition;
    Vector2 vec2offset = new Vector2(offset.x, offset.z);
    float x = vec2offset.x / (cellSize);
    float y = vec2offset.y / (cellSize);

    return new Vector2(Mathf.FloorToInt(x), Mathf.FloorToInt(y));
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
        else
        {
          Debug.Log("textmesh is null for " + x + ", " + y);
        }
      }
    }
  }

  public void SetValue(Vector3 worldPos, CellData value)
  {
    Vector2 xy = GetXY(worldPos);
    SetValue((int)xy.x, (int)xy.y, value);
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
