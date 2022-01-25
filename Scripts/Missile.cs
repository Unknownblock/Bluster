using UnityEngine;

public class Missile : MonoBehaviour
{
    public LayerMask distanceMask;
    public GameObject currentTarget;
    public float explosionForce;
    public float explosionRadius;
    public float minDistanceFromObjects;

    [Header("Movement")] public float speed = 100;
    public float returnSpeed;
    public Vector3 lastKnownPosition = Vector3.zero;

    public Quaternion lookAtRotation;

    [Header("Private Variables")] private Rigidbody _rb;

    private void Start()
    {
        _rb = GetComponent<Rigidbody>();
        currentTarget = PlayerMovement.Instance.gameObject;
    }

    private void FixedUpdate()
    {
        if (lastKnownPosition != currentTarget.transform.position)
        {
            lastKnownPosition = currentTarget.transform.position;

            lookAtRotation = Quaternion.LookRotation(lastKnownPosition - transform.position);
        }

        if (transform.rotation != lookAtRotation)
        {
            transform.rotation =
                Quaternion.RotateTowards(transform.rotation, lookAtRotation, returnSpeed * Time.deltaTime);
        }

        if (Physics.Raycast(transform.position, Vector3.down, minDistanceFromObjects, distanceMask))
            _rb.AddForce(Vector3.up * (speed * Time.deltaTime), ForceMode.Force);

        if (Physics.Raycast(transform.position, Vector3.up, minDistanceFromObjects, distanceMask))
            _rb.AddForce(Vector3.down * (speed * Time.deltaTime), ForceMode.Force);

        if (Physics.Raycast(transform.position, Vector3.right, minDistanceFromObjects, distanceMask))
            _rb.AddForce(Vector3.left * (speed * Time.deltaTime), ForceMode.Force);

        if (Physics.Raycast(transform.position, Vector3.left, minDistanceFromObjects, distanceMask))
            _rb.AddForce(Vector3.right * (speed * Time.deltaTime), ForceMode.Force);

        _rb.AddForce(transform.forward * (speed * Time.deltaTime), ForceMode.Force);
    }

    private void OnCollisionEnter()
    {
        Explosion hitExplosion = Instantiate(PrefabManager.Instance.explosion, transform.position, Quaternion.identity)
            .GetComponent<Explosion>();
        hitExplosion.radius = explosionRadius;
        hitExplosion.force = explosionForce;
        Destroy(gameObject);
    }
}