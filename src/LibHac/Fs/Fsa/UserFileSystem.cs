﻿using System;
using System.Runtime.CompilerServices;
using LibHac.Common;
using LibHac.Fs.Impl;
using LibHac.Fs.Shim;
using LibHac.FsSrv.Sf;
using LibHac.Os;
using LibHac.Util;
using static LibHac.Fs.Impl.AccessLogStrings;
using IFileSystemSf = LibHac.FsSrv.Sf.IFileSystem;

namespace LibHac.Fs.Fsa;

/// <summary>
/// Contains functions for interacting with mounted file systems.
/// </summary>
/// <remarks>Based on FS 13.1.0 (nnSdk 13.4.0)</remarks>
[SkipLocalsInit]
public static class UserFileSystem
{
    public static Result CreateFile(this FileSystemClient fs, U8Span path, long size)
    {
        return fs.CreateFile(path, size, CreateFileOptions.None);
    }

    public static Result DeleteFile(this FileSystemClient fs, U8Span path)
    {
        Result rc;
        U8Span subPath;
        FileSystemAccessor fileSystem;
        Span<byte> logBuffer = stackalloc byte[0x300];

        if (fs.Impl.IsEnabledAccessLog())
        {
            Tick start = fs.Hos.Os.GetSystemTick();
            rc = fs.Impl.FindFileSystem(out fileSystem, out subPath, path);
            Tick end = fs.Hos.Os.GetSystemTick();

            var sb = new U8StringBuilder(logBuffer, true);
            sb.Append(LogPath).Append(path).Append((byte)'"');
            logBuffer = sb.Buffer;

            fs.Impl.OutputAccessLogUnlessResultSuccess(rc, start, end, null, new U8Span(logBuffer));
        }
        else
        {
            rc = fs.Impl.FindFileSystem(out fileSystem, out subPath, path);
        }
        fs.Impl.AbortIfNeeded(rc);
        if (rc.IsFailure()) return rc;

        if (fs.Impl.IsEnabledAccessLog() && fileSystem.IsEnabledAccessLog())
        {
            Tick start = fs.Hos.Os.GetSystemTick();
            rc = fileSystem.DeleteFile(subPath);
            Tick end = fs.Hos.Os.GetSystemTick();

            fs.Impl.OutputAccessLog(rc, start, end, null, new U8Span(logBuffer));
        }
        else
        {
            rc = fileSystem.DeleteFile(subPath);
        }
        fs.Impl.AbortIfNeeded(rc);
        return rc;
    }

    public static Result CreateDirectory(this FileSystemClient fs, U8Span path)
    {
        Result rc;
        U8Span subPath;
        FileSystemAccessor fileSystem;
        Span<byte> logBuffer = stackalloc byte[0x300];

        if (fs.Impl.IsEnabledAccessLog())
        {
            Tick start = fs.Hos.Os.GetSystemTick();
            rc = fs.Impl.FindFileSystem(out fileSystem, out subPath, path);
            Tick end = fs.Hos.Os.GetSystemTick();

            var sb = new U8StringBuilder(logBuffer, true);
            sb.Append(LogPath).Append(path).Append((byte)'"');
            logBuffer = sb.Buffer;

            fs.Impl.OutputAccessLogUnlessResultSuccess(rc, start, end, null, new U8Span(logBuffer));
        }
        else
        {
            rc = fs.Impl.FindFileSystem(out fileSystem, out subPath, path);
        }
        fs.Impl.AbortIfNeeded(rc);
        if (rc.IsFailure()) return rc;

        if (fs.Impl.IsEnabledAccessLog() && fileSystem.IsEnabledAccessLog())
        {
            Tick start = fs.Hos.Os.GetSystemTick();
            rc = fileSystem.CreateDirectory(subPath);
            Tick end = fs.Hos.Os.GetSystemTick();

            fs.Impl.OutputAccessLog(rc, start, end, null, new U8Span(logBuffer));
        }
        else
        {
            rc = fileSystem.CreateDirectory(subPath);
        }
        fs.Impl.AbortIfNeeded(rc);
        return rc;
    }

