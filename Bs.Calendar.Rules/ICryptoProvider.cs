﻿namespace Bs.Calendar.Rules
{
    public interface ICryptoProvider
    {
        string GetKeccakHash(string data);
        string GetMd5Hash(string data);
    }
}