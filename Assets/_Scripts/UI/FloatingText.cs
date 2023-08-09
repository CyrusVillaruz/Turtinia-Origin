using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class FloatingText : MonoBehaviour
{
    public TextMeshProUGUI textMesh;
    public Vector3 floatDirection;
    public float duration = 0.5f;
    public float floatSpeed = 500f;

    float timeElapsed = 0.0f;
    RectTransform rectTransform;
    Color startingColour;

    private void Start()
    {
        rectTransform= GetComponent<RectTransform>();
        floatDirection = new Vector3(0, 1, 0);
        startingColour = textMesh.color;
    }

    private void Update()
    {
        timeElapsed += Time.deltaTime;

        rectTransform.position += floatDirection * floatSpeed * Time.deltaTime;

        textMesh.color = new Color(startingColour.r, startingColour.g, startingColour.b, 1 - (timeElapsed / duration));

        if (timeElapsed >= duration)
        {
            Destroy(gameObject);
        }
    }
}
