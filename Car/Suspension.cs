using System;
using UnityEngine;

public class Suspension : MonoBehaviour
{
   [Header("Assignable Variables")] public float addedWheelPosition;
   public float wheelMoveSpeed = 20f;
   public float wheelGetBackSpeed = 20f;
   public float suspensionLength;
   public float restHeight;
   public float springTravel;
   public float springStiffness;
   public float damperStiffness;
   public float wheelAngleVelocity;
   public float steeringAngle;
   public float traction;
   public float steerTime = 15f;
   public float steerAngle = 37f;
   public float lastCompression;
   public float wheelRadius;
   public GameObject wheel;
   public Transform currentWheel;
   public LayerMask groundLayer;

   [Header("Information Variables")]
   public bool isRearWheel;
   public bool isGrounded;
   public enum Type{RightWheel, LeftWheel}
   public Type type;

   [Header("Don't Care About")]
   private int lastSkid;
   private float _minLength;
   private float _maxLength;
   private float _lastLength;
   public float _restLength;
   private float _springLength;
   private float _springVelocity;
   private float _springForce;
   private float _damperForce;
   public Vector3 hitPos;
   public Vector3 hitNormal;
   public float hitHeight;
   public Vector3 suspensionForce;

   public Rigidbody rb;

   private void Update()
   {
      if (!isRearWheel)
      {
         wheelAngleVelocity = Mathf.Lerp(wheelAngleVelocity, steeringAngle, steerTime * Time.deltaTime);
         transform.localRotation = Quaternion.Euler(Vector3.up * wheelAngleVelocity);
      }
   }

   private void FixedUpdate()
   {
      if (Physics.SphereCast(transform.position, wheelRadius, -transform.up, out var hit, _maxLength + suspensionLength))
      {
         _minLength = _restLength - springTravel;
         _maxLength = _restLength + springTravel;
         
         _lastLength = _springLength;
         _springLength = hit.distance - suspensionLength;
         _springLength = Mathf.Clamp(_springLength, _minLength, _maxLength);
         _springVelocity = (_lastLength - _springLength) / Time.fixedDeltaTime;
         _springForce = springStiffness * (_restLength - _springLength);
         _damperForce = damperStiffness * _springVelocity;

         suspensionForce = (_springForce + _damperForce) * transform.up;

         rb.AddForceAtPosition(suspensionForce, hitPos);
         
         hitPos = hit.point;
         hitNormal = hit.normal;
         hitHeight = hit.distance;
         isGrounded = true;
      }
      
      else
      {
         isGrounded = false;
         hitHeight = suspensionLength + restHeight;
         
         
      }
   }
   
   private void LateUpdate()
   {
      if (traction > 0.05f && hitPos != Vector3.zero && isGrounded)
      {
         if ((bool)Skidmarks.Instance)
         {
            lastSkid = Skidmarks.Instance.AddSkidMark(hitPos + rb.velocity * Time.fixedDeltaTime, hitNormal, traction * 0.9f, lastSkid);
         }
      }
      else
      {
         lastSkid = -1;
      }
   }
}