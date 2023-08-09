using UnityEngine;

public class PlantIndicator : MonoBehaviour
{
    public GameObject indicatorPrefab;
    private GameObject indicatorInstance;

    void Update()
    {
        Vector3 objectViewportPos = Camera.main.WorldToViewportPoint(transform.position);

        if (objectViewportPos.x < 0 || objectViewportPos.x > 1 || objectViewportPos.y < 0 || objectViewportPos.y > 1)
        {
            if (indicatorInstance == null) indicatorInstance = Instantiate(indicatorPrefab, Vector3.zero, Quaternion.identity);

            Vector3 indicatorViewportPos = new Vector3(Mathf.Clamp(objectViewportPos.x, 0.05f, 0.95f), Mathf.Clamp(objectViewportPos.y, 0.05f, 0.95f), objectViewportPos.z);
            Vector3 indicatorWorldPos = Camera.main.ViewportToWorldPoint(indicatorViewportPos);
            indicatorInstance.transform.position = Vector3.Slerp(indicatorInstance.transform.position, indicatorWorldPos, Time.deltaTime * 10f);
        }
        else
        {
            if (indicatorInstance != null) Destroy(indicatorInstance);
        }
    }
}
