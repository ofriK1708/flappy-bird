using System;
using UnityEngine;

namespace Environment
{
    /// <summary>
    /// Moves a pipe pair leftward at a constant speed and signals once it has
    /// scrolled off-screen. Single Responsibility: movement + the "I'm off-screen"
    /// signal only — this class does NOT know pooling exists. Whoever spawned this
    /// pipe (PipeSpawner, coming next) is responsible for releasing it back to the
    /// pool when OffScreen fires.
    /// </summary>
    public class PipeMover : MonoBehaviour
    {
        [Tooltip("Leftward speed in units/second.")] [SerializeField]
        private float scrollSpeed = 3f;

        [Tooltip("World X position at which this pipe counts as off-screen.")] [SerializeField]
        private float despawnX = -12f;
        [Header("Debug")]
        [Tooltip("Editor testing only. When enabled, this pipe starts moving immediately in " +
                 "Start() instead of waiting for PipeSpawner to call ResetForSpawn() — lets you " +
                 "test a single PipePair placed directly in the scene, with no spawner involved.")]
        [SerializeField] private bool shouldBypassSpawner;

        /// <summary>Raised exactly once per spawn, the moment this pipe crosses despawnX.</summary>
        public event Action<PipeMover> OffScreen;

        private bool _onScreen;

        private void Start()
        {
            if(shouldBypassSpawner)
            {
               _onScreen = true;
               Debug.Log("Bypass spawner, pipe appears and move immediately");
            }
        }


        private void Update()
        {
            // TODO: 1. Move transform.position leftward by scrollSpeed * Time.deltaTime.
            //
            //       2. If transform.position.x has crossed despawnX, invoke OffScreen(this).
            //
            //          Careful: Update() keeps running every frame. If you don't guard
            //          against it, you'll invoke OffScreen every single frame this
            //          pipe sits past despawnX, not just once. What happens later when
            //          a pool receives the same object "released" multiple times?
            //          Think about how you'd track "have I already fired this?".
            if (_onScreen)
            {
                transform.position += Vector3.left * (scrollSpeed * Time.deltaTime);
                if (transform.position.x < despawnX)
                {
                    _onScreen = false;
                    OffScreen?.Invoke(this);
                    Debug.Log("Pipe moves off screen");
                }
            }
        }

        /// <summary>
        /// Called by PipeSpawner right after getting this pipe from the pool.
        /// newDespawnX overrides the eyeballed Inspector value with the spawner's
        /// camera-computed bound, so this stays correct at any screen size — the
        /// Inspector value now only matters for the shouldBypassSpawner debug path.
        /// </summary>
        public void ResetForSpawn(float speed, float newDespawnX)
        {
            scrollSpeed = speed;
            despawnX = newDespawnX;
            _onScreen = true;
        }
    }
}