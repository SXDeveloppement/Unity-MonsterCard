using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScrollBarDisplay : MonoBehaviour
{
    public GameObject background;
    public GameObject cursor;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void initScrollbar(int maxPosition, int actualPosition) {
        if (maxPosition > 1) {
            gameObject.SetActive(true);
            float maxHeight = background.GetComponent<SpriteRenderer>().size.y * 0.99f;
            float cursorHeight = maxHeight / maxPosition;
            float firstPosition = (maxHeight - cursorHeight) / 2;
            float cursorPosition = firstPosition - cursorHeight * (actualPosition - 1);
            cursor.GetComponent<SpriteRenderer>().size = new Vector2(cursor.GetComponent<SpriteRenderer>().size.x, cursorHeight);
            cursor.transform.localPosition = new Vector3(cursor.transform.localPosition.x, cursorPosition, cursor.transform.localPosition.z);
        } else {
            gameObject.SetActive(false);
        }
    }
}
