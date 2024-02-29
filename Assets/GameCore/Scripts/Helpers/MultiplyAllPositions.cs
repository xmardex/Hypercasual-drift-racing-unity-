using UnityEngine;

[ExecuteInEditMode]
public class MultiplyAllPositions : MonoBehaviour
{
    [SerializeField] private bool _update = false;

    void Update()
    {
        if (_update)
        {
            MultiplyPositions();
            _update = false;
        }
    }

    void MultiplyPositions()
    {

        GameObject[] allObjects = GameObject.FindObjectsOfType<GameObject>();


        foreach (GameObject obj in allObjects)
        {

            if (obj.transform.parent == null)
            {

                Vector3 currentPosition = obj.transform.position;


                currentPosition.x *= 2.25f;
                currentPosition.y *= 2.25f;
                currentPosition.z *= 2.25f;


                obj.transform.position = currentPosition;
            }
        }
    }
}