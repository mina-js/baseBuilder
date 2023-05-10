using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using static UnityEngine.InputSystem.InputAction;

public class GameController : MonoBehaviour
{
  public InputActionAsset inputAction;
  public Transform debugTextParent;
  public Transform renderersParent;
  public Transform highlightsParent;
  public GridLayer layerToTarget;
  public List<GridLayer> gridLayers;
  public Sprite spriteToPlace;
  public int numRows = 20;
  public int numCols = 20;
  public Upgradeable upgradeable;
  bool isUpgrading;

  [Range(0, 2)]
  public int upgradeableChoiceIdx;

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
    upgradeableChoiceIdx = 0;
    isUpgrading = false;
    inputAction.FindActionMap("Building").FindAction("Click").performed += ctx => MouseClick(ctx);
  }

  void MouseClick(CallbackContext ctx)
  {
    Vector2 mousePosition = inputAction.FindAction("Pos").ReadValue<Vector2>();
    layerToTarget.grid.SetValue(mousePosition, "sprite", spriteToPlace);
  }

  void Update()
  {
    HideOrShowLayers();
    if (upgradeable != null && !isUpgrading) HighlightUpgradeable();
    if (upgradeable == null && isUpgrading)
    {
      isUpgrading = false;
      layerToTarget.RemoveCellHighlights();
    }
    if (isUpgrading)
    {
      layerToTarget.placeSpriteAtCell(upgradeable.cell, upgradeable.optionSprites[upgradeableChoiceIdx]);
    }

  }

  void HighlightUpgradeable()
  {
    isUpgrading = true;
    GridLayer gridWithUpgradeable = gridLayers.Find(gridLayer => gridLayer.id == upgradeable.gridId);
    layerToTarget = gridWithUpgradeable;
    layerToTarget.HighlightCell(upgradeable.cell);
  }

  void HideOrShowLayers()
  {
    foreach (GridLayer gridLayer in gridLayers)
    {
      if (gridLayer == layerToTarget && !gridLayer.gameObject.activeSelf)
      {
        gridLayer.gameObject.SetActive(true);
        gridLayer.debugParent.gameObject.SetActive(true);
      }
      else if (gridLayer != layerToTarget && gridLayer.gameObject.activeSelf)
      {
        gridLayer.gameObject.SetActive(false);
        gridLayer.debugParent.gameObject.SetActive(false);

      }
    }
  }
}
