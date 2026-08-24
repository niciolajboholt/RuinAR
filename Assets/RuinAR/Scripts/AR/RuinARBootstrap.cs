using Unity.XR.CoreUtils;
using UnityEngine;
using UnityEngine.XR.ARFoundation;
using UnityEngine.XR.ARSubsystems;

namespace RuinAR.AR
{
    public static class RuinARBootstrap
    {
        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
        private static void CreateRuntime()
        {
            if (Object.FindFirstObjectByType<RuinARPrototypeController>() != null)
                return;

            var sessionObject = new GameObject("AR Session");
            sessionObject.AddComponent<ARSession>();
            sessionObject.AddComponent<ARInputManager>();

            var originObject = new GameObject("XR Origin");
            var xrOrigin = originObject.AddComponent<XROrigin>();

            var cameraOffset = new GameObject("Camera Offset");
            cameraOffset.transform.SetParent(originObject.transform, false);

            var cameraObject = new GameObject("AR Camera");
            cameraObject.tag = "MainCamera";
            cameraObject.transform.SetParent(cameraOffset.transform, false);
            var camera = cameraObject.AddComponent<Camera>();
            camera.clearFlags = CameraClearFlags.SolidColor;
            camera.backgroundColor = new Color(0.07f, 0.10f, 0.14f);
            camera.nearClipPlane = 0.05f;
#if UNITY_EDITOR
            cameraObject.transform.localPosition = new Vector3(0f, 2.4f, -6f);
            cameraObject.transform.localRotation = Quaternion.Euler(8f, 0f, 0f);
#endif
            cameraObject.AddComponent<AudioListener>();
            cameraObject.AddComponent<ARCameraManager>();
            cameraObject.AddComponent<ARCameraBackground>();
            cameraObject.AddComponent<AROcclusionManager>().requestedEnvironmentDepthMode = EnvironmentDepthMode.Fastest;

            xrOrigin.CameraFloorOffsetObject = cameraOffset;
            xrOrigin.Camera = camera;

            var planeManager = originObject.AddComponent<ARPlaneManager>();
            planeManager.requestedDetectionMode = PlaneDetectionMode.Horizontal | PlaneDetectionMode.Vertical;
            var raycastManager = originObject.AddComponent<ARRaycastManager>();

            var lightObject = new GameObject("Preview Sun");
            var previewLight = lightObject.AddComponent<Light>();
            previewLight.type = LightType.Directional;
            previewLight.intensity = 1.2f;
            lightObject.transform.rotation = Quaternion.Euler(48f, -35f, 0f);

            var controller = new GameObject("RuinAR Prototype Controller").AddComponent<RuinARPrototypeController>();
            controller.Configure(raycastManager, planeManager, camera);
        }
    }
}

