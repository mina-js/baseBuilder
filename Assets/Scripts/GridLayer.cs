using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GridLayer : MonoBehaviour
{
  public string id;
  MeshRenderer meshRenderer;
  LayerMask layerMask;
  public CellGrid grid;
  GameController gameController;
  public Transform debugParent;
  public Transform renderersParent;
  public GameObject highlightPrefab;

  public Builder builder;

  void Start()
  {
    layerMask = LayerMask.GetMask(gameObject.name.ToLower());
    gameController = GameObject.Find("GameController").GetComponent<GameController>();
    meshRenderer = GetComponent<MeshRenderer>();
    InitializeGrid();
    builder = new Builder(grid, gameController.buildParent, this);
  }

  void InitializeGrid()
  {
    //todo make this pull up something you saved instead of starting over every time

    Bounds bounds = meshRenderer.bounds;
    float cellSize = bounds.size.x / gameController.numCols;
    Vector3 originPoint = new Vector3(bounds.min.x, meshRenderer.transform.position.y, bounds.min.z);

    debugParent = gameController.debugTextParent.Find(id);
    renderersParent = gameController.renderersParent.Find(id);

    grid = new CellGrid(id, gameController.numCols, gameController.numRows, cellSize, originPoint, debugParent, renderersParent, layerMask, gameController.isDebugging);
  }

  public void HighlightCell(Vector2Int cell)
  {
    Debug.Log("should highlight " + cell);
    Vector3 worldPos = grid.GetWorldPosition((int)cell.x, (int)cell.y);

    //shift to center in the cell using grid.cellsize
    worldPos.x += grid.cellSize / 2;
    worldPos.z += grid.cellSize / 2;

    //instantiate the highlightPrefab at worldPos and rotate by 90 degrees on x axis
    GameObject highlight = Instantiate(highlightPrefab, worldPos, Quaternion.Euler(90, 0, 0));
    highlight.transform.SetParent(gameController.highlightsParent, false);
  }

  public void PlaceSpriteAtCell(Vector2Int cell, Sprite sprite, Vector2 XZOffsetFromTileCenter)
  {
    grid.SetValue(cell.x, cell.y, "sprite", sprite, XZOffsetFromTileCenter);
  }

  public void RemoveCellHighlights()
  {
    foreach (Transform child in gameController.highlightsParent)
    {
      Destroy(child.gameObject);
    }
  }
}
