// %BANNER_BEGIN%
// ---------------------------------------------------------------------
// %COPYRIGHT_BEGIN%
// <copyright file="MLWebViewNativeBindings.cs" company="Magic Leap, Inc">
//
// Copyright (c) 2018 Magic Leap, Inc. All Rights Reserved.
// Use of this file is governed by your Early Access Terms and Conditions.
// This software is an Early Access Product.
//
// </copyright>
// %COPYRIGHT_END%
// ---------------------------------------------------------------------
// %BANNER_END%

#if UNITY_MAGICLEAP || UNITY_ANDROID

namespace UnityEngine.XR.MagicLeap
{
    using System;
    using System.Runtime.InteropServices;
    using UnityEngine.XR.MagicLeap.Native;

    /// <summary>
    /// MLWebView class exposes static functions that allows an application to instantiate a hardware
    /// accelerated WebView and interact with it(via "mouse" and "keyboard" events).
    /// </summary>
    public partial class MLWebView
    {
        /// <summary>
        /// See ml_webview.h for additional comments.
        /// </summary>
        private class NativeBindings : Native.MagicLeapNativeBindings
        {
            /// <summary>
            /// The delegate for the before reasource loaded event.
            /// </summary>
            /// <param name="resourceUrl">The url of the resource about to be loaded.</param>
            /// <param name="userData">User defined data.</param>
            public delegate void OnBeforeResourceLoadCallback([MarshalAs(UnmanagedType.LPStr)] string resourceUrl, IntPtr userData);

            /// <summary>
            /// The delegate for the reasource loaded event.
            /// </summary>
            /// <param name="isMainFrame">Whether this event was for the main frame.</param>
            /// <param name="httpStatusCode">The standard http status code.</param>
            /// <param name="userData">User defined data.</param>
            public delegate void OnLoadEndCallback([MarshalAs(UnmanagedType.I1)] bool isMainFrame, int httpStatusCode, IntPtr userData);

            /// <summary>
            /// The delegate for the reasource load error event.
            /// </summary>
            /// <param name="isMainFrame">True if this event was for the main frame.</param>
            /// <param name="httpStatusCode">Http status code for the URL load failure.</param>
            /// <param name="errorStr">The stringified version of the error code.</param>
            /// <param name="failedUrl">The url that caused the load error.</param>
            /// <param name="userData">User defined data.</param>
            public delegate void OnLoadErrorCallback([MarshalAs(UnmanagedType.I1)] bool isMainFrame, int httpStatusCode, [MarshalAs(UnmanagedType.LPStr)] string errorStr, [MarshalAs(UnmanagedType.LPStr)] string failedUrl, IntPtr userData);

            /// <summary>
            /// The delegate for the certificate error event.
            /// </summary>
            /// <param name="errorCode">Error code for ssl error.</param>
            /// <param name="url">The url associated to the certificate error.</param>
            /// <param name="errorMessage">Certificate error short description.</param>
            /// <param name="details">Certificate error details.</param>
            /// <param name="userData">User defined data.</param>
            [return: MarshalAs(UnmanagedType.I1)]
            public delegate bool OnCertificateErrorCallback(int errorCode, [MarshalAs(UnmanagedType.LPStr)] string url, [MarshalAs(UnmanagedType.LPStr)] string errorMessage, [MarshalAs(UnmanagedType.LPStr)] string details, IntPtr userData);

            /// <summary>
            /// The delegate for the keyboard show event.
            /// </summary>
            /// <param name="x">Horizontal position of the input field.</param>
            /// <param name="y">Vertical position of the input field.</param>
            /// <param name="width">Width of the input field.</param>
            /// <param name="height">Height of the input field.</param>
            /// <param name="textInputFlags">One or combination of TextInputFlags.</param>
            /// <param name="textInputType">One of TextInputType.</param>
            /// <param name="userData">User defined data.</param>
            public delegate void OnShowKeyboardCallback(int x, int y, int width, int height, TextInputFlags textInputFlags, TextInputType textInputType, IntPtr userData);

            /// <summary>
            /// The delegate for the keyboard dismiss event.
            /// </summary>
            /// <param name="userData">User defined data.</param>
            public delegate void OnKeyboardDismissCallback(IntPtr userData);

            /// <summary>
            /// The delegate for the webview destroy event.
            /// </summary>
            /// <param name="userData">User defined data.</param>
            public delegate void OnDestroyCallback(IntPtr userData);

            /// <summary>
            /// The delegate for the webview service connected event.
            /// </summary>
            public delegate void OnServiceConnectedCallback(IntPtr userData);

            /// <summary>
            /// The delegate for the webview service disconnected event.
            /// </summary>
            public delegate void OnServiceDisconnectedCallback(IntPtr userData);

