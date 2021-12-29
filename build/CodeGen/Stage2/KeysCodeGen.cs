﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Security.Cryptography;
using LibHac.Common;
using LibHac.Common.Keys;
using LibHac.Crypto;
using static LibHacBuild.CodeGen.Common;

namespace LibHacBuild.CodeGen.Stage2;

public static class KeysCodeGen
{
    private static string InputMainKeyFileName = "IncludedKeys.txt";
    private static string GeneratedFilePath = "LibHac/Common/Keys/DefaultKeySet.Generated.cs";

    public static void Run()
    {
        KeySet keySet = CreateKeySet();

        WriteOutput(GeneratedFilePath, BuildDefaultKeySetFile(keySet));
    }

    private static string BuildDefaultKeySetFile(KeySet keySet)
    {
        var sb = new IndentingStringBuilder();

        sb.AppendLine(GetHeader());
        sb.AppendLine();

        sb.AppendLine("using System;");
        sb.AppendLine();

        sb.AppendLine("namespace LibHac.Common.Keys;");
        sb.AppendLine();

        sb.AppendLine("internal static partial class DefaultKeySet");
        sb.AppendLineAndIncrease("{");

        BuildArray(sb, "RootKeysDev", SpanHelpers.AsReadOnlyByteSpan(in keySet.KeyStruct.RootKeysDev));
        BuildArray(sb, "RootKeysProd", SpanHelpers.AsReadOnlyByteSpan(in keySet.KeyStruct.RootKeysProd));
        BuildArray(sb, "KeySeeds", SpanHelpers.AsReadOnlyByteSpan(in keySet.KeyStruct.KeySeeds));
        BuildArray(sb, "StoredKeysDev", SpanHelpers.AsReadOnlyByteSpan(in keySet.KeyStruct.StoredKeysDev));
        BuildArray(sb, "StoredKeysProd", SpanHelpers.AsReadOnlyByteSpan(in keySet.KeyStruct.StoredKeysProd));
        BuildArray(sb, "DerivedKeysDev", SpanHelpers.AsReadOnlyByteSpan(in keySet.KeyStruct.DerivedKeysDev));
        BuildArray(sb, "DerivedKeysProd", SpanHelpers.AsReadOnlyByteSpan(in keySet.KeyStruct.DerivedKeysProd));
        BuildArray(sb, "DeviceKeys", SpanHelpers.AsReadOnlyByteSpan(in keySet.KeyStruct.DeviceKeys));
        BuildArray(sb, "RsaSigningKeysDev", SpanHelpers.AsReadOnlyByteSpan(in keySet.KeyStruct.RsaSigningKeysDev));
        BuildArray(sb, "RsaSigningKeysProd", SpanHelpers.AsReadOnlyByteSpan(in keySet.KeyStruct.RsaSigningKeysProd));
        BuildArray(sb, "RsaKeys", SpanHelpers.AsReadOnlyByteSpan(in keySet.KeyStruct.RsaKeys));

        sb.DecreaseAndAppendLine("}");

        return sb.ToString();
    }

    private static void BuildArray(IndentingStringBuilder sb, string name, ReadOnlySpan<byte> data)
    {
        sb.AppendSpacerLine();
        sb.Append($"private static ReadOnlySpan<byte> {name} => new byte[]");

        if (data.IsZeros())
        {
            sb.AppendLine(" { };");
            return;
        }

        sb.AppendLine();
        sb.AppendLineAndIncrease("{");

        for (int i = 0; i < data.Length; i++)
        {
            if (i % 16 != 0) sb.Append(" ");
            sb.Append($"0x{data[i]:x2}");

            if (i != data.Length - 1)
            {
                sb.Append(",");
                if (i % 16 == 15) sb.AppendLine();
            }
        }

        sb.AppendLine();
        sb.DecreaseAndAppendLine("};");
    }

    private static KeySet CreateKeySet()
    {
        var keySet = new KeySet();

        // Populate the key set with all the keys in IncludedKeys.txt
        using (Stream keyFile = GetResource(InputMainKeyFileName))
        {
            List<KeyInfo> list = KeySet.CreateKeyInfoList();
            ExternalKeyReader.ReadMainKeys(keySet, keyFile, list);
        }

        // Recover all the RSA key parameters and write the key to the key set
        RSAParameters betaNca0Params =
            Rsa.RecoverParameters(BetaNca0Modulus, StandardPublicExponent, BetaNca0Exponent);

        betaNca0Params.D.AsSpan().CopyTo(keySet.BetaNca0KeyAreaKey.PrivateExponent.Data);
        betaNca0Params.DP.AsSpan().CopyTo(keySet.BetaNca0KeyAreaKey.Dp.Data);
        betaNca0Params.DQ.AsSpan().CopyTo(keySet.BetaNca0KeyAreaKey.Dq.Data);
        betaNca0Params.Exponent.AsSpan().CopyTo(keySet.BetaNca0KeyAreaKey.PublicExponent.Data);
        betaNca0Params.InverseQ.AsSpan().CopyTo(keySet.BetaNca0KeyAreaKey.InverseQ.Data);
        betaNca0Params.Modulus.AsSpan().CopyTo(keySet.BetaNca0KeyAreaKey.Modulus.Data);
        betaNca0Params.P.AsSpan().CopyTo(keySet.BetaNca0KeyAreaKey.P.Data);
        betaNca0Params.Q.AsSpan().CopyTo(keySet.BetaNca0KeyAreaKey.Q.Data);

        // First populate the prod RSA keys
        keySet.SetMode(KeySet.Mode.Prod);

        StandardPublicExponent.CopyTo(keySet.NcaHeaderSigningKeys[0].PublicExponent.Data);
        StandardPublicExponent.CopyTo(keySet.NcaHeaderSigningKeys[1].PublicExponent.Data);
        NcaHdrFixedKeyModulus0Prod.CopyTo(keySet.NcaHeaderSigningKeys[0].Modulus.Data);
        NcaHdrFixedKeyModulus1Prod.CopyTo(keySet.NcaHeaderSigningKeys[1].Modulus.Data);

        StandardPublicExponent.CopyTo(keySet.AcidSigningKeys[0].PublicExponent.Data);
        StandardPublicExponent.CopyTo(keySet.AcidSigningKeys[1].PublicExponent.Data);
        AcidFixedKeyModulus0Prod.CopyTo(keySet.AcidSigningKeys[0].Modulus.Data);
        AcidFixedKeyModulus1Prod.CopyTo(keySet.AcidSigningKeys[1].Modulus.Data);

        StandardPublicExponent.CopyTo(keySet.Package2SigningKey.PublicExponent.Data);
        Package2FixedKeyModulusProd.CopyTo(keySet.Package2SigningKey.Modulus.Data);

        // Populate the dev RSA keys
        keySet.SetMode(KeySet.Mode.Dev);

        StandardPublicExponent.CopyTo(keySet.NcaHeaderSigningKeys[0].PublicExponent.Data);
        StandardPublicExponent.CopyTo(keySet.NcaHeaderSigningKeys[1].PublicExponent.Data);
        NcaHdrFixedKeyModulus0Dev.CopyTo(keySet.NcaHeaderSigningKeys[0].Modulus.Data);
        NcaHdrFixedKeyModulus1Dev.CopyTo(keySet.NcaHeaderSigningKeys[1].Modulus.Data);

        StandardPublicExponent.CopyTo(keySet.AcidSigningKeys[0].PublicExponent.Data);
        StandardPublicExponent.CopyTo(keySet.AcidSigningKeys[1].PublicExponent.Data);
        AcidFixedKeyModulus0Dev.CopyTo(keySet.AcidSigningKeys[0].Modulus.Data);
        AcidFixedKeyModulus1Dev.CopyTo(keySet.AcidSigningKeys[1].Modulus.Data);

        StandardPublicExponent.CopyTo(keySet.Package2SigningKey.PublicExponent.Data);
        Package2FixedKeyModulusDev.CopyTo(keySet.Package2SigningKey.Modulus.Data);

        return keySet;
    }

