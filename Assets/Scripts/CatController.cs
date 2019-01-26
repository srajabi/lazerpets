using UnityEngine;

public class CatController : MonoBehaviour
{
    void Start()
    {
        
    }

    void Update()
    {
        transform.position += Vector3.up * Time.deltaTime;
    }
}
