using Assets.Scripts;
using Platformer.Gameplay;
using Platformer.UI;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;
using static Assets.Scripts.Mechanics.MechanicEnums;
using static Platformer.Core.Simulation;

namespace Platformer.Mechanics
{

    public class PlayerActionController : MonoBehaviour
    {
        public MetaGameController meta;
        public GameObject bulletPrefab;
        private string bindingOutput = string.Empty;
        [SerializeField]
        public PlayerController player;
        public AudioClip jumpAudio;
        private Vector2 playerInput;
        private float sprintTimeAccumulator = 0f;
        [SerializeField]
        public TextMeshProUGUI PlayerActionsDebugText;
        [SerializeField]
        public TextMeshProUGUI AnimationDebuggingText;
        [SerializeField]
        public TextMeshProUGUI InputDebuggingText;
        [SerializeField]
        public TextMeshProUGUI ControllerNameDebuggingText;

        [SerializeField]
        public TextMeshProUGUI TouchScreenSupportDebuggingText;

        public GameObject leftHitbox;
        public GameObject rightHitbox;

        private List<string> Keypresses = new List<string>();

        private bool showControllerDebugLastState = Constants.DisplayControllerDebugInfo;
        private InputDevice controller;
        public bool IsFacingRight { get; private set; }
        public bool IsFacingLeft { get; private set; }


        private void Start()
        {
            if (player == null)
            {
                player = GameObject.Find("Protagonist").GetComponent<PlayerController>();
            }
            IsFacingLeft = player.spriteRenderer.flipX;
            IsFacingRight = !player.spriteRenderer.flipX;
            ConfigureControls();
            DisableHitboxes();
        }

        private void OnEnable()
        {
            InputSystem.onDeviceChange += OnDeviceChanged;
        }

        private void OnDisable()
        {
            InputSystem.onDeviceChange -= OnDeviceChanged;
        }

        private bool HasTouchScreen => Input.touchSupported;

        private void OnDeviceChanged(InputDevice device, InputDeviceChange change)
        {
                switch (change)
                {
                    case InputDeviceChange.Added:
                        // Handle device added
                        break;
                    case InputDeviceChange.Removed:
                        // Handle device removed - this can be used to infer a "fallback"
                        break;
                    case InputDeviceChange.Disconnected:
                        // Handle device disconnected - this can also infer a "fallback"
                        break;
                        // There are other event types you can handle
                }

            // Update the UI with the last used device
            controller = device;
        }

        private string GetDeviceDebugText()
        {
            string result = string.Empty;
            if (controller == null)
            {
                result = "Virtual Gamepad UI in use or no input controller detected";
            }
            else if (controller is Gamepad gamepad)
            {
                // Check the name of the gamepad to identify the type
                var name = gamepad.name.ToLowerInvariant();
                if (name.Contains("dualshock") || name.Contains("playstation"))
                {
                    result = "PlayStation Controller in use";
                }
                else if (name.Contains("xbox"))
                {
                    result = "Xbox Controller in use";
                }
                else if (name.Contains("switch"))
                {
                    result = "Switch Controller in use";
                }
                else
                {
                    // If none of the above, default to this
                    result = $"{gamepad.name} in use";
                }
            }
            else if (controller is Keyboard)
            {
                result = "Keyboard in use";
            }
            else if (controller is Mouse)
            {
                result = "Mouse in use";
            }
            else
            {
                // If it's not a gamepad, keyboard, or mouse, you can default to this
                result = $"{controller.displayName} in use";
            }

            return result;
        }

        private void ConfigureControls()
        {
            var playerActionMap = player.controls.FindActionMap("Player");
            playerActionMap.Enable();

            var grabAction = playerActionMap.FindAction("Grab");
            grabAction.performed += OnGrabPerformed;
            grabAction.canceled += OnGrabCanceled;

            var jumpAction = playerActionMap.FindAction("Jump");
            jumpAction.performed += OnJumpPerformed;
            jumpAction.canceled += OnJumpCanceled;

            var sprintAction = playerActionMap.FindAction("Sprint");
            sprintAction.performed += OnSprintPerformed;
            sprintAction.canceled += OnSprintCanceled;

            var slideAction = playerActionMap.FindAction("Dash");
            slideAction.performed += OnDashPerformed;

            var moveLeftAction = playerActionMap.FindAction("MoveLeft");
            moveLeftAction.performed += OnMoveLeftPerformed;
            moveLeftAction.canceled += OnMoveCanceled;

            var moveRightAction = playerActionMap.FindAction("MoveRight");
            moveRightAction.performed += OnMoveRightPerformed;
            moveRightAction.canceled += OnMoveCanceled;

            var upAction = playerActionMap.FindAction("Up");
            upAction.performed += OnUpPerformed;

            var meleeAction = playerActionMap.FindAction("MeleeAttack");
            meleeAction.performed += OnMeleeActionPerformed;

            PrintDebugControlConfiguration();
        }


