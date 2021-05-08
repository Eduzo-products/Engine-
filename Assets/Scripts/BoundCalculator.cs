using UnityEngine;

public class BoundCalculator : MonoBehaviour
{

    public float y, x;


    private void Start()
    {
        //ModelHeightWidthGetter();
    }
    public void CalculateLocalBoundsUsingEncapsulate()
    {

        Bounds bounds = new Bounds(this.transform.position, Vector3.zero);
        foreach (Renderer renderer in GetComponentsInChildren<Renderer>())
        {
            bounds.Encapsulate(renderer.bounds);
        }
        Vector3 localCenter = bounds.center - this.transform.position;
        bounds.center = localCenter;
        y = bounds.size.y;
        x = bounds.size.x;

    }
    public float ModelHeightGetter()
    {
        Renderer[] rends = GetComponentsInChildren<Renderer>(true);
        Bounds bounds = rends[0].bounds;
        foreach (Renderer rend in rends)
        {
            bounds = bounds.GrowBounds(rend.bounds);
        }
        Vector3 center = bounds.center;
        Debug.Log("Model Height Getter !");
        y = bounds.size.y;
        x = bounds.size.x;

        return bounds.size.y;
    }

    public void ModelHeightWidthGetter()
    {
        if (transform.childCount == 0)
        {
            Renderer rends = GetComponent<Renderer>();
            Bounds bounds = rends.bounds;

            Vector3 center = bounds.center;
            y = bounds.size.y;
            x = bounds.size.x;
        }
        else
        {
            Renderer[] rends = GetComponentsInChildren<Renderer>();

            foreach (Renderer rend in rends)
            {
                Bounds bounds = rend.bounds;

                bounds = bounds.GrowBounds(rend.bounds);

                Vector3 center = bounds.center;
                y = bounds.size.y;
                x = bounds.size.x;
            }

        }
        Debug.Log("Model Height Width Getter !");
    }
}

public static class BoundsExtension
{
    public static Bounds GrowBounds(this Bounds a, Bounds b)
    {
        Vector3 max = Vector3.Max(a.max, b.max);
        Vector3 min = Vector3.Min(a.min, b.min);

        a = new Bounds((max + min) * 0.5f, max - min);
        return a;
    }
}