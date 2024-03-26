using Assets.Scripts;
using Platformer.Gameplay;
using Platformer.Mechanics.Stats;
using Platformer.Model;
using System.Collections;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;
using static Platformer.Core.Simulation;

namespace Platformer.Mechanics
{
    public class BossController : KinematicObject
    {

        public int moveDirection;
        public AudioClip respawnAudio;
        public AudioClip ouchAudio;
        public Collider2D collider2d { get; set; }
        public readonly PlatformerModel model = GetModel<PlatformerModel>();
        [SerializeField]
        public BossActionController bossActions;
        public AudioSource audioSource;
        public Health health;
        public Stamina stamina;
        public bool controlEnabled = false;
        [SerializeField]
        public InputActionAsset controls;
        [SerializeField]
        public float staminaRegenTimer = 0f;
        private bool IsFlying = false;
        private bool IsHovering = false;
        private float FlightXDirection = 0f;
        private float FlightYDirection = 0f;
        [SerializeField]
        public bool IsFacingLeft = true;
        public AudioClip ouch;
        internal PatrolPath.Mover mover;
        internal AnimationController control;
        internal Collider2D _collider;
        internal AudioSource _audio;

        [SerializeField]
        private bool disengage = false; // New override flag

        public Bounds Bounds => _collider.bounds;

        Vector2 move;
        public SpriteRenderer spriteRenderer { get; set; }
        public bool IsFacingRight => !IsFacingLeft;
        internal Animator animator;


        void OnCollisionEnter2D(Collision2D collision)
        {

            switch (collision.collider.tag)
            {
                case "Weapon":
                    var wbc = Schedule<WeaponBossCollision>();
                    wbc.player = collision.gameObject.GetComponent<PlayerController>();
                    wbc.boss = this;
                    wbc.collider = collision;

                    break;
                case "Player":
                    var pbc = Schedule<PlayerBossCollision>();
                    pbc.player = collision.gameObject.GetComponent<PlayerController>(); ;
                    pbc.boss = this;
                    pbc.collider = collision;
                    break;
                default:
                    break;
            }
        }


        void Awake()
        {
            control = GetComponent<AnimationController>();
            _collider = GetComponent<Collider2D>();
            _audio = GetComponent<AudioSource>();
            audioSource = GetComponent<AudioSource>();
            collider2d = GetComponent<Collider2D>();
            spriteRenderer = GetComponent<SpriteRenderer>();
            animator = GetComponent<Animator>();

            rigidbody2D = GetComponent<Rigidbody2D>();

            var isBossScene = gameObject.scene.name == gameObject.name;
            var isRunningInEditor = Application.isEditor;

            if (isBossScene && !(disengage && isRunningInEditor))
            {
                animator.SetBool("engaged", true);
            }
            else
            {
                animator.enabled = false;
                StartCoroutine(StartIdleAnimation());
            }
        }

        IEnumerator StartIdleAnimation()
        {
            float delay = Random.Range(0.0f, 1.03f);
            Debug.Log($"Delay: {delay} before starting animator for {gameObject.name}");
            yield return new WaitForSeconds(delay);
            if (gameObject.name == "FinalBoss")
            {
                animator.SetInteger("animation", 1); //idle-b
            }
            else
            {
                animator.SetInteger("animation", 13); //idle-b
            }
            animator.enabled = true;
        }


        protected override void Update()
        {
            //Update sprite renderer flipX based on move direction
            if (move.x > 0.01f)
            {
                spriteRenderer.flipX = false;
                IsFacingLeft = false;
            }
            else if (move.x < -0.01f)
            {
                spriteRenderer.flipX = true;
                IsFacingLeft = true;
            }

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


        protected override void ComputeVelocity()
        {

            if (!IsFlying && !IsHovering)
            {
                if (moveDirection != 0)
                {
                    if (moveDirection > 0)
                    {
                        transform.eulerAngles = new Vector3(0, -230, 0);
                    }
                    else
                    {
                        transform.eulerAngles = new Vector3(0, 230, 0);
                    }
                    move.x = moveDirection;
                }
                else
                {
                    move.x = 0; // Stop movement when moveDirection is zero
                }

                ApplyGravity();

                targetVelocity = move * Constants.DefaultMaxSpeed;
            }
            else
            {

                if (!IsHovering)
                {
                    //Vector2 flightDirection = new Vector2(FlightXDirection, FlightYDirection).normalized;
                    //targetVelocity = flightDirection * Constants.CutsceneFinalBossFlySpeed;
                    velocity.y = FlightYDirection * Constants.FlySpeed;
                    velocity.x = FlightXDirection * Constants.FlySpeed;

                    move.x = Mathf.Clamp(velocity.x, - Constants.DefaultMaxSpeed, Constants.DefaultMaxSpeed);
                    targetVelocity = move * Constants.DefaultMaxSpeed;
                }
                else
                {
                    //counteract gravity
                    velocity.y = Constants.HoverYVelocity;
                    move.x = 0;
                    targetVelocity = move * Constants.DefaultMaxSpeed;
                }
            }


        }


        public void ApplyGravity(float gravity = Constants.DefaultGravity)
        {
            if (gravity == 0)
            {
                velocity.y = 0;
                return;
            }

            velocity.y += -gravity * Time.deltaTime;

        }

        /// <summary>
        /// Fly in the specified direction using a vector.
        /// </summary>
        /// <param name="flightVector">A string representing the flight direction vector.
        /// Examples:
        /// "0,1" - Upward vertical flight.
        /// "0,-1" - Downward vertical flight.
        /// "1,0" - Rightward horizontal flight.
        /// "-1,0" - Leftward horizontal flight.
        /// "1,1" - Up-right diagonal flight.
        /// "-1,1" - Up-left diagonal flight.
        /// "1,-1" - Down-right diagonal flight.
        /// "-1,-1" - Down-left diagonal flight.
        /// </param>
        public void Fly(string flightVector)
        {
            ApplyGravity(0);
            IsFlying = true;
            FlightXDirection = float.Parse(flightVector.Split(',')[0]);
            FlightYDirection = float.Parse(flightVector.Split(',')[1]);
        }

        public void Hover()
        {
            IsHovering = true;
        }

        public void HoverStop()
        {

            IsHovering = false;
        }

        public void RotateX(float xRotation)
        {
            Vector3 currentRotation = transform.eulerAngles;
            currentRotation.x = xRotation;
            transform.eulerAngles = currentRotation;
        }

        public void StartWalking(bool right)
        {
            spriteRenderer.flipX = !right;
            rigidbody2D.velocity = new Vector2(-Constants.CutsceneFinalBossWalkSpeed, rigidbody2D.velocity.y);

        }
        public void StopWalking()
        {

            rigidbody2D.velocity = new Vector2(0f, rigidbody2D.velocity.y);
        }
        public void TransformZ(float z)
        {
            // Get the current position of the GameObject
            Vector3 currentPos = transform.position;

            // Update the z value
            currentPos.z = z;

            // Set the updated position back to the GameObject
            transform.position = currentPos;

        }
    }
}