using UnityEngine;

/// <summary>
/// Some useful extension methods for the VectorX types.
/// </summary>
static public class VectorExtensions
{
    static public Vector3 WithX(this Vector3 v, float x) { return new Vector3(x, v.y, v.z); }
    static public Vector3 WithY(this Vector3 v, float y) { return new Vector3(v.x, y, v.z); }
    static public Vector3 WithZ(this Vector3 v, float z) { return new Vector3(v.x, v.y, z); }

    static public Vector2 WithX(this Vector2 v, float x) { return new Vector2(x, v.y); }
    static public Vector2 WithY(this Vector2 v, float y) { return new Vector2(v.x, y); }

    static public Vector3 AsVector3WithX(this Vector2 v, float x) { return new Vector3(x, v.x, v.y); }
    static public Vector3 AsVector3WithY(this Vector2 v, float y) { return new Vector3(v.x, y, v.y); }
    static public Vector3 AsVector3WithZ(this Vector2 v, float z) { return new Vector3(v.x, v.y, z); }
}