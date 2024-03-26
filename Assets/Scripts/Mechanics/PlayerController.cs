using Assets.Scripts;
using Platformer.Mechanics.Stats;
using Platformer.Model;
using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;
using static Assets.Scripts.Mechanics.MechanicEnums;
using static Platformer.Core.Simulation;

namespace Platformer.Mechanics
{
    public class PlayerController : KinematicObject
    {

        public int moveDirection;
        public AudioClip respawnAudio;
        public AudioClip ouchAudio;
        [SerializeField]
        public PlayerActionController playerActions;
        public Collider2D collider2d { get; set; }
        public readonly PlatformerModel model = GetModel<PlatformerModel>();
        public AudioSource audioSource;
        public Health health;
        public Stamina stamina;
        [SerializeField]
        public bool controlEnabled;
        [SerializeField]
        public InputActionAsset controls;
        [SerializeField]
        public float staminaRegenTimer = 0f;
        public bool IsFacingRight => spriteRenderer.flipX == false;
        public bool IsFacingLeft => spriteRenderer.flipX == true;
        public InputActionAsset Controls
        {
            get { return controls; }
            set { controls = value; }
        }

        Vector2 move;
        public SpriteRenderer spriteRenderer { get; set; }
        internal Animator animator;
        public Bounds Bounds => collider2d.bounds;


        public void EnablePlayerControl()
        {
            controlEnabled = true;
        }

        public void DisablePlayerControl()
        {
            controlEnabled = false;
        }

        void Awake()
        {
            audioSource = GetComponent<AudioSource>();
            collider2d = GetComponent<Collider2D>();
            spriteRenderer = GetComponent<SpriteRenderer>();
            animator = GetComponent<Animator>();
            rigidbody2D = GetComponent<Rigidbody2D>();
            //DontDestroyOnLoad(this);
        }

        public void OnCollisionEnter2D(Collision2D collision)
        {
            //Debug.Log("Collision detected with name: " + collision.collider.name + "\n Collision detected with tag: " + collision.collider.tag);
            //if (collision.collider.name == "Enemy" && playerActions.isDashing)
            //{
            //    //Debug.Log("Detected Enemy in front of player");
            //    CapsuleCollider2D enemyCollider = collision.collider.GetComponent<CapsuleCollider2D>();
            //    enemyCollider.isTrigger = true;
            //    StartCoroutine(ResetColliderAfterSlide(enemyCollider));
            //}           
        }
        IEnumerator ResetColliderAfterSlide(CapsuleCollider2D collider)
        {
            yield return new WaitForSeconds(Constants.DashDuration);
            collider.isTrigger = false;
        }

        protected override void Update()
        {
            RegenerateStamina();
            base.Update();
        }

        private void RegenerateStamina()
        {
            staminaRegenTimer += Time.deltaTime;
            if (staminaRegenTimer >= 0.1f) // Check if 1/10th of a second has passed
            {
                stamina.Increment(0.1f); // Increment stamina by 1/10th of a unit
                staminaRegenTimer -= 0.1f; // Decrease the timer by 0.1f instead of resetting to 0
            }
        }

        public void UpdateMoveDirection(int direction)
        {
            moveDirection = direction;
        }

        public void StopSlide()
        {
            // Reset velocity or modify movement logic to stop sliding
            velocity = Vector2.zero; // Or appropriate logic to stop sliding
        }


        protected override void ComputeVelocity()
        {
            if (controlEnabled)
            {
                if (playerActions?.jumpState == ActionState.InAction)
                {
                    JumpAnimation();
                }


        
                if (playerActions?.jump ?? false && IsGrounded)
                {
                    velocity.y = Constants.JumpTakeOffSpeed * model.jumpModifier;
                }
                else if (playerActions?.stopJump ?? false)
                {
                    if (velocity.y > 0)
                    {
                        velocity.y = velocity.y * model.jumpDeceleration;
                    }
                }

                if (moveDirection != 0)
                {
                    spriteRenderer.flipX = moveDirection < 0;
                    move.x = moveDirection;
                }
                else
                {
                    move.x = 0; // Stop movement when moveDirection is zero
                }

                if (playerActions?.grabState == ActionState.Acting)
                {
                    //ApplyGravity(Constants.PercentageOfGravityWhileGrabbing);
                }


                if (playerActions.IsSprinting)
                {
                   move.x = move.x * Constants.SprintSpeedMultiplier;
                }
                else
                {
                    move.x = Mathf.Clamp(move.x, -Constants.DefaultMaxSpeed, Constants.DefaultMaxSpeed);
                }

                targetVelocity = move * Constants.DefaultMaxSpeed;
            }

            //ApplyGravity();
            GroundedAnimation();
        }

        public void ApplyGravity(float gravity = Constants.DefaultGravity)
        {
            if (!IsGrounded)
            {
                velocity.y += -gravity * Time.deltaTime;
            }
        }

        public void GroundedAnimation()
        {
            animator.SetBool("grounded", IsGrounded);
            animator.SetFloat("velocityY", Mathf.Abs(velocity.x) / Constants.DefaultMaxSpeed);
        }

        public void JumpAnimation()
        {
            animator.SetBool("grounded", !IsGrounded);
            animator.SetFloat("velocityY", velocity.y);
        }



        public void StartWalking(bool right)
        {
            if (right)
            {
                rigidbody2D.velocity = new Vector2(Constants.CutsceneProtagonistWalkSpeed, rigidbody2D.velocity.y);
            }
            else
            {
                spriteRenderer.flipX = true;
                rigidbody2D.velocity = new Vector2(-Constants.CutsceneProtagonistWalkSpeed, rigidbody2D.velocity.y);

            }
        }
        public void DisableAnimator()
        {
            animator.enabled = false;
        }

        public void EnableAnimator()
        {
            animator.enabled = true;
        }
        public void StopWalking()
        {

            rigidbody2D.velocity = new Vector2(0f, rigidbody2D.velocity.y);
        }
    }
}