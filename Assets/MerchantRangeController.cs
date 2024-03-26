using Assets.Scripts;
using Platformer.Mechanics;
using System;
using System.Collections;
using UnityEngine;

public class MerchantRangeController : MonoBehaviour
{
    public OverrideControllerAssigner overrideControllerAssigner;
    public DialogueManager dialogueManager;

    private void OnTriggerExit2D(Collider2D collision)
    {
       dialogueManager.HideDialogues();
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player") && !Constants.UnlockedSword)
        {
            collision.GetComponent<PlayerController>().controlEnabled = false;
            dialogueManager.Show("merchant-first-sword");
            StartCoroutine(EnableControlAfterDialogue(collision));
        }
    }

    private IEnumerator EnableControlAfterDialogue(Collider2D collision)
    {
        //wait three seconds then hide dialogue and enable control and set sword bool
        yield return new WaitForSeconds(5);
        dialogueManager.HideDialogues();
        collision.GetComponent<PlayerController>().controlEnabled = true;
        Constants.UnlockedSword = true;
    }
}