    public static Result DeleteDirectory(this FileSystemClient fs, U8Span path)
    {
        Result rc;
        U8Span subPath;
        FileSystemAccessor fileSystem;
        Span<byte> logBuffer = stackalloc byte[0x300];

        if (fs.Impl.IsEnabledAccessLog())
        {
            Tick start = fs.Hos.Os.GetSystemTick();
            rc = fs.Impl.FindFileSystem(out fileSystem, out subPath, path);
            Tick end = fs.Hos.Os.GetSystemTick();

            var sb = new U8StringBuilder(logBuffer, true);
            sb.Append(LogPath).Append(path).Append((byte)'"');
            logBuffer = sb.Buffer;

            fs.Impl.OutputAccessLogUnlessResultSuccess(rc, start, end, null, new U8Span(logBuffer));
        }
        else
        {
            rc = fs.Impl.FindFileSystem(out fileSystem, out subPath, path);
        }
        fs.Impl.AbortIfNeeded(rc);
        if (rc.IsFailure()) return rc;

        if (fs.Impl.IsEnabledAccessLog() && fileSystem.IsEnabledAccessLog())
        {
            Tick start = fs.Hos.Os.GetSystemTick();
            rc = fileSystem.DeleteDirectory(subPath);
            Tick end = fs.Hos.Os.GetSystemTick();

            fs.Impl.OutputAccessLog(rc, start, end, null, new U8Span(logBuffer));
        }
        else
        {
            rc = fileSystem.DeleteDirectory(subPath);
        }
        fs.Impl.AbortIfNeeded(rc);
        return rc;
    }

    public static Result DeleteDirectoryRecursively(this FileSystemClient fs, U8Span path)
    {
        Result rc;
        U8Span subPath;
        FileSystemAccessor fileSystem;
        Span<byte> logBuffer = stackalloc byte[0x300];

        if (fs.Impl.IsEnabledAccessLog())
        {
            Tick start = fs.Hos.Os.GetSystemTick();
            rc = fs.Impl.FindFileSystem(out fileSystem, out subPath, path);
            Tick end = fs.Hos.Os.GetSystemTick();

            var sb = new U8StringBuilder(logBuffer, true);
            sb.Append(LogPath).Append(path).Append((byte)'"');
            logBuffer = sb.Buffer;

            fs.Impl.OutputAccessLogUnlessResultSuccess(rc, start, end, null, new U8Span(logBuffer));
        }
        else
        {
            rc = fs.Impl.FindFileSystem(out fileSystem, out subPath, path);
        }
        fs.Impl.AbortIfNeeded(rc);
        if (rc.IsFailure()) return rc;

        if (fs.Impl.IsEnabledAccessLog() && fileSystem.IsEnabledAccessLog())
        {
            Tick start = fs.Hos.Os.GetSystemTick();
            rc = fileSystem.DeleteDirectoryRecursively(subPath);
            Tick end = fs.Hos.Os.GetSystemTick();

            fs.Impl.OutputAccessLog(rc, start, end, null, new U8Span(logBuffer));
        }
        else
        {
            rc = fileSystem.DeleteDirectoryRecursively(subPath);
        }
        fs.Impl.AbortIfNeeded(rc);
        return rc;
    }

    public static Result CleanDirectoryRecursively(this FileSystemClient fs, U8Span path)
    {
        Result rc;
        U8Span subPath;
        FileSystemAccessor fileSystem;
        Span<byte> logBuffer = stackalloc byte[0x300];

        if (fs.Impl.IsEnabledAccessLog())
        {
            Tick start = fs.Hos.Os.GetSystemTick();
            rc = fs.Impl.FindFileSystem(out fileSystem, out subPath, path);
            Tick end = fs.Hos.Os.GetSystemTick();

            var sb = new U8StringBuilder(logBuffer, true);
            sb.Append(LogPath).Append(path).Append((byte)'"');
            logBuffer = sb.Buffer;

            fs.Impl.OutputAccessLogUnlessResultSuccess(rc, start, end, null, new U8Span(logBuffer));
        }
        else
        {
            rc = fs.Impl.FindFileSystem(out fileSystem, out subPath, path);
        }
        fs.Impl.AbortIfNeeded(rc);
        if (rc.IsFailure()) return rc;

        if (fs.Impl.IsEnabledAccessLog() && fileSystem.IsEnabledAccessLog())
        {
            Tick start = fs.Hos.Os.GetSystemTick();
            rc = fileSystem.CleanDirectoryRecursively(subPath);
            Tick end = fs.Hos.Os.GetSystemTick();

            fs.Impl.OutputAccessLog(rc, start, end, null, new U8Span(logBuffer));
        }
        else
        {
            rc = fileSystem.CleanDirectoryRecursively(subPath);
        }
        fs.Impl.AbortIfNeeded(rc);
        return rc;
    }

