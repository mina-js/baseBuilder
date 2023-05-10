using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Builder
{
  CellGrid grid;

  Transform newBuild;

  public Builder(CellGrid grid)
  {
    this.grid = grid;
  }

  public void startBuilding(Vector2 screenPos)
  {
    Vector3 worldPos = grid.GetSnappedWorldPos(screenPos);
    AddCubeAtWorldPos(worldPos);
  }

  public void dragWhileBuilding(Vector2 screenPos)
  {
    //TODO: maybe expand the shape instead of just adding more? or collapse them when added? hmm..
    Vector3 worldPos = grid.GetSnappedWorldPos(screenPos);
    AddCubeAtWorldPos(worldPos);
  }

  public void endBuilding()
  {
    Debug.Log("ENDED");
    newBuild = null;
  }

  void AddCubeAtWorldPos(Vector3 worldPos)
  {
    newBuild = GameObject.CreatePrimitive(PrimitiveType.Cube).transform;
    newBuild.position = worldPos;
    newBuild.localScale = new Vector3(grid.cellSize, grid.cellSize, grid.cellSize);
  }
}