        void OnDrawGizmos()
        {
            Constants.DisplayAnimationDebugInfo = true;
            Constants.DisplayPlayerActionDebugInfo = true;
            Constants.DisplayKeyPresses = true;
            Constants.DisplayControllerDebugInfo = true;
        }


        private void OnMeleeActionPerformed(InputAction.CallbackContext context)
        {
            leftHitbox.SetActive(!IsFacingRight); // Enable left hitbox if facing left
            rightHitbox.SetActive(IsFacingRight); // Enable right hitbox if facing right

            if (isDashing && !isDashAttacking)
            {
                InitAction("isDashAttacking", Constants.DashAttackDuration);
                isDashAttacking = true;
            }
            else if (!isAttacking)
            {
                InitAction("isAttacking", Constants.MeleeAttackDuration);
                isAttacking = true;
            }

            StartCoroutine(DisableHitboxes());
        }

        private void InitAction(string name, float duration)
        {
            SetFlagAndAnimation(name, true);
            StartCoroutine(ResetFlagAndAnimation(name, duration));
        }

        public IEnumerator DisableHitboxes()
        {
            yield return new WaitForSeconds(Constants.MeleeAttackDuration);
            leftHitbox.SetActive(false);
            rightHitbox.SetActive(false);
        }

        private void SetFlag(string propertyName, bool value)
        {
            var prop = GetType().GetField(propertyName, BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.Public);
            if (prop != null)
            {
                prop.SetValue(this, value);
            }

        }
        private void SetFlag(string propertyName, float value)
        {
            GetType().GetField(propertyName, BindingFlags.NonPublic | BindingFlags.Instance)?.SetValue(this, value);
        }

        private void SetFlagAndAnimation(string name, bool value)
        {
            SetFlag(name, value);
            player.animator.SetBool(name, value);
        }

        private IEnumerator ResetFlagAndAnimation(string name, float duration)
        {
            yield return new WaitForSeconds(duration);
            GetType().GetField(name, BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.Public)?.SetValue(this, false);
            player.animator.SetBool(name, false);
        }

        private void PrintDebugControlConfiguration()
        {
            if (bindingOutput == string.Empty)
            {
                var actionBindings = new Dictionary<string, List<string>>();

                foreach (var action in player.Controls.FindActionMap("Player").actions)
                {
                    foreach (var binding in action.bindings)
                    {
                        string deviceName = FormatDeviceName(binding.path.Split('/')[0].Replace("<", "").Replace(">", "")); // Extract and format device name
                        string controlName = binding.path.Split('/').Last();

                        string formattedBinding = $"{controlName} ({deviceName})";

                        if (!actionBindings.ContainsKey(action.name))
                        {
                            actionBindings[action.name] = new List<string>();
                        }
                        actionBindings[action.name].Add(formattedBinding);
                    }
                }
                var fullBindings = new List<string>();
                foreach (var actionBinding in actionBindings)
                {
                    string actionName = actionBinding.Key;
                    string bindings = string.Join(", ", actionBinding.Value);
                    fullBindings.Add($"{actionName} = {bindings}");
                }

                bindingOutput = string.Join("\n", fullBindings);
                Debug.Log(bindingOutput);
            }
        }

        #region Jumping
        [SerializeField]
        public ActionState jumpState = ActionState.Ready;
        [SerializeField]
        public bool jump;
        public bool IsJumping => jumpState == ActionState.Preparing || jumpState == ActionState.Acting || jumpState == ActionState.InAction || !stopJump;
        [SerializeField]
        public bool stopJump;

        #endregion

        #region Grabbing
        [SerializeField]
        public ActionState grabState = ActionState.Ready;
        public bool IsGrabbing => grabState == ActionState.Acting || grabState == ActionState.InAction || !stopGrab;
        [SerializeField]
        public bool stopGrab;
        #endregion