            /// <summary>
            /// Struct to define the cursor's state.
            /// </summary>
            [StructLayout(LayoutKind.Sequential)]
            public struct CursorState
            {
                /// <summary>
                /// Version of this struct.
                /// </summary>
                public uint Version;

                /// <summary>
                /// Horizontal position of the cursor.
                /// </summary>
                public uint XPosition;

                /// <summary>
                /// Vertical position of the cursor.
                /// </summary>
                public uint YPosition;

                /// <summary>
                /// Should be one or combination of #MLWebViewEventFlags.
                /// </summary>
                public EventFlags Modifiers;

                /// <summary>
                /// Create and return an initialized version of this struct.
                /// </summary>
                /// <returns>A new instance of this struct.</returns>
                public static CursorState Create(uint xPosition, uint yPosition, EventFlags modifiers)
                {
                    return new CursorState()
                    {
                        Version = 1,
                        XPosition = xPosition,
                        YPosition = yPosition,
                        Modifiers = modifiers
                    };
                }
            };

            /// <summary>
            /// Struct to define webview initialization.
            /// </summary>
            [StructLayout(LayoutKind.Sequential)]
            public struct Settings
            {
                /// <summary>
                /// Version of this struct.
                /// </summary>
                public uint Version;

                /// <summary>
                /// Horizontal size of webview in pixels.
                /// </summary>
                public uint Width;

                /// <summary>
                /// Vertical size of webview in pixels.
                /// </summary>
                public uint Height;

                /// <summary>
                /// JavaVM* pointer to use for Android up-calls.
                /// </summary>
                public IntPtr ApplicationVm;

                /// <summary>
                /// jobject to android.  content.  Context instance for Android up-calls.
                /// </summary>
                public IntPtr Context;

                /// <summary>
                /// Event callbacks for interacting with webview.
                /// </summary>
                public EventCallbacks Callbacks;

                /// <summary>
                /// Create and return an initialized version of this struct.
                /// </summary>
                /// <returns>A new instance of this struct.</returns>
                public static Settings Create(GCHandle gcHandle, uint width, uint height)
                {
                    return new Settings()
                    {
                        Version = 1,
                        Width = width,
                        Height = height,
                        ApplicationVm = GetJavaVM(),
                        Context = GetAppContext(),
                        Callbacks = EventCallbacks.Create(gcHandle)
                    };
                }
            };

            /// <summary>
            /// Event handler for MLWebView callbacks.  This structure must be initialized by calling #MLWebViewEventCallbacksInit
            /// before use.
            /// </summary>
            [StructLayout(LayoutKind.Sequential)]
            public struct EventCallbacks
            {
                /// <summary>
                /// Version of this struct
                /// </summary>
                public uint Version;

                /// <summary>
                /// User data passed to every callback.
                /// </summary>
                public IntPtr UserData;

                /// <summary>
                /// Called to notify when a resource will loadeded.
                /// </summary>
                public OnBeforeResourceLoadCallback OnBeforeResourceLoad;

                /// <summary>
                /// Called to notify load completion.
                /// </summary>
                public OnLoadEndCallback OnLoadEnd;

                /// <summary>
                /// Called if there was any error during loading.  These errors could be due to connectivity, certificate errors etc.
                /// </summary>
                public OnLoadErrorCallback OnLoadError;

                /// <summary>
                /// Called when there is any certificate error.
                /// </summary>
                public OnCertificateErrorCallback OnCertificateError;

                /// <summary>
                /// Called when user selects an input field.
                /// </summary>
                public OnShowKeyboardCallback OnShowKeyboard;

                /// <summary>
                /// Called when user deselects an input field and the keyboard should be dismissed.
                /// </summary>
                public OnKeyboardDismissCallback OnKeyboardDismiss;

                /// <summary>
                /// Called when webview is destroyed.
                /// </summary>
                public OnDestroyCallback OnDestroy;

                /// <summary>
                /// This callback is used to pass notify user of service connection.
                /// </summary>
                public OnServiceConnectedCallback OnServiceConnected;

                /// <summary>
                /// This callback is used to notify user that service is disconnect.
                /// </summary>
                public OnServiceDisconnectedCallback OnServiceDisconnected;

