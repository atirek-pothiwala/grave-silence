using UnityEngine;
using UnityEngine.InputSystem;
using GraveSilence.Core;

namespace GraveSilence.Player
{
    /// <summary>
    /// Wires InputManager actions to player components.
    /// </summary>
    public class PlayerInputHandler : MonoBehaviour
    {
        [SerializeField] private ThirdPersonController movement;
        [SerializeField] private UmbralAbilities abilities;
        [SerializeField] private StealthTakedown takedown;

        private InputManager input;

        private void Awake()
        {
            movement ??= GetComponent<ThirdPersonController>();
            abilities ??= GetComponent<UmbralAbilities>();
            takedown ??= GetComponent<StealthTakedown>();
        }

        private void OnEnable()
        {
            input = InputManager.Instance;
            if (input == null) return;

            input.SprintAction.performed += OnSprintStart;
            input.SprintAction.canceled += OnSprintEnd;
            input.CrouchAction.performed += OnCrouchStart;
            input.CrouchAction.canceled += OnCrouchEnd;
            input.InteractAction.performed += OnInteract;
            input.Ability1Action.performed += OnAbility1;
            input.Ability2Action.performed += OnAbility2;
            input.Ability3Action.performed += OnAbility3;
            input.Ability4Action.performed += OnAbility4;
            input.PauseAction.performed += OnPause;
        }

        private void OnDisable()
        {
            if (input == null) return;

            input.SprintAction.performed -= OnSprintStart;
            input.SprintAction.canceled -= OnSprintEnd;
            input.CrouchAction.performed -= OnCrouchStart;
            input.CrouchAction.canceled -= OnCrouchEnd;
            input.InteractAction.performed -= OnInteract;
            input.Ability1Action.performed -= OnAbility1;
            input.Ability2Action.performed -= OnAbility2;
            input.Ability3Action.performed -= OnAbility3;
            input.Ability4Action.performed -= OnAbility4;
            input.PauseAction.performed -= OnPause;
        }

        private void Update()
        {
            if (input == null || movement == null) return;
            movement.Move(input.MoveAction.ReadValue<Vector2>());
        }

        private void OnSprintStart(InputAction.CallbackContext _) => movement?.SetSprinting(true);
        private void OnSprintEnd(InputAction.CallbackContext _) => movement?.SetSprinting(false);
        private void OnCrouchStart(InputAction.CallbackContext _) => movement?.SetCrouching(true);
        private void OnCrouchEnd(InputAction.CallbackContext _) => movement?.SetCrouching(false);
        private void OnInteract(InputAction.CallbackContext _) => takedown?.TryTakedown();
        private void OnAbility1(InputAction.CallbackContext _) => abilities?.TryUmbralStep();
        private void OnAbility2(InputAction.CallbackContext _) => abilities?.TryUmbralCloak();
        private void OnAbility3(InputAction.CallbackContext _) => abilities?.TryUmbralLure();
        private void OnAbility4(InputAction.CallbackContext _) => abilities?.TryUmbralStrike();
        private void OnPause(InputAction.CallbackContext _) => GameManager.Instance?.TogglePause();
    }
}
