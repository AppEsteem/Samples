
### How to use AppEsteem's Self Regulating Client Library
**Pre requisites:** *Visualstudio_ 2010* and *later*

> Steps to download and link vendor application to SRCL library:

**Downloading SRCL library:**
1. Login to our portal [appesteem] <https://customer.appesteem.com/>
2. Download SRCL library.(SRCL can be downloaded after company and application is registered successfully with AppEsteem).
3. After successful download in provided zip file make sure you have following folders.
    * Bin (library files to be linked to the application) 
    * Example (Shows how to use library) 
    * Include (Library include files) 
    * Source (Source files to be included within the compilation)
4. Download seal from appesteem portal(or may be for now provided as part of zip file).

**Follow steps below to link SRCL to application.** 
1. Copy bin folder and add as an additional library for the linker.
* To do this go to Visual studio Solution Explorer, right click on your application and select properties -> Linker tab -> Additional Library directories and provide path for the "bin" folder
2. Copy srcl.h, lib_srcl_autolink.h to include folder. 
3. Copy srcl_init.cpp, registration.cpp (downloaded seal) to the source folder.  
4. If you are using precompiled headers in your project, configure the source/srcl_init.cpp file as not using precompiled headers.
* To do this go to Visual studio Solution Explorer right click on your application and select properties -> C++ tab -> Precompiled Headers -> Precompiled Header and select Not Using Precompiled Headers.

5. Add following macro to your code
  ==================================================   
  Begin of modifications to the original program in order to display notifications.
  ==================================================   

#define USE_APPESTEEM_SRCL 

#ifdef USE_APPESTEEM_SRCL 

>// Initialization helper.

>// Contains the appid, options and seal. Uses the notification function.

>//This include could be replaced by adding it separately to the compilation.

>// If added to compilation no modifications whatsoever are needed. 

#include "source/srcl_init.cpp"  
#endif 

============================================

End of modifications to the original program in order to display notifications.

============================================   

7. You can also check examples on how to use SRCL in your application.
8. Build application after finishing all the steps above.
