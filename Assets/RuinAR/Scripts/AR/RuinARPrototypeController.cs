using System.Collections;
using System.Collections.Generic;
using System.IO;
using RuinAR.Core;
using UnityEngine;
using UnityEngine.XR.ARFoundation;
using UnityEngine.XR.ARSubsystems;

namespace RuinAR.AR
{
    public sealed class RuinARPrototypeController : MonoBehaviour
    {
        private enum ExperienceStage
        {
            ChooseImage,
            Analyzing,
            Ready,
            Placed
        }

        private readonly List<ARRaycastHit> raycastHits = new();
        private ARRaycastManager raycastManager;
        private ARPlaneManager planeManager;
        private Camera arCamera;
        private LocationServiceController locationController;
        private RuinSiteData site;
        private GameObject reconstruction;
        private Texture2D selectedImage;
        private string selectedImageName;
        private ExperienceStage stage = ExperienceStage.ChooseImage;
        private float analysisProgress;
        private EvidenceStatus? activeFilter;
        private string message = "Vælg et billede af den ruin, du vil undersøge.";

#if UNITY_EDITOR
        private float desktopRotation;
#endif

        public void Configure(ARRaycastManager raycasts, ARPlaneManager planes, Camera camera)
        {
            raycastManager = raycasts;
            planeManager = planes;
            arCamera = camera;
        }

        private void Start()
        {
            site = OfflineRuinPackageStore.LoadOrCreate();
            locationController = gameObject.AddComponent<LocationServiceController>();
        }

        private void Update()
        {
#if UNITY_EDITOR
            UpdateDesktopPreview();
#endif
            if (stage != ExperienceStage.Ready || reconstruction != null || raycastManager == null)
                return;

            if (!TryGetPointerRelease(out var screenPosition))
                return;

            if (!raycastManager.Raycast(screenPosition, raycastHits,
                    TrackableType.PlaneWithinPolygon | TrackableType.FeaturePoint))
                return;

            PlaceAt(raycastHits[0].pose);
        }

        private static bool TryGetPointerRelease(out Vector2 screenPosition)
        {
            if (Input.touchCount > 0)
            {
                var touch = Input.GetTouch(0);
                screenPosition = touch.position;
                return touch.phase == TouchPhase.Ended;
            }

            screenPosition = Input.mousePosition;
            return Input.GetMouseButtonDown(0);
        }

        private void PlaceAt(Pose pose)
        {
            reconstruction = ProceduralReconstructionFactory.CreatePrototype(pose);
            stage = ExperienceStage.Placed;
            SetPlaneVisibility(false);
            ApplyFilter();
            message = "Demomodellen er placeret. Farverne viser dokumentationsniveau.";
        }

        private void PlaceInFrontOfCamera()
        {
            if (arCamera == null)
                return;

            var forward = Vector3.ProjectOnPlane(arCamera.transform.forward, Vector3.up).normalized;
            if (forward.sqrMagnitude < 0.1f)
                forward = Vector3.forward;

            var position = arCamera.transform.position + forward * 11.5f;
            position.y = 0f;
            PlaceAt(new Pose(position, Quaternion.Euler(0f, -18f, 0f)));
#if UNITY_EDITOR
            desktopRotation = -18f;
            message = "Desktopdemo · Piletaster: drej ruinen.";
#endif
        }

        private void ChooseImage()
        {
#if UNITY_EDITOR
            var path = UnityEditor.EditorUtility.OpenFilePanelWithFilters(
                "Vælg et billede af en ruin",
                string.Empty,
                new[] { "Billeder", "png,jpg,jpeg" });

            if (string.IsNullOrEmpty(path))
                return;

            var imageBytes = File.ReadAllBytes(path);
            if (selectedImage != null)
                Destroy(selectedImage);

            selectedImage = new Texture2D(2, 2);
            selectedImage.LoadImage(imageBytes);
            selectedImageName = Path.GetFileName(path);
            StartCoroutine(AnalyzeImage());
#else
            selectedImageName = "Kamerabillede";
            StartCoroutine(AnalyzeImage());
#endif
        }

