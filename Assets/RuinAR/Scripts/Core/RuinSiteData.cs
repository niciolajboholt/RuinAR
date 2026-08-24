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
                id = "kalo-slotsruin",
                displayName = "Kalø Slotsruin",
                latitude = 56.274636,
                longitude = 10.466666,
                elements = new List<RuinElementData>
                {
                    new()
                    {
                        id = "surviving-masonry",
                        displayName = "Synlige ruiner og grundplan",
                        evidenceStatus = EvidenceStatus.Documented,
                        confidence = 1f,
                        sourceTitle = "Fortidsmindeguide og Naturstyrelsen"
                    },
                    new()
                    {
                        id = "upper-walls",
                        displayName = "Øvre mure og borgbygninger",
                        evidenceStatus = EvidenceStatus.Probable,
                        confidence = 0.65f,
                        sourceTitle = "Kildebaseret prototypefortolkning"
                    },
                    new()
                    {
                        id = "roofs-and-battlements",
                        displayName = "Tage og brystværn",
                        evidenceStatus = EvidenceStatus.AiGenerated,
                        confidence = 0.25f,
                        sourceTitle = "Visuel fortolkning uden faglig godkendelse"
                    }
                },
                timeline = new List<TimelineVersionData>
                {
                    new() { id = "erik-menved", displayName = "Erik Menveds borg", year = 1313 },
                    new() { id = "valdemar", displayName = "Valdemar Atterdags udbygning", year = 1343 },
                    new() { id = "gustav-vasa", displayName = "Gustav Vasa på Kalø", year = 1519 },
                    new() { id = "demolition", displayName = "Nedrivningen begynder", year = 1672 },
                    new() { id = "present", displayName = "Kalø Slotsruin i dag", year = 2026 }
                }
            };
        }
    }
}