    public static Result RenameFile(this FileSystemClient fs, U8Span currentPath, U8Span newPath)
    {
        Result rc;
        U8Span currentSubPath, newSubPath;
        FileSystemAccessor currentFileSystem, newFileSystem;
        Span<byte> logBuffer = stackalloc byte[0x300];

        // Get the file system accessor for the current path
        if (fs.Impl.IsEnabledAccessLog())
        {
            Tick start = fs.Hos.Os.GetSystemTick();
            rc = fs.Impl.FindFileSystem(out currentFileSystem, out currentSubPath, currentPath);
            Tick end = fs.Hos.Os.GetSystemTick();

            var sb = new U8StringBuilder(logBuffer, true);
            sb.Append(LogPath).Append(currentPath).Append(LogNewPath).Append(newPath).Append((byte)'"');
            logBuffer = sb.Buffer;

            fs.Impl.OutputAccessLogUnlessResultSuccess(rc, start, end, null, new U8Span(logBuffer));
        }
        else
        {
            rc = fs.Impl.FindFileSystem(out currentFileSystem, out currentSubPath, currentPath);
        }
        fs.Impl.AbortIfNeeded(rc);
        if (rc.IsFailure()) return rc;

        // Get the file system accessor for the new path
        if (fs.Impl.IsEnabledAccessLog())
        {
            Tick start = fs.Hos.Os.GetSystemTick();
            rc = fs.Impl.FindFileSystem(out newFileSystem, out newSubPath, newPath);
            Tick end = fs.Hos.Os.GetSystemTick();

            fs.Impl.OutputAccessLogUnlessResultSuccess(rc, start, end, null, new U8Span(logBuffer));
        }
        else
        {
            rc = fs.Impl.FindFileSystem(out newFileSystem, out newSubPath, newPath);
        }
        fs.Impl.AbortIfNeeded(rc);
        if (rc.IsFailure()) return rc;

        // Rename the file
        if (fs.Impl.IsEnabledAccessLog() && currentFileSystem.IsEnabledAccessLog())
        {
            Tick start = fs.Hos.Os.GetSystemTick();

            rc = currentFileSystem != newFileSystem
                ? ResultFs.RenameToOtherFileSystem.Log()
                : currentFileSystem.RenameFile(currentSubPath, newSubPath);

            Tick end = fs.Hos.Os.GetSystemTick();

            fs.Impl.OutputAccessLog(rc, start, end, null, new U8Span(logBuffer));
        }
        else
        {
            rc = currentFileSystem != newFileSystem
                ? ResultFs.RenameToOtherFileSystem.Log()
                : currentFileSystem.RenameFile(currentSubPath, newSubPath);
        }
        fs.Impl.AbortIfNeeded(rc);
        return rc;
    }

