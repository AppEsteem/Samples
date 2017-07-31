# .NET SRCL Samples

This repository contains samples showing how to use the SRCL library from a managed C# application.
## Please follow instrustions below to link srcl to the application.
# 
_**Obtaining Seal from AppEsteem**_
1) Login to AppEsteem portal <https://customer.appesteem.com/>
2) Register your application with AppEsteem.
3) After successful registration request an AppEsteem seal by going to the “Manage my apps” page and clicking on the “Request Seal” button for desired application.
4) You will receive an email from AppEsteem requesting additional information about the application.
5) After receiving all the necessary information AppEsteem will provide you the seal (AESEAL.json).

_**Downloading AppEsteem SRCL Library**_
1) From Visual Studio select menu Tools -> NuGet Package Manager ->
 Manage NuGet Package for Solution.
2) Select Browse and search for appesteem.You should be able to see AppEsteem.SRCL.CSharp listed.

 ![GitHub Logo](../media/CSharp_FindPackage.png)

3) Select the application to be linked to AppEsteem SRCL Library and click Install button.

![GitHub Logo](../media/csharp_InstallMessage_2.png)

4) A screen will be displayed to Review changes and to proceed with installation.

![GitHub Logo](../media/csharp_ReviewChanges_3.png)

5) Select OK button to continue.

6) Message is displayed on the screen that the installation is  finished.

7) From NuGet package solution innstallation can be verified with AppEsteem SRCL checked and Uninstall button enabled.

8) Modify your main function to be wrapped within the initialization object by doing:
      
        static int Main(string[] args)
        {
            using (var srcl = new SRCL.Init())
            {
                // Your main function code goes here...
            }
        }

![GitHub Logo](../media/includingsrclincode.png)  

9) Include the seal (AESEAL.json) obtained from AppEsteem to the application as Embedded Resource.

![GitHub Logo](../media/Embeddedseal.png)

10) In your projects you must correctly set the value of assembly ,version and company in Assembly Information. Alternatively you may use a version Win32 resource with valid "OriginalFilename", "ProductVersion" and "CompanyName". This information is required, either in the assembly configuration or Win32 resources.

11) Ensure the users of your program have the Visual C++ Redistributable run time components for the Visual Studio version you are using.

12) Build the application with seal included.

13) Application has to be built successfully.

14) Now when the application is run SRCL library should be able to send notifications/telemetry to  the server.