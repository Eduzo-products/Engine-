using UnityEngine;

public class ObjectTransformComponent : MonoBehaviour
{
    public ObjectTransform currentTransform = new ObjectTransform();

    private void Start()
    {
        PublishContent.OnSaveContentClick += OnSaveTransform;
    }

    private void OnDestroy()
    {
        PublishContent.OnSaveContentClick -= OnSaveTransform;
    }

    private void OnSaveTransform()
    {
        SetTransform(transform.position, transform.eulerAngles, transform.localScale);
        ScriptsManager._toolContent.objectNewTransforms.Add(currentTransform);
    }

    public void SetSelFTransform()
    {
        SetTransform(transform.position, transform.eulerAngles, transform.localScale);
    }

    public void SetTransform(Vector3 pos, Vector3 Rot, Vector3 Scale)
    {
        SetPosition(pos);
        SetRotation(Rot);
        SetScale(Scale);

        currentTransform.objectName = gameObject.name;
        currentTransform.objectFullPath = ScriptsManager.GetGameObjectPath(gameObject);
    }

    public void SetPosition(Vector3 pos)
    {
        currentTransform.position = new ObjectPosition();
        currentTransform.position.x = pos.x;
        currentTransform.position.y = pos.y;
        currentTransform.position.z = pos.z;
    }

    public void SetRotation(Vector3 rot)
    {
        currentTransform.rotation = new ObjectRotation();
        currentTransform.rotation.x = rot.x;
        currentTransform.rotation.y = rot.y;
        currentTransform.rotation.z = rot.z;
    }

    public void SetScale(Vector3 scale)
    {
        currentTransform.scale = new ObjectScale();
        currentTransform.scale.x = scale.x;
        currentTransform.scale.y = scale.y;
        currentTransform.scale.z = scale.z;
    }
}
