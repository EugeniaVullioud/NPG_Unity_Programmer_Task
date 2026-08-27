using System.Collections.Generic;
using UnityEngine;
namespace Game.Items
{
    /// <summary>
    /// Provides highlighted visual feedback for a world pickup.
    /// </summary>
    public sealed class WorldPickupFeedback : MonoBehaviour, IWorldPickupFeedback
    {
        [SerializeField] Renderer[] renderers;
        [SerializeField] Color highlightedColor = Color.yellow;
        [SerializeField] GameObject outline;

        readonly Dictionary<Renderer, Color> _originalColors = new();

        bool _highlighted;

        static readonly int BaseColorProperty = Shader.PropertyToID("_BaseColor");

        static readonly int ColorProperty = Shader.PropertyToID("_Color");

        MaterialPropertyBlock _propertyBlock;

        /// <summary>
        /// Initializes the material property block and caches the original
        /// renderer colors used when restoring feedback.
        /// </summary>
        void Awake()
        {
            _propertyBlock = new MaterialPropertyBlock();

            CacheOriginalColors();
            ResetFeedback();
        }

        /// <summary>
        /// Restores the pickup's default visual state when disabled.
        /// </summary>
        void OnDisable()
        {
            ResetFeedback();
        }

        /// <summary>
        /// Enables or disables the pickup's highlighted visual state.
        /// </summary>
        public void SetHighlighted(bool highlighted)
        {
            if (_highlighted == highlighted) return;
            _highlighted = highlighted;

            if (outline != null) outline.SetActive(highlighted);

            if (highlighted) ApplyHighlight();
            else RestoreOriginal();
        }

        /// <summary>
        /// Resets all visual feedback to its default state.
        /// </summary>
        public void ResetFeedback()
        {
            _highlighted = false;
            if (outline != null) outline.SetActive(false);

            RestoreOriginal();
        }

        /// <summary>
        /// Caches the original color of each configured renderer so that
        /// highlighted colors can later be restored.
        /// </summary>
        void CacheOriginalColors()
        {
            _originalColors.Clear();

            foreach (Renderer renderer in renderers)
            {
                if (renderer == null) continue;

                Material material = renderer.sharedMaterial;

                if (material == null) continue;

                if (material.HasProperty(BaseColorProperty))
                {
                    _originalColors[renderer] = material.GetColor(BaseColorProperty);
                }
                else if (material.HasProperty(ColorProperty))
                {
                    _originalColors[renderer] = material.GetColor(ColorProperty);
                }
            }
        }

        /// <summary>
        /// Applies the configured highlight color to all configured renderers
        /// using a <see cref="MaterialPropertyBlock"/>.
        /// </summary>
        void ApplyHighlight()
        {
            foreach (Renderer renderer in renderers)
            {
                if (renderer == null) continue;

                Material material = renderer.sharedMaterial;

                if (material == null) continue;

                renderer.GetPropertyBlock(_propertyBlock);

                if (material.HasProperty(BaseColorProperty))
                {
                    _propertyBlock.SetColor(BaseColorProperty, highlightedColor);
                }
                else if (material.HasProperty(ColorProperty))
                {
                    _propertyBlock.SetColor(ColorProperty, highlightedColor);
                }
                renderer.SetPropertyBlock(_propertyBlock);
            }
        }

        /// <summary>
        /// Restores the original colors cached from the configured renderers.
        /// </summary>
        void RestoreOriginal()
        {
            foreach (Renderer renderer in renderers)
            {
                if (renderer == null) continue;

                renderer.GetPropertyBlock(_propertyBlock);

                if (_originalColors.TryGetValue(renderer, out Color originalColor))
                {
                    Material material = renderer.sharedMaterial;

                    if (material != null && material.HasProperty(BaseColorProperty))
                    {
                        _propertyBlock.SetColor(BaseColorProperty, originalColor);
                    }
                    else if (material != null && material.HasProperty(ColorProperty))
                    {
                        _propertyBlock.SetColor(ColorProperty, originalColor);
                    }

                    renderer.SetPropertyBlock(_propertyBlock);
                }
                else
                {
                    renderer.SetPropertyBlock(null);
                }
            }
        }
    }
}