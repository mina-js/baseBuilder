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

  //hmm theres gotta be a better way
  public int numBlocks;
}
