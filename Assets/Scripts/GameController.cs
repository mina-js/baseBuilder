using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using static UnityEngine.InputSystem.InputAction;

public enum PlacementMode
{
  None,
  Building,
  Upgrading
}

public class GameController : MonoBehaviour
{
  public PlacementMode placementMode;
  public InputActionAsset inputAction;
  public Transform debugTextParent;
  public Transform renderersParent;
  public Transform highlightsParent;
  public GridLayer layerToTarget;
  public List<GridLayer> gridLayers;
  public Sprite spriteToPlace;
  public int numRows = 20;
  public int numCols = 20;
  public bool isDebugging = true;
  public Upgradeable upgradeable;
  bool isUpgrading;

  public Vector2 screenPos;

  bool isTouching;

  [Range(0, 2)]
  public int upgradeableChoiceIdx;


  void Start()
  {
    isTouching = false;

    upgradeableChoiceIdx = 0;
    isUpgrading = upgradeable != null;

    BindInputActions();
  }

  void BindInputActions()
  {
    inputAction.FindActionMap("Building").FindAction("Click").performed += ctx => { StartCoroutine(OnTouch(ctx)); };
    inputAction.FindActionMap("Building").FindAction("Click").canceled += ctx => { isTouching = false; };

    inputAction.FindAction("Pos").performed += context => screenPos = context.ReadValue<Vector2>();
  }

  IEnumerator OnTouch(CallbackContext ctx)
  {
    OnStartOfTouch(ctx);

    while (isTouching)
    {
      OnDrag(ctx);
      yield return null;
    }

    OnTouchEnd(ctx);
  }

  void OnStartOfTouch(CallbackContext ctx = default(CallbackContext))
  {
    isTouching = true;

    if (isAddingUpgrades())
    {
      AddUpgrade();
    }
    else if (isBuilding())
    {
      layerToTarget.builder.startBuilding(screenPos);
    }
  }

  void OnDrag(CallbackContext ctx = default(CallbackContext))
  {
    if (!isBuilding()) return;
    layerToTarget.builder.dragWhileBuilding(screenPos);
  }

  void OnTouchEnd(CallbackContext ctx = default(CallbackContext))
  {
    isTouching = false;
    if (!isBuilding()) return;
    layerToTarget.builder.endBuilding();
  }

  //TODO: this would be from a button press or something
  void OnValidate()
  {
    if (isUpgrading && layerToTarget.id == upgradeable.gridId && isAddingUpgrades())
      layerToTarget.PlaceSpriteAtCell(upgradeable.cell, upgradeable.optionSprites[upgradeableChoiceIdx], upgradeable.XZOffsetFromTileCenter);
  }

  void AddUpgrade()
  {
    float centeringOffset = layerToTarget.grid.cellSize / 2f; //default to centering them
    layerToTarget.grid.SetValue(screenPos, "sprite", spriteToPlace, new Vector2(centeringOffset, centeringOffset));
  }

  void Update()
  {
    HighlightUpgradeable();
    HideOrShowLayers();
  }

  void HighlightUpgradeable()
  {
    if (upgradeable != null && !isUpgrading)
    {
      isUpgrading = true;
      GridLayer gridWithUpgradeable = gridLayers.Find(gridLayer => gridLayer.id == upgradeable.gridId);
      layerToTarget = gridWithUpgradeable;
      layerToTarget.HighlightCell(upgradeable.cell);
    }
    else if (upgradeable == null && isUpgrading)
    {
      isUpgrading = false;
      layerToTarget.RemoveCellHighlights();
    }
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

  void OnEnable()
  {
    inputAction.Enable();
  }

  void OnDisable()
  {
    inputAction.Disable();
  }

  bool isAddingUpgrades()
  {
    return placementMode == PlacementMode.Upgrading;
  }

  bool isBuilding()
  {
    return placementMode == PlacementMode.Building;
  }
}
