using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class positionScript : MonoBehaviour
{
    public Vector3 target_pos = Vector3.zero;
    public Vector3 originalScale = Vector3.one;
    // Start is called before the first frame update
    void Start()
    {
        originalScale = transform.localScale;
    }
    public bool flag, eflag, dflag = true;
    // Update is called once per frame
    //void Update()
    //{
    //    if (Input.GetKeyDown(KeyCode.Space))
    //    {
    //        if (flag)
    //        {
    //            StartCoroutine(DoAThingOverTime(Vector3.one, Vector3.zero, 1f));
    //            flag = false;
    //        }
    //        else
    //        {
    //            StartCoroutine(DoAThingOverTime(Vector3.zero, Vector3.one, 1f));
    //            flag = true;
    //        }
    //    }
    //}
    public void enableAnimation(float time)
    {
        //if (!eflag)
        //{
        print("enableAnimation  " + gameObject.name);
        StartCoroutine(DoAThingOverTimeEnable(Vector3.zero, originalScale, time));
        eflag = true;
        // }
    }
    public void disableAnimation(float time)
    {
        //if (eflag)
        //{
        StartCoroutine(DoAThingOverTime(originalScale, Vector3.zero, time));
        eflag = false;
        // }
    }
    IEnumerator DoAThingOverTimeEnable(Vector3 scale, Vector3 zeroscale, float duration)
    {

        for (float t = 0f; t < duration; t += Time.deltaTime)
        {
            float normalizedTime = t / duration;
            //right here, you can now use normalizedTime as the third parameter in any Lerp from start to end
            transform.localScale = Vector3.Lerp(scale, zeroscale, normalizedTime);
            //print(transform.localScale);
            yield return null;
        }
        transform.localScale = zeroscale; //without this, the value will end at something like 0.9992367
    }
    IEnumerator DoAThingOverTime(Vector3 scale, Vector3 zeroscale, float duration)
    {

        for (float t = 0f; t < duration; t += Time.deltaTime)
        {
            float normalizedTime = t / duration;
            //right here, you can now use normalizedTime as the third parameter in any Lerp from start to end
            transform.localScale = Vector3.Lerp(scale, zeroscale, normalizedTime);
            //print(transform.localScale);
            yield return null;
        }
        transform.localScale = zeroscale; //without this, the value will end at something like 0.9992367
    }
    IEnumerator MoveFromTo(Transform objectToMove, Vector3 a, Vector3 b, float speed)
    {
        float step = (speed / (a - b).magnitude) * Time.fixedDeltaTime;
        float t = 0;
        while (t <= 1.0f)
        {
            t += step; // Goes from 0 to 1, incrementing by step each time
            objectToMove.position = Vector3.Lerp(a, b, t); // Move objectToMove closer to b
            yield return new WaitForFixedUpdate();         // Leave the routine and return here in the next frame
        }
        objectToMove.position = b;
    }
    public void moveObject(Transform obj)
    {
        StartCoroutine(MoveFromTo(obj, transform.position, target_pos, 20f));
    }
}
