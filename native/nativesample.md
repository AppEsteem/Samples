
### How to use AppEsteem's Self Regulating Client Library
**Pre requisites:** *Visualstudio_ 2010* and *later*

> Please follow instructions below to download and link vendor application to SRCL library:

_**Obtaining Seal from AppEsteem**_
1) Log in to the AppEsteem portal
* Go to <http://appesteem.com> and click on the 
"SIGN IN / SIGN UP" navigation menu link.

![GitHub Logo](../media/createAccount_1.png)

* If you already have an AppEsteem account, click the Sign In button and sign in with email and password you used to register with AppEsteem.

![GitHub Logo](../media/createAccount_2.png)

* If you do not have an AppEsteem account, create it:
    
    - Click on the Create Account link.
    - Enter your Email address and click the Get Verification Code button.

    ![GitHub Logo](../media/createAccount_3.png)

    - Enter the Verification Code you received by email and click the Verify button.

    ![GitHub Logo](../media/createAccount_4.png)

    - Enter Your Name, Password, Confirm Password and click the Submit button.

    ![GitHub Logo](../media/createAccount_5.png)

* Sign in with email and password you used to register with AppEsteem.

    ![GitHub Logo](../media/createAccount_6.png)

2) Register your company (you only need to do it once) :
* Click the REGISTER button under the "You do not have any Company registered" message. 

![GitHub Logo](../media/registerCompany1.png)

* Enter the company Name and Website. Click the Register button. Upon successful registration you will be navigated to the AppEsteem Portal page.

![GitHub Logo](../media/registerCompany2.png)

3) Sign the AppEsteem Certification Agreement:

* Under COMMIT click on “Agree to the Certification Agreement”.

![GitHub Logo](../media/agreeCertificationAgreement_1.png)

* Read the Certification Agreement by clicking on the Certification Agreement link, check the "I agree with the Certification Agreement" checkbox and click the Next button.

![GitHub Logo](../media/agreeCertificationAgreement_2.png)

* Upon successfully signed the Certification Agreement you will see the check mark next to the "Agree to the Certification Agreement” link on the Portal page.

![GitHub Logo](../media/agreeCertificationAgreement_3.png)

4) Register your application:
* Under COMMIT click on “Manage my apps”.

![GitHub Logo](../media/registerApplication_1.png)

* There are two ways to start the application registration:
  - If this is your first application you will see the message "No apps have been registered for Sample Company". Click the ADD NEW APP button.

  ![GitHub Logo](../media/registerApplication_2.png)

  - Othervise you will see the list of registered applications. Click the "+" button.

  ![GitHub Logo](../media/registerApplication_5.png)

* Enter Application Name and Application Type and click the Submit button.

![GitHub Logo](../media/registerApplication_3.png)

* After the successful registration you will see the "Success!" message. Click on the "Manage your applications" link.

![GitHub Logo](../media/registerApplication_4.png)

* The registered application will appear in the "Apps In Development" table.

![GitHub Logo](../media/registerApplication_5.png)

5) Request the application-specific AppEsteem seal by clicking on the “Request Seal” button. This will generate an email to AppEstseem.
6) You will receive an email from AppEsteem requesting additional information about the application within 2 business days.
7) After receiving all the necessary information AppEsteem will provide you the seal (registration.cpp).

_**Downloading AppEsteem SRCL Library**_
1) From visual studio select menu Tools -> NuGet Package Manager ->
 Manage Nuget Package for solution.
2) Select Browse and search for appesteem.You should be able to see AppEsteem.SRCL.CPP listed.

    ![GitHub Logo](../media/cpp_FindPackage_1.png)
3) Select the application to be linked to AppEsteem SRCL Library and click install button.

    ![GitHub Logo](../media/cpp_InstallPackage_2.png)
5) A screen will be displayed to Review changes and to proceed with installation.

    ![GitHub Logo](../media/cpp_ReviewChanges_3.png)
6) Select OK button to continue.
7) We can now see the message on the screen that the installation is  finished.

    ![GitHub Logo](../media/cpp_InstallMessage_4.png)
8) From NuGet package solution we can now see the AppEsteem SRCL checked and uninstall button enabled.

    ![GitHub Logo](../media/cpp_InstallVerification_5.png)
9) Include the seal(registration.cpp) obtained from AppEsteem to the application source files.
10) Build the application with seal included.
11) Now when the application is run SRCL library should be able to send notifications/Telemetry to  the server.

*Known issues:*

 1) Error while building your application :
"unexpected end of file while looking for while looking for precompiled header". 

    *Steps to resolve:* If you are using precompiled headers in your project, configure precompiled headers not using Precompiled Headers.
 _To do this go to Visual studio Solution Explorer right click on your application and select properties -> C++ tab -> Precompiled Headers -> Precompiled Header and select Not Using Precompiled Headers._

 **Steps to uninstall SRCL Library**

1) From visual studio select menu Tools -> NuGet Package Manager ->
 Manage Nuget Package for solution.

2) Select Installed option and then select   AppEsteem.SRCL.CPP listed. 
    
3) Select the application linked to AppEsteem SRCL Library and click uninstall button.

    ![GitHub Logo](../media/Uninstall_SRCL_CPP.png)
    
4) A screen will be displayed to Review changes and to proceed with uninstallation.

    ![GitHub Logo](../media/Uninstall_Review.png)
6) Select OK button to continue.
7) We can now see the message on the screen that the uninstallation is  finished.

    ![GitHub Logo](../media/Uninstall_Confirmation.png)