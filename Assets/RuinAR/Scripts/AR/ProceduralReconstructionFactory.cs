using RuinAR.Core;
using UnityEngine;

namespace RuinAR.AR
{
    public enum RuinPrototypeType
    {
        Longhouse,
        Chapel,
        Tower
    }

    public readonly struct RuinVisualProfile
    {
        public RuinPrototypeType Type { get; }
        public float Width { get; }
        public float Depth { get; }
        public float DamageVariation { get; }

        public string DisplayName => Type switch
        {
            RuinPrototypeType.Chapel => "Kapelprofil",
            RuinPrototypeType.Tower => "Tårnprofil",
            _ => "Langhusprofil"
        };

        public RuinVisualProfile(RuinPrototypeType type, float width, float depth, float damageVariation)
        {
            Type = type;
            Width = width;
            Depth = depth;
            DamageVariation = damageVariation;
        }

        public static RuinVisualProfile Default => new(RuinPrototypeType.Longhouse, 6.2f, 5.2f, 1f);
    }

    public static class ProceduralReconstructionFactory
    {
        private static readonly Color DocumentedStone = new(0.30f, 0.58f, 0.38f);
        private static readonly Color ProbableStone = new(0.86f, 0.62f, 0.20f);
        private static readonly Color AiInterpretation = new(0.55f, 0.28f, 0.68f);

        public static RuinVisualProfile AnalyzePrototypeImage(Texture2D image)
        {
            if (image == null)
                return RuinVisualProfile.Default;

            var pixels = image.GetPixels32();
            var stride = Mathf.Max(1, pixels.Length / 1200);
            long brightnessTotal = 0;
            long contrastTotal = 0;
            var sampleCount = 0;
            var signature = 17;
            Color32 previous = pixels.Length > 0 ? pixels[0] : new Color32();

            unchecked
            {
                for (var index = 0; index < pixels.Length; index += stride)
                {
                    var pixel = pixels[index];
                    brightnessTotal += pixel.r * 3 + pixel.g * 6 + pixel.b;
                    contrastTotal += Mathf.Abs(pixel.r - previous.r)
                                     + Mathf.Abs(pixel.g - previous.g)
                                     + Mathf.Abs(pixel.b - previous.b);
                    signature = signature * 31 + pixel.r * 3 + pixel.g * 5 + pixel.b * 7;
                    previous = pixel;
                    sampleCount++;
                }
            }

            var aspect = image.height > 0 ? (float)image.width / image.height : 1f;
            var brightness = sampleCount > 0 ? brightnessTotal / (sampleCount * 10f * 255f) : 0.5f;
            var contrast = sampleCount > 0 ? contrastTotal / (sampleCount * 3f * 255f) : 0f;

            RuinPrototypeType type;
            if (aspect > 1.38f)
                type = RuinPrototypeType.Longhouse;
            else if (contrast > 0.24f || brightness < 0.32f)
                type = RuinPrototypeType.Tower;
            else
                type = RuinPrototypeType.Chapel;

            var variation = 0.82f + Mathf.Abs(signature % 37) / 100f;
            var width = type switch
            {
                RuinPrototypeType.Tower => Mathf.Lerp(4.2f, 5f, brightness),
                RuinPrototypeType.Chapel => Mathf.Lerp(5.2f, 6.2f, brightness),
                _ => Mathf.Clamp(5.8f + (aspect - 1f) * 1.2f, 6f, 7.6f)
            };
            var depth = type switch
            {
                RuinPrototypeType.Tower => width,
                RuinPrototypeType.Chapel => width * 1.28f,
                _ => Mathf.Lerp(4.7f, 5.7f, 1f - contrast)
            };

            return new RuinVisualProfile(type, width, depth, variation);
        }

        public static GameObject CreatePrototype(Pose pose, RuinVisualProfile profile)
        {
            var root = new GameObject($"Prototype Reconstruction · {profile.DisplayName}");
            root.transform.SetPositionAndRotation(pose.position, pose.rotation);

            switch (profile.Type)
            {
                case RuinPrototypeType.Chapel:
                    CreateChapel(root.transform, profile);
                    break;
                case RuinPrototypeType.Tower:
                    CreateTower(root.transform, profile);
                    break;
                default:
                    CreateLonghouse(root.transform, profile);
                    break;
            }

            return root;
        }

