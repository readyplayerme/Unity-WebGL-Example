using UnityEngine;

namespace ReadyPlayerMe
{
    public static class AvatarAnimatorHelper
    {
        private const string ANIMATOR_CONTROLLER_NAME = "Avatar Animator";
        private static RuntimeAnimatorController animatorController;

        public static void SetupAnimator(BodyType bodyType, GameObject avatar)
        {
            if (bodyType != BodyType.FullBody)
            {
                return;
            }

            if (animatorController == null)
            {
                animatorController = Resources.Load<RuntimeAnimatorController>(ANIMATOR_CONTROLLER_NAME);
            }

            var animator = avatar.GetComponent<Animator>();
            animator.runtimeAnimatorController = animatorController;
        }
    }
}
