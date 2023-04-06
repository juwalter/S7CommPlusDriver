﻿#region License
/******************************************************************************
 * S7CommPlusDriver
 * 
 * Copyright (C) 2023 Thomas Wiens, th.wiens@gmx.de
 *
 * This file is part of S7CommPlusDriver.
 *
 * S7CommPlusDriver is free software: you can redistribute it and/or modify
 * it under the terms of the GNU Lesser General Public License as
 * published by the Free Software Foundation, either version 3 of the
 * License, or (at your option) any later version.
 /****************************************************************************/
#endregion

using System;
using System.Collections.Generic;
using System.IO;

namespace S7CommPlusDriver
{
    class GetMultiVariablesRequest : IS7pSendableObject
    {
        public byte ProtocolVersion;
        public UInt16 SequenceNumber;
        public UInt32 SessionId;
        byte TransportFlags = 0x34;
        public UInt32 LinkId = 0;       // für Variablen lesen = 0
        public List<ItemAddress> AddressList = new List<ItemAddress>();

        public bool WithIntegrityId = true;
        public UInt32 IntegrityId;

        public GetMultiVariablesRequest(byte protocolVersion)
        {
            ProtocolVersion = protocolVersion;
        }

        public byte GetProtocolVersion()
        {
            return ProtocolVersion;
        }

        public int Serialize(Stream buffer)
        {
            int ret = 0;
            UInt32 fieldCount = 0;
            ret += S7p.EncodeByte(buffer, Opcode.Request);
            ret += S7p.EncodeUInt16(buffer, 0);                               // Reserved
            ret += S7p.EncodeUInt16(buffer, Functioncode.GetMultiVariables);
            ret += S7p.EncodeUInt16(buffer, 0);                               // Reserved
            ret += S7p.EncodeUInt16(buffer, SequenceNumber);
            ret += S7p.EncodeUInt32(buffer, SessionId);
            ret += S7p.EncodeByte(buffer, TransportFlags);

            // Request set
            ret += S7p.EncodeUInt32(buffer, LinkId);
            ret += S7p.EncodeUInt32Vlq(buffer, (UInt32)AddressList.Count);
            foreach (ItemAddress adr in AddressList)
            {
                fieldCount += adr.GetNumberOfFields();
            }
            ret += S7p.EncodeUInt32Vlq(buffer, fieldCount);

            foreach (ItemAddress adr in AddressList)
            {
                ret += adr.Serialize(buffer);
            }
            ret += S7p.EncodeObjectQualifier(buffer);

            if (WithIntegrityId)
            {
                ret += S7p.EncodeUInt32Vlq(buffer, IntegrityId);
            }
            // Füllbytes?
            ret += S7p.EncodeUInt32(buffer, 0);

            return ret;
        }

        public override string ToString()
        {
            string s = "";
            s += "<GetMultiVariablesRequest>" + Environment.NewLine;
            s += "<ProtocolVersion>" + ProtocolVersion.ToString() + "</ProtocolVersion>" + Environment.NewLine;
            s += "<SequenceNumber>" + SequenceNumber.ToString() + "</SequenceNumber>" + Environment.NewLine;
            s += "<SessionId>" + SessionId.ToString() + "</SessionId>" + Environment.NewLine;
            s += "<TransportFlags>" + TransportFlags.ToString() + "</TransportFlags>" + Environment.NewLine;
            s += "<RequestSet>" + Environment.NewLine;
            s += "<LinkId>" + LinkId.ToString() + "</LinkId>" + Environment.NewLine;
            s += "<ItemCount>" + AddressList.Count.ToString() + "</ItemCount>" + Environment.NewLine;
            UInt32 fieldCount = 0;
            foreach (ItemAddress adr in AddressList)
            {
                fieldCount += adr.GetNumberOfFields();
            }
            s += "<NumberOfFields>" + fieldCount.ToString() + "</NumberOfFields>" + Environment.NewLine;
            s += "<AddressList>" + Environment.NewLine;
            foreach (ItemAddress adr in AddressList)
            {
                s += adr.ToString();
            }
            s += "</AddressList>" + Environment.NewLine;
            s += "</RequestSet>" + Environment.NewLine;
            s += "</GetMultiVariablesRequest>" + Environment.NewLine;
            return s;
        }
    }
}