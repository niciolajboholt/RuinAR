using RuinAR.Core;
using UnityEngine;

namespace RuinAR.AR
{
    public static class ProceduralReconstructionFactory
    {
        private static readonly Color DocumentedStone = new(0.30f, 0.58f, 0.38f);
        private static readonly Color ProbableBrick = new(0.86f, 0.62f, 0.20f);
        private static readonly Color AiInterpretation = new(0.55f, 0.28f, 0.68f);

        public static GameObject CreatePrototype(Pose pose)
        {
            var root = new GameObject("Kalø Slotsruin · Kildebaseret prototype");
            root.transform.SetPositionAndRotation(pose.position, pose.rotation);

            CreateCastleBank(root.transform);
            CreateRingWall(root.transform);
            CreateGatehouse(root.transform);
            CreateCourtyardBuildings(root.transform);
            CreateMainTower(root.transform);

            return root;
        }

        private static void CreateCastleBank(Transform root)
        {
            CreatePart(root, "Dokumenteret borgbanke og grundplan", new Vector3(0f, 0.08f, 0f),
                new Vector3(9.4f, 0.16f, 8f), DocumentedStone, EvidenceStatus.Documented);
            CreatePart(root, "Dokumenteret forgård", new Vector3(-3.25f, 0.18f, -2.55f),
                new Vector3(2.3f, 0.20f, 2.2f), DocumentedStone, EvidenceStatus.Documented);
        }

        private static void CreateRingWall(Transform root)
        {
            const float halfWidth = 4.45f;
            const float halfDepth = 3.75f;
            const float thickness = 0.38f;

            CreatePart(root, "Dokumenteret nordlig ringmur", new Vector3(0f, 0.62f, halfDepth),
                new Vector3(9f, 1.08f, thickness), DocumentedStone, EvidenceStatus.Documented);
            CreatePart(root, "Dokumenteret vestlig ringmur", new Vector3(-halfWidth, 0.72f, 0f),
                new Vector3(thickness, 1.28f, 7.5f), DocumentedStone, EvidenceStatus.Documented);
            CreatePart(root, "Dokumenteret østlig ringmur", new Vector3(halfWidth, 0.82f, -0.6f),
                new Vector3(thickness, 1.48f, 6.3f), DocumentedStone, EvidenceStatus.Documented);
            CreatePart(root, "Dokumenteret sydmur vest", new Vector3(-3.55f, 0.55f, -halfDepth),
                new Vector3(1.8f, 0.94f, thickness), DocumentedStone, EvidenceStatus.Documented);
            CreatePart(root, "Dokumenteret sydmur øst", new Vector3(1.25f, 0.67f, -halfDepth),
                new Vector3(4.8f, 1.18f, thickness), DocumentedStone, EvidenceStatus.Documented);

            CreatePart(root, "Sandsynlig nordlig ringmur", new Vector3(-1.2f, 2.0f, halfDepth),
                new Vector3(6.6f, 1.7f, 0.30f), ProbableBrick, EvidenceStatus.Probable);
            CreatePart(root, "Sandsynlig vestlig ringmur", new Vector3(-halfWidth, 1.95f, 0.5f),
                new Vector3(0.30f, 1.45f, 5.5f), ProbableBrick, EvidenceStatus.Probable);
            CreatePart(root, "Sandsynlig sydøstlig ringmur", new Vector3(1.25f, 1.85f, -halfDepth),
                new Vector3(4.8f, 1.15f, 0.30f), ProbableBrick, EvidenceStatus.Probable);
        }

        private static void CreateGatehouse(Transform root)
        {
            var gate = new GameObject("Porttårn og vindebro");
            gate.transform.SetParent(root, false);
            gate.transform.localPosition = new Vector3(-2.5f, 0f, -3.45f);

            CreatePart(gate.transform, "Dokumenteret portfundament vest", new Vector3(-0.75f, 0.7f, 0f),
                new Vector3(0.75f, 1.4f, 1.3f), DocumentedStone, EvidenceStatus.Documented);
            CreatePart(gate.transform, "Dokumenteret portfundament øst", new Vector3(0.75f, 0.7f, 0f),
                new Vector3(0.75f, 1.4f, 1.3f), DocumentedStone, EvidenceStatus.Documented);
            CreatePart(gate.transform, "Sandsynligt porttårn vest", new Vector3(-0.75f, 2.3f, 0f),
                new Vector3(0.68f, 1.8f, 1.2f), ProbableBrick, EvidenceStatus.Probable);
            CreatePart(gate.transform, "Sandsynligt porttårn øst", new Vector3(0.75f, 2.3f, 0f),
                new Vector3(0.68f, 1.8f, 1.2f), ProbableBrick, EvidenceStatus.Probable);
            CreatePart(gate.transform, "Sandsynlig portoverligger", new Vector3(0f, 3.05f, 0f),
                new Vector3(1.2f, 0.34f, 1.1f), ProbableBrick, EvidenceStatus.Probable);
            CreatePart(gate.transform, "AI-fortolket porttag", new Vector3(0f, 3.45f, 0f),
                new Vector3(2.35f, 0.20f, 1.65f), AiInterpretation, EvidenceStatus.AiGenerated);
        }