    private static ReadOnlySpan<byte> StandardPublicExponent => new byte[]
    {
            0x01, 0x00, 0x01
    };

    private static ReadOnlySpan<byte> BetaNca0Modulus => new byte[]
    {
        0xAD, 0x58, 0xEE, 0x97, 0xF9, 0x47, 0x90, 0x7D, 0xF9, 0x29, 0x5F, 0x1F, 0x39, 0x68, 0xEE, 0x49,
        0x4C, 0x1E, 0x8D, 0x84, 0x91, 0x31, 0x5D, 0xE5, 0x96, 0x27, 0xB2, 0xB3, 0x59, 0x7B, 0xDE, 0xFD,
        0xB7, 0xEB, 0x40, 0xA1, 0xE7, 0xEB, 0xDC, 0x60, 0xD0, 0x3D, 0xC5, 0x50, 0x92, 0xAD, 0x3D, 0xC4,
        0x8C, 0x17, 0xD2, 0x37, 0x66, 0xE3, 0xF7, 0x14, 0x34, 0x38, 0x6B, 0xA7, 0x2B, 0x21, 0x10, 0x9B,
        0x73, 0x49, 0x15, 0xD9, 0x2A, 0x90, 0x86, 0x76, 0x81, 0x6A, 0x10, 0xBD, 0x74, 0xC4, 0x20, 0x55,
        0x25, 0xA8, 0x02, 0xC5, 0xA0, 0x34, 0x36, 0x7B, 0x66, 0x47, 0x2C, 0x7E, 0x47, 0x82, 0xA5, 0xD4,
        0xA3, 0x42, 0x45, 0xE8, 0xFD, 0x65, 0x72, 0x48, 0xA1, 0xB0, 0x44, 0x10, 0xEF, 0xAC, 0x1D, 0x0F,
        0xB5, 0x12, 0x19, 0xA8, 0x41, 0x0B, 0x76, 0x3B, 0xBC, 0xF1, 0x4A, 0x10, 0x46, 0x22, 0xB8, 0xF1,
        0xBC, 0x21, 0x81, 0x69, 0x9B, 0x63, 0x6F, 0xD7, 0xB9, 0x60, 0x2A, 0x9A, 0xE5, 0x2C, 0x47, 0x72,
        0x59, 0x65, 0xA2, 0x21, 0x60, 0xC4, 0xFC, 0xB0, 0xD7, 0x6F, 0x42, 0xC9, 0x0C, 0xF5, 0x76, 0x7D,
        0xF2, 0x5C, 0xE0, 0x80, 0x0F, 0xEE, 0x45, 0x7E, 0x4E, 0x3A, 0x8D, 0x9C, 0x5B, 0x5B, 0xD9, 0xD1,
        0x43, 0x94, 0x2C, 0xC7, 0x2E, 0xB9, 0x4A, 0xE5, 0x3E, 0x15, 0xDD, 0x43, 0x00, 0xF7, 0x78, 0xE7,
        0x7C, 0x39, 0xB0, 0x4D, 0xC5, 0xD1, 0x1C, 0xF2, 0xB4, 0x7A, 0x2A, 0xEA, 0x0A, 0x8E, 0xB9, 0x13,
        0xB4, 0x4F, 0xD7, 0x5B, 0x4D, 0x7B, 0x43, 0xB0, 0x3A, 0x9A, 0x60, 0x22, 0x47, 0x91, 0x78, 0xC7,
        0x10, 0x64, 0xE0, 0x2C, 0x69, 0xD1, 0x66, 0x3C, 0x42, 0x2E, 0xEF, 0x19, 0x21, 0x89, 0x8E, 0xE1,
        0xB0, 0xB4, 0xD0, 0x17, 0xA1, 0x0F, 0x73, 0x98, 0x5A, 0xF6, 0xEE, 0xC0, 0x2F, 0x9E, 0xCE, 0xC5
    };

