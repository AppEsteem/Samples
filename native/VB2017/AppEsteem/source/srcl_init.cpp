// Message for the user: Do not modify this file!
// This is an example of a file provided by AppEsteem

#include <windows.h>
#include "../include/srcl.h"
#include "../include/libsrcl_auto_link.h"

// Include the registration
//#include "registration.cpp"

#pragma comment(lib, "bcrypt.lib")
#pragma comment(lib, "winhttp.lib")
#pragma comment(lib, "netapi32.lib")
#pragma comment(lib, "ws2_32.lib")

#ifdef __cplusplus

namespace {

// simple example class
struct initializer
{
    initializer()
    {
        // call dll initialization funtions
#if (_MANAGED == 1) || (_M_CEE == 1) || (defined SRCL_USE_AS_DLL)  
        hdll = ::LoadLibraryW(wsrcl_dll_name);
        if (hdll)
        {
            BOOL(WINAPI *srcl_init)();
            (FARPROC&)srcl_init = ::GetProcAddress(hdll, "srcl_1");
            if (srcl_init)
                srcl_init();
        }
#else  
        srcl_init();
#endif  
    }
    ~initializer()
    {
        // call dll uninitialization funtions
#if (_MANAGED == 1) || (_M_CEE == 1) || (defined SRCL_USE_AS_DLL) 
        if (hdll)
        {
            VOID(WINAPI *srcl_term)();
            (FARPROC&)srcl_term = ::GetProcAddress(hdll, "srcl_2");
            if (srcl_term)
                srcl_term();
            FreeLibrary(hdll);
            hdll = NULL;
        }
#else  
        srcl_term();
#endif  
    }

#if (_MANAGED == 1) || (_M_CEE == 1) || (defined SRCL_USE_AS_DLL) 
    static HINSTANCE hdll;
#endif
};

#if (_MANAGED == 1) || (_M_CEE == 1) || (defined SRCL_USE_AS_DLL) 
HINSTANCE initializer::hdll = NULL;
#endif

initializer init;

}

#else

void __srlc_init()
{
    // call dll initialization funtions
    srcl_init();
}

void __srlc_uninit()
{
    // call dll uninitialization funtions
    srcl_term();
}

typedef void cb_4875638478(void);

#pragma data_seg(".CRT$XIU")
static cb_4875638478 *autostart[] = { __srlc_init };

#pragma data_seg(".CRT$XPU")
static cb_4875638478 *autoexit[] = { __srlc_uninit };

#pragma data_seg()    /* reset data-segment */

#endif
