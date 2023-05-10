using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class GridUtils
{

  public static SpriteRenderer CreateSpriteRenderer(CellData data, Transform parent = null, Vector3 localPos = default(Vector3), int sortingOrder = 0)
  {
    GameObject gameObject = new GameObject("world_sprite", typeof(SpriteRenderer));
    Transform transform = gameObject.transform;
    transform.SetParent(parent, false);

    //scale x and y by 100
    transform.localScale = new Vector3(100, 100, 1);
    //rotate about y axis 45 degrees
    transform.localEulerAngles = new Vector3(0, 45, 0);
    //make sure the bottom is above the gorund
    transform.localPosition = new Vector3(localPos.x, localPos.y + 0.5f, localPos.z);

    SpriteRenderer spriteRenderer = gameObject.GetComponent<SpriteRenderer>();
    // transform.localPosition = localPos;
    spriteRenderer.sortingOrder = sortingOrder;

    return spriteRenderer;
  }

  public static TextMesh CreateWorldText(CellData data, Transform parent = null, Vector3 localPos = default(Vector3), int fontSize = 40, Color color = default(Color), TextAnchor textAnchor = TextAnchor.MiddleCenter, int sortingOrder = 0)
  {
    //if (parent == null) parent = this.parent;

    GameObject gameObject = new GameObject("World_Text", typeof(TextMesh));

    Transform transform = gameObject.transform;
    transform.SetParent(parent, false);
    transform.localPosition = localPos;

    //make it flat on the xy plane
    transform.localEulerAngles = new Vector3(90, 0, 0);

    TextMesh textMesh = gameObject.GetComponent<TextMesh>();

    textMesh.anchor = textAnchor;
    textMesh.alignment = TextAlignment.Center;
    textMesh.text = data.value.ToString();
    textMesh.fontSize = fontSize;
    textMesh.color = color;
    textMesh.GetComponent<MeshRenderer>().sortingOrder = sortingOrder;

    return textMesh;
  }

}
