using UnityEngine;

namespace BoomerShooter.Weapons
{
    public class WeaponObject : MonoBehaviour
    {
        public WeaponData data;
        public Transform[] muzzlePoints;
        public Animator animator;

        private Vector3 _originalLocalPosition;
        private Quaternion _originalLocalRotation;

        private void Awake()
        {
            _originalLocalPosition = transform.localPosition;
            _originalLocalRotation = transform.localRotation;
        }

        private void OnDisable()
        {
            if (animator != null && !animator.gameObject.activeInHierarchy)
            {
                return;
            }

            ResetWeaponTransform();
        }

        public void ResetWeaponTransform()
        {
            if (animator != null)
            {
                animator.Rebind();
                animator.Update(0f);
            }
            transform.localPosition = _originalLocalPosition;
            transform.localRotation = _originalLocalRotation;
        }

        public void TriggerFire(int muzzleIndex)
        {
            if (animator == null) return;

            if (muzzlePoints.Length > 1)
            {
                if (muzzleIndex == 0)
                {
                    animator.SetTrigger("Fire_Right");
                }
                else
                {
                    animator.SetTrigger("Fire_Left");
                }
            }
            else
            {
                animator.SetTrigger("Fire");
            }
        }

        public void TriggerSpin()
        {
            if (animator != null) animator.SetTrigger("Spin");
        }

        public void SetMoving(bool isMoving, float speedMultiplier)
        {
            if (animator == null) return;
            animator.SetBool("IsMoving", isMoving);
            animator.SetFloat("MoveSpeed", speedMultiplier);
        }
    }
}