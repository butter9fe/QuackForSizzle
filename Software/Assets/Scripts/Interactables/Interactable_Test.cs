using UnityEngine;
using CookOrBeCooked.Systems.InteractionSystem;

/// <summary>
/// 
/// </summary>
public class Interactable_Test : InteractableObjectBase
{
    public override void OnActionCancelled()
    {
        // Do nothing
    }

    public override void OnActionHeld()
    {
        // Do nothing
    }

    public override void OnActionPerformed()
    {
        Debug.Log("Interacted: " + gameObject.name);
    }
}