    public static Result RenameDirectory(this FileSystemClient fs, U8Span currentPath, U8Span newPath)
    {
        Result rc;
        U8Span currentSubPath, newSubPath;
        FileSystemAccessor currentFileSystem, newFileSystem;
        Span<byte> logBuffer = stackalloc byte[0x300];

        // Get the file system accessor for the current path
        if (fs.Impl.IsEnabledAccessLog())
        {
            Tick start = fs.Hos.Os.GetSystemTick();
            rc = fs.Impl.FindFileSystem(out currentFileSystem, out currentSubPath, currentPath);
            Tick end = fs.Hos.Os.GetSystemTick();

            var sb = new U8StringBuilder(logBuffer, true);
            sb.Append(LogPath).Append(currentPath).Append(LogNewPath).Append(newPath).Append((byte)'"');
            logBuffer = sb.Buffer;

            fs.Impl.OutputAccessLogUnlessResultSuccess(rc, start, end, null, new U8Span(logBuffer));
        }
        else
        {
            rc = fs.Impl.FindFileSystem(out currentFileSystem, out currentSubPath, currentPath);
        }
        fs.Impl.AbortIfNeeded(rc);
        if (rc.IsFailure()) return rc;

        // Get the file system accessor for the new path
        if (fs.Impl.IsEnabledAccessLog())
        {
            Tick start = fs.Hos.Os.GetSystemTick();
            rc = fs.Impl.FindFileSystem(out newFileSystem, out newSubPath, newPath);
            Tick end = fs.Hos.Os.GetSystemTick();

            fs.Impl.OutputAccessLogUnlessResultSuccess(rc, start, end, null, new U8Span(logBuffer));
        }
        else
        {
            rc = fs.Impl.FindFileSystem(out newFileSystem, out newSubPath, newPath);
        }
        fs.Impl.AbortIfNeeded(rc);
        if (rc.IsFailure()) return rc;

        // Rename the directory
        if (fs.Impl.IsEnabledAccessLog() && currentFileSystem.IsEnabledAccessLog())
        {
            Tick start = fs.Hos.Os.GetSystemTick();

            rc = currentFileSystem != newFileSystem
                ? ResultFs.RenameToOtherFileSystem.Log()
                : currentFileSystem.RenameDirectory(currentSubPath, newSubPath);

            Tick end = fs.Hos.Os.GetSystemTick();

            fs.Impl.OutputAccessLog(rc, start, end, null, new U8Span(logBuffer));
        }
        else
        {
            rc = currentFileSystem != newFileSystem
                ? ResultFs.RenameToOtherFileSystem.Log()
                : currentFileSystem.RenameDirectory(currentSubPath, newSubPath);
        }
        fs.Impl.AbortIfNeeded(rc);
        return rc;
    }

    public static Result GetEntryType(this FileSystemClient fs, out DirectoryEntryType type, U8Span path)
    {
        UnsafeHelpers.SkipParamInit(out type);

        Result rc;
        U8Span subPath;
        FileSystemAccessor fileSystem;
        Span<byte> logBuffer = stackalloc byte[0x300];

        if (fs.Impl.IsEnabledAccessLog())
        {
            Tick start = fs.Hos.Os.GetSystemTick();
            rc = fs.Impl.FindFileSystem(out fileSystem, out subPath, path);
            Tick end = fs.Hos.Os.GetSystemTick();

            var idString = new IdString();
            var sb = new U8StringBuilder(logBuffer, true);
            sb.Append(LogPath).Append(path).Append(LogEntryType)
                .Append(idString.ToString(AccessLogImpl.DereferenceOutValue(in type, rc)));
            logBuffer = sb.Buffer;

            fs.Impl.OutputAccessLogUnlessResultSuccess(rc, start, end, null, new U8Span(logBuffer));
        }
        else
        {
            rc = fs.Impl.FindFileSystem(out fileSystem, out subPath, path);
        }
        fs.Impl.AbortIfNeeded(rc);
        if (rc.IsFailure()) return rc;

        if (fs.Impl.IsEnabledAccessLog() && fileSystem.IsEnabledAccessLog())
        {
            Tick start = fs.Hos.Os.GetSystemTick();
            rc = fileSystem.GetEntryType(out type, subPath);
            Tick end = fs.Hos.Os.GetSystemTick();

            var idString = new IdString();
            var sb = new U8StringBuilder(logBuffer, true);
            sb.Append(LogPath).Append(path).Append(LogEntryType)
                .Append(idString.ToString(AccessLogImpl.DereferenceOutValue(in type, rc)));
            logBuffer = sb.Buffer;

            fs.Impl.OutputAccessLog(rc, start, end, null, new U8Span(logBuffer));
        }
        else
        {
            rc = fileSystem.GetEntryType(out type, subPath);
        }
        fs.Impl.AbortIfNeeded(rc);
        return rc;
    }

