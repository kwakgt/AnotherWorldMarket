using System.Collections;
using Unity.VisualScripting;
using UnityEngine;

public class PriceText : MonoBehaviour
{
    RectTransform rectTransform;
    float speed = 0.5f;
    int height = 2;
    Vector2 start = Vector2.up;

    void Awake()
    {
        rectTransform = GetComponent<RectTransform>();
        rectTransform.localPosition = start;   //시작위치
    }

    void OnEnable()
    { 
        StartCoroutine(MoveUpText());
    }

    void OnDisable()
    {
        rectTransform.localPosition = start;   //시작위치
    }

    IEnumerator MoveUpText()
    {
        while (rectTransform.localPosition.y <= height)
        {
            rectTransform.Translate(start * Time.deltaTime * speed);
            yield return null;
        }

        gameObject.SetActive(false);
    }
}
