// ReSharper disable RedundantUsingDirective

using System;
using UnityEngine;
using System.Collections;
using static ReadyPlayerMe.ExtensionMethods;

#if UNITY_ANDROID
using UnityEngine.Android;
#endif

namespace ReadyPlayerMe
{
    public enum AudioProviderType
    {
        Microphone = 0,
        AudioClip = 1
    }

    [DisallowMultipleComponent]
    [AddComponentMenu("Ready Player Me/Voice Handler", 0)]
    public class VoiceHandler : MonoBehaviour
    {
        private const string MOUTH_OPEN_BLEND_SHAPE_NAME = "mouthOpen";
        private const int AMPLITUDE_MULTIPLIER = 10;
        private const int AUDIO_SAMPLE_LENGTH = 4096;

        private float[] audioSample = new float[AUDIO_SAMPLE_LENGTH];

        private SkinnedMeshRenderer headMesh;
        private SkinnedMeshRenderer beardMesh;
        private SkinnedMeshRenderer teethMesh;

        private const float MOUTH_OPEN_MULTIPLIER = 100f;

        private int mouthOpenBlendShapeIndexOnHeadMesh = -1;
        private int mouthOpenBlendShapeIndexOnBeardMesh = -1;
        private int mouthOpenBlendShapeIndexOnTeethMesh = -1;

        // ReSharper disable InconsistentNaming
        public AudioClip AudioClip;
        public AudioSource AudioSource;
        public AudioProviderType AudioProvider = AudioProviderType.Microphone;

        private void Start()
        {
            headMesh = GetMeshAndSetIndex(MeshType.HeadMesh, ref mouthOpenBlendShapeIndexOnHeadMesh);
            beardMesh = GetMeshAndSetIndex(MeshType.BeardMesh, ref mouthOpenBlendShapeIndexOnBeardMesh);
            teethMesh = GetMeshAndSetIndex(MeshType.TeethMesh, ref mouthOpenBlendShapeIndexOnTeethMesh);

#if UNITY_IOS
            CheckIOSMicrophonePermission().Run();
#elif UNITY_ANDROID
            CheckAndroidMicrophonePermission().Run();
#elif UNITY_STANDALONE || UNITY_EDITOR
            InitializeAudio();
#endif
        }

        private void Update()
        {
            var value = GetAmplitude();
            SetBlendShapeWeights(value);
        }

        public void InitializeAudio()
        {
            try
            {
                if (AudioSource == null)
                {
                    AudioSource = gameObject.AddComponent<AudioSource>();
                }

                switch (AudioProvider)
                {
                    case AudioProviderType.Microphone:
                        SetMicrophoneSource();
                        break;
                    case AudioProviderType.AudioClip:
                        SetAudioClipSource();
                        break;
                }
            }
            catch (Exception e)
            {
                Debug.LogError($"VoiceHandler.Initialize:/n" + e);
            }
        }

        private void SetMicrophoneSource()
        {
#if UNITY_WEBGL
            Debug.Log("Microphone is not supported in WebGL.");
#else
            AudioSource.clip = Microphone.Start(null, true, 1, 44100);
            AudioSource.loop = true;
            AudioSource.mute = true;
            AudioSource.Play();
#endif
        }

        private void SetAudioClipSource()
        {
            AudioSource.clip = AudioClip;
            AudioSource.loop = false;
            AudioSource.mute = false;
            AudioSource.Stop();
        }

        public void PlayCurrentAudioClip()
        {
            AudioSource.Play();
        }

        public void PlayAudioClip(AudioClip audioClip)
        {
            AudioClip = AudioSource.clip = audioClip;
            PlayCurrentAudioClip();
        }

        private float GetAmplitude()
        {
            if (AudioSource != null && AudioSource.clip != null && AudioSource.isPlaying)
            {
                var amplitude = 0f;
                AudioSource.clip.GetData(audioSample, AudioSource.timeSamples);

                foreach (var sample in audioSample)
                {
                    amplitude += Mathf.Abs(sample);
                }

                return Mathf.Clamp01(amplitude / audioSample.Length * AMPLITUDE_MULTIPLIER);
            }

            return 0;
        }

        #region Blend Shape Movement

        private SkinnedMeshRenderer GetMeshAndSetIndex(MeshType meshType, ref int index)
        {
            var mesh = gameObject.GetMeshRenderer(meshType);
            if (mesh != null)
            {
                index = mesh.sharedMesh.GetBlendShapeIndex(MOUTH_OPEN_BLEND_SHAPE_NAME);
            }

            return mesh;
        }

        private void SetBlendShapeWeights(float weight)
        {
            SetBlendShapeWeight(headMesh, mouthOpenBlendShapeIndexOnHeadMesh);
            SetBlendShapeWeight(beardMesh, mouthOpenBlendShapeIndexOnBeardMesh);
            SetBlendShapeWeight(teethMesh, mouthOpenBlendShapeIndexOnTeethMesh);

            void SetBlendShapeWeight(SkinnedMeshRenderer mesh, int index)
            {
                if (index >= 0)
                {
                    mesh.SetBlendShapeWeight(index, weight * MOUTH_OPEN_MULTIPLIER);
                }
            }
        }

        #endregion

        #region Permissions

#if UNITY_IOS
        private IEnumerator CheckIOSMicrophonePermission()
        {
            yield return Application.RequestUserAuthorization(UserAuthorization.Microphone);
            if (Application.HasUserAuthorization(UserAuthorization.Microphone))
            {
                InitializeAudio();
            }
            else
            {
                StartCoroutine(CheckIOSMicrophonePermission());
            }
        }
#endif

#if UNITY_ANDROID
        private IEnumerator CheckAndroidMicrophonePermission()
        {
            var wait = new WaitUntil(() =>
            {
                Permission.RequestUserPermission(Permission.Microphone);

                return Permission.HasUserAuthorizedPermission(Permission.Microphone);
            });

            yield return wait;

            InitializeAudio();
        }
#endif

        #endregion

        private void OnDestroy()
        {
            audioSample = null;
        }
    }
}
