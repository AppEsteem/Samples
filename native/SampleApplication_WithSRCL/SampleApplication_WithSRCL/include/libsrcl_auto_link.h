// Modified from boost::config library. The original file contained
// the copyright below.

//  (C) Copyright John Maddock 2003.
//  Use, modification and distribution are subject to the
//  Boost Software License, Version 1.0. (See accompanying file
//  LICENSE_1_0.txt or copy at http://www.boost.org/LICENSE_1_0.txt)

 /*
  *   LOCATION:    see http://www.boost.org for most recent version.
  *   FILE         auto_link.hpp
  *   VERSION      see <boost/version.hpp>
  *   DESCRIPTION: Automatic library inclusion for Borland/Microsoft compilers.
  */

/*************************************************************************

USAGE:
~~~~~~

Before including this header you may define the following macro:

SRCL_LIB_DIAGNOSTIC: Optional: when set the header will print out the name
                     of the library selected (useful for debugging).

These macros will be undef'ed at the end of the header, further this header
has no include guards - so be sure to include it only once from your library!

Algorithm:
~~~~~~~~~~

Libraries for Microsoft compilers are automatically selected here,
the name of the lib is selected according to the following formula:

LIB_PREFIX
   + extract
   + LIB_TOOLSET
   + LIB_THREAD_OPT
   + LIB_RT_OPT

These are defined as:

LIB_PREFIX:     "lib" for static libraries otherwise "".

LIB_TOOLSET:    The compiler toolset name (71, 80 etc).

LIB_RT_OPT:     A suffix that indicates the runtime library used,
                contains one or more of the following letters:

                s      static runtime (dynamic if not present).
                d      debug build (release if not present).

***************************************************************************/

//
// Only include what follows for known and supported compilers:
//
#if defined(_MSC_VER) // Microsoft Visual C++

//
// error check:
//
#if defined(__MSVC_RUNTIME_CHECKS) && !defined(_DEBUG)
#  pragma message("Using the /RTC option without specifying a debug runtime will lead to linker errors")
#  pragma message("Hint: go to the code generation options and switch to one of the debugging runtimes")
#  error "Incompatible build options"
#endif
//
// select toolset:
//
#if (_MSC_VER == 1310)

   // vc71:
#  define LIB_TOOLSET "71"

#elif (_MSC_VER == 1400)

   // vc80:
#  define LIB_TOOLSET "80"

#elif (_MSC_VER == 1500)

   // vc90:
#  define LIB_TOOLSET "90"

#elif (_MSC_VER == 1600)

   // vc100:
#  define LIB_TOOLSET "100"

#elif (_MSC_VER == 1700)

   // vc110:
#  define LIB_TOOLSET "110"

#elif (_MSC_VER == 1800)

   // vc120:
#  define LIB_TOOLSET "120"

#elif (_MSC_VER == 1900)

   // vc140:
#  define LIB_TOOLSET "140"

#elif (_MSC_VER >= 1910)

   // vc141:
#  define LIB_TOOLSET "141"

#endif

//
// verify thread opt:
//
#if !defined(_MT) && !defined(__MT__)
#  error "Compiler threading support is not turned on. Please set the correct command line options for threading: either /MT /MTd /MD or /MDd"
#endif

#  ifdef _DLL

#    if defined(_DEBUG)
#       define LIB_RT_OPT "d"
#    else
#       define LIB_RT_OPT ""
#    endif

#  else

#    if defined(_DEBUG)
#       define LIB_RT_OPT "sd"
#    else
#       define LIB_RT_OPT "s"
#    endif

#  endif

//
// select linkage opt:
//
#define LIB_PREFIX "lib"

//
// select architecture:
//
#if (defined(_WIN64))
#  define LIB_ARCH "64"
#else
#  define LIB_ARCH ""
#endif

//
// now include the lib:
//
#if defined(LIB_PREFIX) \
      && defined(LIB_TOOLSET) \
      && defined(LIB_RT_OPT)

#if (_MANAGED == 1) || (_M_CEE == 1) || (defined SRCL_USE_AS_DLL) 
#  define W(x)          W_(x)
#  define W_(x)         L ## x
#  define N(x)          x
#  define SRCL_DLL_NAME_(t)  t("srcl") t(LIB_TOOLSET) t(LIB_RT_OPT) t(LIB_ARCH) t(".dll")
   const CHAR srcl_dll_name[] = SRCL_DLL_NAME_(N);
   const WCHAR wsrcl_dll_name[] = SRCL_DLL_NAME_(W);
#  undef W
#  undef W_
#  undef N
#  undef SRCL_DLL_NAME_
#  ifdef SRCL_LIB_DIAGNOSTIC
#    pragma message ("Using the dll: " "srcl" LIB_TOOLSET LIB_RT_OPT LIB_ARCH ".dll")
#  endif
#else
#  pragma comment(lib, LIB_PREFIX "srcl" LIB_TOOLSET LIB_RT_OPT LIB_ARCH ".lib")
#  ifdef SRCL_LIB_DIAGNOSTIC
#    pragma message ("Linking to lib file: " LIB_PREFIX "srcl" LIB_TOOLSET LIB_RT_OPT LIB_ARCH ".lib")
#  endif
#endif

#else
#  error "some required macros where not defined (internal logic error)."
#endif

#endif // _MSC_VER

//
// finally undef any macros we may have set:
//
#ifdef LIB_PREFIX
#  undef LIB_PREFIX
#endif
#if defined(LIB_TOOLSET)
#  undef LIB_TOOLSET
#endif
#if defined(LIB_RT_OPT)
#  undef LIB_RT_OPT
#endif
#undef LIB_ARCH
