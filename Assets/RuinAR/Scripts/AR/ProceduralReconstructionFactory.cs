using RuinAR.Core;
using UnityEngine;

namespace RuinAR.AR
{
    public static class ProceduralReconstructionFactory
    {
        private static readonly Color DocumentedStone = new(0.30f, 0.58f, 0.38f);
        private static readonly Color ProbableStone = new(0.86f, 0.62f, 0.20f);
        private static readonly Color AiTimber = new(0.55f, 0.28f, 0.68f);

        public static GameObject CreatePrototype(Pose pose)
        {
            var root = new GameObject("Prototype Reconstruction");
            root.transform.SetPositionAndRotation(pose.position, pose.rotation);

            CreatePart(root.transform, "Dokumenteret fundament", new Vector3(0f, 0.08f, 0f),
                new Vector3(6.2f, 0.16f, 5.2f), DocumentedStone, EvidenceStatus.Documented);
            CreateWallRun(root.transform, "Dokumenteret nordmur", new Vector3(0f, 0.55f, 2.42f),
                new Vector3(6f, 1.1f, 0.36f), DocumentedStone, EvidenceStatus.Documented);
            CreateWallRun(root.transform, "Dokumenteret vestmur", new Vector3(-2.82f, 0.7f, 0f),
                new Vector3(0.36f, 1.4f, 4.5f), DocumentedStone, EvidenceStatus.Documented);
            CreateWallRun(root.transform, "Dokumenteret sydmur venstre", new Vector3(-2.05f, 0.65f, -2.42f),
                new Vector3(1.9f, 1.3f, 0.36f), DocumentedStone, EvidenceStatus.Documented);
            CreateWallRun(root.transform, "Dokumenteret sydmur højre", new Vector3(2.05f, 0.65f, -2.42f),
                new Vector3(1.9f, 1.3f, 0.36f), DocumentedStone, EvidenceStatus.Documented);

            CreatePart(root.transform, "Sandsynlig vestlig overmur", new Vector3(-2.82f, 2.15f, 0f),
                new Vector3(0.30f, 1.5f, 4.5f), ProbableStone, EvidenceStatus.Probable);
            CreatePart(root.transform, "Sandsynlig nordlig overmur", new Vector3(0f, 2.05f, 2.42f),
                new Vector3(6f, 1.9f, 0.30f), ProbableStone, EvidenceStatus.Probable);
            CreatePart(root.transform, "Sandsynlig sydfacade venstre", new Vector3(-2.05f, 2.0f, -2.42f),
                new Vector3(1.9f, 1.4f, 0.30f), ProbableStone, EvidenceStatus.Probable);
            CreatePart(root.transform, "Sandsynlig sydfacade højre", new Vector3(2.05f, 2.0f, -2.42f),
                new Vector3(1.9f, 1.4f, 0.30f), ProbableStone, EvidenceStatus.Probable);
            CreatePart(root.transform, "Sandsynlig overligger", new Vector3(0f, 2.85f, -2.42f),
                new Vector3(2.2f, 0.32f, 0.36f), ProbableStone, EvidenceStatus.Probable);
            CreatePart(root.transform, "Sandsynlig østmur bag", new Vector3(2.82f, 1.9f, 1.5f),
                new Vector3(0.30f, 2.7f, 1.55f), ProbableStone, EvidenceStatus.Probable);
            CreatePart(root.transform, "Sandsynlig østmur front", new Vector3(2.82f, 1.9f, -1.5f),
                new Vector3(0.30f, 2.7f, 1.55f), ProbableStone, EvidenceStatus.Probable);

            CreateColumn(root.transform, "Sandsynlig søjle venstre", new Vector3(-1.25f, 1.45f, -0.35f));
            CreateColumn(root.transform, "Sandsynlig søjle højre", new Vector3(1.25f, 1.45f, -0.35f));

            var leftRoof = CreatePart(root.transform, "AI-genereret vestligt tagfald", new Vector3(-1.45f, 3.6f, 0f),
                new Vector3(3.25f, 0.16f, 5.25f), AiTimber, EvidenceStatus.AiGenerated);
            leftRoof.transform.localRotation = Quaternion.Euler(0f, 0f, 25f);

            var rightRoof = CreatePart(root.transform, "AI-genereret østligt tagfald", new Vector3(1.45f, 3.6f, 0f),
                new Vector3(3.25f, 0.16f, 5.25f), AiTimber, EvidenceStatus.AiGenerated);
            rightRoof.transform.localRotation = Quaternion.Euler(0f, 0f, -25f);

            return root;
        }

        private static void CreateWallRun(Transform parent, string name, Vector3 position, Vector3 scale,
            Color color, EvidenceStatus evidenceStatus)
        {
            var wall = CreatePart(parent, name, position, scale, color, evidenceStatus);
            wall.transform.localRotation = Quaternion.Euler(0f, 0f, Random.Range(-1.2f, 1.2f));
        }

        private static void CreateColumn(Transform parent, string name, Vector3 position)
        {
            var column = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
            column.name = name;
            column.transform.SetParent(parent, false);
            column.transform.localPosition = position;
            column.transform.localScale = new Vector3(0.36f, 1.45f, 0.36f);
            column.AddComponent<RuinElementMarker>().Configure(EvidenceStatus.Probable);
            column.GetComponent<Renderer>().material.color = ProbableStone;
        }

        private static GameObject CreatePart(Transform parent, string name, Vector3 localPosition,
            Vector3 localScale, Color color, EvidenceStatus evidenceStatus)
        {
            var part = GameObject.CreatePrimitive(PrimitiveType.Cube);
            part.name = name;
            part.transform.SetParent(parent, false);
            part.transform.localPosition = localPosition;
            part.transform.localScale = localScale;
            part.AddComponent<RuinElementMarker>().Configure(evidenceStatus);
            part.GetComponent<Renderer>().material.color = color;
            return part;
        }
    }
}

