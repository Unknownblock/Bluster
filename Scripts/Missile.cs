using UnityEngine;

public class Missile : MonoBehaviour
{
    public LayerMask distanceMask;
    public GameObject currentTarget;
    public float explosionForce;
    public float explosionRadius;
    public float force;
    public float minDistanceFromObjects;

    [Header("Movement")]
    public float speed = 15;
    public float rotateSpeed = 95;

    [Header("Prediction")]
    public float maxDistancePredict = 100;

    public float minDistancePredict = 5;
    public float maxTimePrediction = 5;
    public Vector3 standardPrediction;
    public Vector3 deviatedPrediction;

    [Header("Deviation")] 
    public float deviationAmount = 50;
    public float deviationSpeed = 2;

    [Header("Private Variables")] 
    private Rigidbody _rb;
    
    private void Start()
    {
        _rb = GetComponent<Rigidbody>();
        currentTarget = PlayerMovement.Instance.gameObject;
    }

    private void FixedUpdate()
    {
        if (Physics.Raycast(transform.position, Vector3.down, minDistanceFromObjects, distanceMask))
            GetComponent<Rigidbody>().AddForce(Vector3.up * (force * Time.deltaTime), ForceMode.VelocityChange);

        if (Physics.Raycast(transform.position, Vector3.up, minDistanceFromObjects, distanceMask))
            GetComponent<Rigidbody>().AddForce(Vector3.down * (force * Time.deltaTime), ForceMode.VelocityChange);

        if (Physics.Raycast(transform.position, Vector3.right, minDistanceFromObjects, distanceMask))
            GetComponent<Rigidbody>().AddForce(Vector3.left * (force * Time.deltaTime), ForceMode.VelocityChange);

        if (Physics.Raycast(transform.position, Vector3.left, minDistanceFromObjects, distanceMask))
            GetComponent<Rigidbody>().AddForce(Vector3.right * (force * Time.deltaTime), ForceMode.VelocityChange);

        GetComponent<Rigidbody>().AddForce(transform.forward * (force * Time.deltaTime), ForceMode.VelocityChange);
        
        _rb.AddForce(gameObject.transform.forward * (speed * Time.deltaTime));

        var leadTimePercentage = Mathf.InverseLerp(minDistancePredict, maxDistancePredict,
            Vector3.Distance(gameObject.transform.position, currentTarget.transform.position));

        PredictMovement(leadTimePercentage);

        AddDeviation(leadTimePercentage);

        RotateRocket();
    }

    private void PredictMovement(float leadTimePercentage)
    {
        var predictionTime = Mathf.Lerp(0, maxTimePrediction, leadTimePercentage);

        standardPrediction = currentTarget.transform.position + currentTarget.GetComponent<Rigidbody>().velocity * predictionTime;
    }

    private void AddDeviation(float leadTimePercentage)
    {
        var deviation = new Vector3(Mathf.Cos(Time.time * deviationSpeed), 0, 0);

        var predictionOffset = transform.TransformDirection(deviation) * (deviationAmount * leadTimePercentage);

        deviatedPrediction = standardPrediction + predictionOffset;
    }

    private void RotateRocket()
    {
        var heading = deviatedPrediction - transform.position;

        var rotation = Quaternion.LookRotation(heading);
        _rb.MoveRotation(Quaternion.RotateTowards(transform.rotation, rotation, rotateSpeed * Time.deltaTime));
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