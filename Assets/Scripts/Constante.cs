using UnityEngine;
using UnityEditor;

public class Constante {
    #region Public constante
    public const float SCALE_CARD_HAND = 1;
    public const float SCALE_CARD_HAND_ZOOM = 1.5f;
    public const float SCALE_CARD_DRAG_HAND = 1;
    public const float SCALE_CARD_DRAG_BOARD = 0.7f;
    public const float SCALE_CARD_BOARD = 0.7f;
    public const float SCALE_CARD_BOARD_ZOOM = 1.4f;
    public const float SCALE_CARD_ACTION = 0.7f;
    public const float SCALE_CARD_ACTION_ZOOM = 1.4f;

    public const float SCALE_ABILITY_BOARD = 0.9f;
    public const float SCALE_ABILITY_BOARD_ZOOM = 1.4f;
    public const float SCALE_ABILITY_ACTION = 0.7f;
    public const float SCALE_ABILITY_ACTION_ZOOM = 1.4f;

    public const float SCALE_ABILITY_TEAM = 0.7f;
    public const float SCALE_ABILITY_TEAM_ZOOM = 1.4f;
    #endregion

    public static Vector3 FlatScale(float floatScale) {
        return new Vector3(floatScale, floatScale, floatScale);
    }

    public static Vector3 ScaleComparedParent(float floatScale, GameObject GO_parent) {
        return new Vector3(
            floatScale / GO_parent.transform.lossyScale.x,
            floatScale / GO_parent.transform.lossyScale.y,
            floatScale / GO_parent.transform.lossyScale.z
            );
    }
}