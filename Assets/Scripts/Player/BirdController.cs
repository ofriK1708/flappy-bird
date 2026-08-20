using UnityEngine;
using UnityEngine.InputSystem;

namespace FlappyBird.Player
{
    /// <summary>
    /// Reads flap input and applies the jump impulse to the bird's Rigidbody2D.
    /// Single Responsibility: this class ONLY moves the bird in response to input.
    /// Collision/death handling and scoring belong in their own components later
    /// (e.g. BirdCollisionHandler), which can subscribe to events raised from here
    /// instead of this class knowing about game-over or score logic.
    /// </summary>
    [RequireComponent(typeof(Rigidbody2D))]
    public class BirdController : MonoBehaviour
    {
        [Header("Input")]
        [Tooltip("Reference to the 'Flap' action in the Input Actions asset.")]
        [SerializeField] private InputActionReference flapAction;

        [Header("Flap Settings")]
        [Tooltip("Upward speed (m/s) applied to the bird on each flap.")]
        [SerializeField] private float flapVelocity = 5f;

        private Rigidbody2D rb;

        private void Awake()
        {
            rb = GetComponent<Rigidbody2D>();
        }

        private void OnEnable()
        {
            // TODO: two things need to happen here, in this order:
            //  1. Enable the action itself (an InputActionReference is NOT auto-enabled
            //     unless something like a PlayerInput component does it for you).
            //  2. Subscribe to its "performed" callback so OnFlap runs on input.
            // Think about: what happens if flapAction is left unassigned in the Inspector?
        }

        private void OnDisable()
        {
            // TODO: mirror OnEnable exactly, in reverse:
            //  1. Unsubscribe the callback.
            //  2. Disable the action.
            // Why does this matter? If this object gets disabled/re-enabled or destroyed
            // without unsubscribing, what could go wrong?
        }

        private void OnFlap(InputAction.CallbackContext context)
        {
            // TODO: call Jump()
        }

        private void Jump()
        {
            // TODO: apply the flap impulse to rb.
            //
            // Hint: if you only ever ADD upward velocity, a flap during a fast fall
            // will feel weak/inconsistent (old velocity + new velocity blend oddly).
            // Flappy Bird's classic feel comes from resetting vertical velocity to 0
            // right before applying flapVelocity, so every flap feels identical.
            //
            // rb.linearVelocity is the Unity 6 Rigidbody2D API (replaces .velocity).
        }
    }
}
