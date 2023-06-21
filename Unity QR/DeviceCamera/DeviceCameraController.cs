using UnityEngine;
using UnityEngine.UI;
using System.Linq;
using System.Collections;
using System;

#if PLATFORM_ANDROID
using UnityEngine.Android;
#endif

public class DeviceCameraController : MonoBehaviour
{
    public RawImage image;
    public RectTransform imageParent;
    public AspectRatioFitter imageFitter;

    // Device cameras
    private WebCamDevice frontCameraDevice;
    private WebCamDevice backCameraDevice;
    private WebCamDevice activeCameraDevice;

    private WebCamTexture frontCameraTexture;
    private WebCamTexture backCameraTexture;
    private WebCamTexture activeCameraTexture;

    // Image rotation
    private Vector3 rotationVector = new Vector3(0f, 0f, 0f);

    // Image uvRect
    private Rect defaultRect = new Rect(0f, 0f, 1f, 1f);
    private Rect fixedRect = new Rect(0f, 1f, 1f, -1f);

    // Image Parent's scale
    private Vector3 defaultScale = new Vector3(1f, 1f, 1f);
    private Vector3 fixedScale = new Vector3(-1f, 1f, 1f);


    private bool isCamAvailable;
    private bool canUpdateCameraRender = true;


    private int requestedWidth = 3840;
    private int requestedHeight = 2160;


    private bool cameraPermissionGranted = false;
    private Action<bool> OnRequestPermissionCallbackAction;


    void Start()
    {
        requestedWidth = Screen.width;
        requestedHeight = Screen.height;

        CheckAndSetDeviceCameraAccess();
        CheckAndSetUpDeviceCamera();
    }


    // Make adjustments to image every frame to be safe, since Unity isn't 
    // guaranteed to report correct data as soon as device camera is started
    void Update()
    {
        UpdateCameraRender();
    }


    #region CAMERA ACCESS - PERMISSION HANDLING


    public void CheckAndSetDeviceCameraAccess()
    {
#if PLATFORM_ANDROID
        cameraPermissionGranted = Permission.HasUserAuthorizedPermission(Permission.Camera);
#else
        cameraPermissionGranted = Application.HasUserAuthorization(UserAuthorization.WebCam);
#endif
    }

    public bool IsCameraPermissionGranted()
    {
        return cameraPermissionGranted;
    }

    public void RequestForDeviceCameraAccess(Action<bool> callback)
    {
        OnRequestPermissionCallbackAction = callback;

#if PLATFORM_ANDROID
        var callbacks = new PermissionCallbacks();
        callbacks.PermissionDenied += PermissionCallbacks_PermissionDenied;
        callbacks.PermissionGranted += PermissionCallbacks_PermissionGranted;
        callbacks.PermissionDeniedAndDontAskAgain += PermissionCallbacks_PermissionDeniedAndDontAskAgain;
        Permission.RequestUserPermission(Permission.Camera, callbacks);
#else

        StartCoroutine(CameraPermissionRequestCoroutine());
#endif

    }

    internal void PermissionCallbacks_PermissionDeniedAndDontAskAgain(string permissionName)
    {
        cameraPermissionGranted = false;
        OnRequestPermissionCallbackAction?.Invoke(cameraPermissionGranted);
    }

    internal void PermissionCallbacks_PermissionGranted(string permissionName)
    {
        cameraPermissionGranted = true;
        CheckAndSetUpDeviceCamera();
        OnRequestPermissionCallbackAction?.Invoke(cameraPermissionGranted);
    }

    internal void PermissionCallbacks_PermissionDenied(string permissionName)
    {
        cameraPermissionGranted = false;
        OnRequestPermissionCallbackAction?.Invoke(cameraPermissionGranted);
    }

    private IEnumerator CameraPermissionRequestCoroutine()
    {
        yield return Application.RequestUserAuthorization(UserAuthorization.WebCam);
        if (Application.HasUserAuthorization(UserAuthorization.WebCam))
        {
            cameraPermissionGranted = true;
            Debug.Log("webcam found");
        }
        else
        {
            cameraPermissionGranted = false;
            Debug.Log("webcam not found");
        }
        CheckAndSetUpDeviceCamera();
        OnRequestPermissionCallbackAction?.Invoke(cameraPermissionGranted);
    }

    #endregion


    #region DEVICE CAMERA SETUP

    public void CheckAndSetUpDeviceCamera()
    {
        if (cameraPermissionGranted)
        {
            SetupDeviceCamera();
        }
    }

