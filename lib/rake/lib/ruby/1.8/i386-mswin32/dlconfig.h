#ifndef DLCONFIG_H
#define DLCONFIG_H
#define MAX_ARG           0
#define MAX_CALLBACK 10
#define CALLBACK_TYPES 8
#define USE_DLSTACK
#define WITH_TYPE_CHAR
#define WITH_TYPE_SHORT
#define WITH_TYPE_LONG
#define WITH_TYPE_DOUBLE
#define WITH_TYPE_FLOAT
#if !defined(HAVE_WINDOWS_H)
# define HAVE_WINDOWS_H
#endif

#endif /* DLCONFIG_H */
