using Assets.Scripts;
using Platformer.Core;
using Platformer.Model;
using UnityEngine;

namespace Platformer.Mechanics
{
    /// <summary>
    /// This class exposes the the game model in the inspector, and ticks the
    /// simulation.
    /// </summary> 


    public class GameController : MonoBehaviour
    {


        public float timeScaleMultiplier;

        void Awake()
        {
            AppLogger.Initialize();
            //AppLogger.Logger.Information("GameController Awake and logger initialized");

            // Find and assign the player object dynamically
            AssignPlayer();
        }

        void AssignPlayer()
        {
            // Assuming the player has a tag "Player"
            GameObject playerGameObject = GameObject.FindGameObjectWithTag("Player");
            if (playerGameObject != null)
            {
                // Assuming the playerGameObject has a PlayerController component
                PlayerController playerController = playerGameObject.GetComponent<PlayerController>();
                if (playerController != null)
                {
                    model.player = playerController;
                }
                else
                {
                    Debug.LogError("PlayerController component not found on player game object.");
                }
            }
            else
            {
                Debug.LogError("Player game object not found.");
            }
        }


        public static GameController Instance { get; private set; }

        //This model field is public and can be therefore be modified in the 
        //inspector.
        //The reference actually comes from the InstanceRegister, and is shared
        //through the simulation and events. Unity will deserialize over this
        //shared reference when the scene loads, allowing the model to be
        //conveniently configured inside the inspector.
        public PlatformerModel model = Simulation.GetModel<PlatformerModel>();


        void OnEnable()
        {
            Instance = this;
            //Time.timeScale = 5f;
        }

        void OnDisable()
        {
            if (Instance == this) Instance = null;
        }

        void Update()
        {
            if (Instance == this) Simulation.Tick();
        }

        public void NormalSpeed()
        {
            Time.timeScale = Constants.RegularTimeScale;
        }

        public void HyperSpeed()
        {
            Time.timeScale = Constants.SkipTimeScale;
        }

        public void LightSpeed()
        {
            Time.timeScale = Constants.LightSpeedScale;
        }

        public void TimeScaleMultiplier()
        {
            Time.timeScale = timeScaleMultiplier;
        }
    }
}