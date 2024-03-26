using Platformer.Mechanics;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;


public class SceneChange3DController : MonoBehaviour
{
    public string SceneName;
    public int PercentageChance;
    public List<string> MonsterTypes;
    public string SceneType;
    [SerializeField]
    private TextMeshProUGUI textMeshPro;
    private bool prepareToSceneChange = false;

    private void OnTriggerExit(Collider collision)
    {
        if (collision.CompareTag("Player"))
        {
            if (textMeshPro != null)
            {
                textMeshPro.text = "";
                prepareToSceneChange = false;
            }
            else
            {
                Debug.LogWarning("TextMeshPro component not found on the scene change object.");
            }
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            BoxCollider[] colliders = GetComponents<BoxCollider>();

            if (IsApproachCollider(colliders, GetComponent<BoxCollider>()))
            {
                prepareToSceneChange = true;
                if (textMeshPro != null)
                {
                    textMeshPro.text = $"Scene: {SceneName}";
                }
                else
                {
                    Debug.LogWarning("TextMeshPro component not found on the scene change object.");
                }
            }
            else
            {
                ChangeScene();
            }
        }
    }

    public void OnCollisionEnter(Collision collision)
    {
        if (collision.collider.CompareTag("Player"))
        {
            ChangeScene();
        }
    }

    private void Update()
    {
        var playerActions = GameObject.Find("PlayerActions").GetComponent<PlayerActionController>();
        if (prepareToSceneChange && (playerActions.isAttacking || playerActions.isClimbing))
        {
            ChangeScene();
        }
    }

    private bool IsApproachCollider(BoxCollider[] colliders, BoxCollider collider)
    {
        // Assuming the first collider is the approach collider
        return collider == colliders[0];
    }

    private void ChangeScene()
    {
        SceneManager.LoadScene(SceneName);
    }
}
