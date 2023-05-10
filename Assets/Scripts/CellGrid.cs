using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CellGrid
{
  string id;
  int width;
  int height;
  public float cellSize;
  public bool isDebugging;
  Transform debugTextParent;
  Transform renderersParent;
  LayerMask layerMask;
  CellData[,] gridArray;
  Vector3 originPosition;

  public CellGrid(
    string id, int width, int height,
    float cellSize, Vector3 originPosition,
    Transform debugTextParent,
    Transform renderersParent,
    LayerMask layerMask,
    bool isDebugging
  )
  {
    this.id = id;

    this.width = width;
    this.height = height;
    this.cellSize = cellSize;
    this.originPosition = originPosition;

    this.debugTextParent = debugTextParent;
    this.renderersParent = renderersParent;

    this.layerMask = layerMask;

    this.isDebugging = isDebugging;

    gridArray = new CellData[width, height];

    CreateGrid();
  }

  void CreateGrid()
  {
    for (int x = 0; x < gridArray.GetLength(0); x++)
    {
      for (int y = 0; y < gridArray.GetLength(1); y++)
      {
        gridArray[x, y] = new CellData { id = (x + "_" + y).ToString(), value = x * width + y, renderers = new CellRenderers() };
        Vector3 adjustedCenterPositionOfTile = GetWorldPosition(x, y) + new Vector3(cellSize, 0, cellSize) * 0.5f;
        //TODO: the grid appearing is actually useful, but needs to bea ble to turn off easily, figure it out baby
        if (isDebugging) gridArray[x, y].renderers.textMesh = GridUtils.CreateWorldText(gridArray[x, y], debugTextParent, adjustedCenterPositionOfTile, 350, Color.white, TextAnchor.MiddleCenter, 0);
        gridArray[x, y].renderers.spriteRenderer = GridUtils.CreateSpriteRenderer(gridArray[x, y], renderersParent, adjustedCenterPositionOfTile, 0);

        // Debug.DrawLine(GetWorldPosition(x, y), GetWorldPosition(x, y + 1), Color.white, 100f);
        // Debug.DrawLine(GetWorldPosition(x, y), GetWorldPosition(x + 1, y), Color.white, 100f);
      }
    }

    // Debug.DrawLine(GetWorldPosition(0, height), GetWorldPosition(width, height), Color.white, 100f);
    // Debug.DrawLine(GetWorldPosition(width, 0), GetWorldPosition(width, height), Color.white, 100f);
  }

  public Vector3 GetWorldPosition(int x, int z)
  {
    return new Vector3(x, 0, z) * cellSize + originPosition;
  }

  public Vector2 GetXZ(Vector2 screenPos)
  {
    Ray ray = Camera.main.ScreenPointToRay(screenPos);

    Physics.Raycast(
      origin: ray.origin,
      direction: ray.direction,
      hitInfo: out RaycastHit hit,
      maxDistance: Mathf.Infinity,
      layerMask: this.layerMask.value
    );

    if (hit.collider == null)
    {
      return new Vector2(-1, -1);
    }

    Vector3 offset = hit.point - originPosition;
    Vector2 vec2offset = new Vector2(offset.x, offset.z);
    vec2offset /= cellSize;

    return new Vector2(Mathf.FloorToInt(vec2offset.x), Mathf.FloorToInt(vec2offset.y));
  }

  //Base version of SetValue, other overloads just lead here
  public void SetValue(int x, int z, CellData data)
  {
    if (x >= 0 && z >= 0 && x < width && z < height)
    {
      data.renderers = data.renderers == null ? this.gridArray[x, z].renderers : data.renderers;
      this.gridArray[x, z] = data;
      TriggerGridCellChanged(x, z);
    }
  }

  public void SetValue(Vector3 worldPos, CellData value)
  {
    Vector2 xz = GetXZ(worldPos);

    if (xz.x < 0 && xz.y < 0) return;

    SetValue((int)xz.x, (int)xz.y, value);
  }

  public void SetValue(Vector3 worldPos, string fieldName, object value)
  {
    Vector2 xz = GetXZ(worldPos);

    if (xz.x < 0 && xz.y < 0) return;

    SetValue((int)xz.x, (int)xz.y, fieldName, value);
  }

  public void SetValue(int x, int y, string fieldName, object value)
  {
    CellData data = GetValue(x, y);

    data?.GetType()?.GetField(fieldName)?.SetValue(data, value);

    SetValue(x, y, data);
  }

  public void TriggerGridCellChanged(int x, int z)
  {
    //for now it just keeps numbers going, will eventually update sprites too
    if (isDebugging && gridArray[x, z].renderers.textMesh != null)
    {
      Debug.Log("updating text");
      gridArray[x, z].renderers.textMesh.text = gridArray[x, z].value.ToString();
    }

    if (gridArray[x, z].renderers.spriteRenderer != null)
    {
      Debug.Log("updating sprite");
      gridArray[x, z].renderers.spriteRenderer.sprite = gridArray[x, z].sprite;

      //make sure the sprite renderes lowest y value is still above its layer
      Vector3 currentPos = gridArray[x, z].renderers.spriteRenderer.transform.position;
      Bounds bounds = gridArray[x, z].renderers.spriteRenderer.bounds;

      if (currentPos.y <= originPosition.y + bounds.size.y / 2)
      {
        currentPos.y = originPosition.y + bounds.size.y / 2;
      }

      gridArray[x, z].renderers.spriteRenderer.transform.position = currentPos;

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
    Vector2 xz = GetXZ(screenPos);
    return GetValue((int)xz.x, (int)xz.y);
  }
}

