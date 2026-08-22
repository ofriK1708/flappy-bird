using System;

using UnityEngine;

namespace Player
{
    /// <summary>
    /// Detects the bird crashing into a pipe or the ground and signals it via
    /// an event. Single Responsibility: detection only — this class does NOT
    /// know about game state, UI, or what "game over" even means. Whoever
    /// manages game state (GameStateManager, coming next) subscribes to
    /// FatalCrashed and decides what happens.
    ///
    /// Design choice (per project decision): pipes and the ground use trigger
    /// colliders, not solid ones. That means the bird's Rigidbody2D is never
    /// physically stopped on impact — it keeps falling under gravity exactly
    /// as it was, and OnTriggerEnter2D fires purely as a notification with no
    /// physics response. This is what gives the classic Flappy Bird "keep
    /// tumbling after you die" feel instead of an abrupt stop.
    /// </summary>
    public class BirdCollisionHandler : MonoBehaviour
    {
        [Tooltip("Tags that count as a fatal collision (must match Project Settings > Tags).")]
        [SerializeField] private string[] fatalTags = { "Pipe", "Ground" };

        public event Action FatalCrashed;

        private bool _hasCrashed;

        private void OnTriggerEnter2D(Collider2D other)
        {
            if (_hasCrashed) return;

            if (IsFatalCrash(other))
            {
                _hasCrashed = true;
                FatalCrashed?.Invoke();
                Debug.Log($"Fatal Crashed happened with {other.name}!");
            }
        }

        private bool IsFatalCrash(Collider2D other)
        {
            foreach (string fatalTag in fatalTags)
            {
                if (other.CompareTag(fatalTag))
                {
                    return true;
                }
            }
            return false;
        }
    }
}
