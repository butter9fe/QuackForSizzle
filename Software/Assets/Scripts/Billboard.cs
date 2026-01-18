using System;
using System.Collections;
using System.Collections.Generic;
using QuackForSizzle.Player;
using UnityEngine;

namespace CookOrBeCooked.Utility
{
    public class Billboard : MonoBehaviour
    {
        public enum BillboardAxis
        {
            // Rotate about an individual axis.
            X,
            Y,
            Z,
            XY,
            XZ,
            YZ,
            // Rotate about all axes.
            XYZ,
        }

        [SerializeField]
        [Tooltip("Only Y and XYZ work")]
        private BillboardAxis axis = BillboardAxis.XYZ;

        [SerializeField]
        private bool updateOnce = true;
        //private bool updatedBefore = false;

        private Transform mainCameraTrans;  // reference to Camera.main.transform

        private void OnEnable()
        {
            // Get mainCamera reference from the Scene, if any
            // (This is for objects that are spawned after the level is loaded)
            if (mainCameraTrans == null)
                GetMainCamera();
        }

        private void Start()
        {
            // Update the viewing rotation once.
            if (updateOnce)
            {
                UpdateRotation();
                //updatedBefore = true;
                // Has been updated before, no longer need to update again. Can disable itself.
                this.enabled = false;
            }
        }

        private void LateUpdate()
        {
            // If update should only run once, and it has not run yet, run the update.
            if (updateOnce) //&& !updatedBefore)
            {
                UpdateRotation();
                //updatedBefore = true;
                // Has been updated before, no longer need to update again. Can disable itself.
                this.enabled = false;
                return;
            }
            else
            {
                // Only update the viewing rotation if the transform has been modified.
                //if (transform.hasChanged) // Unnecessary check
                {
                    // Update rotation
                    UpdateRotation();
                    // Reset flag
                    transform.hasChanged = false;
                }
            }
        }

        /// <summary>
        /// Get Camera.main and assign to mainCameraTrans.
        /// Note that objects only instantiated at runtime need to fetch Camera.main on their first use.
        /// </summary>
        private void GetMainCamera()
        {
            Camera mainCamera = Camera.main;
            if (mainCamera != null)
                mainCameraTrans = mainCamera.transform;
        }

        private void UpdateRotation()
        {
            // Get a Vector that points from the Camera to the target.
            Vector3 forward;
            Vector3 up;

            switch (axis)
            {
                case BillboardAxis.Y:
                    up = transform.up; // Fixed up
                    forward = Vector3.ProjectOnPlane(transform.forward, up);
                    break;

                case BillboardAxis.XYZ:
                default:
                    if (mainCameraTrans == null)    // fallback code if no camera
                    {
                        forward = transform.forward;
                        up = transform.up;
                    }
                    else
                    {
                        forward = mainCameraTrans.forward;
                        up = mainCameraTrans.up;
                    }
                    break;
            }

            // Calculate and apply the rotation required to reorient the object
            Quaternion newRot = Quaternion.LookRotation(forward, up);
            if (transform.rotation != newRot)
                transform.rotation = newRot;    // Only modify the transform if there has been a change
        }
    }
}