    private static ReadOnlySpan<byte> BetaNca0Exponent => new byte[]
    {
        0x3C, 0x66, 0x37, 0x44, 0x26, 0xAC, 0x63, 0xD1, 0x30, 0xE6, 0xD4, 0x68, 0xF9, 0xC4, 0xF0, 0xFA,
        0x03, 0x16, 0xC6, 0x32, 0x81, 0xB0, 0x94, 0xC9, 0xF1, 0x26, 0xC5, 0xE2, 0x2D, 0xF4, 0xB6, 0x3E,
        0xEB, 0x3D, 0x82, 0x18, 0xA7, 0xC9, 0x8B, 0xD1, 0x03, 0xDD, 0xF2, 0x09, 0x60, 0x02, 0x12, 0xFA,
        0x8F, 0xE1, 0xA0, 0xF2, 0x82, 0xDC, 0x3D, 0x74, 0x01, 0xBA, 0x02, 0xF0, 0x8D, 0x5B, 0x89, 0x00,
        0xD1, 0x0B, 0x8F, 0x1C, 0x4A, 0xF3, 0x6E, 0x96, 0x8E, 0x03, 0x19, 0xF0, 0x19, 0x66, 0x58, 0xE9,
        0xB2, 0x24, 0x37, 0x4B, 0x0A, 0xC6, 0x06, 0x91, 0xBA, 0x92, 0x64, 0x13, 0x5F, 0xF1, 0x4A, 0xBC,
        0xAB, 0x61, 0xE5, 0x20, 0x08, 0x62, 0xB7, 0x8E, 0x4D, 0x20, 0x30, 0xA7, 0x42, 0x0B, 0x53, 0x58,
        0xEC, 0xBB, 0x70, 0xCB, 0x2A, 0x56, 0xC7, 0x0C, 0x8B, 0x89, 0xFB, 0x47, 0x6E, 0x58, 0x9C, 0xDD,
        0xB2, 0xE5, 0x4F, 0x49, 0x52, 0x0B, 0xD9, 0x96, 0x30, 0x8D, 0xDE, 0xC9, 0x0F, 0x6A, 0x82, 0xC7,
        0xE8, 0x20, 0xB6, 0xB3, 0x95, 0xDD, 0xEB, 0xDF, 0xF7, 0x25, 0x23, 0x6B, 0xF8, 0x5B, 0xD4, 0x81,
        0x7A, 0xBC, 0x94, 0x13, 0x30, 0x59, 0x28, 0xC8, 0xC9, 0x3A, 0x5D, 0xCC, 0x8D, 0xFD, 0x1A, 0xE1,
        0xCB, 0xA4, 0x1D, 0xD4, 0x45, 0xF1, 0xBF, 0x87, 0x6C, 0x0E, 0xB1, 0x44, 0xC7, 0x88, 0x62, 0x2B,
        0x43, 0xAD, 0x75, 0xE6, 0x69, 0xFF, 0xD3, 0x39, 0xF5, 0x7F, 0x2A, 0xA2, 0x5F, 0x7A, 0x5E, 0xE6,
        0xEF, 0xCB, 0x2F, 0x2C, 0x90, 0xE6, 0x4B, 0x2D, 0x94, 0x62, 0xE8, 0xEC, 0x54, 0x7B, 0x94, 0xB7,
        0xFB, 0x72, 0x05, 0xFB, 0xB3, 0x23, 0xCA, 0xF8, 0xD4, 0x5C, 0xF6, 0xAC, 0x7D, 0xEC, 0x47, 0xC6,
        0xD3, 0x22, 0x5D, 0x7C, 0x15, 0xDD, 0x48, 0xE9, 0xBF, 0xA8, 0x99, 0x33, 0x02, 0x79, 0xD3, 0x65
    };

    private static ReadOnlySpan<byte> NcaHdrFixedKeyModulus0Prod => new byte[]
    {
        0xBF, 0xBE, 0x40, 0x6C, 0xF4, 0xA7, 0x80, 0xE9, 0xF0, 0x7D, 0x0C, 0x99, 0x61, 0x1D, 0x77, 0x2F,
        0x96, 0xBC, 0x4B, 0x9E, 0x58, 0x38, 0x1B, 0x03, 0xAB, 0xB1, 0x75, 0x49, 0x9F, 0x2B, 0x4D, 0x58,
        0x34, 0xB0, 0x05, 0xA3, 0x75, 0x22, 0xBE, 0x1A, 0x3F, 0x03, 0x73, 0xAC, 0x70, 0x68, 0xD1, 0x16,
        0xB9, 0x04, 0x46, 0x5E, 0xB7, 0x07, 0x91, 0x2F, 0x07, 0x8B, 0x26, 0xDE, 0xF6, 0x00, 0x07, 0xB2,
        0xB4, 0x51, 0xF8, 0x0D, 0x0A, 0x5E, 0x58, 0xAD, 0xEB, 0xBC, 0x9A, 0xD6, 0x49, 0xB9, 0x64, 0xEF,
        0xA7, 0x82, 0xB5, 0xCF, 0x6D, 0x70, 0x13, 0xB0, 0x0F, 0x85, 0xF6, 0xA9, 0x08, 0xAA, 0x4D, 0x67,
        0x66, 0x87, 0xFA, 0x89, 0xFF, 0x75, 0x90, 0x18, 0x1E, 0x6B, 0x3D, 0xE9, 0x8A, 0x68, 0xC9, 0x26,
        0x04, 0xD9, 0x80, 0xCE, 0x3F, 0x5E, 0x92, 0xCE, 0x01, 0xFF, 0x06, 0x3B, 0xF2, 0xC1, 0xA9, 0x0C,
        0xCE, 0x02, 0x6F, 0x16, 0xBC, 0x92, 0x42, 0x0A, 0x41, 0x64, 0xCD, 0x52, 0xB6, 0x34, 0x4D, 0xAE,
        0xC0, 0x2E, 0xDE, 0xA4, 0xDF, 0x27, 0x68, 0x3C, 0xC1, 0xA0, 0x60, 0xAD, 0x43, 0xF3, 0xFC, 0x86,
        0xC1, 0x3E, 0x6C, 0x46, 0xF7, 0x7C, 0x29, 0x9F, 0xFA, 0xFD, 0xF0, 0xE3, 0xCE, 0x64, 0xE7, 0x35,
        0xF2, 0xF6, 0x56, 0x56, 0x6F, 0x6D, 0xF1, 0xE2, 0x42, 0xB0, 0x83, 0x40, 0xA5, 0xC3, 0x20, 0x2B,
        0xCC, 0x9A, 0xAE, 0xCA, 0xED, 0x4D, 0x70, 0x30, 0xA8, 0x70, 0x1C, 0x70, 0xFD, 0x13, 0x63, 0x29,
        0x02, 0x79, 0xEA, 0xD2, 0xA7, 0xAF, 0x35, 0x28, 0x32, 0x1C, 0x7B, 0xE6, 0x2F, 0x1A, 0xAA, 0x40,
        0x7E, 0x32, 0x8C, 0x27, 0x42, 0xFE, 0x82, 0x78, 0xEC, 0x0D, 0xEB, 0xE6, 0x83, 0x4B, 0x6D, 0x81,
        0x04, 0x40, 0x1A, 0x9E, 0x9A, 0x67, 0xF6, 0x72, 0x29, 0xFA, 0x04, 0xF0, 0x9D, 0xE4, 0xF4, 0x03
    };

