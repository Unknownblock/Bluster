// MeshesDeformation
using UnityEngine;

public class Deform : MonoBehaviour
{
	[SerializeField]
	private MeshFilter[] meshFilters;

	[SerializeField]
	private MeshCollider[] colliders;

	[SerializeField]
	private float impactDamage = 1f;

	[SerializeField]
	private float deformationRadius = 0.5f;

	[SerializeField]
	private float maxDeformation = 0.5f;

	[SerializeField]
	private float minVelocity = 2f;

	private float delayTimeDeform = 0.1f;

	private float minVertsDistanceToRestore = 0.002f;

	private float vertsRestoreSpeed = 2f;

	private Vector3[][] originalVertices;

	private float nextTimeDeform;

	private bool isRepairing;

	private bool isRepaired;

	private void Start()
	{
		originalVertices = new Vector3[meshFilters.Length][];
		for (int i = 0; i < meshFilters.Length; i++)
		{
			originalVertices[i] = meshFilters[i].mesh.vertices;
			meshFilters[i].mesh.MarkDynamic();
		}
	}

	private void Update()
	{
		if (Input.GetKeyDown(KeyCode.R) && !isRepairing)
		{
			isRepairing = true;
		}
		RestoreMesh();
	}

	private void DeformationMesh(Mesh mesh, Transform localTransform, Vector3 contactPoint, Vector3 contactVelocity, int i)
	{
		bool flag = false;
		Vector3 vector = localTransform.InverseTransformPoint(contactPoint);
		Vector3 vector2 = localTransform.InverseTransformDirection(contactVelocity);
		Vector3[] vertices = mesh.vertices;
		for (int j = 0; j < vertices.Length; j++)
		{
			float magnitude = (vector - vertices[j]).magnitude;
			if (magnitude <= deformationRadius)
			{
				vertices[j] += vector2 * (deformationRadius - magnitude) * impactDamage;
				Vector3 vector3 = vertices[j] - originalVertices[i][j];
				if (vector3.magnitude > maxDeformation)
				{
					vertices[j] = originalVertices[i][j] + vector3.normalized * maxDeformation;
				}
				flag = true;
			}
		}
		if (flag)
		{
			mesh.vertices = vertices;
			mesh.RecalculateNormals();
			mesh.RecalculateBounds();
			if (colliders.Length != 0 && colliders[i] != null)
			{
				colliders[i].sharedMesh = mesh;
			}
		}
	}

	private void OnCollisionEnter(Collision collision)
	{
		if (!(Time.time > nextTimeDeform) || !(collision.relativeVelocity.magnitude > minVelocity))
		{
			return;
		}
		isRepaired = false;
		Vector3 point = collision.contacts[0].point;
		Vector3 contactVelocity = collision.relativeVelocity * 0.02f;
		for (int i = 0; i < meshFilters.Length; i++)
		{
			if (meshFilters[i] != null)
			{
				DeformationMesh(meshFilters[i].mesh, meshFilters[i].transform, point, contactVelocity, i);
			}
		}
		nextTimeDeform = Time.time + delayTimeDeform;
	}

	private void RestoreMesh()
	{
		if (isRepaired || !isRepairing)
		{
			return;
		}
		isRepaired = true;
		for (int i = 0; i < meshFilters.Length; i++)
		{
			Mesh mesh = meshFilters[i].mesh;
			Vector3[] vertices = mesh.vertices;
			Vector3[] array = originalVertices[i];
			for (int j = 0; j < vertices.Length; j++)
			{
				vertices[j] += (array[j] - vertices[j]) * Time.deltaTime * vertsRestoreSpeed;
				if ((array[j] - vertices[j]).magnitude > minVertsDistanceToRestore)
				{
					isRepaired = false;
				}
			}
			mesh.vertices = vertices;
			mesh.RecalculateNormals();
			mesh.RecalculateBounds();
			if (colliders[i] != null)
			{
				colliders[i].sharedMesh = mesh;
			}
		}
		if (!isRepaired)
		{
			return;
		}
		isRepairing = false;
		for (int k = 0; k < meshFilters.Length; k++)
		{
			if (colliders[k] != null)
			{
				colliders[k].sharedMesh = meshFilters[k].mesh;
			}
		}
	}
}