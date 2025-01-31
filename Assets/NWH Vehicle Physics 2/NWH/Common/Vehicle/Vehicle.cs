﻿using UnityEngine;
using UnityEngine.Events;

namespace NWH
{
    /// <summary>
    /// Base class for all NWH vehicles.
    /// </summary>
    [DisallowMultipleComponent]
    [RequireComponent(typeof(Rigidbody))]
    public abstract class Vehicle : MonoBehaviour
    {
        /// <summary>
        ///     True if vehicle is awake. Different from disabled. Disable will deactivate the vehicle fully while putting the
        ///     vehicle to sleep will only force the highest lod so that some parts of the vehicle can remain working if configured
        ///     so.
        ///     Set to false if vehicle is parked and otherwise not in focus, but needs to function.
        ///     Call Wake() to wake or Sleep() to put to sleep.
        /// </summary>
        public bool IsAwake
        {
            get => _isAwake;
        }
        
        /// <summary>
        ///     Called when vehicle is put to sleep.
        /// </summary>
        [Tooltip("    Called when vehicle is put to sleep.")]
        public UnityEvent onSleep = new UnityEvent();

        /// <summary>
        ///     Called when vehicle is woken up.
        /// </summary>
        [Tooltip("    Called when vehicle is woken up.")]
        public UnityEvent onWake = new UnityEvent();
        
        public enum MultiplayerInstanceType
        {
            Local,
            Remote
        }

        /// <summary>
        ///     Cached Time.deltaTime value.
        /// </summary>
        [Tooltip("    Cached Time.deltaTime value.")]
        public float deltaTime = 0.02f;
        
        /// <summary>
        ///     Cached Time.fixedDeltaTime value.
        /// </summary>
        [Tooltip("    Cached Time.fixedDeltaTime value.")]
        public float fixedDeltaTime = 0.02f;
        
        /// <summary>
        ///     Determines if vehicle is running locally is synchronized over active multiplayer framework.
        /// </summary>
        [Tooltip("    Determines if vehicle is running locally is synchronized over active multiplayer framework.")]
        public MultiplayerInstanceType multiplayerInstanceType = MultiplayerInstanceType.Local;
        
        /// <summary>
        ///     Cached value of vehicle rigidbody.
        /// </summary>
        [Tooltip("    Vehicle rigidbody.")]
        public Rigidbody vehicleRigidbody;

        /// <summary>
        ///     Cached value of vehicle transform.
        /// </summary>
        [Tooltip("    Cached value of vehicle transform.")]
        public Transform vehicleTransform;
        
        /// <summary>
        ///     Cached acceleration in local coordinates (z-forward)
        /// </summary>
        public Vector3 LocalAcceleration { get; private set; }

        /// <summary>
        ///     Cached acceleration in forward direction in local coordinates (z-forward).
        /// </summary>
        public float LocalForwardAcceleration { get; private set; }

        /// <summary>
        ///     Velocity in forward direction in local coordinates (z-forward).
        /// </summary>
        public float LocalForwardVelocity { get; private set; }
        
        /// <summary>
        ///     Velocity in m/s in local coordinates.
        /// </summary>
        public Vector3 LocalVelocity { get; private set; }

        /// <summary>
        ///     Speed of the vehicle in the forward direction. ALWAYS POSITIVE.
        ///     For positive/negative version use SpeedSigned.
        /// </summary>
        public float Speed
        {
            get => LocalForwardVelocity < 0 ? -LocalForwardVelocity : LocalForwardVelocity;
        }

        /// <summary>
        ///     Speed of the vehicle in the forward direction. Can be positive (forward) or negative (reverse).
        ///     Equal to LocalForwardVelocity.
        /// </summary>
        public float SpeedSigned
        {
            get => LocalForwardVelocity;
        }

        /// <summary>
        /// Cached velocity of the vehicle in world coordinates.
        /// </summary>
        public Vector3 Velocity { get; private set; }

        /// <summary>
        /// Cached velocity magnitude of the vehicle in world coordinates.
        /// </summary>
        public float VelocityMagnitude { get; private set; }

        /// <summary>
        /// Should be true when camera is inside vehicle (cockpit, cabin, etc.).
        /// Used for audio effects.
        /// </summary>
        public bool cameraInsideVehicle = false;
        
        [SerializeField]
        protected bool _isAwake = true;
        
        private Vector3 _prevLocalVelocity;

        public virtual void Awake()
        {
            vehicleTransform = transform;
            vehicleRigidbody = GetComponent<Rigidbody>();
        }

        public virtual void Update()
        {
            deltaTime = Time.deltaTime;
        }
        
        public virtual void FixedUpdate()
        {
            fixedDeltaTime = Time.fixedDeltaTime;

            // Calculate velocity and accelerationMag
            _prevLocalVelocity = LocalVelocity;
            Velocity = vehicleRigidbody.velocity;
            LocalVelocity = transform.InverseTransformDirection(Velocity);
            LocalAcceleration = (LocalVelocity - _prevLocalVelocity) / fixedDeltaTime;
            LocalForwardVelocity = LocalVelocity.z;
            LocalForwardAcceleration = LocalAcceleration.z;
            VelocityMagnitude = Velocity.magnitude;
        }

        public virtual void Sleep()
        {
            _isAwake = false;
            onSleep.Invoke();   
        }

        public virtual void Wake()
        {
            _isAwake = true;
            onWake.Invoke();
        }
    }

}
