using System.Collections;
using UnityEngine;
using UnityEngine.Playables;
using LookingGlass;

namespace TimelineExtention
{
	public class HoloPlayPlayable : PlayableBehaviour
	{
        private struct ClipData{            
            public Vector3 localPosition;
            public float fov;
            public float nearParam;
            public float farParam;

            public float size;

            public ClipData(HoloPlayClip clip){
                this.localPosition = clip.localPosition;
                this.fov = clip.fov;
                this.nearParam = clip.nearParam;
                this.farParam = clip.farParam;
                this.size = clip.size;
            }
        }
        private ClipData clipData;

        public void SetData(HoloPlayClip clip)
        {
            clipData = new ClipData(clip);
        }

        public override void ProcessFrame(Playable playable, FrameData info, object playerData){
            Holoplay holoplay = playerData as Holoplay;
            if( holoplay == null){
                Debug.LogError("HoloPlay is null");
                return;
            }
            if( !holoplay.gameObject.activeInHierarchy || !holoplay.enabled ){
                return;
            }
            holoplay.fov = clipData.fov;
            holoplay.transform.localPosition = clipData.localPosition;
            holoplay.nearClipFactor = clipData.nearParam;
            holoplay.farClipFactor = clipData.farParam;
            holoplay.size = clipData.size;
            #if UNITY_EDITOR
            if(!UnityEditor.EditorApplication.isPlaying){
                holoplay.RenderQuilt();
            }
            #endif
        }

    }
}