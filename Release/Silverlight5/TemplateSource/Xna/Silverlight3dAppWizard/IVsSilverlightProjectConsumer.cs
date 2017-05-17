using System.Runtime.InteropServices;

namespace Silverlight3dAppWizard
{
    [Guid("52d400e0-6bf2-4e4e-ade3-79ec3047d34c"), InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
    public interface IVsSilverlightProject
    {
    }

    [Guid("07388F2A-8086-43eb-9C71-48B199C2EB8E"), InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
    public interface IVsSilverlightProjectConsumer
    {
        void LinkToSilverlightProject([MarshalAs(UnmanagedType.LPWStr)] [In] string pszDestFolderUrl, [MarshalAs(UnmanagedType.VariantBool)] [In] bool bEnableSilverlightDebugging, [MarshalAs(UnmanagedType.VariantBool)] [In] bool bUseCfgSpecificFolders, [MarshalAs(UnmanagedType.Interface)] [In] IVsSilverlightProject pSilverlightProject);
    }
}
