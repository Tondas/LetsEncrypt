using LetsEncrypt.Core.Entities;
using System;

namespace LetsEncrypt.Test
{
    public class BaseUT : Startup
    {
        protected string ContactEmail = "au@turingion.com";
        protected Uri EnviromentUri = ApiEnvironment.LetsEncryptV2Staging;
    }
}