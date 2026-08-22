using UnityEngine;
using UnityEngine.Pool;
using Random = UnityEngine.Random;

namespace Environment
{
    /// <summary>
    /// Spawns PipePair instances from an object pool at a fixed interval,
    /// positioning each one just off the right edge of the camera with a
    /// randomized vertical gap. Owns all pooling — PipeMover itself has zero
    /// knowledge that pooling exists; it just moves and raises OffScreen.
    /// </summary>
    public class PipeSpawner : MonoBehaviour
    {
        [Header("References")]
        [Tooltip("The PipePair prefab to spawn (must have a PipeMover component).")]
        [SerializeField]
        private PipeMover pipePrefab;

        [Tooltip("The camera whose view bounds determine spawn/despawn X and the gap's Y range.")] [SerializeField]
        private Camera gameCamera;

        [Header("Spawn Settings")] [Tooltip("Seconds between each pipe spawn.")] [SerializeField]
        private float spawnInterval = 1.5f;

        [Tooltip("Leftward speed (units/second) given to every spawned pipe.")] [SerializeField]
        private float scrollSpeed = 3f;

        [Tooltip("Extra world-space margin beyond the camera edge before a pipe spawns/despawns.")] [SerializeField]
        private float edgeBuffer = 1f;

        [Tooltip("Minimum distance the randomized gap center must keep from the top/bottom camera edge.")]
        [SerializeField]
        private float gapVerticalMargin = 1f;

        private ObjectPool<PipeMover> _pool;
        private float _spawnX;
        private float _despawnX;
        private float _minGapY;
        private float _maxGapY;
        private float _timeUntilNextSpawn;

        private void Awake()
        {
            // TODO: compute _spawnX, _despawnX, _minGapY, _maxGapY from gameCamera here.
            //
            // For an orthographic camera:
            //   float halfHeight = gameCamera.orthographicSize;
            //   float halfWidth  = halfHeight * gameCamera.aspect;
            //
            // _spawnX   = camera's right edge + edgeBuffer
            // _despawnX = camera's left edge  - edgeBuffer
            // _minGapY / _maxGapY = camera's bottom/top edge, pulled inward by gapVerticalMargin
            //
            // Don't hardcode around world origin (0,0) — base everything off
            // gameCamera.transform.position, in case the camera isn't centered there.
            float halfHeight = gameCamera.orthographicSize;
            float halfWidth = halfHeight * gameCamera.aspect;


            float cameraX = gameCamera.transform.position.x;
            _spawnX = cameraX + halfWidth + edgeBuffer;
            _despawnX = cameraX - halfWidth - edgeBuffer;

            float cameraY = gameCamera.transform.position.y;
            _minGapY = cameraY - halfHeight + gapVerticalMargin;
            _maxGapY = cameraY + halfHeight - gapVerticalMargin;


            _pool = new ObjectPool<PipeMover>(
                createFunc: CreatePipe,
                actionOnGet: OnPipeTakenFromPool,
                actionOnRelease: OnPipeReturnedToPool,
                actionOnDestroy: OnPipeDestroyed,
                collectionCheck: true,
                defaultCapacity: 4,
                maxSize: 10);
        }

        private void Update()
        {
            // TODO: count _timeUntilNextSpawn down by Time.deltaTime. Once it reaches
            // zero or below, call SpawnPipe() and reset the timer back to spawnInterval.
            _timeUntilNextSpawn -= Time.deltaTime;
            if (_timeUntilNextSpawn <= 0f)
            {
                SpawnPipe();
                _timeUntilNextSpawn = spawnInterval;
            }
        }

        private void SpawnPipe()
        {
            // TODO:
            //  1. Get a pipe from the pool: var pipe = _pool.Get();
            //  2. Position it at (_spawnX, a random Y, 0f) — use
            //     Random.Range(_minGapY, _maxGapY) for the Y.
            //  3. Call pipe.ResetForSpawn(scrollSpeed, _despawnX) to activate it with
            //     the bounds this spawner just computed.
            // getting the pipe and setting pos
            var pipe = _pool.Get();
            float randomYPos = Random.Range(_minGapY, _maxGapY);
            pipe.transform.position = new Vector3(_spawnX, randomYPos, pipe.transform.position.z);

            // resting it to be ready for spawning
            pipe.ResetForSpawn(scrollSpeed, _despawnX);
        }

        /// <summary>ObjectPool calls this exactly once per NEW pipe instance it needs to create.</summary>
        private PipeMover CreatePipe()
        {
            // TODO: Instantiate(pipePrefab, transform), and return the resulting PipeMover.
            //
            // This is also where you must subscribe to the new pipe's OffScreen event —
            // and ONLY here, not in OnPipeTakenFromPool below. Why? CreatePipe runs once
            // per physical GameObject the pool ever creates. OnPipeTakenFromPool runs
            // every time that SAME object is reused from the pool. C# events don't
            // deduplicate identical subscriptions — if you subscribed in the "get" callback
            // instead, every reuse would stack another "+=" onto the invocation list, so
            // after a few spawn/despawn cycles the same pipe's off-screen event would call
            // your handler 2x, then 3x, then 4x per despawn.
            PipeMover pipe = Instantiate(pipePrefab, transform);
            pipe.OffScreen += HandlePipeOffScreen;
            return pipe;
        }

        private void OnPipeTakenFromPool(PipeMover pipe)
        {
            // TODO: activate the pipe's GameObject.
            pipe.gameObject.SetActive(true);
        }

        private void OnPipeReturnedToPool(PipeMover pipe)
        {
            // TODO: deactivate the pipe's GameObject.
            pipe.gameObject.SetActive(false);
        }

        private void OnPipeDestroyed(PipeMover pipe)
        {
            // TODO: Destroy(pipe.gameObject). Only runs if the pool exceeds maxSize.
            Destroy(pipe.gameObject);
        }

        /// <summary>Subscribed once per pipe, in CreatePipe. Fires when that pipe scrolls off-screen.</summary>
        private void HandlePipeOffScreen(PipeMover pipe)
        {
            // TODO: return it to the pool.
            _pool.Release(pipe);
        }
    }
}