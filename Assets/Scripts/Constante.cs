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

    public const int TIME_MULLIGAN = 20; // Temps pour faire son mulligan
    public const float MULLIGAN_REFRESH_RATE = 0.5f; // Temps de latence max de la validation du mulligan (en seconde)
    public const int MULLIGAN_SHOW_TIMER_IF_ABOVE = 10; // Affiche le timer du mulligan si il reste moins de temps que la valeur
    public const int TIME_PHASE = 10; // Temps d'une phase en seconde
    public const int MAX_CARD_IN_HAND = 8; // Le nombre de carte maximum que l'on peut avoir dans la main d'un joueur
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