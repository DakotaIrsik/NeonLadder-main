using System.Collections.Generic;

namespace Assets.Scripts
{
    public static class Constants
    {

        #region Player Unlocks etc, need to move to real storage
        public static bool UnlockedSword { get; set; } = true;
        public static List<string> DefeatedBosses { get; set; } = new List<string>();
        #endregion


        public static string Player => nameof(Player);
        public static string PlayerWeapon => nameof(PlayerWeapon);
        public static string EnemyWeapon = "EnemyWeapon";
        public static string BossWeapon = "BossWeapon";



        #region Settings
        public static bool DisplayKeyPresses { get; set; } = true;
        public static bool DisplayAnimationDebugInfo { get; set; } = true;
        public static bool DisplayPlayerActionDebugInfo { get; set; } = true;
        public static bool DisplayControllerDebugInfo { get; set; } = true;
        public static bool DisplayTouchScreenDebugInfo { get; set;} = true;


        #endregion

        #region Bosses
        public static float BossSpeed = 2.5f;
        #endregion

        #region world
        public const float DefaultGravity = 9.81f;
        public static float DefaultRayLength = 0.5f;
        public static float DefaultRayWidth = 0.05f;
        public const float RegularTimeScale = 1f;
        public const float SkipTimeScale = 10f;
        public const float LightSpeedScale = 100f;
        public static float IconAnimationDefaultDuration = 1.1f;
        public const int SceneChangesPerLevel = 3;

        //movement
        public const float HoverYVelocity = 0.25f;
        public const float StaminaRegenRate = 5.0f;
        public const float FlySpeed = 1f;
        #endregion

        #region player
        public const float DefaultMaxSpeed = 4f;
        public const float DefaultJumpTakeOffSpeed = 13f;
        public const float DefaultCharacterScale = 1f;
        public const float DefaultMaxHealth = 3f;
        public const float DefaultMaxStamina = 100f;
        public const float DefaultKnockbackDuration = 0.25f; // Duration of the knockback effect
        public const float DefaultKnockbackSpeed = 2f;
        public const float DefaultKnockMultiplier = 1f;
        

        public static float JumpTakeOffSpeed { get; set; } = DefaultJumpTakeOffSpeed;
        public static float CrouchScale { get; set; } = DefaultCharacterScale * 0.5f;
        public static float CutsceneProtagonistWalkSpeed { get; set; } = DefaultMaxSpeed * 0.4f;
        public static float CutsceneFinalBossWalkSpeed { get; set; } = DefaultMaxSpeed;
        public static float MaxHealth { get; set; } = DefaultMaxHealth;
        public static float MaxStamina { get; set; } = DefaultMaxStamina;
        #endregion


        #region actions
        /// <summary>
        /// The JumpCutOffFactor is a multiplier used to reduce the upward velocity of the player's character when the jump button is released before reaching the maximum jump height. This factor allows for variable jump heights, providing a more dynamic and responsive jumping mechanic in the game. Here's a detailed explanation and some sample values:
        /// </summary>
        public const float JumpCutOffFactor = 0.5f;
        public const float JumpDuration = 1.0f;
        public const float JumpCooldown = 0.5f;
        public const float JumpStaminaCost = 2.0f;
        public const float WallJumpForce = 5.0f;

        public const float SprintDuration = 100f;
        public const float SprintCooldown = 1f;
        public const float SprintSpeedMultiplier = 1.5f;
        public const float SprintStaminaCost = 3f;

        public const float DashDuration = 0.6f;
        public const float DashCooldown = 1f;
        public const float DashSpeedMultiplier = 2f;
        public const float DashStaminaCost = 50f;

        public const float GrabDuration = 0.5f;
        public const float GrabCooldown = 1f;
        public const float GrabSpeedMultiplier = 1f;
        public const float PercentageOfGravityWhileGrabbing = 0.0f; 
        public const float GrabStaminaCost = 25f;

        public const float MeleeAttackDuration = 1.1f;
        public const float MeleeAttackScaleX = 0.5f;
        public const float MeleeAttackScaleY = 0.5f;
        public const float MeleeAttackScaleZ = 0.5f;
        public const float MeleeStaminaCost = 5.0f;


        public const float DashAttackDuration = 0.9f;

        public const float RangedAttackDuration = 0.1f;
        public const float RangedAttackStaminaCost = 5.0f;
        public const float RangedAttackMaxOnScreen = 3.0f;
        public const float RangedAttackLifeTime = 3.0f;
        public const float RangedAttackSpeed = 10.0f;

        public const float ClimbDuration = 0.8f;



        public const float CrouchStaminaCost = 25.0f;

        #endregion


        #region scenes
        public static List<string> MinorEnemyLevels = new List<string> { "Minor-Enemy-1", "Minor-Enemy-2", "Minor-Enemy-3" };
        public static List<string> MajorEnemyLevels = new List<string> { "Major-Enemy-1", "Major-Enemy-2", "Major-Enemy-3" };
        public static List<string> Bosses = new List<string> { "Wrath", "Gluttony", "Pride", "Sloth", "Envy", "Lust", "Greed" };
        public static List<string> ShopLevels = new List<string> { "Shop-1", "Shop-2", "Shop-3" };
        public static List<string> MinorEnemies = new List<string> { "Chili", "Kiwi", "Eggy", "Langsat" };
        public static List<string> MajorEnemies = new List<string> { "BlackKnight", "Werewolf", "FlyingDemon" };
        #endregion

    }
}

