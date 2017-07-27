# .NET SRCL Samples

This repository contains samples showing how to use the SRCL library from a managed C# application.
## Please follow instrustions below to link srcl to the application.
# 
_**Obtaining Seal from AppEsteem**_
1) Login to AppEsteem portal <https://customer.appesteem.com/>
2) Register your application. 
3) After successful registration seal will be provided from AppEsteem.

_**Downloading AppEsteem SRCL Library**_
1) From Visual Studio select menu Tools -> NuGet Package Manager ->
 Manage NuGet Package for Solution.
2) Select Browse and search for appesteem.You should be able to see AppEsteem.SRCL.CSharp listed.

 ![GitHub Logo](../media/CSharp_FindPackage.png)
3) Select the application to be linked to AppEsteem SRCL Library and click Install button.

    ![GitHub Logo](../media/csharp_InstallMessage_2.png)
5) A screen will be displayed to Review changes and to proceed with installation.

    ![GitHub Logo](../media/csharp_ReviewChanges_3.png)
6) Select OK button to continue.
7) We can now see the message on the screen that the installation is  finished.

8) From NuGet package solution we can now see the AppEsteem SRCL checked and Uninstall button enabled.

9) Modify your main function to be wrapped within the initialization object by doing:
      
        static int Main(string[] args)
        {
            using (var srcl = new SRCL.Init())
            {
                // Your main function code goes here...
            }
        }

    ![GitHub Logo](../media/includingsrclincode.png)    
10) Include the seal (AESEAL.json) obtained from AppEsteem to the application as Embedded Resource.

    ![GitHub Logo](../media/Embeddedseal.png)
11) In your projects you must correctly set the value of assembly ,version and company in Assembly Information. Alternatively you may use a version Win32 resource with valid "OriginalFilename", "ProductVersion" and "CompanyName". This information is required, either in the assembly configuration or Win32 resources.
12) Ensure the users of your program have the Visual C++ Redistributable run time components for the Visual Studio version you are using.
13) Build the application with seal included.
14) Application has to be build successfully.
15) Now when the application is run SRCL library should be able to send notifications/Telemetry to  the server.