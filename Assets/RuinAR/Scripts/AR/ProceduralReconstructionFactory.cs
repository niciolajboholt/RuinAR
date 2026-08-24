using RuinAR.Core;
using UnityEngine;

namespace RuinAR.AR
{
    public static class ProceduralReconstructionFactory
    {
        public static GameObject CreatePrototype(Pose pose)
        {
            var root = new GameObject("Prototype Reconstruction");
            root.transform.SetPositionAndRotation(pose.position, pose.rotation);

            CreatePart(root.transform, "Dokumenteret fundament", new Vector3(0f, 0.1f, 0f),
                new Vector3(4.2f, 0.2f, 3.2f), new Color(0.2f, 0.75f, 0.35f), EvidenceStatus.Documented);

            CreatePart(root.transform, "Sandsynlig nordmur", new Vector3(0f, 1.25f, 1.5f),
                new Vector3(4f, 2.5f, 0.2f), new Color(0.95f, 0.7f, 0.15f), EvidenceStatus.Probable);
            CreatePart(root.transform, "Sandsynlig sydmur", new Vector3(0f, 1.25f, -1.5f),
                new Vector3(4f, 2.5f, 0.2f), new Color(0.95f, 0.7f, 0.15f), EvidenceStatus.Probable);
            CreatePart(root.transform, "Sandsynlig vestmur", new Vector3(-1.9f, 1.25f, 0f),
                new Vector3(0.2f, 2.5f, 3f), new Color(0.95f, 0.7f, 0.15f), EvidenceStatus.Probable);
            CreatePart(root.transform, "Sandsynlig østmur", new Vector3(1.9f, 1.25f, 0f),
                new Vector3(0.2f, 2.5f, 3f), new Color(0.95f, 0.7f, 0.15f), EvidenceStatus.Probable);

            var roof = CreatePart(root.transform, "AI-genereret tag", new Vector3(0f, 3.15f, 0f),
                new Vector3(4.4f, 0.25f, 3.6f), new Color(0.65f, 0.3f, 0.85f), EvidenceStatus.AiGenerated);
            roof.transform.localRotation = Quaternion.Euler(0f, 0f, 10f);

            return root;
        }

        private static GameObject CreatePart(
            Transform parent,
            string name,
            Vector3 localPosition,
            Vector3 localScale,
            Color color,
            EvidenceStatus evidenceStatus)
        {
            var part = GameObject.CreatePrimitive(PrimitiveType.Cube);
            part.name = name;
            part.transform.SetParent(parent, false);
            part.transform.localPosition = localPosition;
            part.transform.localScale = localScale;
            part.AddComponent<RuinElementMarker>().Configure(evidenceStatus);

            var renderer = part.GetComponent<Renderer>();
            renderer.material.color = color;
            return part;
        }
    }
}

