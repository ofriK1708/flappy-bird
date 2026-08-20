using UnityEngine;
using UnityEngine.InputSystem;

namespace Player
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
        [Header("Input")] [Tooltip("Reference to the 'Flap' action in the Input Actions asset.")] [SerializeField]
        private InputActionReference flapAction;

        [Header("Flap Settings")] [Tooltip("Upward speed (m/s) applied to the bird on each flap.")] [SerializeField]
        private float flapVelocity = 5f;

        private Rigidbody2D _rb;

        private void Awake()
        {
            _rb = GetComponent<Rigidbody2D>();
        }

        private void OnEnable()
        {
            if (flapAction != null)
            {
                flapAction.action.Enable();
                flapAction.action.performed += OnFlap;
            }
            else
            {
                Debug.LogError($"flap action on {gameObject.name} is null",this);
            }
        }

        private void OnDisable()
        {
            if (flapAction != null)
            {
                flapAction.action.Disable();
                flapAction.action.performed -= OnFlap;
            }
            else
            {
                Debug.LogError($"flap action on {gameObject.name} is null",this);
            }
        }

        private void OnFlap(InputAction.CallbackContext context)
        {
            Jump();
        }

        private void Jump()
        {
            // Overwriting (not adding to) linearVelocity keeps every flap identical
            // regardless of current fall speed, and guarantees zero horizontal drift.
            _rb.linearVelocity = Vector2.up * flapVelocity;
        }
    }
}