﻿using System;
using LibHac.Common;
using LibHac.Common.FixedArrays;

namespace LibHac.Fs;

internal struct MountName
{
    private Array16<byte> _nameArray;
    public Span<byte> Name => _nameArray.Items;

    public override string ToString() => new U8Span(Name).ToString();
}