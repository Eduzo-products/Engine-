using System;
using UnityEngine;

public class CheckResutls : MonoBehaviour
{
    int[] a = new int[5];
    public void TestLoop()
    {
      //  a.l
        for (int i = 0; i < 5; i++)
        {

        }
    }
    static void main()
    {
        Console.WriteLine("check run");
    }

    private void Start()
    {
        var ameObject = new GameObject();
        print(ameObject.transform.forward);

        print(ameObject.transform.rotation.normalized);

        ameObject.transform.Rotate(13f, -18f, 130);
        print(ameObject.transform.rotation.normalized);


    }
}

