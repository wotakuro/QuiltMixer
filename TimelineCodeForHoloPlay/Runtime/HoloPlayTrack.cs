using System.Collections;
using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.Timeline;

namespace TimelineExtention
{
	[TrackClipType(typeof(HoloPlayClip))]
    [TrackBindingType(typeof(LookingGlass.Holoplay))]
	public class HoloPlayTrack : TrackAsset {

#if UNITY_EDITOR
        private PlayableDirector playableDirector;
#endif

        protected override Playable CreatePlayable(PlayableGraph graph, GameObject gameObject, TimelineClip clip)
        {
            HoloPlayClip enemyClip = clip.asset as HoloPlayClip;
#if UNITY_EDITOR
            this.playableDirector = gameObject.GetComponent<PlayableDirector>();
#endif            
            return base.CreatePlayable(graph, gameObject, clip);
        }
#if UNITY_EDITOR
        public void RebuildGraph()
        {
            if (playableDirector != null)
            {
                playableDirector.RebuildGraph();
                playableDirector.Evaluate();
            }
        }
#endif
    }
}