        #region Sprinting
        [SerializeField]
        public float sprintSpeed = Constants.DefaultMaxSpeed * Constants.SprintSpeedMultiplier;
        [SerializeField]
        public float sprintDuration = Constants.SprintDuration; // seconds
        [SerializeField]
        public ActionState sprintState = ActionState.Ready;
        public bool IsSprinting => sprintState == ActionState.Acting;
        [SerializeField]
        public bool stopSprint;
        #endregion

        #region Dashing
        [SerializeField]
        public float dashSpeed = Constants.DefaultMaxSpeed * Constants.DashSpeedMultiplier;
        [SerializeField]
        public float dashDuration = Constants.DashDuration; // seconds
        [SerializeField]
        public ActionState dashState = ActionState.Ready;
        [SerializeField]
        public bool stopDash;
        public bool isDashing = false;
        #endregion


        #region Knockback
        [SerializeField]
        public float knockbackDuration = Constants.DefaultKnockbackDuration;
        [SerializeField]
        public float knockbackSpeed = Constants.DefaultKnockbackSpeed;
        [SerializeField]
        public bool knockback;


        [SerializeField]
        public ActionState knockbackState = ActionState.Ready;
        public bool IsKnockedBack => knockbackState == ActionState.Preparing || sprintState == ActionState.Acting || sprintState == ActionState.InAction || knockback;
        #endregion

        #region Melee
        [SerializeField]
        public bool isAttacking = false;
        #endregion

        #region Dash Attack
        [SerializeField]
        public bool isDashAttacking = false;
        #endregion


        #region Climbinb
        [SerializeField]
        public bool isClimbing = false;
        #endregion



        private void Update()
        {
            IsFacingLeft = player.spriteRenderer.flipX;
            IsFacingRight = !player.spriteRenderer.flipX;

            ////hack, need to add input buffering.
            if (Keypresses.Contains("MoveRight") && player.velocity.x == 0)
            {
                // Reapply the move right input
                player.moveDirection = 1;
                playerInput = new Vector2(1, 0); // Adjust as necessary 
            }
            // Similarly, check for "MoveLeft"
            else if (Keypresses.Contains("MoveLeft") && player.velocity.x == 0)
            {
                // Reapply the move left input
                player.moveDirection = -1;
                playerInput = new Vector2(-1, 0); // Adjust as necessary
            }

            if (player.controlEnabled)
            {
                UpdateJumpState(player.IsGrounded);
                UpdateGrabState(player.velocity);
                UpdateSprintState(playerInput, player.velocity);
                UpdateDashState(playerInput, player.velocity);
                //DashAnimation();
                UpdateKnockbackstate();
                UpdateJumpAnimationParameters();
            }

            if (showControllerDebugLastState != Constants.DisplayControllerDebugInfo)
            {
                showControllerDebugLastState = Constants.DisplayControllerDebugInfo;
            }

            if (PlayerActionsDebugText != null)
            {
                PlayerActionsDebugText.gameObject.SetActive(Constants.DisplayPlayerActionDebugInfo);
                if (Constants.DisplayPlayerActionDebugInfo)
                {
                    PlayerActionsDebugText.text = GetPlayerActionParameters();
                }
            }


            if (AnimationDebuggingText != null)
            {
                AnimationDebuggingText.gameObject.SetActive(Constants.DisplayAnimationDebugInfo);
                if (Constants.DisplayAnimationDebugInfo)
                {
                    AnimationDebuggingText.text = GetAnimationParameters();
                }
            }


            if (InputDebuggingText != null)
            {
                InputDebuggingText.gameObject.SetActive(Constants.DisplayKeyPresses);
                if (Constants.DisplayKeyPresses)
                {
                    InputDebuggingText.text = $"Keys: {string.Join(", ", Keypresses)}";
                }
            }

            if (TouchScreenSupportDebuggingText != null)
            {
                TouchScreenSupportDebuggingText.gameObject.SetActive(Constants.DisplayTouchScreenDebugInfo);
                if (Constants.DisplayTouchScreenDebugInfo)
                {
                    TouchScreenSupportDebuggingText.text = $"Touch Screen support: {HasTouchScreen}";
                }
            }

            if (ControllerNameDebuggingText != null)
            {
                ControllerNameDebuggingText.gameObject.SetActive(Constants.DisplayControllerDebugInfo);
                if (Constants.DisplayControllerDebugInfo)
                {
                    ControllerNameDebuggingText.text = $"Controller: {GetDeviceDebugText()}";
                }
            }

        }

