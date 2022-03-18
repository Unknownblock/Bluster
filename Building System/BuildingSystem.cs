using UnityEngine;

public class BuildingSystem : MonoBehaviour
{
    public int amountOfBuilds;
    public float rotateSpeed;
    public float buildRange;
    public float buildRadius;
    public float snapRadius;
    public float objectDetectionRadius;
    public BuildTarget buildTarget;
    public BuildTarget currentBuildTarget;
    public Vector3 hitPoint;
    public LayerMask buildDetectionMask;
    public LayerMask buildMask;

    private void Update()
    {
        BuildPlacement();
        BuildInput();
        Build();
        RotatingTheBuild();
    }

    private void BuildPlacement()
    {
        if (currentBuildTarget != null)
        {
            if (Physics.SphereCast(MoveCamera.Instance.transform.position, buildRadius, MoveCamera.Instance.gameObject.transform.forward, out var hit, buildRange, buildMask))
            {
                currentBuildTarget.isSelected = true;
                
                hitPoint = hit.point;
                
                if (currentBuildTarget != null && currentBuildTarget.buildState != BuildTarget.BuildState.IsPlaced)
                {
                    currentBuildTarget.buildState = BuildTarget.BuildState.IsNotPlaced;
                }
                
                if (hit.transform != currentBuildTarget.transform)
                {
                    if (hit.transform.GetComponent<BuildTarget>() != null && hit.transform.GetComponent<BuildTarget>().buildState == BuildTarget.BuildState.IsPlaced)
                    {
                        foreach (var everySnapVector in hit.transform.GetComponent<BuildTarget>().snapVectors)
                        {
                            var canSnap = Vector3.Distance(hit.transform.GetComponent<BuildTarget>().transform.position + everySnapVector.position, hitPoint) < snapRadius;
                            
                            if (canSnap)
                            {
                                currentBuildTarget.transform.position = hit.transform.GetComponent<BuildTarget>().transform.position + everySnapVector.position;
                                currentBuildTarget.transform.rotation = Quaternion.Euler(everySnapVector.rotation);
                            }
                        }
                    }

                    else
                    {
                        currentBuildTarget.transform.position = hit.point + new Vector3(hit.normal.x, 0f, hit.normal.z) * (currentBuildTarget.distanceFromBuild / 2);
                    }
                }
            }

            else
            {
                currentBuildTarget.isSelected = false;
            }
        }
    }

    private void BuildInput()
    {
        if (currentBuildTarget != null)
        {
            if (Physics.CheckSphere(currentBuildTarget.transform.position, objectDetectionRadius, buildDetectionMask))
            {
                currentBuildTarget.isFailed = true;
            }

            if (!Physics.CheckSphere(currentBuildTarget.transform.position, objectDetectionRadius, buildDetectionMask))
            {
                currentBuildTarget.isFailed = false;
            }

            if (Input.GetKeyDown(InputManager.Instance.shootKey) && !currentBuildTarget.isFailed)
            {
                if (currentBuildTarget.isSelected)
                {
                    currentBuildTarget.buildState = BuildTarget.BuildState.IsPlaced;
                    currentBuildTarget = null;
                }
            }
        }
    }

    private void Build()
    {
        if (currentBuildTarget == null && amountOfBuilds > 0)
        {
            currentBuildTarget = Instantiate(buildTarget, hitPoint, Quaternion.identity);
            currentBuildTarget.buildState = BuildTarget.BuildState.IsNotPlaced;
            amountOfBuilds--;
        }
    }

    private void RotatingTheBuild()
    {
        var playerInput = PlayerInput.Instance;
        
        if (Input.GetKey(KeyCode.R))
        {
            playerInput.isLocked = true;
            var addRotation = new Vector3(Input.GetAxisRaw("Mouse X"), Input.GetAxisRaw("Mouse Y"), 0f);
            currentBuildTarget.transform.localRotation *= Quaternion.Euler(addRotation);
        }

        else
        {
            playerInput.isLocked = false;
        }
    }
}
