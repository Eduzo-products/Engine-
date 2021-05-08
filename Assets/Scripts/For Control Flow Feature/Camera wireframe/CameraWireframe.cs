using UnityEngine;
//[ExecuteInEditMode]


//class MyClassC
//{
//    int a;
//    public MyClassC(int a)
//    {
//        Debug.Log("constructor C");
//    }

   
//}


//class MyClassA:MyClassC
//{
//    int a;
//    public MyClassA(int a)
//    {
    
//        Debug.Log("constructor A");
//    }

//    public void abc()
//    {
//        Debug.Log("A");
//    }
//}

//class MyClassB : MyClassA
//{
//    int a;
//    public MyClassB(int a)
//    {
//        Debug.Log("constructor B");
//    }

//    public void abc()
//    {
//        Debug.Log("B");
//    }
//}
public class CameraWireframe : MonoBehaviour
{
    [SerializeField]
    Camera camera;
    [SerializeField]
    LineRenderer lineIndicatingFrustum;
    public Material outline;
    // Start is called before the first frame update
    void Start()
    {

      //  MyClassB b = new MyClassB(5);
   //     MyClassA a = b;
     //   a.abc();


        // Calculate the planes from the main camera's view frustum
        Plane[] planes = GeometryUtility.CalculateFrustumPlanes(camera);

        // Create a "Plane" GameObject aligned to each of the calculated planes
        //   for (int i = 0; i < 6; ++i)
        //  {
        GameObject p = GameObject.CreatePrimitive(PrimitiveType.Plane);
        p.name = "Camera view frame =  " + planes[5].ToString();
        //  p.transform.position = -planes[5].normal * planes[5].distance;
        //  p.transform.rotation = Quaternion.FromToRotation(Vector3.up, planes[5].normal);

        p.GetComponent<MeshRenderer>().sharedMaterial = outline;
        //   p.transform.localScale = planes[i].
        //  }

        //  MeshFilter m =  gameObject.AddComponent<MeshFilter>();
        //gameObject.AddComponent<MeshRenderer>();
        float pos = (camera.nearClipPlane + 0.01f);

        p.transform.position = camera.transform.position + camera.transform.forward * pos;
        p.transform.LookAt(camera.transform);
        p.transform.Rotate(90.0f, 0.0f, 0.0f);

        float h = (Mathf.Tan(camera.fieldOfView * Mathf.Deg2Rad * 0.5f) * pos * 2f) / 10.0f;

        p.transform.localScale = new Vector3(h * camera.aspect, 1.0f, h);

        //  lineIndicatingFrustum = camera.GenerateFrustumMesh( lineIndicatingFrustum);

    }

    // Update is called once per frame
    void Update()
    {

    }
}
public static class CameraExtention
{
    private static int[] m_VertOrder = new int[24]
    {
         0,1,2,3, // near
         6,5,4,7, // far
         0,4,5,1, // left
         3,2,6,7, // right
         1,5,6,2, // top
         0,3,7,4  // bottom
    };
    private static int[] m_Indices = new int[36]
    {
          0,  1,  2,  3,  0,  2, // near
          4,  5,  6,  7,  4,  6, // far
          8,  9, 10, 11,  8, 10, // left
         12, 13, 14, 15, 12, 14, // right
         16, 17, 18, 19, 16, 18, // top
         20, 21, 22, 23, 20, 22, // bottom
    }; //              |______|---> shared vertices

    public static LineRenderer GenerateFrustumMesh(this Camera cam,  LineRenderer line)
    {
        LineRenderer meshLine = line;
        Vector3[] v = new Vector3[8];
        v[0] = v[4] = new Vector3(0, 0, 0);
        v[1] = v[5] = new Vector3(0, 1, 0);
        v[2] = v[6] = new Vector3(1, 1, 0);
        v[3] = v[7] = new Vector3(1, 0, 0);
        v[0].z = v[1].z = v[2].z = v[3].z = cam.nearClipPlane;
        v[4].z = v[5].z = v[6].z = v[7].z = cam.farClipPlane;
        // Transform viewport --> world --> local
        for (int i = 0; i < 8; i++)
        {
            v[i] = cam.transform.InverseTransformPoint(cam.ViewportToWorldPoint(v[i]));
            Debug.Log(v[i]);
        }

        Vector3[] vertices = new Vector3[24];
        Vector3[] normals = new Vector3[24];

        // Split vertices for each face (8 vertices --> 24 vertices)
        for (int i = 0; i < 24; i++)
            vertices[i] = v[m_VertOrder[i]];

        // Calculate facenormal
        for (int i = 0; i < 6; i++)
        {
            Vector3 faceNormal = Vector3.Cross(vertices[i * 4 + 2] - vertices[i * 4 + 1], vertices[i * 4 + 0] - vertices[i * 4 + 1]).normalized;
            normals[i * 4 + 0] = normals[i * 4 + 1] = normals[i * 4 + 2] = normals[i * 4 + 3] = faceNormal;
        }

        //  mesh.vertices = vertices;
        meshLine.SetPositions(v);

      //  mesh.normals = normals;
       // mesh.triangles = m_Indices;
        return meshLine;
    }
}