        private static void CreateLonghouse(Transform root, RuinVisualProfile profile)
        {
            var halfWidth = profile.Width * 0.5f;
            var halfDepth = profile.Depth * 0.5f;
            var lowWall = 1.05f * profile.DamageVariation;

            CreateFoundation(root, profile.Width, profile.Depth);
            CreatePart(root, "Dokumenteret bagmur", new Vector3(0f, lowWall * 0.5f, halfDepth - 0.18f),
                new Vector3(profile.Width, lowWall, 0.36f), DocumentedStone, EvidenceStatus.Documented);
            CreatePart(root, "Dokumenteret vestmur", new Vector3(-halfWidth + 0.18f, lowWall * 0.58f, 0f),
                new Vector3(0.36f, lowWall * 1.16f, profile.Depth), DocumentedStone, EvidenceStatus.Documented);
            CreateDoorFacade(root, halfWidth, -halfDepth + 0.18f, lowWall);

            CreatePart(root, "Sandsynlig bagmur", new Vector3(0f, 2.05f, halfDepth - 0.18f),
                new Vector3(profile.Width, 1.9f, 0.30f), ProbableStone, EvidenceStatus.Probable);
            CreatePart(root, "Sandsynlig vestmur", new Vector3(-halfWidth + 0.18f, 2.1f, 0f),
                new Vector3(0.30f, 1.7f, profile.Depth), ProbableStone, EvidenceStatus.Probable);
            CreatePart(root, "Sandsynlig østmur", new Vector3(halfWidth - 0.18f, 1.9f, 0.75f),
                new Vector3(0.30f, 2.5f, profile.Depth * 0.58f), ProbableStone, EvidenceStatus.Probable);

            CreateColumn(root, "Sandsynlig søjle venstre", new Vector3(-profile.Width * 0.2f, 1.4f, -0.3f));
            CreateColumn(root, "Sandsynlig søjle højre", new Vector3(profile.Width * 0.2f, 1.4f, -0.3f));
            CreateGabledRoof(root, profile.Width, profile.Depth, 3.55f);
        }

        private static void CreateChapel(Transform root, RuinVisualProfile profile)
        {
            var halfWidth = profile.Width * 0.5f;
            var halfDepth = profile.Depth * 0.5f;
            var lowWall = 0.95f * profile.DamageVariation;

            CreateFoundation(root, profile.Width, profile.Depth);
            CreatePart(root, "Dokumenteret vestmur", new Vector3(-halfWidth + 0.18f, lowWall * 0.5f, 0f),
                new Vector3(0.36f, lowWall, profile.Depth), DocumentedStone, EvidenceStatus.Documented);
            CreatePart(root, "Dokumenteret østmur", new Vector3(halfWidth - 0.18f, lowWall * 0.65f, 0f),
                new Vector3(0.36f, lowWall * 1.3f, profile.Depth), DocumentedStone, EvidenceStatus.Documented);
            CreateDoorFacade(root, halfWidth, -halfDepth + 0.18f, lowWall);

            CreatePart(root, "Sandsynlig skib vest", new Vector3(-halfWidth + 0.18f, 2.15f, 0f),
                new Vector3(0.30f, 2.4f, profile.Depth), ProbableStone, EvidenceStatus.Probable);
            CreatePart(root, "Sandsynlig skib øst", new Vector3(halfWidth - 0.18f, 2.15f, 0f),
                new Vector3(0.30f, 2.4f, profile.Depth), ProbableStone, EvidenceStatus.Probable);

            var apse = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
            apse.name = "Sandsynlig rund apsis";
            apse.transform.SetParent(root, false);
            apse.transform.localPosition = new Vector3(0f, 1.35f, halfDepth + 0.45f);
            apse.transform.localScale = new Vector3(halfWidth * 0.8f, 1.35f, 1.15f);
            apse.AddComponent<RuinElementMarker>().Configure(EvidenceStatus.Probable);
            apse.GetComponent<Renderer>().material.color = ProbableStone;

            CreateGabledRoof(root, profile.Width, profile.Depth * 1.05f, 3.75f);
        }

