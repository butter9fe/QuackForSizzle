using UnityEngine;
using System.Collections;
using QuackForSizzle;
using QuackForSizzle.Player;

namespace CookOrBeCooked.Systems.InteractionSystem
{
    /// <summary>
    /// Base class for Interactable Objects, i.e. objects which players can interact with and highlight.
    /// 
    /// <para>
    /// This script should be added to a GameObject with the layer
    /// from <see cref="InteractionHandlerBase"/>'s <see cref="InteractablePriorityConfig"/> LayerMask!
    /// </para>
    /// 
    /// In most cases, this will be something like <c>"Interactables"</c>
    /// </summary>
    [RequireComponent(typeof(Collider))]
    public abstract class InteractableObjectBase : MonoBehaviour
    {
        #region Properties
        [Foldout("Config")]
        [Tooltip("Priority of interaction when competing amongst other interactables." +
        "\nThis takes precedence OVER distance!" +
        "\nHigher number => Higher Priority")]
        [SerializeField] protected int _priority = 0;
        public int Priority => _priority;

        /// <summary>
        /// Whether object can be interacted with. 
        /// This can be overridden to include custom checks 
        /// (eg: correct player type, inventory status)
        /// </summary>
        [Tooltip("Whether object can be interacted with.")]
        [SerializeField] protected bool _canInteract = true;
        public virtual bool CanInteract { get => _canInteract; protected set => _canInteract = value; }

        [Foldout("Highlighting")]
        [SerializeField] protected InteractableHighlightConfig _highlightConfig;
        [SerializeField] protected Transform _renderersParent;
        [SerializeField] protected bool _enableHighlighting = true;

        private Renderer[] _renderers = new Renderer[0]; // List of Renderer to highlight
        private bool _isSelected = false;
        public bool IsSelected => _isSelected;
        public void SetIsSelected(bool selected, PlayerNumber? currPlayer = null)
        {
            _isSelected = selected;

            if (selected)
            {
                OnSelectedEvent?.Invoke(this);
                _currSelectedPlayer = currPlayer.Value;
            }
            else
            {
                OnUnselectedEvent?.Invoke(this);
                _currSelectedPlayer = null;
            }
        }

        protected PlayerNumber? _currSelectedPlayer = null;
        #endregion Properties

        #region Events
        public System.Action<InteractableObjectBase> OnSelectedEvent { get; protected set; }
        public System.Action<InteractableObjectBase> OnUnselectedEvent { get; protected set; }
        #endregion Events

        #region LifeCycle Methods
        protected virtual void Awake()
        {
            if (_renderersParent == null)
                _renderersParent = this.transform;
        }

        protected virtual void OnEnable()
        {
            StartCoroutine(GetRenderers());
            _isSelected = false;

            OnSelectedEvent += HighlightRenderers;
            OnUnselectedEvent += UnhighlightRenderers;
        }

        private void OnDisable()
        {
            OnSelectedEvent -= HighlightRenderers;
            OnUnselectedEvent -= UnhighlightRenderers;
        }
        #endregion LifeCycle Methods

        #region Public Methods
        public abstract void OnActionPerformed(PlayerNumber playerNumber);
        public abstract void OnActionHeld(PlayerNumber playerNumber);
        public abstract void OnActionCancelled(PlayerNumber playerNumber);

        public void SetRenderersParent(Transform newRenderersParent)
        {
            _renderersParent = newRenderersParent;

            if (newRenderersParent != null)
                StartCoroutine(GetRenderers());
            else
            {
                // No more _renderers, cleanup
                UnsetEmissionColorInRenderers();    // unhighlight
                _renderers = new Renderer[0];    // clear array
            }
        }
        #endregion Public Methods

        #region Highlighting Methods
        private IEnumerator GetRenderers()
        {
            // Wait 1 frame to get _renderers
            yield return new WaitForEndOfFrame();
            _renderers = _renderersParent.GetComponentsInChildren<Renderer>(true);
        }

        protected virtual void HighlightRenderers(InteractableObjectBase obj)
        {
            // Highlight renderers
            if (_enableHighlighting) // if highlighting _renderers is enabled
                SetEmissionColorInRenderers();
        }

        protected virtual void UnhighlightRenderers(InteractableObjectBase obj)
        {
            // Unhighlight renderers
            if (_enableHighlighting) // if highlighting _renderers is enabled
                UnsetEmissionColorInRenderers();
        }

        private void SetEmissionColorInRenderers()
        {
            foreach (Renderer r in _renderers)
            {
                for (int i = 0; i < r.materials.Length; i++)
                {
                    r.materials[i].SetColor("_EmissionColor", _highlightConfig.HighlightColor * _highlightConfig.HighlightIntensity);
                }
            }
        }
        private void UnsetEmissionColorInRenderers()
        {
            foreach (Renderer r in _renderers)
            {
                // Sometimes we get a missing ref. exception when renderer was destroyed
                if (r == null)
                    continue;

                for (int i = 0; i < r.materials.Length; i++)
                {
                    r.materials[i].SetColor("_EmissionColor", Color.black);
                }
            }
        }
        #endregion Highlighting Methods
    }
}