        //gets all parameters from animator and specifies string like this:
        //ParameterName1: Value1
        //ParameterName2: Value2
        private string GetAnimationParameters()
        {
            var result = "Animation Parms \r\n";
            foreach (var parameter in player.animator.parameters)
            {
                if (parameter.type == AnimatorControllerParameterType.Bool)
                {
                    result += $"{parameter.name}: {player.animator.GetBool(parameter.name)}\n";
                }
                else if (parameter.type == AnimatorControllerParameterType.Float)
                {
                    result += $"{parameter.name}: {player.animator.GetFloat(parameter.name)}\n";
                }
                else if (parameter.type == AnimatorControllerParameterType.Int)
                {
                    result += $"{parameter.name}: {player.animator.GetInteger(parameter.name)}\n";
                }
            }
            return $"{result}";
        }


        private string GetPlayerActionParameters()
        {
            var result = "Action Parms\r\n";
            result += $"VelocityX: {player.velocity.x}\n";
            result += $"VelocityY: {player.velocity.y}\n";
            result += $"IsGrounded: {player.IsGrounded}\n";
            result += $"isAttacking: {isAttacking}\n";
            result += $"IsJumping: {IsJumping}\n";
            result += $"isDashing: {isDashing}\n";
            result += $"isDashAttacking: {isDashAttacking}\n";
            result += $"isClimbing: {isClimbing}\n";
            result += $"jumpState: {jumpState}\n";
            result += $"sprintDuration: {sprintDuration}\n";
            result += $"sprintState: {sprintState}\n";
            result += $"dashDuration: {dashDuration}\n";
            result += $"dashState: {dashState}\n";
            result += $"IsSprinting: {IsSprinting}\n";
            return result;
        }

        void UpdateJumpAnimationParameters()
        {
            bool isGrounded = player.IsGrounded;
            float verticalVelocity = player.velocity.y;

            // Update the Animator parameters
            player.animator.SetFloat("velocityX", Mathf.Abs(player.velocity.x));
            player.animator.SetFloat("velocityY", verticalVelocity);
            player.animator.SetBool("grounded", isGrounded);

            // Define a small threshold for floating-point imprecision
            float fallingThreshold = -0.01f;

            // Check if the player has just started falling
            if (!isGrounded && verticalVelocity < fallingThreshold)
            {
                player.animator.SetBool("isFalling", true);
            }
            // Check if the player has landed
            else if (isGrounded)
            {
                player.animator.SetBool("isFalling", false);
            }
        }


        public void UpdateDashState(Vector2 move, Vector2 velocity)
        {
            var playerRigidbody = player.GetComponent<Rigidbody2D>();
            switch (dashState)
            {
                case ActionState.Ready:
                    dashDuration = 0f;
                    break;
                case ActionState.Preparing:
                    dashDuration = Constants.DashDuration; // Reset the sprint duration
                    dashState = ActionState.Acting;
                    break;
                case ActionState.Acting:
                    if (dashDuration <= 0f)
                    {
                        dashState = ActionState.Acted;
                    }
                    else if (playerInput != Vector2.zero)
                    {
                        Vector3 direction = new Vector3(playerInput.x, 0, playerInput.y);
                        playerRigidbody.velocity = direction * dashSpeed;
                        dashDuration -= Time.deltaTime;
                    }
                    else
                    {
                        velocity.x = Mathf.Sign(move.x) * Constants.DashSpeedMultiplier;
                        dashDuration -= Time.deltaTime;
                    }
                    break;
                case ActionState.Acted:
                    dashDuration = 0f;
                    playerRigidbody.velocity = Vector2.zero;
                    dashState = ActionState.Ready;
                    isDashing = false;
                    stopDash = false;
                    break;
            }
        }

