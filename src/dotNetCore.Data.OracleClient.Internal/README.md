# Company Internal NuGet Package

## Purpose

The purpose of this project is to generate a NuGet
package which contains the shared Oracle InstantClient libraries. The
benefit is that those libraries do not need to be installed as a separate
step and also (in Windows) the PATH environment variable does not have
to be set.

The package may _not_ be posted publicly since it contains Oracle
libraries and the license does not allow for that.
That is, once you create the package, put it in your internal NuGet
repository.

This setup is to generate a package using the 11.x Instant Client libraries
so that old, unsupported versions of Oracle can be accessed. If you want to
use a different library version, you will need to alter the `.csproj` file.

**If you are accessing Oracle 11+, consider
 [The Official .NET Core Oracle Driver](https://www.nuget.org/packages/Oracle.ManagedDataAccess.Core).**

## Caveats / Notes

### Build on Linux

* Git-Bash / Windows does not handle the executable bit, which needs to be set on the
  .so files. If you are building for Linux/Mac + Windows, use a UNIX host for the build.

* The source directories are also symbolic links to the main tree, which may or may not play
  well with git-bash.

* You will need the `patchelf` utility.

### Linux Still Needs a Linker Path Set

`libclntsh.so` depends on `libnnz11.so`. However, dotnet will only find the first library
and let the OS link the second, which will fail since, by default, the linker does not look in the
same directory where `libclntsh.so` is located (as part of the nuget package).

To work around this, use `patchelf` to set the search path for the main shared-object library to
be the same directory in which it is located:

```bash
patchelf --set-rpath '$ORIGIN/.' libclntsh.so
```

Note the use of single quotes here, it is very important that the `$ORIGIN` has the `$` in
the string. [ld-linux man page](http://man7.org/linux/man-pages/man8/ld-linux.so.8.html)

Mac users can do a similar thing with `install_name_tool` -- see https://stackoverflow.com/questions/13769141/can-i-change-rpath-in-an-already-compiled-binary

## Building

Download and unzip the InstantClient zip files for the Oracle version and OS
you want to support. The files shown below are assuming version 11.x.

Put the Windows DLLs into lib-win-x64:

```bash
mkdir lib-win-x64
cp instantclient_11_2/oci.dll lib-win-x64
cp instantclient_11_2/orannzsbb11.dll lib-win-x64
cp instantclient_11_2/oraocci11.dll lib-win-x64
cp instantclient_11_2/oraociei11.dll lib-win-x64
```

Put the Linux shared object files into lib-linux-x64. Note they
must be renamed, and change the rpath of `libclntsh.so`:

```bash
mkdir lib-linux-x64
cp instantclient_11_2/libclntsh.so.11.1 lib-linux-x64/libclntsh.so
cp instantclient_11_2/libnnz11.so.11.1 lib-linux-x64/libnnz11.so
patchelf --set-rpath '$ORIGIN/.' lib-linux-x64/libclntsh.so
```

Build the NuGet package:

```bash
dotnet pack -c Release
```

Copy the package to your local NuGet repository:

```bash
cp bin/Release/dotNetCore.Data.OracleClient.Internal.1.0.1.nupkg /path/to/local/NuGet/repo
```

Finally, you can add the package to your project:

```bash
dotnet add package dotNetCore.Data.OracleClient11.Internal
```

END

[//]: # ( spell-checker:ignore patchelf libclntsh libnnz nuget rpath mkdir instantclient orannzsbb oraocci oraociei nupkg repo )