                /// <summary>
                /// Create and return an initialized version of this struct.
                /// <param name="userData">Pointer to user data to be used to reference the originating web view tab</param>
                /// </summary>
                /// <returns>A new instance of this struct.</returns>
                public static EventCallbacks Create(GCHandle gcHandle)
                {
                    return new EventCallbacks()
                    {
                        Version = 1u,
                        UserData = GCHandle.ToIntPtr(gcHandle),
                        OnBeforeResourceLoad = HandleOnBeforeResourceLoad,
                        OnLoadEnd = HandleOnLoadEnd,
                        OnLoadError = HandleOnLoadError,
                        OnCertificateError = HandleOnCertificateError,
                        OnShowKeyboard = HandleOnShowKeyboard,
                        OnKeyboardDismiss = HandleOnKeyboardDismiss,
                        OnDestroy = HandleOnDestroy,
                        OnServiceConnected = HandleServiceConnected,
                        OnServiceDisconnected = HandleServiceDisconnected
                    };
                }
            };

            /// <summary>
            /// Returns pointer to JavaVM that is required by WebView API.
            /// </summary>
            /// <returns>pointer to JavaVM</returns>
            [DllImport(CUtilsDLL, CallingConvention = CallingConvention.Cdecl)]
            public static extern IntPtr GetJavaVM();

            /// <summary>
            /// Returns pointer to App Context that is required by WebView API.
            /// </summary>
            /// <returns>pointer to JavaVM</returns>
            [DllImport(CUtilsDLL, CallingConvention = CallingConvention.Cdecl)]
            public static extern IntPtr GetAppContext();

            /// <summary>
            /// Create a MLWebView. The MLWebView will be ready to use once this function returns with MLResult_OK.
            /// </summary>
            /// <param name="handle">Handle that is ready to use with all other MLWebView API calls.</param>
            /// <param name="settings">The initialization paramaters for the webview.</param>
            /// <returns>MLResult.Code.Ok if the MLWebView is ready for use.</returns>
            /// <returns>MLResult.Code.UnspecifiedFailure if Unable to create the MLWebView.</returns>
            /// <returns>MLResult.Code.InvalidParam if the parameter was null pointer.</returns>
            /// <returns>MLResult.Code.PermissionDenied its missing the permission(s).</returns>
            [DllImport(MLWebViewDll, CallingConvention = CallingConvention.Cdecl)]
            public static extern MLResult.Code MLWebViewCreate(out ulong handle, ref Settings settings);

            /// <summary>
            /// Destroy a MLWebView. The MLWebView will be terminated by this function call and the handle shall no longer be valid.
            /// </summary>
            /// <param name="handle"></param>
            /// <returns>MLResult.Code.Ok if was destroyed successfully.</returns>
            /// <returns>MLResult.Code.UnspecifiedFailure if an error occurred destroying the MLWebView.</returns>
            /// <returns>MLResult.Code.InvalidParam if its unable to find the specified MLWebView handle.</returns>
            [DllImport(MLWebViewDll, CallingConvention = CallingConvention.Cdecl)]
            public static extern MLResult.Code MLWebViewDestroy(ulong handle);

            /// <summary>
            /// Specify the event handler for an MLWebView.
            /// </summary>
            /// <param name="handle">The MLWebView to link the event handler.</param>
            /// <param name="callbacks">The event handler to link to the MLWebView.</param>
            /// <param name="userData">Data that will be passed to your event handler when triggered.</param>
            /// <returns>MLResult.Code.Ok if event handler was set.</returns>
            /// <returns>MLResult.Code.InvalidParam if its unable to find the specified MLWebView handle.</returns>
            [DllImport(MLWebViewDll, CallingConvention = CallingConvention.Cdecl)]
            public static extern MLResult.Code MLWebViewSetEventCallbacks(ulong handle, EventCallbacks callbacks, IntPtr userData);

            /// <summary>
            /// Acquires next available frame buffer for rendering.
            /// </summary>
            /// <param name="handle">The webview being accessed.</param>
            /// <param name="bufferHandle">The buffer that is ready to be rendered. A value of 0 indicates no frame is ready.</param>
            /// <returns>MLResult.Code.Ok if frame is ready.</returns>
            /// <returns>MLResult.Code.InvalidParam if its nable to find the specified MLWebView handle..</returns>
            [DllImport(MLWebViewDll, CallingConvention = CallingConvention.Cdecl)]
            public static extern MLResult.Code MLWebViewAcquireNextAvailableFrame(ulong handle, out IntPtr hwBuffer);

            /// <summary>
            /// Release a frame acquired by #MLWebViewAcquireNextAvailableFrame.
            /// </summary>
            /// <param name="handle">The webview being accessed.<</param>
            /// <param name="bufferHandle">The frame being released.</param>
            /// <returns>MLResult.Code.Ok if frame was successfully released.</returns>
            /// <returns>MLResult.Code.UnspecifiedFailure if error occurred releasing the frame.</returns>
            /// <returns>MLResult.Code.InvalidParam if its unable to find the specified MLWebView handle.</returns>
            [DllImport(MLWebViewDll, CallingConvention = CallingConvention.Cdecl)]
            public static extern MLResult.Code MLWebViewReleaseFrame(ulong handle, IntPtr hwBuffer);

