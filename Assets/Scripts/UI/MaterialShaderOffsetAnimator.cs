using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MaterialShaderOffsetAnimator : MonoBehaviour
{
    // Scroll main texture based on time

    [SerializeField]
    Vector2 ScrollOffset;
    Renderer rend;

    void Start()
    {
        rend = GetComponent<Renderer>();
    }

    void Update()
    {
        Vector2 offset = Time.time * ScrollOffset;
        rend.material.SetTextureOffset("_MainTex", offset);
    }
}
