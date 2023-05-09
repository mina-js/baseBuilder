using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using static UnityEngine.InputSystem.InputAction;

public class GameController : MonoBehaviour
{
  public InputActionAsset inputAction;
  public Transform parent;
  public GridLayer layerToTarget;
  public List<GridLayer> gridLayers;
  public Sprite spriteToPlace;
  public int numRows = 20;
  public int numCols = 20;

  void OnEnable()
  {
    inputAction.Enable();
  }

  void OnDisable()
  {
    inputAction.Disable();
  }

  void Start()
  {
    inputAction.FindActionMap("Building").FindAction("Click").performed += ctx => MouseClick(ctx);
  }

  void MouseClick(CallbackContext ctx)
  {
    Vector2 mousePosition = inputAction.FindAction("Pos").ReadValue<Vector2>();
    layerToTarget.grid.SetValue(mousePosition, "sprite", spriteToPlace);
  }

  void Update()
  {
    //deactivate any grid layers that aren't the active one
    foreach (GridLayer gridLayer in gridLayers)
    {
      if (gridLayer == layerToTarget && !gridLayer.gameObject.activeSelf)
      {
        gridLayer.gameObject.SetActive(true);
      }
      else if (gridLayer != layerToTarget && gridLayer.gameObject.activeSelf)
      {
        gridLayer.gameObject.SetActive(false);
      }
    }
  }
}
