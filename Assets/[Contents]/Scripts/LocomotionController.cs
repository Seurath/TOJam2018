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
		[SerializeField] 
		private CameraController _cameraController;

		public CameraController CameraController
		{
			get { return _cameraController; }
			set { _cameraController = value; }
		}
		private Camera _camera;
		
		[Tooltip("The component that controls the character.")] 
		[SerializeField]
		private CharacterController _characterController;
		public CharacterController CharacterController
		{
			get { return _characterController; }
			set { _characterController = value; }
		}
		
		
		[Tooltip("Acceleration of movement.")]
		[SerializeField] private float _accelerationTime = 0.2f;
		
		[Tooltip("Turning speed.")]
		[SerializeField] private float _turnTime = 0.2f;
		
		[Tooltip("If true, will run on left shift, if not will walk on left shift.")]
		[SerializeField] private bool _walkByDefault = true;
		
		[Tooltip("Procedural motion speed (if not using root motion).")]
		[SerializeField] private float _moveSpeed = 3f;

		[Tooltip("Whether to animate locomotion using mouse look. " + 
		         "E.g. stepping backwards when looking opposite movement direction.")] 
		[SerializeField] private bool _useMouseLook = true;
		
		[ShowIf("_useMouseLook"), SerializeField]
		private float _mouseHitDistance = 100;
		
		public Vector3 LookTarget { get; private set; }
		
		private Animator _animator;
		[SerializeField] private string _movementAnimParam = "Speed";
		
		private float _speed;
		private float _angleVel;
		private float _speedVel;
		private Vector3 _linearTargetDirection;
		
		
		#region MonoBehaviour
		
		private void Awake()
		{
			_animator = GetComponent<Animator>();
			if (null == _cameraController) { 
				_cameraController = GetComponent<CameraController>();
			}
			if (null != _cameraController)
			{
				_cameraController.enabled = false;
				_camera = _cameraController.GetComponent<Camera>();
			}
			if (null == _characterController) { _characterController = GetComponent<CharacterController>(); }
		}
		
		private void Update()
		{
			ResolveMouseLook();
			Rotate();
			Move();
		}
		
		private void LateUpdate()
		{
			UpdateCamera();
		}
		
		private void OnDrawGizmos()
		{
			Color defaultColor = Gizmos.color;
			if (!Application.isPlaying) { return; }
			
			Gizmos.color = Color.yellow;
			Gizmos.DrawSphere(LookTarget, 0.1f);
			Gizmos.color = defaultColor;
		}
		
		#endregion MonoBehaviour


		#region Locomotion

		private void ResolveMouseLook()
		{
			if (null == CameraController) { return; }
			Ray mouseRay = _camera.ScreenPointToRay(Input.mousePosition);
			RaycastHit hitInfo;
			var hasHit = Physics.Raycast(mouseRay, out hitInfo, _mouseHitDistance, LayerMask.NameToLayer("UI"));
			if (!hasHit) { return; }
			
			// Retrieve the ray hit point.
			Vector3 hitPoint = hitInfo.point;
			
			// Offset the hit point by the mouse look target height.
			Vector3 lookPoint = hitPoint;
			LookTarget = lookPoint;
		}
		
		/// <summary>
		/// Updates character object transform rotation.
		/// </summary>
		private void Rotate()
		{
			// Updating the rotation from input vector.
			Vector3 inputVector = GetInputVector();
			if (inputVector == Vector3.zero) { return; }
			
			Vector3 forward = transform.forward;
			Vector3 targetDirection = CameraController.transform.rotation * inputVector;
			
			var angleForward = Mathf.Atan2(forward.x, forward.z) * Mathf.Rad2Deg;
			var angleTarget = Mathf.Atan2(targetDirection.x, targetDirection.z) * Mathf.Rad2Deg;
			
			// Smoothly rotating the character
			var angle = Mathf.SmoothDampAngle(angleForward, angleTarget, ref _angleVel, _turnTime);
			transform.rotation = Quaternion.AngleAxis(angle, Vector3.up);
		}
		
		private void Move()
		{
			// Speed interpolation
			var speedTarget = _walkByDefault 
				? (Input.GetKey(KeyCode.LeftShift)? 1f: 0.5f)
				: (Input.GetKey(KeyCode.LeftShift)? 0.5f: 1f);
			_speed = Mathf.SmoothDamp(_speed, speedTarget, ref _speedVel, _accelerationTime);

			// Moving the character by root motion
			var animSpeed = GetInputVector().magnitude * _speed;
			_animator.SetFloat(_movementAnimParam, animSpeed);
			
			if (!_animator.hasRootMotion)
			{
				// Use procedural motion as we don't have root motion
				ApplyProceduralMotion(animSpeed);
			}
		}

		private void ApplyProceduralMotion(float animSpeed)
		{
			Vector3 move = transform.forward * animSpeed * _moveSpeed;
			if (CharacterController != null) 
			{
				CharacterController.SimpleMove(move);
			} else {
				transform.position += move * Time.deltaTime;
			}
		}
		
		#endregion Locomotion


		#region Helpers
		
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
		
		private void UpdateCamera()
		{
			if (null == _cameraController) { return; }
			_cameraController.UpdateInput();
			_cameraController.UpdateTransform();
		}
		
		#endregion Helpers

	}
}