            /// <summary>
            /// Go to a URL with the specified MLWebView.  Note that success with this call only indicates that a load will be
            /// attempted.  Caller can be notified about issues loading the URL via the event handler on_load_error.
            /// </summary>
            /// <param name="handle">The webview being accessed.</param>
            /// <param name="url">URL that will be loaded.</param>
            /// <returns>MLResult.Code.Ok if WebView is attempting to load the specified URL.</returns>
            /// <returns>MLResult.Code.InvalidParam if its unable to find the specified MLWebView handle.</returns>
            [DllImport(MLWebViewDll, CallingConvention = CallingConvention.Cdecl)]
            public static extern MLResult.Code MLWebViewGoTo(ulong handle, [MarshalAs(UnmanagedType.LPStr)] string url);

            /// <summary>
            /// Trigger a "Back" action in the MLWebView.  Query MLWebViewCanGoBack before calling this method.  If there is no valid
            /// page to go back to, this method will be no-op.
            /// </summary>
            /// <param name="handle">The webview being accessed.</param>
            /// <returns>MLResult.Code.Ok if WebView Back action was initiated or cannot go back any further.</returns>
            /// <returns>MLResult.Code.InvalidParam if its unable to find the specified MLWebView handle.</returns>
            [DllImport(MLWebViewDll, CallingConvention = CallingConvention.Cdecl)]
            public static extern MLResult.Code MLWebViewGoBack(ulong handle);

            /// <summary>
            /// Trigger a "Forward" action in the MLWebView.  Query #MLWebViewCanGoForward before calling this method.  If there is no
            /// valid page to go forward to, this method will be no-op.
            /// </summary>
            /// <param name="handle">The webview being accessed.</param>
            /// <returns>MLResult.Code.Ok if WebView Forward action was initiated or cannot go forward any further.</returns>
            /// <returns>MLResult.Code.InvalidParam if its unable to find the specified MLWebView handle.</returns>
            [DllImport(MLWebViewDll, CallingConvention = CallingConvention.Cdecl)]
            public static extern MLResult.Code MLWebViewGoForward(ulong handle);

            /// <summary>
            /// Trigger a "Reload" action in the MLWebView.
            /// </summary>
            /// <param name="handle">The webview being accessed.</param>
            /// <returns>MLResult.Code.Ok if WebView Reload action was initiated.</returns>
            /// <returns>MLResult.Code.InvalidParam if its unable to find the specified MLWebView handle.</returns>
            [DllImport(MLWebViewDll, CallingConvention = CallingConvention.Cdecl)]
            public static extern MLResult.Code MLWebViewReload(ulong handle);

            /// <summary>
            /// Get the current URL. Set outUrl to NULL to get the length of the current URL.
            /// </summary>
            /// <param name="handle">The webview being accessed.</param>
            /// <param name="inoutSize">The size that has been allocated in outUrl to hold the URL. This will be set to the actual length of URL if inoutSize and size needed for the URL are different.</param>
            /// <param name="outUrl">The URL up to inoutSize of characters.</param>
            /// <returns>MLResult.Code.Ok if WebView URL was acquired.</returns>
            /// <returns>MLResult.Code.InvalidParam if its unable to find the specified MLWebView handle.</returns>
            [DllImport(MLWebViewDll, CallingConvention = CallingConvention.Cdecl)]
            public static extern MLResult.Code MLWebViewGetUrl(ulong handle, out uint inoutSize, IntPtr outUrl);

            /// <summary>
            /// Checks if the "Back" action is currently valid.
            /// </summary>
            /// <param name="handle">The webview being accessed.</param>
            /// <param name="outCanGoBack">True if "Back" has a valid page to go to.</param>
            /// <returns>MLResult.Code.Ok if status of going "Back" was acquired.</returns>
            /// <returns>MLResult.Code.InvalidParam if its unable to find the specified MLWebView handle.</returns>
            [DllImport(MLWebViewDll, CallingConvention = CallingConvention.Cdecl)]
            public static extern MLResult.Code MLWebViewCanGoBack(ulong handle, [MarshalAs(UnmanagedType.I1)] out bool outCanGoBack);

            /// <summary>
            /// Checks if the "Forward" action is currently valid.
            /// </summary>
            /// <param name="handle">The webview being accessed.</param>
            /// <param name="outCanGoForward">True if "Forward" has a valid page to go to.</param>
            /// <returns>MLResult.Code.Ok if status of going "Forward" was acquired.</returns>
            /// <returns>MLResult.Code.InvalidParam if its unable to find the specified MLWebView handle.</returns>
            [DllImport(MLWebViewDll, CallingConvention = CallingConvention.Cdecl)]
            public static extern MLResult.Code MLWebViewCanGoForward(ulong handle, [MarshalAs(UnmanagedType.I1)] out bool outCanGoForward);

