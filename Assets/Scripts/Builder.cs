using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Builder
{
  CellGrid grid;
  Transform newBuild;
  Transform parent;
  GridLayer layer; //only for coroutine stuff
  bool isAdding = false;
  float timeOutForAdding = 0.1f;

  public Builder(CellGrid grid, Transform parent, GridLayer layer)
  {
    this.grid = grid;
    this.parent = parent;
    this.layer = layer;
  }

  public void startBuilding(Vector2 screenPos)
  {
    AddCubeAtWorldPos(screenPos);
  }

  public void dragWhileBuilding(Vector2 screenPos)
  {
    //TODO: maybe expand the shape instead of just adding more? or collapse them when added? hmm..
    AddCubeAtWorldPos(screenPos);
  }

  public void endBuilding()
  {
    newBuild = null;
  }

  IEnumerator SetIsAddingToFalse()
  {
    yield return new WaitForSeconds(timeOutForAdding);
    isAdding = false;
  }

  void AddCubeAtWorldPos(Vector2 screenPos)
  {
    if (isAdding) return;

    isAdding = true;
    layer.StartCoroutine(SetIsAddingToFalse());

    Vector2 gridPos = grid.GetXZ(screenPos);
    Vector3 worldPos = grid.GetSnappedWorldPos(screenPos);
    CellData cellDataAtPos = grid.GetValue((int)gridPos.x, (int)gridPos.y);

    int numBlocks = cellDataAtPos == null ? 0 : cellDataAtPos.numBlocks;
    newBuild = GameObject.CreatePrimitive(PrimitiveType.Cube).transform;

    //bounce the y position up by the numBlocks * height of the block
    newBuild.position = new Vector3(worldPos.x, worldPos.y + (numBlocks * grid.cellSize), worldPos.z);

    newBuild.localScale = new Vector3(grid.cellSize, grid.cellSize, grid.cellSize);
    newBuild.SetParent(parent, false);

    grid.SetValue((int)gridPos.x, (int)gridPos.y, "numBlocks", numBlocks + 1);
  }
}
