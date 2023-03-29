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

    public static Vector3 ScaleComparedParent(float floatScale, GameObject GO) {
        GameObject GO_parent = GO.transform.parent.gameObject;
        int coefX = 1, coefY = 1, coefZ = 1;
        if (GO.transform.lossyScale.x < 0) coefX = -1;
        if (GO.transform.lossyScale.y < 0) coefY = -1;
        if (GO.transform.lossyScale.z < 0) coefZ = -1;
        return new Vector3(
            floatScale / GO_parent.transform.lossyScale.x * coefX,
            floatScale / GO_parent.transform.lossyScale.y * coefY,
            floatScale / GO_parent.transform.lossyScale.z * coefZ
            );
        //return new Vector3(
        //    floatScale / GO_parent.transform.lossyScale.x,
        //    floatScale / GO_parent.transform.lossyScale.y,
        //    floatScale / GO_parent.transform.lossyScale.z
        //    );
    }
}