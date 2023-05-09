using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using static UnityEngine.InputSystem.InputAction;

public class Testing : MonoBehaviour
{
  MyGrid grid;
  public InputActionAsset inputAction;
  public Transform parent;
  public MeshRenderer meshRenderer;
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

  // Start is called before the first frame update
  void Start()
  {
    Bounds bounds = meshRenderer.bounds;
    float cellSize = bounds.size.x / numCols;
    Vector3 originPoint = new Vector3(bounds.min.x, meshRenderer.transform.position.y, bounds.min.z);

    grid = new MyGrid(numCols, numRows, cellSize, originPoint, parent);

    inputAction.FindActionMap("Building").FindAction("Click").performed += ctx => MouseClick(ctx);
  }

  void MouseClick(CallbackContext ctx)
  {
    Vector2 mousePosition = inputAction.FindAction("Pos").ReadValue<Vector2>();
    grid.SetValue(mousePosition, new CellData { value = 666 });
  }
}
