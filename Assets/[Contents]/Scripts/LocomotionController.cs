using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using UnityEngine;
using RootMotion;
using Sirenix.OdinInspector;

namespace TOJam2018
{
	[RequireComponent(typeof(Animator))]
	[RequireComponent(typeof(CharacterController))]
	public class LocomotionController : MonoBehaviour
	{
		[Tooltip("The component that updates the camera.")]
		[Required, SerializeField] 
		private CameraController _cameraController;
		public CameraController CameraController
		{
			get { return _cameraController; }
			set { _cameraController = value;  }
		}

		[Tooltip("The component that controls the character.")] [Required, SerializeField]
		private CharacterController _characterController;
		public CharacterController CharacterController
		{
			get { return _characterController; }
			set { _characterController = value; }
		}
		
		[Tooltip("Acceleration of movement.")]
		[SerializeField] 
		private float _accelerationTime = 0.2f;

		[Tooltip("Turning speed.")]
		[SerializeField] 
		private float _turnTime = 0.2f;

		[Tooltip("If true, will run on left shift, if not will walk on left shift.")]
		[SerializeField] 
		private bool _walkByDefault = true;

		[Tooltip("Procedural motion speed (if not using root motion).")]
		[SerializeField] 
		private float _moveSpeed = 3f;
		
		private Animator _animator;
		private float _speed;
		private float _angleVel;
		private float _speedVel;
		private Vector3 _linearTargetDirection;

		private void Awake()
		{
			if (null == _cameraController) { 
				_cameraController = GetComponent<CameraController>();
				_cameraController.enabled = false;
			}
			if (null == _characterController) { _characterController = GetComponent<CharacterController>(); }
		}
		
		private void Update()
		{
			Rotate();
			Move();
		}
		
		private void LateUpdate()
		{
			UpdateCamera();
		}
		
		private void Rotate()
		{
			
		}
		
		private void Move()
		{
				
		}
		
		/// <summary>
		/// Reads the Input to get the movement direction.
		/// </summary>
		private Vector3 GetInputVector()
		{
			var d = new Vector3(
				Input.GetAxis("Horizontal"),
				0f,
				Input.GetAxis("Vertical")
			);

			d.z += Mathf.Abs(d.x) * 0.05f;
			d.x -= Mathf.Abs(d.z) * 0.05f;

			return d;
		}
		
		private Vector3 GetInputVectorRaw()
		{
			return new Vector3(
				Input.GetAxisRaw("Horizontal"),
				0f,
				Input.GetAxisRaw("Vertical")
			);
		}

		private void UpdateCamera()
		{
			if (null == _cameraController) { return; }
			_cameraController.UpdateInput();
			_cameraController.UpdateTransform();
		}
	}
}
