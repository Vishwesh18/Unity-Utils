using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class SampleScanPanel : MonoBehaviour
{
    [SerializeField] private DeviceCameraController deviceCameraController;

    private bool _isScanning;


    #if UNITY_ANDROID

        void OnApplicationFocus(bool hasFocus)
        {
            //Debug.Log("CHECK : Application Focused : " + hasFocus);

            if (!hasFocus) return;

            if (isGoToSettingsPopupEnabled)
            {
                deviceCameraController.CheckAndSetDeviceCameraAccess();
                //Debug.Log("CHECK : Application Focused PERMISSION CHECK : " + deviceCameraController.IsCameraPermissionGranted());
                if (deviceCameraController.IsCameraPermissionGranted())
                {
                    isGoToSettingsPopupEnabled = false;
                    cameraAccessPopupTwo.gameObject.SetActive(false);
                    deviceCameraController.CheckAndSetUpDeviceCamera();
                    deviceCameraController.PlayActiveCamera();
                    StartScan();
                    return;
                }
            }

        }

        //void OnApplicationPause(bool pauseStatus)
        //{
        //    Debug.Log("CHECK : Application Paused : " + pauseStatus);
        //}

    #endif



    private void CheckCameraAccessAndStartScanning()    
    {
        if (deviceCameraController.IsCameraPermissionGranted())
        {
            deviceCameraController.PlayActiveCamera();
            StartScan();
            return;
        }
        else
        {
            bool hasRequestedAlready = LocalStorageManager.HasRequestedForCameraAccess();

            if (hasRequestedAlready)
            {
                isGoToSettingsPopupEnabled = true;
                cameraAccessPopupTwo.gameObject.SetActive(true);
                return;
            }

            cameraAccessPopupOne.gameObject.SetActive(true);
        }
    }

    #region SCAN

        private void StartScan()
        {
            Debug.Log("CHECK : Application Scanning!");
            _isScanning = true;
            qrReader.StartScanning();
        }

        private void StopScan()
        {
            _isScanning = false;
            qrReader.StopScanning();
        }

    #endregion

    #region CAMERA ACCESS POPUP

        public void RequestCameraPermissionButtonClicked()
        {
            LocalStorageManager.SetRequestedForCameraAccess();
            cameraAccessPopupOne.gameObject.SetActive(false);
            deviceCameraController.RequestForDeviceCameraAccess(OnPermissionRequestCompleted);
        }

        private void OnPermissionRequestCompleted(bool permissionGranted)
        {
            if (permissionGranted)
            {
                deviceCameraController.PlayActiveCamera();
                StartScan();
            }
            else
            {
                isGoToSettingsPopupEnabled = true;
                cameraAccessPopupTwo.gameObject.SetActive(true);
            }
        }

        public void GoToSettingsButtonClicked()
        {
            OpenSettings();
        }

        public void OpenSettings()
        {
    #if UNITY_EDITOR
            return;
    #endif

    #if UNITY_IOS
            string url = MyNativeBindings.GetSettingsURL();
            Application.OpenURL(url);
    #elif UNITY_ANDROID
            MyNativeBindings.OpenSettings();
    #endif
        }

    #endregion

}