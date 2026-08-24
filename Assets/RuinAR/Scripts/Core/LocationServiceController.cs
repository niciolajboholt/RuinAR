using System.Collections;
using UnityEngine;

#if UNITY_ANDROID
using UnityEngine.Android;
#endif

namespace RuinAR.Core
{
    public sealed class LocationServiceController : MonoBehaviour
    {
        public bool HasLocation { get; private set; }
        public double Latitude { get; private set; }
        public double Longitude { get; private set; }
        public string StatusMessage { get; private set; } = "Lokation afventer";

        private void Start()
        {
            StartCoroutine(StartLocation());
        }

        private IEnumerator StartLocation()
        {
#if UNITY_ANDROID
            if (!Permission.HasUserAuthorizedPermission(Permission.FineLocation))
            {
                Permission.RequestUserPermission(Permission.FineLocation);
                yield return new WaitForSeconds(1f);
            }
#endif

            if (!Input.location.isEnabledByUser)
            {
                StatusMessage = "Lokation er slået fra";
                yield break;
            }

            Input.location.Start(1f, 1f);
            var remainingSeconds = 15;

            while (Input.location.status == LocationServiceStatus.Initializing && remainingSeconds > 0)
            {
                yield return new WaitForSeconds(1f);
                remainingSeconds--;
            }

            if (Input.location.status != LocationServiceStatus.Running)
            {
                StatusMessage = "Lokation kunne ikke bestemmes";
                yield break;
            }

            var data = Input.location.lastData;
            Latitude = data.latitude;
            Longitude = data.longitude;
            HasLocation = true;
            StatusMessage = $"GPS ±{data.horizontalAccuracy:0} m";
        }

        private void OnDestroy()
        {
            if (Input.location.status == LocationServiceStatus.Running)
                Input.location.Stop();
        }
    }
}