        private IEnumerator AnalyzeImage()
        {
            stage = ExperienceStage.Analyzing;
            analysisProgress = 0f;
            message = $"Demoanalyse af {selectedImageName}";

            const float duration = 3f;
            while (analysisProgress < 1f)
            {
                analysisProgress = Mathf.Min(1f, analysisProgress + Time.unscaledDeltaTime / duration);
                yield return null;
            }

            stage = ExperienceStage.Ready;
            message = "Demoanalyse klar · Rekonstruktionen er ikke fagligt verificeret.";
        }

#if UNITY_EDITOR
        private void UpdateDesktopPreview()
        {
            if (reconstruction == null)
                return;

            var turn = Input.GetAxisRaw("Horizontal");
            if (Mathf.Abs(turn) < 0.01f)
                return;

            desktopRotation += turn * 55f * Time.deltaTime;
            reconstruction.transform.rotation = Quaternion.Euler(0f, desktopRotation, 0f);
        }
#endif

        private void ResetPlacement()
        {
            if (reconstruction != null)
                Destroy(reconstruction);

            reconstruction = null;
            stage = ExperienceStage.Ready;
            activeFilter = null;
            SetPlaneVisibility(true);
            message = "Rekonstruktionen er klar til ny placering.";
        }

        private void OnDestroy()
        {
            if (selectedImage != null)
                Destroy(selectedImage);
        }

        private void SetPlaneVisibility(bool visible)
        {
            if (planeManager == null)
                return;

            planeManager.enabled = visible;
            foreach (var plane in planeManager.trackables)
                plane.gameObject.SetActive(visible);
        }

        private void SetFilter(EvidenceStatus? filter)
        {
            activeFilter = filter;
            ApplyFilter();
        }

        private void ApplyFilter()
        {
            if (reconstruction == null)
                return;

            foreach (var marker in reconstruction.GetComponentsInChildren<RuinElementMarker>(true))
                marker.gameObject.SetActive(activeFilter == null || marker.EvidenceStatus == activeFilter);
        }

        private void OnGUI()
        {
            var scale = Mathf.Max(1f, Screen.dpi / 180f);
            var panelWidth = Mathf.Min(Screen.width - 24f * scale, 460f * scale);
            var buttonHeight = 44f * scale;
            var margin = 12f * scale;
            var style = new GUIStyle(GUI.skin.box)
            {
                alignment = TextAnchor.UpperLeft,
                fontSize = Mathf.RoundToInt(16f * scale),
                padding = new RectOffset(14, 14, 12, 12)
            };

            GUI.Box(new Rect(margin, margin, panelWidth, 104f * scale),
                $"RuinAR · {site?.displayName}\n{site?.verificationLabel}\n{message}\n{locationController?.StatusMessage}", style);

            var y = 126f * scale;
            if (stage == ExperienceStage.ChooseImage)
            {
                if (GUI.Button(new Rect(margin, y, panelWidth, buttonHeight),
#if UNITY_EDITOR
                        "Vælg ruinbillede"
#else
                        "Analysér kamerabillede"
#endif
                    ))
                    ChooseImage();
                return;
            }

            if (stage == ExperienceStage.Analyzing)
            {
                GUI.Box(new Rect(margin, y, panelWidth, buttonHeight),
                    $"Demoanalyse · {analysisProgress * 100f:0}%");
                return;
            }

            if (stage == ExperienceStage.Ready)
            {
                if (selectedImage != null)
                {
                    var previewWidth = 112f * scale;
                    var previewHeight = 72f * scale;
                    GUI.DrawTexture(new Rect(margin, y, previewWidth, previewHeight), selectedImage,
                        ScaleMode.ScaleToFit, false);
                    if (GUI.Button(new Rect(margin + previewWidth + margin, y,
                            panelWidth - previewWidth - margin, previewHeight), "Vis rekonstruktion"))
                        PlaceInFrontOfCamera();
                }
                else if (GUI.Button(new Rect(margin, y, panelWidth, buttonHeight), "Vis rekonstruktion"))
                {
                    PlaceInFrontOfCamera();
                }

                return;
            }

            var quarter = (panelWidth - 3f * margin) / 4f;
            if (GUI.Button(new Rect(margin, y, quarter, buttonHeight), "Alle"))
                SetFilter(null);
            if (GUI.Button(new Rect(margin + quarter + margin, y, quarter, buttonHeight), "Dokumenteret"))
                SetFilter(EvidenceStatus.Documented);
            if (GUI.Button(new Rect(margin + 2f * (quarter + margin), y, quarter, buttonHeight), "Sandsynlig"))
                SetFilter(EvidenceStatus.Probable);
            if (GUI.Button(new Rect(margin + 3f * (quarter + margin), y, quarter, buttonHeight), "AI"))
                SetFilter(EvidenceStatus.AiGenerated);

            y += buttonHeight + margin;
            if (GUI.Button(new Rect(margin, y, panelWidth, buttonHeight), "Nulstil placering"))
                ResetPlacement();
        }
    }
}

