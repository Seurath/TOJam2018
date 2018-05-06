using System.Collections;
using System.Collections.Generic;
using RootMotion.FinalIK;
using Sirenix.OdinInspector;
using UnityEngine;

namespace TOJam2018
{
	[RequireComponent(typeof(LocomotionController))]
	public class PlayerCharacterController : MonoBehaviour
	{
		[Required, SerializeField] private AimIK _aimIKHead;
		[Required, SerializeField] private LocomotionController _locomotionController;
		
		[SerializeField] private bool _useMouseLookTarget = true;

		[ShowIf("_useMouseLookTarget"), SerializeField]
		private float _mouseLookHeight = 1.5f;
		
		
		#region MonoBehaviour
		
		private void Awake()
		{
			if (null == _locomotionController) { _locomotionController = GetComponent<LocomotionController>(); }
		}
		
		private void Start()
		{
			if (_useMouseLookTarget)
			{
				StartCoroutine(MouseLookPollingCoroutine());
			}
		}
		
		#endregion MonoBehaviour
		
		
		#region MouseLook
		
		private IEnumerator MouseLookPollingCoroutine()
		{
			while (true)
			{
				if (null == _locomotionController
				    || null == _aimIKHead) { continue; }

				_aimIKHead.solver.target.position = _locomotionController.LookTarget + (Vector3.up * _mouseLookHeight);
				yield return new WaitForEndOfFrame();
			}
		}
		
		#endregion MouseLook
		
	}
}