    public static Result GetFreeSpaceSize(this FileSystemClient fs, out long freeSpace, U8Span path)
    {
        UnsafeHelpers.SkipParamInit(out freeSpace);

        Result rc;
        var subPath = U8Span.Empty;
        FileSystemAccessor fileSystem;
        Span<byte> logBuffer = stackalloc byte[0x300];

        static Result FindImpl(FileSystemClient fs, U8Span path, out FileSystemAccessor fileSystem, ref U8Span subPath)
        {
            if (fs.Impl.IsValidMountName(path))
                return fs.Impl.Find(out fileSystem, path);
            else
                return fs.Impl.FindFileSystem(out fileSystem, out subPath, path);
        }

        if (fs.Impl.IsEnabledAccessLog())
        {
            Tick start = fs.Hos.Os.GetSystemTick();
            rc = FindImpl(fs, path, out fileSystem, ref subPath);
            Tick end = fs.Hos.Os.GetSystemTick();

            var sb = new U8StringBuilder(logBuffer, true);
            sb.Append(LogPath).Append(path).Append((byte)'"').Append(LogSize)
                .AppendFormat(AccessLogImpl.DereferenceOutValue(in freeSpace, rc));
            logBuffer = sb.Buffer;

            fs.Impl.OutputAccessLogUnlessResultSuccess(rc, start, end, null, new U8Span(logBuffer));
        }
        else
        {
            rc = FindImpl(fs, path, out fileSystem, ref subPath);
        }
        fs.Impl.AbortIfNeeded(rc);
        if (rc.IsFailure()) return rc;

        static Result GetImpl(out long freeSpace, FileSystemAccessor fileSystem, U8Span subPath)
        {
            UnsafeHelpers.SkipParamInit(out freeSpace);

            if (subPath.IsEmpty() && StringUtils.Compare(subPath, new[] { (byte)'/' }) != 0)
                return ResultFs.InvalidMountName.Log();

            return fileSystem.GetFreeSpaceSize(out freeSpace, new U8Span(new[] { (byte)'/' }));
        }

        if (fs.Impl.IsEnabledAccessLog() && fileSystem.IsEnabledAccessLog())
        {
            Tick start = fs.Hos.Os.GetSystemTick();
            rc = GetImpl(out freeSpace, fileSystem, subPath);
            Tick end = fs.Hos.Os.GetSystemTick();

            var sb = new U8StringBuilder(logBuffer, true);
            sb.Append(LogPath).Append(path).Append((byte)'"').Append(LogSize)
                .AppendFormat(AccessLogImpl.DereferenceOutValue(in freeSpace, rc));
            logBuffer = sb.Buffer;

            fs.Impl.OutputAccessLog(rc, start, end, null, new U8Span(logBuffer));
        }
        else
        {
            rc = GetImpl(out freeSpace, fileSystem, subPath);
        }
        fs.Impl.AbortIfNeeded(rc);
        return rc;
    }

    public static Result OpenFile(this FileSystemClient fs, out FileHandle handle, U8Span path, OpenMode mode)
    {
        UnsafeHelpers.SkipParamInit(out handle);

        Result rc;
        U8Span subPath;
        FileSystemAccessor fileSystem;
        Span<byte> logBuffer = stackalloc byte[0x300];

        if (fs.Impl.IsEnabledAccessLog())
        {
            Tick start = fs.Hos.Os.GetSystemTick();
            rc = fs.Impl.FindFileSystem(out fileSystem, out subPath, path);
            Tick end = fs.Hos.Os.GetSystemTick();

            var sb = new U8StringBuilder(logBuffer, true);
            sb.Append(LogPath).Append(path).Append((byte)'"').Append(LogOpenMode).AppendFormat((int)mode, 'X');
            logBuffer = sb.Buffer;

            fs.Impl.OutputAccessLogUnlessResultSuccess(rc, start, end, null, new U8Span(logBuffer));
        }
        else
        {
            rc = fs.Impl.FindFileSystem(out fileSystem, out subPath, path);
        }
        fs.Impl.AbortIfNeeded(rc);
        if (rc.IsFailure()) return rc;

        using var file = new UniqueRef<FileAccessor>();
        if (fs.Impl.IsEnabledAccessLog() && fileSystem.IsEnabledAccessLog())
        {
            Tick start = fs.Hos.Os.GetSystemTick();
            rc = fileSystem.OpenFile(ref file.Ref(), subPath, mode);
            Tick end = fs.Hos.Os.GetSystemTick();

            fs.Impl.OutputAccessLog(rc, start, end, file.Get, new U8Span(logBuffer));
        }
        else
        {
            rc = fileSystem.OpenFile(ref file.Ref(), subPath, mode);
        }
        fs.Impl.AbortIfNeeded(rc);
        if (rc.IsFailure()) return rc;

        handle = new FileHandle(file.Release());
        return Result.Success;
    }