            /// <summary>
            /// Moves the WebView mouse.
            /// </summary>
            /// <param name="handle">The webview being accessed.</param>
            /// <param name="cursorState">Information about the mouse movement.</param>
            /// <returns>MLResult.Code.Ok if internal mouse was moved.</returns>
            /// <returns>MLResult.Code.InvalidParam if its unable to find the specified MLWebView handle.</returns>
            [DllImport(MLWebViewDll, CallingConvention = CallingConvention.Cdecl)]

            public static extern MLResult.Code MLWebViewInjectMouseMove(ulong handle, ref CursorState cursorState);

            /// <summary>
            /// Sends a mouse button down/pressed event on a specific location on screen.
            /// </summary>
            /// <param name="handle">The webview being accessed.</param>
            /// <param name="cursorState">Information about the mouse button event.</param>
            /// <param name="buttonType">The mouse button being pressed.</param>
            /// <returns>MLResult.Code.Ok if successful.</returns>
            /// <returns>MLResult.Code.InvalidParam if its unable to find the specified MLWebView handle.</returns>
            [DllImport(MLWebViewDll, CallingConvention = CallingConvention.Cdecl)]
            public static extern MLResult.Code MLWebViewInjectMouseButtonDown(ulong handle, ref CursorState cursorState, MouseButtonType buttonType);

            /// <summary>
            /// Sends a mouse button up/released event on a specific location on screen.
            /// </summary>
            /// <param name="handle">The webview being accessed.</param>
            /// <param name="cursorState">Information about the mouse button event.</param>
            /// <param name="buttonType">The mouse button being released.</param>
            /// <returns>MLResult.Code.Ok if successful.</returns>
            /// <returns>MLResult.Code.InvalidParam if its unable to find the specified MLWebView handle.</returns>
            [DllImport(MLWebViewDll, CallingConvention = CallingConvention.Cdecl)]
            public static extern MLResult.Code MLWebViewInjectMouseButtonUp(ulong handle, ref CursorState cursorState, MouseButtonType buttonType);

            /// <summary>
            /// Sends a printable char keyboard event to MLWebView.
            /// </summary>
            /// <param name="handle">The webview being accessed.</param>
            /// <param name="charUtf32">printable char utf code</param>
            /// <returns>MLResult.Code.Ok if key event was injected.</returns>
            /// <returns>MLResult.Code.InvalidParam if its unable to find the specified MLWebView handle.</returns>
            [DllImport(MLWebViewDll, CallingConvention = CallingConvention.Cdecl)]
            public static extern MLResult.Code MLWebViewInjectChar(ulong handle, uint charUtf32);

            /// <summary>
            /// Sends a key down/pressed event to MLWebView.
            /// </summary>
            /// <param name="handle">The webview being accessed.</param>
            /// <param name="keyCode">MLWebView.KeyCode.</param>
            /// <param name="modifierMask">Should be one or combination of MLWebView.EventFlags.</param>
            /// <returns>MLResult.Code.Ok if key event was injected.</returns>
            /// <returns>MLResult.Code.InvalidParam if its unable to find the specified MLWebView handle.</returns>
            [DllImport(MLWebViewDll, CallingConvention = CallingConvention.Cdecl)]
            public static extern MLResult.Code MLWebViewInjectKeyDown(ulong handle, MLWebView.KeyCode keyCode, uint modifierMask);

            /// <summary>
            /// Sends a key up/release event to MLWebView.
            /// </summary>
            /// <param name="handle">The webview being accessed.</param>
            /// <param name="keyCode">MLWebView.KeyCode.</param>
            /// <param name="modifierMask">Should be one or combination of MLWebView.EventFlags.</param>
            /// <returns>MLResult.Code.Ok if key event was injected.</returns>
            /// <returns>MLResult.Code.InvalidParam if its unable to find the specified MLWebView handle.</returns>
            [DllImport(MLWebViewDll, CallingConvention = CallingConvention.Cdecl)]
            public static extern MLResult.Code MLWebViewInjectKeyUp(ulong handle, MLWebView.KeyCode keyCode, uint modifierMask);

            /// <summary>
            /// Reset zoom level to 1.0.
            /// </summary>
            /// <param name="handle">The webview being accessed.</param>
            /// <returns>MLResult.Code.Ok if MLWebView zoom was reset.</returns>
            /// <returns>MLResult.Code.InvalidParam if its unable to find the specified MLWebView handle.</returns>
            /// <returns>MLResult.Code.UnspecifiedFailure if it failed to reset zoom due to an internal error.</returns>
            [DllImport(MLWebViewDll, CallingConvention = CallingConvention.Cdecl)]
            public static extern MLResult.Code MLWebViewResetZoom(ulong handle);

