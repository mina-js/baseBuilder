using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Builder
{
  CellGrid grid;

  public Builder(CellGrid grid)
  {
    this.grid = grid;
  }

  public void startBuilding(Vector2 screenPos)
  {
    Debug.Log("STARTED " + screenPos);
  }

  public void dragWhileBuilding(Vector2 screenPos)
  {
    Debug.Log("DRAGGED " + screenPos);
  }

  public void endBuilding()
  {
    Debug.Log("ENDED");
  }
}