        public void UpdateSprintState(Vector2 move, Vector2 velocity)
        {
            float staminaCostPerTenthSecond = Constants.SprintStaminaCost * 0.1f;
            switch (sprintState)
            {
                case ActionState.Preparing:
                    sprintDuration = Constants.SprintDuration; // Reset the sprint duration
                    sprintState = ActionState.Acting;
                    stopSprint = false;
                    sprintTimeAccumulator = 0f; // Reset the time accumulator
                    break;

                case ActionState.Acting:
                    if (stopSprint || sprintDuration <= 0)
                    {
                        sprintState = ActionState.Acted;
                    }
                    else
                    {
                        sprintTimeAccumulator += Time.deltaTime;
                        if (sprintTimeAccumulator >= 0.1f)
                        {
                            player.stamina.Decrement(staminaCostPerTenthSecond); // Decrement stamina
                            sprintTimeAccumulator -= 0.1f; // Subtract 0.1 seconds from the accumulator
                        }

                        if (Mathf.Abs(move.x) > 0f)
                        {
                            move.x = move.x * sprintSpeed;
                        }
                        else
                        {
                            move.x = move.x * sprintSpeed * -1;
                        }
                        sprintDuration -= Time.deltaTime;
                    }
                    break;

                case ActionState.Acted:
                    sprintState = ActionState.Ready;
                    stopSprint = false;
                    break;
            }
        }

        public void UpdateJumpState(bool IsGrounded)
        {
            jump = false;
            switch (jumpState)
            {
                case ActionState.Preparing:
                    jumpState = ActionState.Acting;
                    jump = true;
                    stopJump = false;
                    break;
                case ActionState.Acting:
                    if (!IsGrounded)
                    {
                        Schedule<PlayerJumped>().player = player;
                        jumpState = ActionState.InAction;
                    }
                    break;
                case ActionState.InAction:
                    if (IsGrounded)
                    {
                        Schedule<PlayerLanded>().player = player;
                        jumpState = ActionState.Acted;
                    }
                    break;
                case ActionState.Acted:
                    jumpState = ActionState.Ready;
                    break;
            }
        }

        public void UpdateKnockbackstate()
        {
            if (knockback)
            {
                switch (knockbackState)
                {
                    case ActionState.Ready:
                        knockbackState = ActionState.Preparing;
                        break;
                    case ActionState.Preparing:
                        knockbackDuration = Constants.DefaultKnockbackDuration;
                        knockbackState = ActionState.Acting;
                        break;
                    case ActionState.Acting:
                        if (knockbackDuration <= 0 || player.health.current == 0)
                        {
                            knockbackState = ActionState.Acted;
                        }
                        Vector2 moveAmount = (player.spriteRenderer.flipX ? Vector2.right : Vector2.left) * Constants.DefaultKnockbackSpeed * Time.deltaTime;
                        player.transform.position += (Vector3)moveAmount;
                        knockbackDuration -= Time.deltaTime;
                        break;
                    case ActionState.InAction:
                        knockbackState = ActionState.Acted;
                        break;
                    case ActionState.Acted:
                        knockbackState = ActionState.Ready;
                        knockback = false;
                        break;
                }
            }
        }

        public void HandleWallGrab(Vector2 velocity)
        {
            if (grabState == ActionState.Acting)
            {
                if (Mathf.Abs(velocity.y) > 0.1f)
                {
                    velocity.y *= Constants.PercentageOfGravityWhileGrabbing;
                }
                else
                {
                    velocity.y = 0;
                }
            }
        }

        public void UpdateGrabState(Vector2 velocity)
        {
            if (grabState == ActionState.Acting)
            {
                if (velocity.y < 0)
                {
                    jumpState = ActionState.Ready;
                }
            }
        }

        private void OnGrabPerformed(InputAction.CallbackContext context)
        {
            if (player.stamina.IsExhausted) return;
            player.stamina.Decrement(Constants.GrabStaminaCost);
            //if player is facing the wall
            if (player.collider2d.IsTouchingLayers(LayerMask.GetMask("Walls")))
            {
                grabState = ActionState.Acting;
                player.velocity = -Constants.DefaultGravity * Constants.PercentageOfGravityWhileGrabbing * Time.deltaTime * -Vector2.up;

            }
        }

        private void OnGrabCanceled(InputAction.CallbackContext context)
        {
            if (grabState == ActionState.Acting || grabState == ActionState.InAction)
            {
                grabState = ActionState.Ready;
            }
        }

        private void OnSprintPerformed(InputAction.CallbackContext context)
        {
            if (!player.stamina.IsExhausted)
            {
                if (sprintState == ActionState.Ready)
                {
                    sprintState = ActionState.Preparing;
                    stopSprint = false;
                }
            }
        }

