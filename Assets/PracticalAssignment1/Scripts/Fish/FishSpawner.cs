using UnityEngine;

public class FishSpawner : MonoBehaviour
{
    private Camera cam;
    public GameObject fishPrefab;

    void Start()
    {
        cam = Camera.main;
    }

    void Update()
    {
        if (Input.GetMouseButtonDown(1))
        {
            var position = cam.ScreenToWorldPoint(Input.mousePosition);
            position.z = 0;
            GameObject fish = GameObject.Instantiate(fishPrefab);
            fish.transform.position = position;
        }
    }
}
