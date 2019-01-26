using UnityEngine;

public class TheGame : MonoBehaviour
{
    static TheGame instance;
    public static TheGame Instance
    {
        get
        {
            if (instance == null)
            {
                instance = FindObjectOfType<TheGame>();
            }
            if (instance == null)
            {
                GameObject go = new GameObject();
                instance = go.AddComponent<TheGame>();
            }
            return instance;
        }
    }
}