        private void OnSprintCanceled(InputAction.CallbackContext context)
        {
            if (IsSprinting)
            {
                stopSprint = true;
            }
        }
        public void Jump()
        {
            OnJumpPerformed(new InputAction.CallbackContext());
        }

        public void OnJumpPerformed(InputAction.CallbackContext context)
        {
            if (player.stamina.IsExhausted) return;
            player.stamina.Decrement(Constants.JumpStaminaCost);
            if (jumpState == ActionState.Ready || (grabState == ActionState.Acting && player.collider2d.IsTouchingLayers(LayerMask.GetMask("Level"))))
            {
                jumpState = ActionState.Preparing;

                if (grabState == ActionState.Acting)
                {
                    player.velocity.x = Constants.WallJumpForce * (player.spriteRenderer.flipX ? 1 : -1);
                    grabState = ActionState.Ready;
                }
                player.velocity.y = Constants.JumpTakeOffSpeed * player.model.jumpModifier;
            }

            stopJump = true;
            Schedule<PlayerStopJump>().player = player;
        }

        private void OnUpPerformed(InputAction.CallbackContext context)
        {
            InitAction("isClimbing", Constants.ClimbDuration);
        }

        private void OnDashPerformed(InputAction.CallbackContext context)
        {
            InitAction("isDashing", Constants.DashDuration);
            if (player.stamina.IsExhausted) return;
            player.stamina.Decrement(Constants.DashStaminaCost);
            if (dashState == ActionState.Ready)
            {
                isDashing = true;
                dashState = ActionState.Preparing;
                stopDash = false;

                // Determine the dash direction based on the facing direction
                Vector2 dashDirection = DetermineDashDirection();
                // Apply dash in the determined direction
                StartCoroutine(ApplyDash(dashDirection.normalized));
            }
        }

        private Vector2 DetermineDashDirection()
        {
            if (playerInput != Vector2.zero)
            {
                // Use the current input direction if the player is moving
                return new Vector2(playerInput.x, 0);
            }
            else
            {
                // Use the sprite's facing direction if the player is stationary
                if (player.spriteRenderer.flipX)
                {
                    playerInput = new Vector2(-Constants.DashSpeedMultiplier, 0);
                    return Vector2.left;

                }
                else
                {
                    playerInput = new Vector2(Constants.DashSpeedMultiplier, 0);
                    return Vector2.right;
                }
            }
        }

        private IEnumerator ApplyDash(Vector2 direction)
        {
            float elapsedTime = 0;
            Rigidbody2D playerRigidbody = player.GetComponent<Rigidbody2D>();

            while (elapsedTime < dashDuration)
            {
                playerRigidbody.velocity = direction * dashSpeed;

                elapsedTime += Time.deltaTime;
                yield return null;
            }
            // Optionally, reset velocity after dashing
            Keypresses.Remove("Dash");
            playerRigidbody.velocity = Vector2.zero;
        }



        private void OnMoveRightPerformed(InputAction.CallbackContext context)
        {
            Keypresses.Add(context.action.name);
            player.moveDirection = 1;
            playerInput = new Vector2(1, 0); // Assuming right movement is along the positive x-axis
        }

        private void OnMoveLeftPerformed(InputAction.CallbackContext context)
        {
            Keypresses.Add(context.action.name);
            player.moveDirection = -1;
            playerInput = new Vector2(-1, 0); // Assuming left movement is along the negative x-axis
        }

        private void OnMoveCanceled(InputAction.CallbackContext context)
        {
            Keypresses.Remove(context.action.name);
            player.UpdateMoveDirection(0);
            playerInput = Vector2.zero; // No movement input
        }


        private void OnJumpCanceled(InputAction.CallbackContext context)
        {
            if (IsJumping)
            {
                player.velocity.y *= Constants.JumpCutOffFactor;
                stopJump = true;
            }
        }

        private string FormatDeviceName(string deviceName)
        {
            switch (deviceName)
            {
                case "Keyboard":
                    return "Keyboard";
                case "XInputController":
                    return "Xbox"; // maybe also steam?
                case "SwitchProControllerHID":
                    return "Nintendo Switch";
                case "DualShockGamepad":// Add more cases as needed for other devices
                    return "Playstation";
                default:
                    return deviceName; // Return the original name if not recognized
            }
        }
    }
}
