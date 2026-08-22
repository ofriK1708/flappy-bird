using System;
using UnityEngine;

namespace Player
{
    /// <summary>
    /// Detects the bird passing through a pipe's ScoreZone trigger and signals
    /// it. Single Responsibility: detection only — same split as
    /// BirdCollisionHandler, deliberately kept as a separate small component
    /// rather than merged into it, since "did I crash" and "did I score" are
    /// different questions even though both start from OnTriggerEnter2D.
    ///
    /// Notice this needs no "already fired" guard, unlike BirdCollisionHandler.
    /// That guard existed there because the bird can overlap two DIFFERENT
    /// fatal colliders in one frame. Here, every ScoreZone the bird enters is
    /// a distinct pipe it hasn't scored yet — there's nothing to double-fire
    /// against, so adding a guard field would just be unused state.
    /// </summary>
    public class BirdScoreDetector : MonoBehaviour
    {
        [Tooltip("Tag identifying a pipe's score zone (must match Project Settings > Tags).")]
        [SerializeField] private string scoreZoneTag = "ScoreZone";

        public event Action Scored;

        private void OnTriggerEnter2D(Collider2D other)
        {
            if(other.CompareTag(scoreZoneTag)) 
            {
                Scored?.Invoke();
            }
        }
    }
}
