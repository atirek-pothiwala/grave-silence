using UnityEngine;
using UnityEngine.InputSystem;

namespace GraveSilence.Core
{
    /// <summary>
    /// Bridges Unity's new Input System to gameplay systems.
    /// Assign the GraveSilence Input Actions asset in the inspector.
    /// </summary>
    public class InputManager : MonoBehaviour
    {
        public static InputManager Instance { get; private set; }

        [SerializeField] private InputActionAsset inputActions;

        public InputAction MoveAction { get; private set; }
        public InputAction LookAction { get; private set; }
        public InputAction SprintAction { get; private set; }
        public InputAction CrouchAction { get; private set; }
        public InputAction InteractAction { get; private set; }
        public InputAction Ability1Action { get; private set; }
        public InputAction Ability2Action { get; private set; }
        public InputAction Ability3Action { get; private set; }
        public InputAction Ability4Action { get; private set; }
        public InputAction PauseAction { get; private set; }

        private void Awake()
        {
            if (Instance != null && Instance != this)
            {
                Destroy(gameObject);
                return;
            }

            Instance = this;
            DontDestroyOnLoad(gameObject);
            BindActions();
        }

        private void OnEnable()
        {
            inputActions?.Enable();
        }

        private void OnDisable()
        {
            inputActions?.Disable();
        }

        private void BindActions()
        {
            if (inputActions == null) return;

            var playerMap = inputActions.FindActionMap("Player", true);
            MoveAction = playerMap.FindAction("Move", true);
            LookAction = playerMap.FindAction("Look", true);
            SprintAction = playerMap.FindAction("Sprint", true);
            CrouchAction = playerMap.FindAction("Crouch", true);
            InteractAction = playerMap.FindAction("Interact", true);
            Ability1Action = playerMap.FindAction("Ability1", true);
            Ability2Action = playerMap.FindAction("Ability2", true);
            Ability3Action = playerMap.FindAction("Ability3", true);
            Ability4Action = playerMap.FindAction("Ability4", true);
            PauseAction = playerMap.FindAction("Pause", true);
        }
    }
}