            /// <summary>
            /// Zoom in one level.
            /// </summary>
            /// <param name="handle">The webview being accessed.</param>
            /// <returns>MLResult.Code.Ok if MLWebView zoomed in.</returns>
            /// <returns>MLResult.Code.InvalidParam if its unable to find the specified MLWebView handle.</returns>
            /// <returns>MLResult.Code.WebViewResultZoomLimitReached if cannot zoom in any further.</returns>
            /// <returns>MLResult.Code.UnspecifiedFailure if it failed to reset zoom due to an internal error.</returns>
            [DllImport(MLWebViewDll, CallingConvention = CallingConvention.Cdecl)]
            public static extern MLResult.Code MLWebViewZoomIn(ulong handle);

            /// <summary>
            /// Zoom out one level.
            /// </summary>
            /// <param name="handle">The webview being accessed.</param>
            /// <returns>MLResult.Code.Ok if MLWebView zoomed out.</returns>
            /// <returns>MLResult.Code.InvalidParam if its unable to find the specified MLWebView handle.</returns>
            /// <returns>MLResult.Code.WebViewResultZoomLimitReached if cannot zoom out any further.</returns>
            /// <returns>MLResult.Code.UnspecifiedFailure if it failed to reset zoom due to an internal error.</returns>
            [DllImport(MLWebViewDll, CallingConvention = CallingConvention.Cdecl)]
            public static extern MLResult.Code MLWebViewZoomOut(ulong handle);

            /// <summary>
            /// Get the current zoom factor.  The default zoom factor is 1.0.
            /// </summary>
            /// <param name="handle">The webview being accessed.</param>
            /// <param name="outZoomFactor">Current numeric value for zoom factor.</param>
            /// <returns>MLResult.Code.Ok if outZoomFactor parameter was updated with the current zoom value.</returns>
            /// <returns>MLResult.Code.InvalidParam if its unable to find the specified MLWebView handle.</returns>
            /// <returns>MLResult.Code.UnspecifiedFailure if failed to get the zoom factor due to an internal error.</returns>
            [DllImport(MLWebViewDll, CallingConvention = CallingConvention.Cdecl)]
            public static extern MLResult.Code MLWebViewGetZoomFactor(ulong handle, out double outZoomFactor);

            /// <summary>
            /// Triggers a mouse "Scroll" event.
            /// </summary>
            /// <param name="handle">The webview being accessed.</param>
            /// <param name="xPixels">The number of pixels to scroll on the x axis.</param>
            /// <param name="yPixels">The number of pixels to scroll on the y axis.</param>
            /// <returns>MLResult.Code.Ok if MLWebView was scrolled.</returns>
            /// <returns>MLResult.Code.InvalidParam if its unable to find the specified MLWebView handle.</returns>
            [DllImport(MLWebViewDll, CallingConvention = CallingConvention.Cdecl)]
            public static extern MLResult.Code MLWebViewScrollBy(ulong handle, uint xPixels, uint yPixels);

            /// <summary>
            /// Get the entire scrollable size of the webview. 
            /// This should be typically called afer HandleOnLoadEnd to determine the scollable size
            /// of the main frame of the loaded page.Some pages might dynamically resize and this should be called
            /// before each frame draw to correctly determine the scrollable size of the webview.
            /// </summary>
            /// <param name="handle">The webview being accessed.</param>
            /// <param name="width">The number representing the entire width of the webview, in pixels.</param>
            /// <param name="height">The number representing the entire height of the webview, in pixels.</param>
            /// <returns></returns>
            [DllImport(MLWebViewDll, CallingConvention = CallingConvention.Cdecl)]
            public static extern MLResult.Code MLWebViewGetScrollSize(ulong handle, out int width, out int height);

            /// <summary>
            /// Get the scroll offset of the webview.
            /// </summary>
            /// <param name="handle">The webview being accessed.</param>
            /// <param name="x">The number representing the horizontal offset of the webview, in pixels.</param>
            /// <param name="y">The number representing the vertical offset of the webview, in pixels.</param>
            /// <returns></returns>
            [DllImport(MLWebViewDll, CallingConvention = CallingConvention.Cdecl)]
            public static extern MLResult.Code MLWebViewGetScrollOffset(ulong handle, out int x, out int y);

