using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GridLayer : MonoBehaviour
{
  MeshRenderer meshRenderer;
  LayerMask layerMask;
  public MyGrid grid;
  GameController gameController;

  void Start()
  {
    layerMask = LayerMask.GetMask(gameObject.name.ToLower());
    gameController = GameObject.Find("GameController").GetComponent<GameController>();
    meshRenderer = GetComponent<MeshRenderer>();
    InitializeGrid();
  }

  void InitializeGrid()
  {
    //todo make this pull up something you saved instead of starting over every time

    Bounds bounds = meshRenderer.bounds;
    float cellSize = bounds.size.x / gameController.numCols;
    Vector3 originPoint = new Vector3(bounds.min.x, meshRenderer.transform.position.y, bounds.min.z);
    grid = new MyGrid(gameController.numCols, gameController.numRows, cellSize, originPoint, gameController.parent, layerMask);
  }

  //TODO: a method that hides the mesh if not the active layer, so you can see other stuff?
}
