# Including an AppEsteem® seal in your Application - Managed Code Example

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

1. After requesting the seal from AppEsteem you would be receiving AESEAL.json file for integration to Managed and Native code

2. Add the seal AESEAL.json file obtained from AppEsteem to the application as Embedded Resource. To do this select project right click -> go to properties -> select Resources tab -> from Add Resource choose Add Existing File and Select AESEAL.json File

    ![Visual Studio Add Seal](../media/Embeddedseal_2.png)
   
3. Now from AESEAL.json file properties select the Build Action as Embedded Resource

    ![Visual Studio Explorer](../media/EmbeddedSeal_3.png)

4. If you are obfuscating your code please make sure to exclude sealAESEAL.json file from obfuscation.

5. To verify if the seal is included correctly, download the tool "VerifySeal" from the sample repository

   Run "VerifySeal.exe" with the files where seal is included as shown in the example below.

     Ex: dotnet VerifySeal.exe "C:\SealedFiles\SampleApplication.exe"