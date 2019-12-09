using System.Collections;
using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.Timeline;

namespace TimelineExtention
{
	public class HoloPlayClip : PlayableAsset, ITimelineClipAsset
	{
        
        [SerializeField]
        public Vector3 localPosition;
        [SerializeField]
        public float fov = 30;
        [SerializeField]
        public float nearParam = 1;
        [SerializeField]
        public float farParam = 1.5f;
        
        [SerializeField]
        public float size = 0.5f;
        

        public ClipCaps clipCaps
		{
			get { return ClipCaps.None; }
		}

        public GameObject prefab
        {
            get;set;
        }
        public override Playable CreatePlayable(PlayableGraph graph, GameObject owner)
        {

            var playable = ScriptPlayable<HoloPlayPlayable>.Create(graph);

            var behaviour = playable.GetBehaviour();
            behaviour.SetData( this );
			return playable;   
		}
	}
}