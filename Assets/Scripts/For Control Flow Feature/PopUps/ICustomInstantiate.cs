using UnityEngine;

public interface ICustomInstantiate
{
    T Instantiate<T>(GameObject gameObject) where T : Object;
}