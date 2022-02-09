using System;
using UnityEngine;

public class Suspension : MonoBehaviour
{
   [Header("Assignable Variables")] 
   public float suspensionLength;
   public float restHeight;
   public float springTravel;
   public float springStiffness;
   public float damperStiffness;
   public float wheelRadius;
   public Transform currentWheel;

   [Header("Bool Variables")] 
   public bool isGrounded;

   [Header("Don't Care About")]
   private float _minLength;
   private float _maxLength;
   private float _lastLength;
   private float _restLength;
   private float _springLength;
   private float _springVelocity;
   private float _springForce;
   private float _damperForce;
   public Vector3 hitPos;
   public Vector3 hitNormal;
   public float hitHeight;

   private Vector3 suspensionForce;

   private Rigidbody _rb;

   private void Start()
   {
      _rb = transform.parent.GetComponent<Rigidbody>();
   }

   private void FixedUpdate()
   {
      _minLength = _restLength - springTravel;
      _maxLength = _restLength + springTravel;

      if (Physics.Raycast(transform.position, -transform.up, out var hit, _maxLength + wheelRadius))
      {
         _lastLength = _springLength;
         _springLength = hit.distance - wheelRadius;
         _springLength = Mathf.Clamp(_springLength, _minLength, _maxLength);
         _springVelocity = (_lastLength - _springLength) / Time.fixedDeltaTime;
         _springForce = springStiffness * (_restLength - _springLength);
         _damperForce = damperStiffness * _springVelocity;

         suspensionForce = (_springForce + _damperForce) * transform.up;
         
         _rb.AddForceAtPosition(suspensionForce, hit.point);
         
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
}