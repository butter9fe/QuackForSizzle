using CookOrBeCooked.Systems.EventSystem;
using UnityEngine;
namespace QuackForSizzle.Player
{
    [AddComponentMenu("Scripts/PlayerEventManager")]
    public class EventManager : EventManagerBase<Events.PlayerEvent, ArgsBase> { }
}
