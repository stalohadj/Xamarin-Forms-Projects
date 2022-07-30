using System;
using System.Diagnostics;
using Plugin.Geofence;
using Xamarin.Forms;

namespace PriveSports
{
    public class CrossGeofenceListener : IGeofenceListener
    {
        public void OnMonitoringStarted(string region)
        {
            Debug.WriteLine(string.Format("{0} - Monitoring started in region: {1}", CrossGeofence.Id, region));
        }

        public void OnMonitoringStopped()
        {
            Debug.WriteLine(string.Format("{0} - {1}", CrossGeofence.Id, "Monitoring stopped for all regions"));
        }

        public void OnMonitoringStopped(string identifier)
        {
            Debug.WriteLine(string.Format("{0} - {1}: {2}", CrossGeofence.Id, "Monitoring stopped in region", identifier));
        }

        public void OnError(string error)
        {
            Debug.WriteLine(string.Format("{0} - {1}: {2}", CrossGeofence.Id, "Error", error));
        }

        // Note that you must call CrossGeofence.GeofenceListener.OnAppStarted() from your app when you want this method to run.
        public void OnAppStarted()
        {
            Debug.WriteLine(string.Format("{0} - {1}", CrossGeofence.Id, "App started"));
        }

        public void OnRegionStateChanged(GeofenceResult result)
        {
            Debug.WriteLine(string.Format("{0} - {1}", CrossGeofence.Id, result.ToString()));
        }

        public void OnLocationChanged(GeofenceLocation location)
        {
            Debug.WriteLine(string.Format("{0} - {1}", CrossGeofence.Id, location.ToString()));
        }
    }
}

