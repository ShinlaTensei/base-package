using System;
using UnityEngine.Timeline;
using UnityEngine.Playables;
using System.Collections.Generic;
using System.Linq;
using Base.Logging;
using Object = UnityEngine.Object;

namespace Base.Helper
{
    public static class PlayableDirectorExtension
    {
        private static string LOG_FORMAT = "[PlayableExtension]{0}";

        public static void Bind(this PlayableDirector director, string trackName, Object objectToBind)
        {
            PlayableAsset asset = director.playableAsset;
            if (asset == null)
            {
                PDebug.ErrorFormat(LOG_FORMAT, "Bind Failed: Null ref on playable asset");
                return;
            }

            List<PlayableBinding> outputTracks = asset.outputs as List<PlayableBinding>;
            if (outputTracks == null)
            {
                return;
            }

            PlayableBinding trackAsset = outputTracks.Find(track => track.streamName.Equals(trackName));
            director.SetGenericBinding(trackAsset.sourceObject, objectToBind);
        }

        public static void Bind(this PlayableDirector director, string groupName, string trackName, Object objectToBind)
        {
            GroupTrack groupTrack = director.GetGroupTrack(groupName);
            if (groupTrack == null)
            {
                PDebug.ErrorFormat(LOG_FORMAT, $"Can't find the group {groupName}");
                return;
            }

            Bind(director, groupTrack, trackName, objectToBind);
        }

        public static void Bind(this PlayableDirector timeline, GroupTrack groupTrack, string trackName, Object objectToBind)
        {
            foreach (var childTrack in groupTrack.GetChildTracks())
            {
                foreach (var variableBinding in childTrack.outputs)
                {
                    if (variableBinding.streamName == trackName)
                    {
                        var track = variableBinding.sourceObject;
                        timeline.SetGenericBinding(track, objectToBind);
                        return;
                    }
                }
            }

            PDebug.ErrorFormat(LOG_FORMAT, $"Cant find the trackName: {trackName}");
        }

        public static GroupTrack GetGroupTrack(this PlayableDirector director, string groupName)
        {
            if (director == null || director.playableAsset == null || string.IsNullOrEmpty(groupName)) return null;
            TimelineAsset timelineAsset = director.playableAsset as TimelineAsset;
            if (timelineAsset == null) return null;
            foreach (var res in timelineAsset.GetRootTracks())
            {
                if (res is GroupTrack groupTrack && res.name.Equals(groupName))
                {
                    return groupTrack;
                }
            }

            return null;
        }

        public static TrackAsset GetOutputTrack(this PlayableDirector director, string trackName)
        {
            if (director == null || string.IsNullOrEmpty(trackName))
            {
                return null;
            }

            var timelineAsset = director.playableAsset as TimelineAsset;
            if (timelineAsset == null)
            {
                return null;
            }

            var outputTracks = timelineAsset.GetOutputTracks().ToList();
            return outputTracks?.Find(item => item.name.Equals(trackName));
        }

        public static void MuteTrack(this PlayableDirector director, string trackName, bool mute)
        {
            if (director == null || string.IsNullOrEmpty(trackName))
            {
                return;
            }

            TrackAsset trackAsset = director.GetOutputTrack(trackName);
            if (trackAsset == null)
            {
                PDebug.ErrorFormat(LOG_FORMAT, $"Cannot find track name: {trackName}");
                return;
            }

            trackAsset.muted = mute;
        }

        public static void MuteTrack(this PlayableDirector director, IEnumerable<string> trackNames, bool mute)
        {
            List<string> trackNameList = trackNames.ToList();
            if (director == null || !trackNameList.Any())
            {
                return;
            }

            TimelineAsset timelineAsset = director.playableAsset as TimelineAsset;
            if (timelineAsset == null)
            {
                return;
            }

            List<TrackAsset> outputTracks = timelineAsset.GetOutputTracks().ToList();
            foreach (var track in outputTracks)
            {
                if (track != null && trackNameList.Contains(track.name))
                {
                    track.muted = mute;
                }
            }
        }

        public static void MuteGroup(this PlayableDirector director, string groupName, bool mute)
        {
            if (director == null)
            {
                return;
            }

            var timelineAsset = director.playableAsset as TimelineAsset;
            if (timelineAsset == null)
            {
                return;
            }

            foreach (var res in timelineAsset.GetRootTracks())
            {
                if (res is GroupTrack groupTrack && groupTrack.name == groupName)
                {
                    groupTrack.muted = mute;
                }
            }
        }

        public static void PlayAtSpeed(this PlayableDirector director, float speed)
        {
            director.RebuildGraph(); // the graph must be created before getting the playable graph
            director.playableGraph.GetRootPlayable(0).SetSpeed(speed);
            director.Play();
        }

        // public static void PlayWithCustomCompleteDuration(this PlayableDirector timeline, float delay, TweenCallback completeCallback)
        // {
        //     if (timeline == null) return;
        //
        //     timeline.gameObject.SetActive(true);
        //     timeline.JumpToStart();
        //     timeline.Play();
        //     timeline.gameObject.DelayCall(delay, completeCallback);
        // }

        /// <summary>
        /// Return group track duration in ms
        /// </summary>
        /// <param name="director"></param>
        /// <param name="groupName"></param>
        /// <returns></returns>
        public static int GetGroupTrackDuration(this PlayableDirector director, string groupName)
        {
            if (director == null || director.playableAsset == null || string.IsNullOrEmpty(groupName))
            {
                return 0;
            }

            var track = director.GetGroupTrack(groupName);
            if (track == null)
            {
                return 0;
            }

            return (int) (track.GetChildTracks().Max(t => t.duration) * 1000);
        }

