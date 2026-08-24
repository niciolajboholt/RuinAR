using RuinAR.Core;
using UnityEngine;

namespace RuinAR.AR
{
    public sealed class RuinElementMarker : MonoBehaviour
    {
        public EvidenceStatus EvidenceStatus { get; private set; }

        public void Configure(EvidenceStatus status)
        {
            EvidenceStatus = status;
        }
    }
}

