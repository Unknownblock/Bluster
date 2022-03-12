using System;
using UnityEngine;
using UnityEngine.Rendering;

public class BuildTarget : MonoBehaviour
{
    public enum BuildState{IsPlaced, IsNotPlaced}
    public BuildState buildState;

    public bool isSelected;
    public bool isFailed;

    public Vector3[] snapPositions;
    public GameObject[] connectedBuilds;
    
    public Material buildMaterial;
    public Material failMaterial;
    public Material placedMaterial;
    public float distanceFromBuild;

    private void Update()
    {
        if (buildState == BuildState.IsPlaced)
        {
            gameObject.GetComponent<MeshRenderer>().enabled = true;
            transform.GetComponent<MeshRenderer>().material = placedMaterial;
            gameObject.GetComponent<MeshRenderer>().shadowCastingMode = ShadowCastingMode.Off;
            
            gameObject.GetComponent<Collider>().enabled = true;
            gameObject.GetComponent<Collider>().isTrigger = false;
        }
        
        if (buildState == BuildState.IsNotPlaced)
        {
            gameObject.GetComponent<MeshRenderer>().enabled = true;
            gameObject.GetComponent<MeshRenderer>().shadowCastingMode = ShadowCastingMode.On;
            
            gameObject.GetComponent<Collider>().enabled = false;
            gameObject.GetComponent<Collider>().isTrigger = true;

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

    private void OnCollisionStay(Collision other)
    {
        connectedBuilds = new GameObject[other.contacts.Length];
        for (int i = 0; i < other.contacts.Length; i++)
        {
            connectedBuilds[i] = other.contacts[i].otherCollider.gameObject;
        }
    }
}
