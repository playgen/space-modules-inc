using GameWork.Core.Audio.Clip;
using GameWork.Core.Models.Interfaces;

namespace GameWork.Core.Audio.MultiClip
{
	public class MultiClipModel : IModel
	{
		public AudioClipModel[] Clips { get; set; }
	}
}