            /// <summary>
            /// Retrieves the 4x4 texture coordinate transform matrix used by all MLWebView frames.  This transform matrix maps 2D
            /// homogeneous texture coordinates of the form (s, t, 0, 1) with s and t in the inclusive range [0, 1] to the texture coordinate
            /// that should be used to sample that location from the texture.  Sampling the texture outside of the range of this
            /// transform is undefined.  The matrix is stored in column-major order so that it may be passed directly to OpenGL ES via the
            /// glLoadMatrixf or glUniformMatrix4fv functions.
            /// </summary>
            /// <param name="handle">The webview being accessed.</param>
            /// <param name="outMtx">The retrieved matrix.</param>
            /// <returns>MLResult.Code.Ok if constant matrix was retrieved.</returns>
            [DllImport(MLWebViewDll, CallingConvention = CallingConvention.Cdecl)]
            public static extern MLResult.Code MLWebViewGetFrameTransformMatrix(ulong handle, out MLMat4f outMtx);

            /// <summary>
            /// Remove all webview cookies.
            /// </summary>
            /// <param name="handle">The webview being accessed.</param>
            /// <returns>MLResult.Code.Ok if all cookies removed successfully.</returns>
            /// <returns>MLResult.Code.InvalidParam if its unable to find the specified MLWebView handle.</returns>
            /// <returns>MLResult.Code.UnspecifiedFailure if removing all cookies failed due to an internal error.</returns>
            [DllImport(MLWebViewDll, CallingConvention = CallingConvention.Cdecl)]
            public static extern MLResult.Code MLWebViewRemoveAllCookies(ulong handle);

            /// <summary>
            /// Clear the webview cache.
            /// </summary>
            /// <param name="handle">The webview being accessed.</param>
            /// <returns>MLResult.Code.Ok if cache cleared successfully</returns>
            /// <returns>MLResult.Code.InvalidParam if its unable to find the specified MLWebView handle.</returns>
            /// <returns>MLResult.Code.UnspecifiedFailure if clearing cache failed due to an internal error.</returns>
            [DllImport(MLWebViewDll, CallingConvention = CallingConvention.Cdecl)]
            public static extern MLResult.Code MLWebViewClearCache(ulong handle);


            /// <summary>
            /// Callback from the native code to notify when a resource will loadeded.
            /// </summary>
            /// <param name="resourceUrl">The url of the resource about to be loaded.</param>
            /// <param name="userData">User defined data, typically a pointer to the originating MLWebView object.</param>
            [AOT.MonoPInvokeCallback(typeof(OnBeforeResourceLoadCallback))]
            private static void HandleOnBeforeResourceLoad([MarshalAs(UnmanagedType.LPStr)] string resourceUrl, IntPtr userData)
            {
                GCHandle gcHandle = GCHandle.FromIntPtr(userData);
                MLWebView webView = gcHandle.Target as MLWebView;
                MLThreadDispatch.Call(webView, resourceUrl, webView.OnBeforeResourceLoaded);
            }

            /// <summary>
            /// Callback from the native code to notify load completion.
            /// </summary>
            /// <param name="isMainFrame">Whether this event was for the main frame.</param>
            /// <param name="httpStatusCode">The standard http status code.</param>
            /// <param name="userData">User defined data, typically a pointer to the originating MLWebView object.</param>
            [AOT.MonoPInvokeCallback(typeof(OnLoadEndCallback))]
            private static void HandleOnLoadEnd([MarshalAs(UnmanagedType.I1)] bool isMainFrame, int httpStatusCode, IntPtr userData)
            {
                GCHandle gcHandle = GCHandle.FromIntPtr(userData);
                MLWebView webView = gcHandle.Target as MLWebView;
                MLThreadDispatch.Call(webView, isMainFrame, httpStatusCode, webView.OnLoadEnded);
            }

            /// <summary>
            /// Callback from the native code to notify that there was an error during loading.
            /// </summary>
            /// <param name="isMainFrame">If this event was for the main frame.</param>
            /// <param name="httpStatusCode">Http status code for the URL load failure.</param>
            /// <param name="errorStr">The stringified version of the error code.</param>
            /// <param name="failedUrl">The url that caused the load error.</param>
            /// <param name="userData">User defined data, typically a pointer to the originating MLWebView object.</param>
            [AOT.MonoPInvokeCallback(typeof(OnLoadErrorCallback))]
            private static void HandleOnLoadError([MarshalAs(UnmanagedType.I1)] bool isMainFrame, int httpStatusCode, [MarshalAs(UnmanagedType.LPStr)] string errorStr, [MarshalAs(UnmanagedType.LPStr)] string failedUrl, IntPtr userData)
            {
                GCHandle gcHandle = GCHandle.FromIntPtr(userData);
                MLWebView webView = gcHandle.Target as MLWebView;
                MLThreadDispatch.Call(webView, isMainFrame, httpStatusCode, errorStr, failedUrl, webView.OnErrorLoaded);
            }

