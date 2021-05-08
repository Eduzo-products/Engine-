using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class explodeEffect : MonoBehaviour
{
    public GameObject gamObj;
    public int maxRange = 10;
    public float speed = 1f;
    // Start is called before the first frame update
    void Start ( )
    {
        //PublishContent.OnSaveContentClick += onSaveContent;
    }
    private void onSaveContent ( )
    {
        //  ScriptsManager._XRStudioContent.explodeEffect.enabled = 
    }

    // Update is called once per frame
    void Update ( )
    {
        // print((Vector3.forward + Vector3.up).normalized);
        // explode();
        //if (Input.GetKeyDown(KeyCode.A))
        //{
        //    targetPos();
        //}
    }
    public Vector3 origin = Vector3.zero;
    bool flag = true;

    public void targetPos ( )
    {
        //if (gamObj == null)
        //    gamObj = transform.parent.gameObject;
        //int count = gamObj.transform.childCount;
        //MeshRenderer[] mesh;
        //mesh = GetComponentsInChildren<MeshRenderer>();
        //for (int i = 0; i < mesh.Length; i++)
        //{
        //    positionScript local = mesh[i].gameObject.AddComponent<positionScript>();
        //    local.target_pos = new Vector3(Random.Range(-maxRange, maxRange), Random.Range(0, maxRange), Random.Range(-maxRange, maxRange));
        //    local.speed = speed;
        //    local.moveObject(mesh[i].transform);
        //}
    }
    public void playEffect ( )
    {
        targetPos ( );
    }

    public static void addcolliderComp (GameObject obj)
    {
        //  int count = obj.transform.childCount;
        Renderer [ ] mesh;
        mesh = obj.GetComponentsInChildren<Renderer> ( );
        if (mesh != null)
        {
            for (int i = 0; i < mesh.Length; i++)
            {
                mesh [i].gameObject.AddComponent<MeshCollider> ( );
                // mesh[i].gameObject.AddComponent<HighlightSelected>();
            }
        }
    }
}