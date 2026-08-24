using System;
using System.Collections.Generic;

namespace RuinAR.Core
{
    [Serializable]
    public sealed class RuinElementData
    {
        public string id;
        public string displayName;
        public EvidenceStatus evidenceStatus;
        public float confidence;
        public string sourceTitle;
        public string sourceUrl;
    }

    [Serializable]
    public sealed class TimelineVersionData
    {
        public string id;
        public string displayName;
        public int year;
    }

    [Serializable]
    public sealed class RuinSiteData
    {
        public string id;
        public string displayName;
        public double latitude;
        public double longitude;
        public string language = "da";
        public string verificationLabel = "Ikke fagligt verificeret";
        public List<RuinElementData> elements = new();
        public List<TimelineVersionData> timeline = new();

        public static RuinSiteData CreatePrototype()
        {
            return new RuinSiteData
            {
                id = "prototype-ruin",
                displayName = "RuinAR forsøgsruin",
                elements = new List<RuinElementData>
                {
                    new()
                    {
                        id = "foundation",
                        displayName = "Fundament",
                        evidenceStatus = EvidenceStatus.Documented,
                        confidence = 1f,
                        sourceTitle = "Synlig ruin"
                    },
                    new()
                    {
                        id = "walls",
                        displayName = "Murhøjde",
                        evidenceStatus = EvidenceStatus.Probable,
                        confidence = 0.65f,
                        sourceTitle = "Prototypeantagelse"
                    },
                    new()
                    {
                        id = "roof",
                        displayName = "Tag",
                        evidenceStatus = EvidenceStatus.AiGenerated,
                        confidence = 0.25f,
                        sourceTitle = "AI-fortolkning uden faglig godkendelse"
                    }
                },
                timeline = new List<TimelineVersionData>
                {
                    new() { id = "original", displayName = "Oprindelig bygning", year = 1300 },
                    new() { id = "present", displayName = "Ruinen i dag", year = 2026 }
                }
            };
        }
    }
}

