using UnityEngine;

namespace GameWork.Unity.Animation.Components
{
	/// <summary>
	/// Attach this to an animation state to instantiate a specific 
	/// GameObject when this animation state is entered.
	/// </summary>
	public class InstantiateOnEnter : StateMachineBehaviour 
	{
		[SerializeField]
		private GameObject _instantiate;

		public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex) 
		{
			Instantiate(_instantiate);
		}
	}
}