        /// <summary>
        /// Return real timeline duration in ms, based on not muted tracks and groups
        /// </summary>
        /// <param name="director"></param>
        /// <returns></returns>
        public static int GetTimelineDuration(this PlayableDirector director)
        {
            if (director == null || director.playableAsset == null)
            {
                return 0;
            }

            var timelineAsset = director.playableAsset as TimelineAsset;
            if (timelineAsset == null)
            {
                return 0;
            }

            double duration = 0;
            foreach (var res in timelineAsset.GetRootTracks())
            {
                if (res is GroupTrack groupTrack)
                {
                    if (!groupTrack.muted) duration = Math.Max(duration, groupTrack.GetChildTracks().Max(t => t.duration));
                }
                else
                {
                    if (!res.muted) duration = Math.Max(duration, res.duration);
                }
            }

            return (int) (duration * 1000);
        }

        public static void PlayAllGroups(this PlayableDirector director)
        {
            if (director == null || director.playableAsset == null)
            {
                return;
            }

            director.MuteAllGroup(false);
            director.RebuildGraph();
            director.Play();
        }

        public static void JumpToEnd(this PlayableDirector director)
        {
            if (director == null)
            {
                return;
            }

            var time = director.duration;
            director.time = time;
            director.Evaluate();
        }

        public static void JumpToStart(this PlayableDirector director)
        {
            if (director == null)
            {
                return;
            }

            director.time = 0;
            director.Evaluate();
        }

        public static void JumpToTime(this PlayableDirector director, double time)
        {
            if (director == null)
            {
                return;
            }

            director.time = time;
            director.Evaluate();
        }

        public static void MuteAllGroup(this PlayableDirector director, bool mute, bool muteAnotherTrack = false)
        {
            if (director == null)
            {
                return;
            }

            var timelineAsset = director.playableAsset as TimelineAsset;
            if (timelineAsset == null)
            {
                return;
            }

            foreach (var res in timelineAsset.GetRootTracks())
            {
                if (res is GroupTrack groupTrack)
                {
                    groupTrack.muted = mute;
                }
                else
                {
                    res.muted = muteAnotherTrack;
                }
            }
        }
        
        /// <summary>
    /// UnBind object to playable director.
    /// </summary>
    /// <param name="playableDirector"><seealso cref="PlayableDirector"/> to bind.</param>
    /// <param name="trackName">Name of the track to bind.</param>
    public static void UnBind(this PlayableDirector playableDirector, string trackName)
    {
        TimelineAsset timelineAsset = (TimelineAsset) playableDirector.playableAsset;
        foreach (PlayableBinding outputBinding in timelineAsset.outputs)
        {
            if (outputBinding.streamName != trackName) continue;

            Object track = outputBinding.sourceObject;
            playableDirector.ClearGenericBinding(track);
            return;
        }

        PDebug.ErrorFormat(LOG_FORMAT, $"Track {trackName} not found in {timelineAsset} ..");
    }

    public static void BindControlReference(this PlayableDirector timeline, string trackName, Object objectToBind)
    {
        var asset = timeline.playableAsset;
        BindControlReference(timeline, asset.outputs, trackName, objectToBind);
    }

    public static void BindControlReference(this PlayableDirector timeline, string groupName, string trackName, Object objectToBind)
    {
        var groupTrack = timeline.GetGroupTrack(groupName);
        if (groupTrack == null)
        {
            PDebug.ErrorFormat(LOG_FORMAT, $"Cant find the groupName: {groupName}");
            return;
        }
        BindControlReference(timeline, groupTrack, trackName, objectToBind);
    }

    public static void BindControlReference(this PlayableDirector timeline, GroupTrack groupTrack, string trackName, Object objectToBind)
    {
        BindControlReference(timeline, groupTrack.GetChildTracks().SelectMany(ct => ct.outputs), trackName, objectToBind);
    }

    static void BindControlReference(PlayableDirector timeline, IEnumerable<PlayableBinding> bindings, string trackName, Object objectToBind)
    {
        foreach (PlayableBinding binding in bindings)
        {
            if (binding.sourceObject is ControlTrack controlTrack)
            {
                if (controlTrack?.name == trackName)
                {
                    foreach (TimelineClip clip in controlTrack.GetClips())
                    {
                        if (clip.displayName.Equals(objectToBind.name))
                        {
                            ControlPlayableAsset playableClip = (ControlPlayableAsset)clip.asset;

                            timeline.SetReferenceValue(playableClip.sourceGameObject.exposedName, objectToBind);

                            return;
                        }
                    }
                }
            }
        }

        PDebug.ErrorFormat(LOG_FORMAT, $"Cant find the trackName: {trackName}");
    }

    public static void PlayGroup(this PlayableDirector director, string groupName,bool muteAnotherTracks = false)
    {
        if (director == null || director.playableAsset == null)
        {
            return;
        }

        director.MuteAllGroup(true, muteAnotherTracks);
        director.MuteGroup(groupName, false);
        director.RebuildGraph();
        director.Play();
    }

    public static void PlayGroups(this PlayableDirector director, IEnumerable<string> groupNames, bool muteAnotherTracks = false)
    {
        if (director == null)
        {
            return;
        }

        var timelineAsset = director.playableAsset as TimelineAsset;
        if (timelineAsset == null)
        {
            return;
        }

        foreach (var res in timelineAsset.GetRootTracks())
        {
            if (res is GroupTrack groupTrack)
            {
                groupTrack.muted = !groupNames.Contains(groupTrack.name);
            }
            else
            {
                res.muted = muteAnotherTracks;
            }
        }

        director.RebuildGraph();
        director.Play();
    }
    }
}