        private static void CreateCourtyardBuildings(Transform root)
        {
            CreatePart(root, "Sandsynlig fruerstue", new Vector3(-3.55f, 1.65f, 1.25f),
                new Vector3(1.25f, 2.5f, 3.1f), ProbableBrick, EvidenceStatus.Probable);
            CreatePart(root, "Sandsynligt køkken og bryggers", new Vector3(0.35f, 1.35f, -3.05f),
                new Vector3(3.5f, 2f, 1.15f), ProbableBrick, EvidenceStatus.Probable);
            CreatePart(root, "Sandsynlig riddersal og kapel", new Vector3(-0.65f, 1.55f, 3.05f),
                new Vector3(4.8f, 2.25f, 1.1f), ProbableBrick, EvidenceStatus.Probable);

            CreateRoof(root, "AI-fortolket tag over fruerstue", new Vector3(-3.55f, 3.05f, 1.25f),
                new Vector3(1.55f, 0.18f, 3.35f), 10f);
            CreateRoof(root, "AI-fortolket tag over køkkenfløj", new Vector3(0.35f, 2.5f, -3.05f),
                new Vector3(3.8f, 0.18f, 1.45f), -8f);
            CreateRoof(root, "AI-fortolket tag over riddersal", new Vector3(-0.65f, 2.85f, 3.05f),
                new Vector3(5.05f, 0.18f, 1.4f), 8f);
        }

        private static void CreateMainTower(Transform root)
        {
            var tower = new GameObject("Kalø hovedtårn");
            tower.transform.SetParent(root, false);
            tower.transform.localPosition = new Vector3(3.15f, 0f, 2.25f);

            CreateHollowTowerLevel(tower.transform, "Dokumenteret hovedtårn", 1.9f, 3.6f,
                DocumentedStone, EvidenceStatus.Documented);
            CreateHollowTowerLevel(tower.transform, "Sandsynlig øvre hovedtårn", 4.45f, 1.5f,
                ProbableBrick, EvidenceStatus.Probable);

            for (var x = -1; x <= 1; x++)
            {
                CreatePart(tower.transform, "AI-fortolket brystværn", new Vector3(x * 0.75f, 5.55f, 1.15f),
                    new Vector3(0.48f, 0.62f, 0.42f), AiInterpretation, EvidenceStatus.AiGenerated);
                CreatePart(tower.transform, "AI-fortolket brystværn", new Vector3(x * 0.75f, 5.55f, -1.15f),
                    new Vector3(0.48f, 0.62f, 0.42f), AiInterpretation, EvidenceStatus.AiGenerated);
            }
        }

        private static void CreateHollowTowerLevel(Transform root, string name, float centerY, float height,
            Color color, EvidenceStatus status)
        {
            CreatePart(root, name + " nord", new Vector3(0f, centerY, 1.15f),
                new Vector3(2.7f, height, 0.42f), color, status);
            CreatePart(root, name + " syd", new Vector3(0f, centerY, -1.15f),
                new Vector3(2.7f, height, 0.42f), color, status);
            CreatePart(root, name + " vest", new Vector3(-1.15f, centerY, 0f),
                new Vector3(0.42f, height, 1.9f), color, status);
            CreatePart(root, name + " øst", new Vector3(1.15f, centerY, 0f),
                new Vector3(0.42f, height, 1.9f), color, status);
        }

        private static void CreateRoof(Transform root, string name, Vector3 position, Vector3 scale, float tilt)
        {
            var roof = CreatePart(root, name, position, scale, AiInterpretation, EvidenceStatus.AiGenerated);
            roof.transform.localRotation = Quaternion.Euler(0f, 0f, tilt);
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

