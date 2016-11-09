using GameWork.Core.Models.Interfaces;

namespace GameWork.Core.Audio
{
	public class MultiClipModel : IModel
	{
		public AudioClip[] Clips { get; set; }
	}
}