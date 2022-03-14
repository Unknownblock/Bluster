using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

public class BuildTarget : MonoBehaviour
{
    public enum BuildState{IsPlaced, IsNotPlaced}
    public BuildState buildState;

    public string placedLayerMask;

    public bool isSelected;
    public bool isFailed;

    public RotationPosition[] SnapVectors;
    public List<GameObject> connectedBuilds;
    
    public Material buildMaterial;
    public Material failMaterial;
    public Material placedMaterial;
    public float distanceFromBuild;

    private void Update()
    {
        MeshManaging();
    }

    private void MeshManaging()
    {
        if (buildState == BuildState.IsPlaced)
        {
            gameObject.GetComponent<MeshRenderer>().enabled = true;
            transform.GetComponent<MeshRenderer>().material = placedMaterial;
            gameObject.GetComponent<MeshRenderer>().shadowCastingMode = ShadowCastingMode.Off;
            
            gameObject.GetComponent<Collider>().isTrigger = false;

            gameObject.layer = LayerMask.NameToLayer(placedLayerMask);
        }
        
        if (buildState == BuildState.IsNotPlaced)
        {
            gameObject.GetComponent<MeshRenderer>().enabled = true;
            gameObject.GetComponent<MeshRenderer>().shadowCastingMode = ShadowCastingMode.On;
            
            gameObject.GetComponent<Collider>().isTrigger = true;
            
            gameObject.layer = LayerMask.NameToLayer("Ignore Raycast");

            if (!isFailed)
            {
                transform.GetComponent<MeshRenderer>().material = buildMaterial;
            }

            if (isFailed)
            {
                transform.GetComponent<MeshRenderer>().material = failMaterial;
            }
        }

        if (!isSelected)
        {
            gameObject.GetComponent<MeshRenderer>().enabled = false;
        }
    }

    private void OnCollisionEnter(Collision other)
    {
        if (buildState == BuildState.IsPlaced)
        {
            foreach (var everyContacts in other.contacts)
            {
                connectedBuilds.Add(everyContacts.otherCollider.gameObject);
            }
        }
    }

    private void OnCollisionExit(Collision other)
    {
        if (buildState == BuildState.IsPlaced)
        {
            for (int i = 0; i < connectedBuilds.Count; i++)
            {
                connectedBuilds.Remove(connectedBuilds[i]);
            }
        }
    }
}

[SerializeField]
public abstract class RotationPosition
{
    public Vector3 position;
    public Vector3 rotation;
}