    private static ReadOnlySpan<byte> NcaHdrFixedKeyModulus1Prod => new byte[]
    {
        0xAD, 0xE3, 0xE1, 0xFA, 0x04, 0x35, 0xE5, 0xB6, 0xDD, 0x49, 0xEA, 0x89, 0x29, 0xB1, 0xFF, 0xB6,
        0x43, 0xDF, 0xCA, 0x96, 0xA0, 0x4A, 0x13, 0xDF, 0x43, 0xD9, 0x94, 0x97, 0x96, 0x43, 0x65, 0x48,
        0x70, 0x58, 0x33, 0xA2, 0x7D, 0x35, 0x7B, 0x96, 0x74, 0x5E, 0x0B, 0x5C, 0x32, 0x18, 0x14, 0x24,
        0xC2, 0x58, 0xB3, 0x6C, 0x22, 0x7A, 0xA1, 0xB7, 0xCB, 0x90, 0xA7, 0xA3, 0xF9, 0x7D, 0x45, 0x16,
        0xA5, 0xC8, 0xED, 0x8F, 0xAD, 0x39, 0x5E, 0x9E, 0x4B, 0x51, 0x68, 0x7D, 0xF8, 0x0C, 0x35, 0xC6,
        0x3F, 0x91, 0xAE, 0x44, 0xA5, 0x92, 0x30, 0x0D, 0x46, 0xF8, 0x40, 0xFF, 0xD0, 0xFF, 0x06, 0xD2,
        0x1C, 0x7F, 0x96, 0x18, 0xDC, 0xB7, 0x1D, 0x66, 0x3E, 0xD1, 0x73, 0xBC, 0x15, 0x8A, 0x2F, 0x94,
        0xF3, 0x00, 0xC1, 0x83, 0xF1, 0xCD, 0xD7, 0x81, 0x88, 0xAB, 0xDF, 0x8C, 0xEF, 0x97, 0xDD, 0x1B,
        0x17, 0x5F, 0x58, 0xF6, 0x9A, 0xE9, 0xE8, 0xC2, 0x2F, 0x38, 0x15, 0xF5, 0x21, 0x07, 0xF8, 0x37,
        0x90, 0x5D, 0x2E, 0x02, 0x40, 0x24, 0x15, 0x0D, 0x25, 0xB7, 0x26, 0x5D, 0x09, 0xCC, 0x4C, 0xF4,
        0xF2, 0x1B, 0x94, 0x70, 0x5A, 0x9E, 0xEE, 0xED, 0x77, 0x77, 0xD4, 0x51, 0x99, 0xF5, 0xDC, 0x76,
        0x1E, 0xE3, 0x6C, 0x8C, 0xD1, 0x12, 0xD4, 0x57, 0xD1, 0xB6, 0x83, 0xE4, 0xE4, 0xFE, 0xDA, 0xE9,
        0xB4, 0x3B, 0x33, 0xE5, 0x37, 0x8A, 0xDF, 0xB5, 0x7F, 0x89, 0xF1, 0x9B, 0x9E, 0xB0, 0x15, 0xB2,
        0x3A, 0xFE, 0xEA, 0x61, 0x84, 0x5B, 0x7D, 0x4B, 0x23, 0x12, 0x0B, 0x83, 0x12, 0xF2, 0x22, 0x6B,
        0xB9, 0x22, 0x96, 0x4B, 0x26, 0x0B, 0x63, 0x5E, 0x96, 0x57, 0x52, 0xA3, 0x67, 0x64, 0x22, 0xCA,
        0xD0, 0x56, 0x3E, 0x74, 0xB5, 0x98, 0x1F, 0x0D, 0xF8, 0xB3, 0x34, 0xE6, 0x98, 0x68, 0x5A, 0xAD
    };

    private static ReadOnlySpan<byte> AcidFixedKeyModulus0Prod => new byte[]
    {
        0xDD, 0xC8, 0xDD, 0xF2, 0x4E, 0x6D, 0xF0, 0xCA, 0x9E, 0xC7, 0x5D, 0xC7, 0x7B, 0xAD, 0xFE, 0x7D,
        0x23, 0x89, 0x69, 0xB6, 0xF2, 0x06, 0xA2, 0x02, 0x88, 0xE1, 0x55, 0x91, 0xAB, 0xCB, 0x4D, 0x50,
        0x2E, 0xFC, 0x9D, 0x94, 0x76, 0xD6, 0x4C, 0xD8, 0xFF, 0x10, 0xFA, 0x5E, 0x93, 0x0A, 0xB4, 0x57,
        0xAC, 0x51, 0xC7, 0x16, 0x66, 0xF4, 0x1A, 0x54, 0xC2, 0xC5, 0x04, 0x3D, 0x1B, 0xFE, 0x30, 0x20,
        0x8A, 0xAC, 0x6F, 0x6F, 0xF5, 0xC7, 0xB6, 0x68, 0xB8, 0xC9, 0x40, 0x6B, 0x42, 0xAD, 0x11, 0x21,
        0xE7, 0x8B, 0xE9, 0x75, 0x01, 0x86, 0xE4, 0x48, 0x9B, 0x0A, 0x0A, 0xF8, 0x7F, 0xE8, 0x87, 0xF2,
        0x82, 0x01, 0xE6, 0xA3, 0x0F, 0xE4, 0x66, 0xAE, 0x83, 0x3F, 0x4E, 0x9F, 0x5E, 0x01, 0x30, 0xA4,
        0x00, 0xB9, 0x9A, 0xAE, 0x5F, 0x03, 0xCC, 0x18, 0x60, 0xE5, 0xEF, 0x3B, 0x5E, 0x15, 0x16, 0xFE,
        0x1C, 0x82, 0x78, 0xB5, 0x2F, 0x47, 0x7C, 0x06, 0x66, 0x88, 0x5D, 0x35, 0xA2, 0x67, 0x20, 0x10,
        0xE7, 0x6C, 0x43, 0x68, 0xD3, 0xE4, 0x5A, 0x68, 0x2A, 0x5A, 0xE2, 0x6D, 0x73, 0xB0, 0x31, 0x53,
        0x1C, 0x20, 0x09, 0x44, 0xF5, 0x1A, 0x9D, 0x22, 0xBE, 0x12, 0xA1, 0x77, 0x11, 0xE2, 0xA1, 0xCD,
        0x40, 0x9A, 0xA2, 0x8B, 0x60, 0x9B, 0xEF, 0xA0, 0xD3, 0x48, 0x63, 0xA2, 0xF8, 0xA3, 0x2C, 0x08,
        0x56, 0x52, 0x2E, 0x60, 0x19, 0x67, 0x5A, 0xA7, 0x9F, 0xDC, 0x3F, 0x3F, 0x69, 0x2B, 0x31, 0x6A,
        0xB7, 0x88, 0x4A, 0x14, 0x84, 0x80, 0x33, 0x3C, 0x9D, 0x44, 0xB7, 0x3F, 0x4C, 0xE1, 0x75, 0xEA,
        0x37, 0xEA, 0xE8, 0x1E, 0x7C, 0x77, 0xB7, 0xC6, 0x1A, 0xA2, 0xF0, 0x9F, 0x10, 0x61, 0xCD, 0x7B,
        0x5B, 0x32, 0x4C, 0x37, 0xEF, 0xB1, 0x71, 0x68, 0x53, 0x0A, 0xED, 0x51, 0x7D, 0x35, 0x22, 0xFD
    };