            /// <summary>
            /// Callback from the native code whenever there is any certificate error.
            /// </summary>
            /// <param name="errorCode">Error code for ssl error.</param>
            /// <param name="url">The url associated to the certificate error.</param>
            /// <param name="errorMessage">Certificate error short description.</param>
            /// <param name="details">Certificate error details.</param>
            /// <param name="userData">User defined data, typically a pointer to the originating MLWebView object.</param>
            [AOT.MonoPInvokeCallback(typeof(OnCertificateErrorCallback))]
            private static bool HandleOnCertificateError(int errorCode, [MarshalAs(UnmanagedType.LPStr)] string url, [MarshalAs(UnmanagedType.LPStr)] string errorMessage, [MarshalAs(UnmanagedType.LPStr)] string details, IntPtr userData)
            {
                GCHandle gcHandle = GCHandle.FromIntPtr(userData);
                MLWebView webView = gcHandle.Target as MLWebView;
                MLThreadDispatch.Call(webView, errorCode, url, errorMessage, details, webView.IgnoreCertificateError, webView.OnCertificateErrorLoaded);
                return webView.IgnoreCertificateError;
            }

            /// <summary>
            /// Callback from the native code when user selects an input field.
            /// </summary>
            /// <param name="x">Horizontal position of the input field.</param>
            /// <param name="y">Vertical position of the input field.</param>
            /// <param name="width">Width of the input field.</param>
            /// <param name="height">Height of the input field.</param>
            /// <param name="textInputFlags">One or combination of TextInputFlags.</param>
            /// <param name="textInputType">One of TextInputType.</param>
            /// <param name="userData">User defined data, typically a pointer to the originating MLWebView object.</param>
            [AOT.MonoPInvokeCallback(typeof(OnShowKeyboardCallback))]
            private static void HandleOnShowKeyboard(int x, int y, int width, int height, TextInputFlags textInputFlags, TextInputType textInputType, IntPtr userData)
            {
                InputFieldData keyboardShowData = new InputFieldData()
                {
                    X = x,
                    Y = y,
                    Width = width,
                    Height = height,
                    TextInputFlags = textInputFlags,
                    TextInputType = textInputType
                };

                GCHandle gcHandle = GCHandle.FromIntPtr(userData);
                MLWebView webView = gcHandle.Target as MLWebView;
                MLThreadDispatch.Call(webView, keyboardShowData, webView.OnKeyboardShown);
            }

            /// <summary>
            /// Callback from the native code when user deselects an input field and the keyboard should be dismissed.
            /// </summary>
            /// <param name="userData">User defined data, typically a pointer to the originating MLWebView object.</param>
            [AOT.MonoPInvokeCallback(typeof(OnKeyboardDismissCallback))]
            private static void HandleOnKeyboardDismiss(IntPtr userData)
            {
                GCHandle gcHandle = GCHandle.FromIntPtr(userData);
                MLWebView webView = gcHandle.Target as MLWebView;
                MLThreadDispatch.Call(webView, webView.OnKeyboardDismissed);
            }

            /// <summary>
            /// Callback from the native code when webview is destroyed.
            /// </summary>
            /// <param name="userData">User defined data, typically a pointer to the originating MLWebView object.</param>
            [AOT.MonoPInvokeCallback(typeof(OnDestroyCallback))]
            private static void HandleOnDestroy(IntPtr userData)
            {
                GCHandle gcHandle = GCHandle.FromIntPtr(userData);
                MLWebView webView = gcHandle.Target as MLWebView;
                MLThreadDispatch.Call(webView, webView.OnWebViewDestroyed);
            }

            /// <summary>
            ///  Callback from the native code when Service is connected.
            /// </summary>
            /// <param name="userData">User defined data, typically a pointer to the originating MLWebView object.</param>
            [AOT.MonoPInvokeCallback(typeof(OnServiceConnectedCallback))]
            private static void HandleServiceConnected(IntPtr userData)
            {
                GCHandle gcHandle = GCHandle.FromIntPtr(userData);
                MLWebView webView = gcHandle.Target as MLWebView;
                MLThreadDispatch.Call(webView, webView.OnServiceConnected);
            }

            /// <summary>
            /// Callback from the native code when Service is disconnected.
            /// </summary>
            /// <param name="userData">User defined data, typically a pointer to the originating MLWebView object.</param>
            [AOT.MonoPInvokeCallback(typeof(OnServiceDisconnectedCallback))]
            private static void HandleServiceDisconnected(IntPtr userData)
            {
                GCHandle gcHandle = GCHandle.FromIntPtr(userData);
                MLWebView webView = gcHandle.Target as MLWebView;
                MLThreadDispatch.Call(webView, webView.OnServiceDisconnected);
            }
        }
    }
}

#endif