        private static void CreateTower(Transform root, RuinVisualProfile profile)
        {
            var half = profile.Width * 0.5f;
            var lowWall = 1.25f * profile.DamageVariation;

            CreateFoundation(root, profile.Width, profile.Depth);
            CreatePart(root, "Dokumenteret bagmur", new Vector3(0f, lowWall * 0.5f, half - 0.2f),
                new Vector3(profile.Width, lowWall, 0.4f), DocumentedStone, EvidenceStatus.Documented);
            CreatePart(root, "Dokumenteret vestmur", new Vector3(-half + 0.2f, lowWall * 0.5f, 0f),
                new Vector3(0.4f, lowWall, profile.Depth), DocumentedStone, EvidenceStatus.Documented);
            CreateDoorFacade(root, half, -half + 0.2f, lowWall);

            CreatePart(root, "Sandsynlig tårnkerne", new Vector3(0f, 3.1f, half - 0.18f),
                new Vector3(profile.Width, 4.9f, 0.36f), ProbableStone, EvidenceStatus.Probable);
            CreatePart(root, "Sandsynlig vestlig tårnmur", new Vector3(-half + 0.18f, 3.1f, 0f),
                new Vector3(0.36f, 4.9f, profile.Depth), ProbableStone, EvidenceStatus.Probable);
            CreatePart(root, "Sandsynlig østlig tårnmur", new Vector3(half - 0.18f, 3.1f, 0f),
                new Vector3(0.36f, 4.9f, profile.Depth), ProbableStone, EvidenceStatus.Probable);

            for (var side = -1; side <= 1; side += 2)
            {
                for (var index = -1; index <= 1; index++)
                {
                    CreatePart(root, "AI-fortolket brystværn", new Vector3(index * profile.Width * 0.32f, 5.85f, side * (half - 0.2f)),
                        new Vector3(profile.Width * 0.22f, 0.65f, 0.42f), AiInterpretation, EvidenceStatus.AiGenerated);
                    CreatePart(root, "AI-fortolket brystværn", new Vector3(side * (half - 0.2f), 5.85f, index * profile.Depth * 0.32f),
                        new Vector3(0.42f, 0.65f, profile.Depth * 0.22f), AiInterpretation, EvidenceStatus.AiGenerated);
                }
            }
        }

        private static void CreateFoundation(Transform root, float width, float depth)
        {
            CreatePart(root, "Dokumenteret fundament", new Vector3(0f, 0.08f, 0f),
                new Vector3(width + 0.2f, 0.16f, depth + 0.2f), DocumentedStone, EvidenceStatus.Documented);
        }

        private static void CreateDoorFacade(Transform root, float halfWidth, float z, float wallHeight)
        {
            var segmentWidth = halfWidth - 0.8f;
            CreatePart(root, "Dokumenteret facade venstre", new Vector3(-(halfWidth + 0.8f) * 0.5f, wallHeight * 0.5f, z),
                new Vector3(segmentWidth, wallHeight, 0.36f), DocumentedStone, EvidenceStatus.Documented);
            CreatePart(root, "Dokumenteret facade højre", new Vector3((halfWidth + 0.8f) * 0.5f, wallHeight * 0.5f, z),
                new Vector3(segmentWidth, wallHeight, 0.36f), DocumentedStone, EvidenceStatus.Documented);
            CreatePart(root, "Sandsynlig overligger", new Vector3(0f, 2.75f, z),
                new Vector3(1.8f, 0.32f, 0.36f), ProbableStone, EvidenceStatus.Probable);
        }

        private static void CreateGabledRoof(Transform root, float width, float depth, float eaveHeight)
        {
            var panelWidth = width * 0.58f;
            var offset = width * 0.235f;
            var left = CreatePart(root, "AI-genereret vestligt tagfald", new Vector3(-offset, eaveHeight, 0f),
                new Vector3(panelWidth, 0.16f, depth + 0.2f), AiInterpretation, EvidenceStatus.AiGenerated);
            left.transform.localRotation = Quaternion.Euler(0f, 0f, 25f);

            var right = CreatePart(root, "AI-genereret østligt tagfald", new Vector3(offset, eaveHeight, 0f),
                new Vector3(panelWidth, 0.16f, depth + 0.2f), AiInterpretation, EvidenceStatus.AiGenerated);
            right.transform.localRotation = Quaternion.Euler(0f, 0f, -25f);
        }

        private static void CreateColumn(Transform root, string name, Vector3 position)
        {
            var column = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
            column.name = name;
            column.transform.SetParent(root, false);
            column.transform.localPosition = position;
            column.transform.localScale = new Vector3(0.34f, 1.4f, 0.34f);
            column.AddComponent<RuinElementMarker>().Configure(EvidenceStatus.Probable);
            column.GetComponent<Renderer>().material.color = ProbableStone;
        }

        private static GameObject CreatePart(Transform parent, string name, Vector3 position, Vector3 scale,
            Color color, EvidenceStatus evidenceStatus)
        {
            var part = GameObject.CreatePrimitive(PrimitiveType.Cube);
            part.name = name;
            part.transform.SetParent(parent, false);
            part.transform.localPosition = position;
            part.transform.localScale = scale;
            part.AddComponent<RuinElementMarker>().Configure(evidenceStatus);
            part.GetComponent<Renderer>().material.color = color;
            return part;
        }
    }
}