    private static ReadOnlySpan<byte> AcidFixedKeyModulus1Prod => new byte[]
    {
        0xE7, 0xAA, 0x25, 0xC8, 0x01, 0xA5, 0x14, 0x6B, 0x01, 0x60, 0x3E, 0xD9, 0x96, 0x5A, 0xBF, 0x90,
        0xAC, 0xA7, 0xFD, 0x9B, 0x5B, 0xBD, 0x8A, 0x26, 0xB0, 0xCB, 0x20, 0x28, 0x9A, 0x72, 0x12, 0xF5,
        0x20, 0x65, 0xB3, 0xB9, 0x84, 0x58, 0x1F, 0x27, 0xBC, 0x7C, 0xA2, 0xC9, 0x9E, 0x18, 0x95, 0xCF,
        0xC2, 0x73, 0x2E, 0x74, 0x8C, 0x66, 0xE5, 0x9E, 0x79, 0x2B, 0xB8, 0x07, 0x0C, 0xB0, 0x4E, 0x8E,
        0xAB, 0x85, 0x21, 0x42, 0xC4, 0xC5, 0x6D, 0x88, 0x9C, 0xDB, 0x15, 0x95, 0x3F, 0x80, 0xDB, 0x7A,
        0x9A, 0x7D, 0x41, 0x56, 0x25, 0x17, 0x18, 0x42, 0x4D, 0x8C, 0xAC, 0xA5, 0x7B, 0xDB, 0x42, 0x5D,
        0x59, 0x35, 0x45, 0x5D, 0x8A, 0x02, 0xB5, 0x70, 0xC0, 0x72, 0x35, 0x46, 0xD0, 0x1D, 0x60, 0x01,
        0x4A, 0xCC, 0x1C, 0x46, 0xD3, 0xD6, 0x35, 0x52, 0xD6, 0xE1, 0xF8, 0x3B, 0x5D, 0xEA, 0xDD, 0xB8,
        0xFE, 0x7D, 0x50, 0xCB, 0x35, 0x23, 0x67, 0x8B, 0xB6, 0xE4, 0x74, 0xD2, 0x60, 0xFC, 0xFD, 0x43,
        0xBF, 0x91, 0x08, 0x81, 0xC5, 0x4F, 0x5D, 0x16, 0x9A, 0xC4, 0x9A, 0xC6, 0xF6, 0xF3, 0xE1, 0xF6,
        0x5C, 0x07, 0xAA, 0x71, 0x6C, 0x13, 0xA4, 0xB1, 0xB3, 0x66, 0xBF, 0x90, 0x4C, 0x3D, 0xA2, 0xC4,
        0x0B, 0xB8, 0x3D, 0x7A, 0x8C, 0x19, 0xFA, 0xFF, 0x6B, 0xB9, 0x1F, 0x02, 0xCC, 0xB6, 0xD3, 0x0C,
        0x7D, 0x19, 0x1F, 0x47, 0xF9, 0xC7, 0x40, 0x01, 0xFA, 0x46, 0xEA, 0x0B, 0xD4, 0x02, 0xE0, 0x3D,
        0x30, 0x9A, 0x1A, 0x0F, 0xEA, 0xA7, 0x66, 0x55, 0xF7, 0xCB, 0x28, 0xE2, 0xBB, 0x99, 0xE4, 0x83,
        0xC3, 0x43, 0x03, 0xEE, 0xDC, 0x1F, 0x02, 0x23, 0xDD, 0xD1, 0x2D, 0x39, 0xA4, 0x65, 0x75, 0x03,
        0xEF, 0x37, 0x9C, 0x06, 0xD6, 0xFA, 0xA1, 0x15, 0xF0, 0xDB, 0x17, 0x47, 0x26, 0x4F, 0x49, 0x03
    };

    private static ReadOnlySpan<byte> Package2FixedKeyModulusProd => new byte[]
    {
        0x8D, 0x13, 0xA7, 0x77, 0x6A, 0xE5, 0xDC, 0xC0, 0x3B, 0x25, 0xD0, 0x58, 0xE4, 0x20, 0x69, 0x59,
        0x55, 0x4B, 0xAB, 0x70, 0x40, 0x08, 0x28, 0x07, 0xA8, 0xA7, 0xFD, 0x0F, 0x31, 0x2E, 0x11, 0xFE,
        0x47, 0xA0, 0xF9, 0x9D, 0xDF, 0x80, 0xDB, 0x86, 0x5A, 0x27, 0x89, 0xCD, 0x97, 0x6C, 0x85, 0xC5,
        0x6C, 0x39, 0x7F, 0x41, 0xF2, 0xFF, 0x24, 0x20, 0xC3, 0x95, 0xA6, 0xF7, 0x9D, 0x4A, 0x45, 0x74,
        0x8B, 0x5D, 0x28, 0x8A, 0xC6, 0x99, 0x35, 0x68, 0x85, 0xA5, 0x64, 0x32, 0x80, 0x9F, 0xD3, 0x48,
        0x39, 0xA2, 0x1D, 0x24, 0x67, 0x69, 0xDF, 0x75, 0xAC, 0x12, 0xB5, 0xBD, 0xC3, 0x29, 0x90, 0xBE,
        0x37, 0xE4, 0xA0, 0x80, 0x9A, 0xBE, 0x36, 0xBF, 0x1F, 0x2C, 0xAB, 0x2B, 0xAD, 0xF5, 0x97, 0x32,
        0x9A, 0x42, 0x9D, 0x09, 0x8B, 0x08, 0xF0, 0x63, 0x47, 0xA3, 0xE9, 0x1B, 0x36, 0xD8, 0x2D, 0x8A,
        0xD7, 0xE1, 0x54, 0x11, 0x95, 0xE4, 0x45, 0x88, 0x69, 0x8A, 0x2B, 0x35, 0xCE, 0xD0, 0xA5, 0x0B,
        0xD5, 0x5D, 0xAC, 0xDB, 0xAF, 0x11, 0x4D, 0xCA, 0xB8, 0x1E, 0xE7, 0x01, 0x9E, 0xF4, 0x46, 0xA3,
        0x8A, 0x94, 0x6D, 0x76, 0xBD, 0x8A, 0xC8, 0x3B, 0xD2, 0x31, 0x58, 0x0C, 0x79, 0xA8, 0x26, 0xE9,
        0xD1, 0x79, 0x9C, 0xCB, 0xD4, 0x2B, 0x6A, 0x4F, 0xC6, 0xCC, 0xCF, 0x90, 0xA7, 0xB9, 0x98, 0x47,
        0xFD, 0xFA, 0x4C, 0x6C, 0x6F, 0x81, 0x87, 0x3B, 0xCA, 0xB8, 0x50, 0xF6, 0x3E, 0x39, 0x5D, 0x4D,
        0x97, 0x3F, 0x0F, 0x35, 0x39, 0x53, 0xFB, 0xFA, 0xCD, 0xAB, 0xA8, 0x7A, 0x62, 0x9A, 0x3F, 0xF2,
        0x09, 0x27, 0x96, 0x3F, 0x07, 0x9A, 0x91, 0xF7, 0x16, 0xBF, 0xC6, 0x3A, 0x82, 0x5A, 0x4B, 0xCF,
        0x49, 0x50, 0x95, 0x8C, 0x55, 0x80, 0x7E, 0x39, 0xB1, 0x48, 0x05, 0x1E, 0x21, 0xC7, 0x24, 0x4F
    };

