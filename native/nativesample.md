# Including an AppEsteem® seal in your Application - Native Code Example

## Prerequisites

1. AppEsteem Customer Portal account. See [here](../createaccount.md) for instructions
2. Register your company with AppEsteem. See [here](../registercompany.md) for instructions
3. Register an application with AppEsteem. See [here](../registerapplication.md) for instructions

## Obtaining Seal from AppEsteem

1. After the certification is successfully completed you can request the application-specific AppEsteem seal by clicking on the Shield icon (which now will be enabled). This will generate an email to AppEstseem.

    ![Application List](../media/requestElectronicSeal_1.png)

2. You will receive an email from AppEsteem requesting additional information about the application within 2 business days.

3. After receiving all the necessary information AppEsteem will provide you the seal AESEAL.json

## Including the AppEsteem Seal in your code

*Including seal to Native Code*

1. Include the seal (AESEAL.json) obtained from AppEsteem in your application resource files. To do this select project right click on Resource Files folder -> Select Add -> Existing Item -> Choose folder of AESEAL.json file -> select AESEAL.json file and click Add. File AESEAL.json will be added to Resource Files list.

2. In your applications resource file (xxx.rc), add the following line
   ```
   AESEAL RCDATA "AESEAL.json"
   ```
   **Do not** add a resource identifier for AESEAL in resource.h, this is a named resource, rather than using a specific identifier. This will embed the AESEAL.json file within an RCDATA section having the name AESEAL. This approach was chosen to avoid using specific identifiers defined for each application.

## Troubleshooting 
1. Error while building your application if you receive *"unexpected end of file while looking for while looking for precompiled header"*. 

    **Steps to resolve:** If you are using precompiled headers in your project, configure precompiled headers not using Precompiled Headers.  To do this go to Visual studio Solution Explorer right click on your application and select properties -> C++ tab -> Precompiled Headers -> Precompiled Header and select Not Using Precompiled Headers.

2. To verify if the seal is included correctly, download the VerifySeal tool from the samples repository. Run *VerifySeal* with the files where seal is included as shown in the example below.

   Ex: dotnet VerifySeal "C:\SealedFiles\SampleApplication.exe".