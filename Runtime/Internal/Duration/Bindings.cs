using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using UnityEngine;
using System.Runtime.InteropServices;



namespace TapTap.Common.Internal {
  public class Bindings{
#if UNITY_IOS
  public const string DLL_NAME = "__Internal";
// #elif UNITY_STANDALONE_OSX || UNITY_EDITOR_OSX
//   public const string DLL_NAME = "bindings-csharp";
#elif UNITY_ANDROID
  public const string DLL_NAME = "tapsdkcore";
#endif

#if UNITY_IOS || UNITY_ANDROID
  [global::System.Runtime.InteropServices.DllImport(Bindings.DLL_NAME, CharSet =CharSet.Ansi, CallingConvention = CallingConvention.Cdecl)]
  public static extern void TdkOnAppStarted([MarshalAs( UnmanagedType.LPStr )]string data);

  [global::System.Runtime.InteropServices.DllImport(Bindings.DLL_NAME, CharSet =CharSet.Ansi, CallingConvention = CallingConvention.Cdecl)]
  public static extern void TdkOnAppStopped();

  [global::System.Runtime.InteropServices.DllImport(Bindings.DLL_NAME, CharSet =CharSet.Ansi, CallingConvention = CallingConvention.Cdecl)]
  public static extern void TdkOnLogin([MarshalAs( UnmanagedType.LPStr )]string data);

  [global::System.Runtime.InteropServices.DllImport(Bindings.DLL_NAME, CharSet =CharSet.Ansi, CallingConvention = CallingConvention.Cdecl)]
  public static extern void TdkOnLogout();

  [global::System.Runtime.InteropServices.DllImport(Bindings.DLL_NAME, CharSet =CharSet.Ansi, CallingConvention = CallingConvention.Cdecl)]
  public static extern void TdkOnForeground();

  [global::System.Runtime.InteropServices.DllImport(Bindings.DLL_NAME, CharSet =CharSet.Ansi, CallingConvention = CallingConvention.Cdecl)]
  public static extern void TdkOnBackground();

  [global::System.Runtime.InteropServices.DllImport(Bindings.DLL_NAME, CharSet =CharSet.Ansi, CallingConvention = CallingConvention.Cdecl)]
  public static extern void TdkSetExtraParams([MarshalAs( UnmanagedType.LPStr )]string data);


#endif
}

  

}