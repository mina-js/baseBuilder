using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Upgradeable", menuName = "ScriptableObjects/Upgradeable", order = 1)]
public class Upgradeable : ScriptableObject
{
  public string gridId;
  public Vector2Int cell;
  public Vector2 XZOffsetFromTileCenter;
  public List<Sprite> optionSprites;
  public Upgradeable(string gridId, Vector2Int cell)
  {
    this.gridId = gridId;
    this.cell = cell;
  }
}