    public void SetupDeviceCamera()
    {
        // Check for device cameras
        WebCamDevice[] devices = WebCamTexture.devices;
        if (devices.Length == 0)
        {
            isCamAvailable = false;
            Debug.Log("No devices cameras found");
            return;
        }

        for (int i = 0; i < devices.Length; i++)
        {
            WebCamDevice currentDevice = devices[i];
            //Debug.Log($"TEST CAM : Camera Name #{i} : " + currentDevice.name);
            //Debug.Log($"TEST CAM : Camera Kind #{i} : " + currentDevice.kind);

            if (currentDevice.availableResolutions == null) continue;

            //foreach (Resolution resolution in currentDevice.availableResolutions)
            //{
            //    Debug.Log($"TEST CAM : Camera Available Res #{i} : " + resolution);
            //}
        }

        Debug.Log("Cameras found : " + devices.Length);


        // Get the device's cameras and create WebCamTextures with them
        frontCameraDevice = WebCamTexture.devices.Last();
        backCameraDevice = WebCamTexture.devices.First();

        //Debug.Log("Front Cam Device Name : " + frontCameraDevice.name);
        //Debug.Log("Back Cam Device Name : " + backCameraDevice.name);

        frontCameraTexture = new WebCamTexture(frontCameraDevice.name, requestedWidth, requestedHeight);
        backCameraTexture = new WebCamTexture(backCameraDevice.name, requestedWidth, requestedHeight);

        // Set camera filter modes for a smoother looking image
        frontCameraTexture.filterMode = FilterMode.Trilinear;
        backCameraTexture.filterMode = FilterMode.Trilinear;


        //Debug.Log("TEST : SCREEN WIDTH-HEIGHT : " + Screen.width + " : " + Screen.height);
        //Debug.Log("TEST : Front Camera REQ WIDTH-HEIGHT : " + frontCameraTexture.requestedWidth + " : " + frontCameraTexture.requestedHeight);
        //Debug.Log("TEST : Back Camera REQ WIDTH-HEIGHT : " + backCameraTexture.requestedWidth + " : " + backCameraTexture.requestedHeight);


        //Debug.Log("Front Cam Texture : " + frontCameraTexture == null);
        //Debug.Log("Back Cam Texture : " + backCameraTexture == null);

        // Set the camera to use by default
        SetActiveCamera(backCameraTexture);
    }

    // Set the device camera to use and start it
    private void SetActiveCamera(WebCamTexture cameraToUse)
    {
        if (activeCameraTexture != null)
        {
            activeCameraTexture.Stop();
        }

        activeCameraTexture = new WebCamTexture(cameraToUse.deviceName, requestedWidth, requestedHeight);
        activeCameraDevice = WebCamTexture.devices.FirstOrDefault(device =>
            device.name == cameraToUse.deviceName);


        //Debug.Log("Active Cam Texture : " + activeCameraTexture == null);
        //Debug.Log("Active Cam Device name : " + activeCameraDevice.name);

        Texture2D texture = new Texture2D(activeCameraTexture.requestedWidth, activeCameraTexture.requestedHeight);

        image.texture = activeCameraTexture;
        image.material.mainTexture = activeCameraTexture;

        //Todo Comment here..
        //activeCameraTexture.Play();

        //Debug.Log("Active Cam Available Res : " + activeCameraDevice.availableResolutions.Length);
        isCamAvailable = true;
    }

    #endregion


    private void UpdateCameraRender()
    {
        if (isCamAvailable == false) return;

        if (canUpdateCameraRender == false) return;

        // Skip making adjustment for incorrect camera data
        if (activeCameraTexture.width < 100)
        {
            //Debug.Log("Still waiting another frame for correct info...");
            return;
        }

        // Rotate image to show correct orientation 
        rotationVector.z = -activeCameraTexture.videoRotationAngle;
        image.rectTransform.localEulerAngles = rotationVector;

        activeCameraTexture.requestedWidth = requestedWidth;
        activeCameraTexture.requestedHeight = requestedHeight;

        // Set AspectRatioFitter's ratio
        float videoRatioW2H =
            (float)activeCameraTexture.width / (float)activeCameraTexture.height;
        float videoRatioH2W =
           (float)activeCameraTexture.height / (float)activeCameraTexture.width;
        imageFitter.aspectRatio = videoRatioW2H;


        // Unflip if vertically flipped
        image.uvRect =
            activeCameraTexture.videoVerticallyMirrored ? fixedRect : defaultRect;

        // Mirror front-facing camera's image horizontally to look more natural
        imageParent.localScale =
            activeCameraDevice.isFrontFacing ? fixedScale : defaultScale;

    }


    #region EXTERNAL

    // Switch between the device's front and back camera
    public void SwitchCamera()
    {
        SetActiveCamera(activeCameraTexture.Equals(frontCameraTexture) ?
            backCameraTexture : frontCameraTexture);
    }

    public void SetCanUpdateCamera(bool canRenderCam)
    {
        this.canUpdateCameraRender = canRenderCam;
    }

    public WebCamTexture GetActiveWebCamTexture()
    {
        return activeCameraTexture;
    }

    public void PlayActiveCamera()
    {
        if (activeCameraTexture == null) return;

        activeCameraTexture.Play();
        canUpdateCameraRender = true;
    }

    public void PauseActiveCamera()
    {
        if (activeCameraTexture == null) return;

        activeCameraTexture.Pause();
        canUpdateCameraRender = false;
    }

    #endregion

}