
#pragma once
#ifndef _SRCL_H_
#define _SRCL_H_

#ifdef __cplusplus
extern "C" {
#endif

/* external interfaces */

/* initialize srcl */
BOOL WINAPI srcl_init();
BOOL WINAPI srcl_init_1(unsigned char* buf, unsigned long buf_size, char* vendor, char* version, char* filename);

/* terminate srcl */
VOID WINAPI srcl_term();

#ifdef __cplusplus
}
#endif

#endif
