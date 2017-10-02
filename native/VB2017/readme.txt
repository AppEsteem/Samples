This is an example of sealing and applying a taggant to an executable file.

Content:
1. Root folder:
   Contains a hello world project that simulates some user project.
2. AppEsteem folder:
   Files distributed by AppEsteem to users:
   a. The seal (registration.cpp and SampleAp.json)
   b. the taggant (SampleAp.ae).
   c. Root certificates to validate tagged files (IEEErootCA.pem and tsroot.pem)
   d. Source and libraries needed to seal (folders include, lib and source) 
3. Toos folder:
   a. show_seal.exe: AppEsteem tool to show seal content. Run without parameters to see the options.
   b. SignTool.exe: Taggant library tool to add taggants to executables. Run without parameters to see the options.
   c. SSVTest.exe: Taggant library tool to verify taggants added to executables. Run without parameters to see the options.
4. Executables folder:
   Contains the executable resulting from each step of the sealing/tagging/signing process.
   a. SampleApOriginal.exe: The original compiled file from the hello world project
   b. SampleApSealed.exe: The sealed file after following sealing instructions in the documentation
   c. SampleApTagged.exe: The sealed file with a taggant applied using the signing tool
   d. SampleApSigned.exe: The tagged file signed by the original owner

Commands:
1. To generate the sealed file just compile the project release version
2. To apply the taggant to the sealed file:
   SignTool.exe SampleApSealed.exe AppEsteem\SampleAp.ae  -r:AppEsteem\IEEErootCA.pem
3. To verify the taggant in either the tagged file or the signed one: 
   SSVTest.exe AppEsteem\IEEErootCA.pem AppEsteem\tsroot.pem Executables
   where executables is a folder containing the files to validate

Notes:
Both SignTool and SSVTest are example tools in the Taggants Library. They are no meant to be directly used.
The Signing tool must be personalized in a private version to add specific AppEsteem data to the taggants, such as contributor, packer id, etc.
The SSVTest tool has the purpose of showing validation functionality. Real life validation code should be incorporated in security vendor code.
