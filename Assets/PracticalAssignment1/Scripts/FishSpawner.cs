using UnityEngine;

public class FishSpawner : MonoBehaviour
{
    private Camera cam;
    public GameObject fishPrefab;

    void Start()
    {
        cam = Camera.main;
        //fishPrefab = Resources.Load<GameObject>("FISH"); no esta a aquesta ruta
    }

    void Update()
    {
        if (Input.GetMouseButtonDown(1)) // click dret
        {
            var position = cam.ScreenToWorldPoint(Input.mousePosition);
            position.z = 0;
            GameObject fish = GameObject.Instantiate(fishPrefab);
            fish.transform.position = position;
            //fish.transform.Rotate(0, 0, Random.value * 360);
        }
    }
}
