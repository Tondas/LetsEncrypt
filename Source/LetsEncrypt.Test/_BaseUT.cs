using LetsEncrypt.Client.Entities;
using System;
using System.Collections.Generic;

namespace LetsEncrypt.Test
{
    public class BaseUT : Startup
    {
        protected string ContactEmail = "au@turingion.com";
        protected Uri EnviromentUri = ApiEnvironment.LetsEncryptV2Staging;
        protected List<string> Identifiers = new List<string> { "suppo.biz", "*.suppo.biz" };
    }
}