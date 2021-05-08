using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ManipulationScript : MonoBehaviour
{

    public Transform parent;
    public Vector3 originalScale;
    // Start is called before the first frame update
    void Start()
    {
        originalScale = parent.localScale;

    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
