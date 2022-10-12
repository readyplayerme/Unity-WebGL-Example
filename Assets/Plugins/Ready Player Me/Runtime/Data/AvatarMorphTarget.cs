using System.Collections.Generic;
using System.Linq;

namespace ReadyPlayerMe
{
    public static class AvatarMorphTarget
    {
        private static readonly List<string> MorphTargetGroupNames = new List<string>
        {
            "none",
            "Oculus Visemes",
            "ARKit"
        };

        private static readonly List<string> MorphTargetNames = new List<string>
        {
            "viseme_aa",
            "viseme_E",
            "viseme_I",
            "viseme_O",
            "viseme_U",
            "viseme_CH",
            "viseme_DD",
            "viseme_FF",
            "viseme_kk",
            "viseme_nn",
            "viseme_PP",
            "viseme_RR",
            "viseme_sil",
            "viseme_SS",
            "viseme_TH",
            "browDownLeft",
            "browDownRight",
            "browInnerUp",
            "browOuterUpLeft",
            "browOuterUpRight",
            "eyesClosed",
            "eyeBlinkLeft",
            "eyeBlinkRight",
            "eyeSquintLeft",
            "eyeSquintRight",
            "eyeWideLeft",
            "eyeWideRight",
            "eyesLookUp",
            "eyesLookDown",
            "eyeLookDownLeft",
            "eyeLookDownRight",
            "eyeLookUpLeft",
            "eyeLookUpRight",
            "eyeLookInLeft",
            "eyeLookInRight",
            "eyeLookOutLeft",
            "eyeLookOutRight",
            "jawOpen",
            "jawForward",
            "jawLeft",
            "jawRight",
            "noseSneerLeft",
            "noseSneerRight",
            "cheekPuff",
            "cheekSquintLeft",
            "cheekSquintRight",
            "mouthSmileLeft",
            "mouthSmileRight",
            "mouthOpen",
            "mouthSmile",
            "mouthLeft",
            "mouthRight",
            "mouthClose",
            "mouthFunnel",
            "mouthDimpleLeft",
            "mouthDimpleRight",
            "mouthStretchLeft",
            "mouthStretchRight",
            "mouthRollLower",
            "mouthRollUpper",
            "mouthPressLeft",
            "mouthPressRight",
            "mouthUpperUpLeft",
            "mouthUpperUpRight",
            "mouthFrownLeft",
            "mouthFrownRight",
            "mouthPucker",
            "mouthShrugLower",
            "mouthShrugUpper",
            "mouthLowerDownLeft",
            "mouthLowerDownRight",
            "tongueOut"
        };

        public static readonly List<string> MorphTargetAvatarAPI = MorphTargetGroupNames.Concat(MorphTargetNames).ToList();
    }
}
