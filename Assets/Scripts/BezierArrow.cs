using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BezierArrow : MonoBehaviour
{
    #region Public Fields
    [Tooltip("The prefab of arrow head")]
    public GameObject ArrowHeadPrefab;

    [Tooltip("The prefab of arrow node")]
    public GameObject ArrowNodePrefab;

    [Tooltip("The number of arrow node")]
    public int arrowNodeNum;

    [Tooltip("The scale multiplier for arrow nodes")]
    public float scaleFactor = 1f;

    [Tooltip("Color for invalid target")]
    public Color colorInvalidTarget;

    [Tooltip("Color for valid target")]
    public Color colorValidTarget;
    #endregion

    #region Private Fields
    /// <summary>
    /// The position of P0 (the arrow emitter point)
    /// </summary>
    private RectTransform origin;

    /// <summary>
    /// The list of arrow nodes transform
    /// </summary>
    private List<RectTransform> arrowNodes = new List<RectTransform>();

    /// <summary>
    /// The list of control points
    /// </summary>
    private List<Vector2> controlPoints = new List<Vector2>();

    /// <summary>
    /// The factor to determine the position of control point P1, P2
    /// </summary>
    private readonly List<Vector2> controlPointFactors = new List<Vector2> { new Vector2(-0.3f, 0.8f), new Vector2(0.1f, 1.4f) };
    #endregion

    #region Private Methods
    /// <summary>
    /// Executes when the gameObject instantiates
    /// </summary>
    private void Awake() {
        // Gets position of the arrows emitter point
        this.origin = this.GetComponent<RectTransform>();

        // Instantiates the arrow nodes and arrow head
        for (int i = 0; i < this.arrowNodeNum; i++) {
            this.arrowNodes.Add(Instantiate(this.ArrowNodePrefab, this.transform).GetComponent<RectTransform>());
        }

        this.arrowNodes.Add(Instantiate(this.ArrowHeadPrefab, this.transform).GetComponent<RectTransform>());

        // Hides the arrow nodes
        this.arrowNodes.ForEach(a => a.GetComponent<RectTransform>().position = new Vector2(-1000, -1000));

        // Initializes the control points list
        for (int i = 0; i < 4; i++) {
            this.controlPoints.Add(Vector2.zero);
        }
    }

    /// <summary>
    /// Executes every frame
    /// </summary>
    private void Update() {
        // P0 is at the arrow emitter point
        this.controlPoints[0] = new Vector2(this.origin.position.x, this.origin.position.y);

        // P3 is at the mouse position
        Vector3 mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        this.controlPoints[3] = new Vector2(mousePosition.x, mousePosition.y);

        // P1, P2 determines by P0 and P3
        // P1 = P0 + (P3 - P0) * Vector2(-0.3f, 0.8f)
        // P2 = P0 + (P3 - P0) * Vector2(0.1f, 1.4f)
        this.controlPoints[1] = this.controlPoints[0] + (this.controlPoints[3] - this.controlPoints[0]) * this.controlPointFactors[0];
        this.controlPoints[2] = this.controlPoints[0] + (this.controlPoints[3] - this.controlPoints[0]) * this.controlPointFactors[1];

        for (int i = 0; i < this.arrowNodes.Count; i++) {
            // calculate t
            float t = Mathf.Log(1f * i / (this.arrowNodes.Count - 1) + 1f, 2f);

            // Cubic Bezier curve
            // B(t) = (1-t)^3 * P0 + 3 * (1-t)^2 * t * P1 + 3 * (1-t) * t^2 * P2 + t^3 * P3
            this.arrowNodes[i].position =
                Mathf.Pow(1 - t, 3) * this.controlPoints[0] +
                3 * Mathf.Pow(1 - t, 2) * t * this.controlPoints[1] +
                3 * (1 - t) * Mathf.Pow(t, 2) * this.controlPoints[2] +
                Mathf.Pow(t, 3) * this.controlPoints[3];

            this.arrowNodes[i].localPosition = new Vector3(this.arrowNodes[i].localPosition.x, this.arrowNodes[i].localPosition.y, 0);

            // calculates rotations for each arrow node
            if (i > 0) {
                Vector3 euler = new Vector3(0, 0, Vector2.SignedAngle(Vector2.up, this.arrowNodes[i].position - this.arrowNodes[i - 1].position));
                this.arrowNodes[i].rotation = Quaternion.Euler(euler);
            }

            // calculates scales for each arrow node
            float scale = this.scaleFactor * (1 - 0.03f * (this.arrowNodes.Count - 1 - i));
            this.arrowNodes[i].localScale = new Vector3(scale, scale, 1f);
        }

        // the first arrow node's rotation
        this.arrowNodes[0].transform.rotation = this.arrowNodes[1].transform.rotation;
    }
    #endregion

    #region Public Methods
    /// <summary>
    /// Change la couleur des fleches en fonction de la validité de la cible
    /// </summary>
    public void changeColor(bool targetIsValid) {
        Color color;
        if (targetIsValid) {
            color = colorValidTarget;
        } else {
            color = colorInvalidTarget;
        }

        foreach (SpriteRenderer spriteRender in transform.GetComponentsInChildren<SpriteRenderer>()) {
            spriteRender.color = color;
        }
    }

    #endregion

}