    private static ReadOnlySpan<byte> NcaHdrFixedKeyModulus0Dev => new byte[]
    {
        0xD8, 0xF1, 0x18, 0xEF, 0x32, 0x72, 0x4C, 0xA7, 0x47, 0x4C, 0xB9, 0xEA, 0xB3, 0x04, 0xA8, 0xA4,
        0xAC, 0x99, 0x08, 0x08, 0x04, 0xBF, 0x68, 0x57, 0xB8, 0x43, 0x94, 0x2B, 0xC7, 0xB9, 0x66, 0x49,
        0x85, 0xE5, 0x8A, 0x9B, 0xC1, 0x00, 0x9A, 0x6A, 0x8D, 0xD0, 0xEF, 0xCE, 0xFF, 0x86, 0xC8, 0x5C,
        0x5D, 0xE9, 0x53, 0x7B, 0x19, 0x2A, 0xA8, 0xC0, 0x22, 0xD1, 0xF3, 0x22, 0x0A, 0x50, 0xF2, 0x2B,
        0x65, 0x05, 0x1B, 0x9E, 0xEC, 0x61, 0xB5, 0x63, 0xA3, 0x6F, 0x3B, 0xBA, 0x63, 0x3A, 0x53, 0xF4,
        0x49, 0x2F, 0xCF, 0x03, 0xCC, 0xD7, 0x50, 0x82, 0x1B, 0x29, 0x4F, 0x08, 0xDE, 0x1B, 0x6D, 0x47,
        0x4F, 0xA8, 0xB6, 0x6A, 0x26, 0xA0, 0x83, 0x3F, 0x1A, 0xAF, 0x83, 0x8F, 0x0E, 0x17, 0x3F, 0xFE,
        0x44, 0x1C, 0x56, 0x94, 0x2E, 0x49, 0x83, 0x83, 0x03, 0xE9, 0xB6, 0xAD, 0xD5, 0xDE, 0xE3, 0x2D,
        0xA1, 0xD9, 0x66, 0x20, 0x5D, 0x1F, 0x5E, 0x96, 0x5D, 0x5B, 0x55, 0x0D, 0xD4, 0xB4, 0x77, 0x6E,
        0xAE, 0x1B, 0x69, 0xF3, 0xA6, 0x61, 0x0E, 0x51, 0x62, 0x39, 0x28, 0x63, 0x75, 0x76, 0xBF, 0xB0,
        0xD2, 0x22, 0xEF, 0x98, 0x25, 0x02, 0x05, 0xC0, 0xD7, 0x6A, 0x06, 0x2C, 0xA5, 0xD8, 0x5A, 0x9D,
        0x7A, 0xA4, 0x21, 0x55, 0x9F, 0xF9, 0x3E, 0xBF, 0x16, 0xF6, 0x07, 0xC2, 0xB9, 0x6E, 0x87, 0x9E,
        0xB5, 0x1C, 0xBE, 0x97, 0xFA, 0x82, 0x7E, 0xED, 0x30, 0xD4, 0x66, 0x3F, 0xDE, 0xD8, 0x1B, 0x4B,
        0x15, 0xD9, 0xFB, 0x2F, 0x50, 0xF0, 0x9D, 0x1D, 0x52, 0x4C, 0x1C, 0x4D, 0x8D, 0xAE, 0x85, 0x1E,
        0xEA, 0x7F, 0x86, 0xF3, 0x0B, 0x7B, 0x87, 0x81, 0x98, 0x23, 0x80, 0x63, 0x4F, 0x2F, 0xB0, 0x62,
        0xCC, 0x6E, 0xD2, 0x46, 0x13, 0x65, 0x2B, 0xD6, 0x44, 0x33, 0x59, 0xB5, 0x8F, 0xB9, 0x4A, 0xA9
    };

