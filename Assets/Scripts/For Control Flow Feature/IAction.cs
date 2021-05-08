using System;
using UnityEngine;

namespace XrStudio_ControlFlow
{
    public  interface IAction
    {
        int actionLength { get; set; }
        int actionNum { get; set; }
        void PlayAction();

    }

    public class a
    {
        public int c;

        public void xyz()
        {
            Debug.Log("xyz == class a");
        }
        public virtual void Test()
        {
            Debug.Log("test == class a");

        }

    }
    public class b : a
    {
     
        public new void xyz()
        {
            Debug.Log("xyz == class b");

        }
        public void u()
        {
            xyz();
        }
        public override void Test()
        {
            Debug.Log("test == class b");
        }

    }

    public class c 
    {
        public void TestObj()
        {
            a objA = new b();
            objA.xyz();
            objA.Test();
        }
    }

    public class D:MonoBehaviour
    {
        private void Awake()
        {
            c objC = new c();
            objC.TestObj();
        }

    }
}