    public static Result OpenFile(this FileSystemClient fs, out FileHandle handle, ref UniqueRef<IFile> file, OpenMode mode)
    {
        var accessor = new FileAccessor(fs.Hos, ref file, null, mode);
        handle = new FileHandle(accessor);

        return Result.Success;
    }

    public static Result OpenDirectory(this FileSystemClient fs, out DirectoryHandle handle, U8Span path,
        OpenDirectoryMode mode)
    {
        UnsafeHelpers.SkipParamInit(out handle);

        Result rc;
        U8Span subPath;
        FileSystemAccessor fileSystem;
        Span<byte> logBuffer = stackalloc byte[0x300];

        if (fs.Impl.IsEnabledAccessLog())
        {
            Tick start = fs.Hos.Os.GetSystemTick();
            rc = fs.Impl.FindFileSystem(out fileSystem, out subPath, path);
            Tick end = fs.Hos.Os.GetSystemTick();

            var sb = new U8StringBuilder(logBuffer, true);
            sb.Append(LogPath).Append(path).Append((byte)'"').Append(LogOpenMode).AppendFormat((int)mode, 'X');
            logBuffer = sb.Buffer;

            fs.Impl.OutputAccessLogUnlessResultSuccess(rc, start, end, null, new U8Span(logBuffer));
        }
        else
        {
            rc = fs.Impl.FindFileSystem(out fileSystem, out subPath, path);
        }
        fs.Impl.AbortIfNeeded(rc);
        if (rc.IsFailure()) return rc;

        using var accessor = new UniqueRef<DirectoryAccessor>();

        if (fs.Impl.IsEnabledAccessLog() && fileSystem.IsEnabledAccessLog())
        {
            Tick start = fs.Hos.Os.GetSystemTick();
            rc = fileSystem.OpenDirectory(ref accessor.Ref(), subPath, mode);
            Tick end = fs.Hos.Os.GetSystemTick();

            fs.Impl.OutputAccessLog(rc, start, end, accessor.Get, new U8Span(logBuffer));
        }
        else
        {
            rc = fileSystem.OpenDirectory(ref accessor.Ref(), subPath, mode);
        }
        fs.Impl.AbortIfNeeded(rc);
        if (rc.IsFailure()) return rc;

        handle = new DirectoryHandle(accessor.Release());
        return Result.Success;
    }

    private static Result CommitImpl(FileSystemClient fs, U8Span mountName,
        [CallerMemberName] string functionName = "")
    {
        Result rc;
        FileSystemAccessor fileSystem;
        Span<byte> logBuffer = stackalloc byte[0x30];

        if (fs.Impl.IsEnabledAccessLog())
        {
            Tick start = fs.Hos.Os.GetSystemTick();
            rc = fs.Impl.Find(out fileSystem, mountName);
            Tick end = fs.Hos.Os.GetSystemTick();

            var sb = new U8StringBuilder(logBuffer, true);
            sb.Append(LogName).Append(mountName).Append((byte)'"');
            logBuffer = sb.Buffer;

            fs.Impl.OutputAccessLogUnlessResultSuccess(rc, start, end, null, new U8Span(logBuffer), functionName);
        }
        else
        {
            rc = fs.Impl.Find(out fileSystem, mountName);
        }
        fs.Impl.AbortIfNeeded(rc);
        if (rc.IsFailure()) return rc;

        if (fs.Impl.IsEnabledAccessLog() && fileSystem.IsEnabledAccessLog())
        {
            Tick start = fs.Hos.Os.GetSystemTick();
            rc = fileSystem.Commit();
            Tick end = fs.Hos.Os.GetSystemTick();

            fs.Impl.OutputAccessLog(rc, start, end, null, new U8Span(logBuffer), functionName);
        }
        else
        {
            rc = fileSystem.Commit();
        }
        fs.Impl.AbortIfNeeded(rc);
        return rc;
    }