    private static ReadOnlySpan<byte> NcaHdrFixedKeyModulus1Dev => new byte[]
    {
        0x9A, 0xBC, 0x88, 0xBD, 0x0A, 0xBE, 0xD7, 0x0C, 0x9B, 0x42, 0x75, 0x65, 0x38, 0x5E, 0xD1, 0x01,
        0xCD, 0x12, 0xAE, 0xEA, 0xE9, 0x4B, 0xDB, 0xB4, 0x5E, 0x36, 0x10, 0x96, 0xDA, 0x3D, 0x2E, 0x66,
        0xD3, 0x99, 0x13, 0x8A, 0xBE, 0x67, 0x41, 0xC8, 0x93, 0xD9, 0x3E, 0x42, 0xCE, 0x34, 0xCE, 0x96,
        0xFA, 0x0B, 0x23, 0xCC, 0x2C, 0xDF, 0x07, 0x3F, 0x3B, 0x24, 0x4B, 0x12, 0x67, 0x3A, 0x29, 0x36,
        0xA3, 0xAA, 0x06, 0xF0, 0x65, 0xA5, 0x85, 0xBA, 0xFD, 0x12, 0xEC, 0xF1, 0x60, 0x67, 0xF0, 0x8F,
        0xD3, 0x5B, 0x01, 0x1B, 0x1E, 0x84, 0xA3, 0x5C, 0x65, 0x36, 0xF9, 0x23, 0x7E, 0xF3, 0x26, 0x38,
        0x64, 0x98, 0xBA, 0xE4, 0x19, 0x91, 0x4C, 0x02, 0xCF, 0xC9, 0x6D, 0x86, 0xEC, 0x1D, 0x41, 0x69,
        0xDD, 0x56, 0xEA, 0x5C, 0xA3, 0x2A, 0x58, 0xB4, 0x39, 0xCC, 0x40, 0x31, 0xFD, 0xFB, 0x42, 0x74,
        0xF8, 0xEC, 0xEA, 0x00, 0xF0, 0xD9, 0x28, 0xEA, 0xFA, 0x2D, 0x00, 0xE1, 0x43, 0x53, 0xC6, 0x32,
        0xF4, 0xA2, 0x07, 0xD4, 0x5F, 0xD4, 0xCB, 0xAC, 0xCA, 0xFF, 0xDF, 0x84, 0xD2, 0x86, 0x14, 0x3C,
        0xDE, 0x22, 0x75, 0xA5, 0x73, 0xFF, 0x68, 0x07, 0x4A, 0xF9, 0x7C, 0x2C, 0xCC, 0xDE, 0x45, 0xB6,
        0x54, 0x82, 0x90, 0x36, 0x1F, 0x2C, 0x51, 0x96, 0xC5, 0x0A, 0x53, 0x5B, 0xF0, 0x8B, 0x4A, 0xAA,
        0x3B, 0x68, 0x97, 0x19, 0x17, 0x1F, 0x01, 0xB8, 0xED, 0xB9, 0x9A, 0x5E, 0x08, 0xC5, 0x20, 0x1E,
        0x6A, 0x09, 0xF0, 0xE9, 0x73, 0xA3, 0xBE, 0x10, 0x06, 0x02, 0xE9, 0xFB, 0x85, 0xFA, 0x5F, 0x01,
        0xAC, 0x60, 0xE0, 0xED, 0x7D, 0xB9, 0x49, 0xA8, 0x9E, 0x98, 0x7D, 0x91, 0x40, 0x05, 0xCF, 0xF9,
        0x1A, 0xFC, 0x40, 0x22, 0xA8, 0x96, 0x5B, 0xB0, 0xDC, 0x7A, 0xF5, 0xB7, 0xE9, 0x91, 0x4C, 0x49
    };

    private static ReadOnlySpan<byte> AcidFixedKeyModulus0Dev => new byte[]
    {
        0xD6, 0x34, 0xA5, 0x78, 0x6C, 0x68, 0xCE, 0x5A, 0xC2, 0x37, 0x17, 0xF3, 0x82, 0x45, 0xC6, 0x89,
        0xE1, 0x2D, 0x06, 0x67, 0xBF, 0xB4, 0x06, 0x19, 0x55, 0x6B, 0x27, 0x66, 0x0C, 0xA4, 0xB5, 0x87,
        0x81, 0x25, 0xF4, 0x30, 0xBC, 0x53, 0x08, 0x68, 0xA2, 0x48, 0x49, 0x8C, 0x3F, 0x38, 0x40, 0x9C,
        0xC4, 0x26, 0xF4, 0x79, 0xE2, 0xA1, 0x85, 0xF5, 0x5C, 0x7F, 0x58, 0xBA, 0xA6, 0x1C, 0xA0, 0x8B,
        0x84, 0x16, 0x14, 0x6F, 0x85, 0xD9, 0x7C, 0xE1, 0x3C, 0x67, 0x22, 0x1E, 0xFB, 0xD8, 0xA7, 0xA5,
        0x9A, 0xBF, 0xEC, 0x0E, 0xCF, 0x96, 0x7E, 0x85, 0xC2, 0x1D, 0x49, 0x5D, 0x54, 0x26, 0xCB, 0x32,
        0x7C, 0xF6, 0xBB, 0x58, 0x03, 0x80, 0x2B, 0x5D, 0xF7, 0xFB, 0xD1, 0x9D, 0xC7, 0xC6, 0x2E, 0x53,
        0xC0, 0x6F, 0x39, 0x2C, 0x1F, 0xA9, 0x92, 0xF2, 0x4D, 0x7D, 0x4E, 0x74, 0xFF, 0xE4, 0xEF, 0xE4,
        0x7C, 0x3D, 0x34, 0x2A, 0x71, 0xA4, 0x97, 0x59, 0xFF, 0x4F, 0xA2, 0xF4, 0x66, 0x78, 0xD8, 0xBA,
        0x99, 0xE3, 0xE6, 0xDB, 0x54, 0xB9, 0xE9, 0x54, 0xA1, 0x70, 0xFC, 0x05, 0x1F, 0x11, 0x67, 0x4B,
        0x26, 0x8C, 0x0C, 0x3E, 0x03, 0xD2, 0xA3, 0x55, 0x5C, 0x7D, 0xC0, 0x5D, 0x9D, 0xFF, 0x13, 0x2F,
        0xFD, 0x19, 0xBF, 0xED, 0x44, 0xC3, 0x8C, 0xA7, 0x28, 0xCB, 0xE5, 0xE0, 0xB1, 0xA7, 0x9C, 0x33,
        0x8D, 0xB8, 0x6E, 0xDE, 0x87, 0x18, 0x22, 0x60, 0xC4, 0xAE, 0xF2, 0x87, 0x9F, 0xCE, 0x09, 0x5C,
        0xB5, 0x99, 0xA5, 0x9F, 0x49, 0xF2, 0xD7, 0x58, 0xFA, 0xF9, 0xC0, 0x25, 0x7D, 0xD6, 0xCB, 0xF3,
        0xD8, 0x6C, 0xA2, 0x69, 0x91, 0x68, 0x73, 0xB1, 0x94, 0x6F, 0xA3, 0xF3, 0xB9, 0x7D, 0xF8, 0xE0,
        0x72, 0x9E, 0x93, 0x7B, 0x7A, 0xA2, 0x57, 0x60, 0xB7, 0x5B, 0xA9, 0x84, 0xAE, 0x64, 0x88, 0x69
    };

