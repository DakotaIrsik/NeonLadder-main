using Platformer.Mechanics;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;


public class SceneChange2DController : MonoBehaviour
{
    public string SceneName;
    public int PercentageChance;
    public List<string> MonsterTypes;
    public string SceneType;
    private TextMeshProUGUI textMeshPro;
    private bool prepareToSceneChange = false;

    private void Start()
    {
        textMeshPro = GetComponent<TextMeshProUGUI>();
    }

    private void OnTriggerExit2D(Collider2D collision)
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

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            BoxCollider2D[] colliders = GetComponents<BoxCollider2D>();

            if (IsApproachCollider(colliders, GetComponent<BoxCollider2D>()))
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

    public void OnCollisionEnter2D(Collision2D collision)
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

    private bool IsApproachCollider(BoxCollider2D[] colliders, BoxCollider2D collider)
    {
        // Assuming the first collider is the approach collider
        return collider == colliders[0];
    }

    private void ChangeScene()
    {
        SceneManager.LoadScene(SceneName);
    }
}