    public static Result Commit(this FileSystemClient fs, ReadOnlySpan<U8String> mountNames)
    {
        // Todo: Add access log

        if (mountNames.Length < 0)
            return ResultFs.InvalidCommitNameCount.Log();

        if (mountNames.Length > 10)
            return ResultFs.InvalidCommitNameCount.Log();

        if (mountNames.Length == 0)
            return Result.Success;

        using var commitManager = new SharedRef<IMultiCommitManager>();
        using SharedRef<IFileSystemProxy> fileSystemProxy = fs.Impl.GetFileSystemProxyServiceObject();

        Result rc = fileSystemProxy.Get.OpenMultiCommitManager(ref commitManager.Ref());
        if (rc.IsFailure()) return rc;

        for (int i = 0; i < mountNames.Length; i++)
        {
            rc = fs.Impl.Find(out FileSystemAccessor accessor, mountNames[i]);
            if (rc.IsFailure()) return rc;

            using SharedRef<IFileSystemSf> fileSystem = accessor.GetMultiCommitTarget();
            if (!fileSystem.HasValue)
                return ResultFs.UnsupportedCommitTarget.Log();

            rc = commitManager.Get.Add(ref fileSystem.Ref());
            if (rc.IsFailure()) return rc;
        }

        rc = commitManager.Get.Commit();
        if (rc.IsFailure()) return rc;

        return Result.Success;
    }

    public static Result Commit(this FileSystemClient fs, U8Span mountName, CommitOption option)
    {
        Result rc;
        FileSystemAccessor fileSystem;
        Span<byte> logBuffer = stackalloc byte[0x40];

        if (fs.Impl.IsEnabledAccessLog())
        {
            Tick start = fs.Hos.Os.GetSystemTick();
            rc = fs.Impl.Find(out fileSystem, mountName);
            Tick end = fs.Hos.Os.GetSystemTick();

            var sb = new U8StringBuilder(logBuffer, true);
            sb.Append(LogName).Append(mountName).Append(LogCommitOption).AppendFormat((int)option.Flags, 'X');
            logBuffer = sb.Buffer;

            fs.Impl.OutputAccessLogUnlessResultSuccess(rc, start, end, null, new U8Span(logBuffer));
        }
        else
        {
            rc = fs.Impl.Find(out fileSystem, mountName);
        }
        fs.Impl.AbortIfNeeded(rc);
        if (rc.IsFailure()) return rc;

        if (fs.Impl.IsEnabledAccessLog() && fileSystem.IsEnabledAccessLog())
        {
            Tick start = fs.Hos.Os.GetSystemTick();
            rc = RunCommit(fs, option, fileSystem);
            Tick end = fs.Hos.Os.GetSystemTick();

            fs.Impl.OutputAccessLog(rc, start, end, null, new U8Span(logBuffer));
        }
        else
        {
            rc = RunCommit(fs, option, fileSystem);
        }
        fs.Impl.AbortIfNeeded(rc);
        return rc;

        static Result RunCommit(FileSystemClient fs, CommitOption option, FileSystemAccessor fileSystem)
        {
            if ((option.Flags & (CommitOptionFlag.ClearRestoreFlag | CommitOptionFlag.SetRestoreFlag)) == 0)
            {
                return fileSystem.Commit();
            }

            if (option.Flags != CommitOptionFlag.ClearRestoreFlag &&
                option.Flags != CommitOptionFlag.SetRestoreFlag)
            {
                return ResultFs.InvalidCommitOption.Log();
            }

            Result rc = fileSystem.GetSaveDataAttribute(out SaveDataAttribute attribute);
            if (rc.IsFailure()) return rc;

            if (attribute.ProgramId == SaveData.InvalidProgramId)
                attribute.ProgramId = SaveData.AutoResolveCallerProgramId;

            var extraDataMask = new SaveDataExtraData();
            extraDataMask.Flags = SaveDataFlags.Restore;

            var extraData = new SaveDataExtraData();
            extraDataMask.Flags = option.Flags == CommitOptionFlag.SetRestoreFlag
                ? SaveDataFlags.Restore
                : SaveDataFlags.None;

            return fs.Impl.WriteSaveDataFileSystemExtraData(SaveDataSpaceId.User, in attribute, in extraData,
                in extraDataMask);
        }
    }

    public static Result Commit(this FileSystemClient fs, U8Span mountName)
    {
        return CommitImpl(fs, mountName);
    }

    public static Result CommitSaveData(this FileSystemClient fs, U8Span mountName)
    {
        return CommitImpl(fs, mountName);
    }
}