    private static ReadOnlySpan<byte> AcidFixedKeyModulus1Dev => new byte[]
    {
        0xBC, 0xA5, 0x6A, 0x7E, 0xEA, 0x38, 0x34, 0x62, 0xA6, 0x10, 0x18, 0x3C, 0xE1, 0x63, 0x7B, 0xF0,
        0xD3, 0x08, 0x8C, 0xF5, 0xC5, 0xC4, 0xC7, 0x93, 0xE9, 0xD9, 0xE6, 0x32, 0xF3, 0xA0, 0xF6, 0x6E,
        0x8A, 0x98, 0x76, 0x47, 0x33, 0x47, 0x65, 0x02, 0x70, 0xDC, 0x86, 0x5F, 0x3D, 0x61, 0x5A, 0x70,
        0xBC, 0x5A, 0xCA, 0xCA, 0x50, 0xAD, 0x61, 0x7E, 0xC9, 0xEC, 0x27, 0xFF, 0xE8, 0x64, 0x42, 0x9A,
        0xEE, 0xBE, 0xC3, 0xD1, 0x0B, 0xC0, 0xE9, 0xBF, 0x83, 0x8D, 0xC0, 0x0C, 0xD8, 0x00, 0x5B, 0x76,
        0x90, 0xD2, 0x4B, 0x30, 0x84, 0x35, 0x8B, 0x1E, 0x20, 0xB7, 0xE4, 0xDC, 0x63, 0xE5, 0xDF, 0xCD,
        0x00, 0x5F, 0x81, 0x5F, 0x67, 0xC5, 0x8B, 0xDF, 0xFC, 0xE1, 0x37, 0x5F, 0x07, 0xD9, 0xDE, 0x4F,
        0xE6, 0x7B, 0xF1, 0xFB, 0xA1, 0x5A, 0x71, 0x40, 0xFE, 0xBA, 0x1E, 0xAE, 0x13, 0x22, 0xD2, 0xFE,
        0x37, 0xA2, 0xB6, 0x8B, 0xAB, 0xEB, 0x84, 0x81, 0x4E, 0x7C, 0x1E, 0x02, 0xD1, 0xFB, 0xD7, 0x5D,
        0x11, 0x84, 0x64, 0xD2, 0x4D, 0xBB, 0x50, 0x00, 0x67, 0x54, 0xE2, 0x77, 0x89, 0xBA, 0x0B, 0xE7,
        0x05, 0x57, 0x9A, 0x22, 0x5A, 0xEC, 0x76, 0x1C, 0xFD, 0xE8, 0xA8, 0x18, 0x16, 0x41, 0x65, 0x03,
        0xFA, 0xC4, 0xA6, 0x31, 0x5C, 0x1A, 0x7F, 0xAB, 0x11, 0xC8, 0x4A, 0x99, 0xB9, 0xE6, 0xCF, 0x62,
        0x21, 0xA6, 0x72, 0x47, 0xDB, 0xBA, 0x96, 0x26, 0x4E, 0x2E, 0xD4, 0x8C, 0x46, 0xD6, 0xA7, 0x1A,
        0x6C, 0x32, 0xA7, 0xDF, 0x85, 0x1C, 0x03, 0xC3, 0x6D, 0xA9, 0xE9, 0x68, 0xF4, 0x17, 0x1E, 0xB2,
        0x70, 0x2A, 0xA1, 0xE5, 0xE1, 0xF3, 0x8F, 0x6F, 0x63, 0xAC, 0xEB, 0x72, 0x0B, 0x4C, 0x4A, 0x36,
        0x3C, 0x60, 0x91, 0x9F, 0x6E, 0x1C, 0x71, 0xEA, 0xD0, 0x78, 0x78, 0xA0, 0x2E, 0xC6, 0x32, 0x6B
    };

    private static ReadOnlySpan<byte> Package2FixedKeyModulusDev => new byte[]
    {
        0xB3, 0x65, 0x54, 0xFB, 0x0A, 0xB0, 0x1E, 0x85, 0xA7, 0xF6, 0xCF, 0x91, 0x8E, 0xBA, 0x96, 0x99,
        0x0D, 0x8B, 0x91, 0x69, 0x2A, 0xEE, 0x01, 0x20, 0x4F, 0x34, 0x5C, 0x2C, 0x4F, 0x4E, 0x37, 0xC7,
        0xF1, 0x0B, 0xD4, 0xCD, 0xA1, 0x7F, 0x93, 0xF1, 0x33, 0x59, 0xCE, 0xB1, 0xE9, 0xDD, 0x26, 0xE6,
        0xF3, 0xBB, 0x77, 0x87, 0x46, 0x7A, 0xD6, 0x4E, 0x47, 0x4A, 0xD1, 0x41, 0xB7, 0x79, 0x4A, 0x38,
        0x06, 0x6E, 0xCF, 0x61, 0x8F, 0xCD, 0xC1, 0x40, 0x0B, 0xFA, 0x26, 0xDC, 0xC0, 0x34, 0x51, 0x83,
        0xD9, 0x3B, 0x11, 0x54, 0x3B, 0x96, 0x27, 0x32, 0x9A, 0x95, 0xBE, 0x1E, 0x68, 0x11, 0x50, 0xA0,
        0x6B, 0x10, 0xA8, 0x83, 0x8B, 0xF5, 0xFC, 0xBC, 0x90, 0x84, 0x7A, 0x5A, 0x5C, 0x43, 0x52, 0xE6,
        0xC8, 0x26, 0xE9, 0xFE, 0x06, 0xA0, 0x8B, 0x53, 0x0F, 0xAF, 0x1E, 0xC4, 0x1C, 0x0B, 0xCF, 0x50,
        0x1A, 0xA4, 0xF3, 0x5C, 0xFB, 0xF0, 0x97, 0xE4, 0xDE, 0x32, 0x0A, 0x9F, 0xE3, 0x5A, 0xAA, 0xB7,
        0x44, 0x7F, 0x5C, 0x33, 0x60, 0xB9, 0x0F, 0x22, 0x2D, 0x33, 0x2A, 0xE9, 0x69, 0x79, 0x31, 0x42,
        0x8F, 0xE4, 0x3A, 0x13, 0x8B, 0xE7, 0x26, 0xBD, 0x08, 0x87, 0x6C, 0xA6, 0xF2, 0x73, 0xF6, 0x8E,
        0xA7, 0xF2, 0xFE, 0xFB, 0x6C, 0x28, 0x66, 0x0D, 0xBD, 0xD7, 0xEB, 0x42, 0xA8, 0x78, 0xE6, 0xB8,
        0x6B, 0xAE, 0xC7, 0xA9, 0xE2, 0x40, 0x6E, 0x89, 0x20, 0x82, 0x25, 0x8E, 0x3C, 0x6A, 0x60, 0xD7,
        0xF3, 0x56, 0x8E, 0xEC, 0x8D, 0x51, 0x8A, 0x63, 0x3C, 0x04, 0x78, 0x23, 0x0E, 0x90, 0x0C, 0xB4,
        0xE7, 0x86, 0x3B, 0x4F, 0x8E, 0x13, 0x09, 0x47, 0x32, 0x0E, 0x04, 0xB8, 0x4D, 0x5B, 0xB0, 0x46,
        0x71, 0xB0, 0x5C, 0xF4, 0xAD, 0x63, 0x4F, 0xC5, 0xE2, 0xAC, 0x1E, 0xC4, 0x33, 0x96, 0x09, 0x7B
    };
}