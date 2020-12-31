# Let's Encrypt C# library

[![NuGet](https://img.shields.io/nuget/dt/LetsEncrypt.Client.svg)](https://www.nuget.org/packages/LetsEncrypt.Client) 
[![NuGet](https://img.shields.io/nuget/vpre/LetsEncrypt.Client.svg)](https://www.nuget.org/packages/LetsEncrypt.Client)

Solution consist of 2 projects:
* **LetsEncrypt.Client** (.Net Standard Library - available as [nuget package](https://www.nuget.org/packages/LetsEncrypt.Client/1.0.0))
* **LetsEncrypt.ConsoleApp** (.Net Core Console application)

#### LetsEncrypt.Client

LetsEncrypt.Client is simple and straightforward C# implementation of [ACME](https://en.wikipedia.org/wiki/Automated_Certificate_Management_Environment) client for [Let's Encrypt](https://letsencrypt.org/) certificates. Library is based on **.NET Standard 2.1+**.
It uses Let's Encrypt **v2 API** and this library is primary oriented for generation of **wildcard** certificates as .pfx. 

#### LetsEncrypt.ConsoleApp

LetsEncrypt.ConsoleApp is C# implementation|usage of previous LetsEncrypt.Client library based on **.NET Core 3.1**. It is simple **console application** which generates Let's Encrypt certificates. 


## LetsEncrypt.Client

### Usage

Add [LetsEncrypt.Client](https://www.nuget.org/packages/LetsEncrypt.Client/1.0.0) as nuget package (or manual **.dll reference**) to your project.

First step is to create client object to specific environment ([staging](https://letsencrypt.org/docs/staging-environment/) or production ... use staging environment first to avoid [rate limits](https://letsencrypt.org/docs/rate-limits/)):

```cs
var acmeClient = new AcmeClient(ApiEnvironment.LetsEncryptV2Staging);
```

... and let's start:

### Account

Create new account: 
```cs
var account = await acmeClient.CreateNewAccountAsync("your@email.com");
```

### Order

When you want to generate wildcard certificate, I recommend to specify these 2 identifiers: `domain.com` and  `*.domain.com` as follows:
```cs
var order = await acmeClient.NewOrderAsync(account, new List<string> { "domain.com", "*.domain.com" });
```

### Authorization

Wildcard certificates must by authorized by **DNS challenge** only. So go one by one and create DNS TXT record. 
```cs
var challenges = await acmeClient.GetDnsChallenges(account, order);

foreach (var challenge in challenges)
{  
    var dnsText = challenge.VerificationValue;
    // value can be e.g.: eBAdFvukOz4Qq8nIVFPmNrMKPNlO8D1cr9bl8VFFsJM

    // Create DNS TXT record e.g.:
    // key: _acme-challenge.your.domain.com 
    // value: eBAdFvukOz4Qq8nIVFPmNrMKPNlO8D1cr9bl8VFFsJM
}
```

##### Example no.1: 

You want to generate simple certificate for:
* `domain.com`
  
DNS TXT must contains 1 record:
* key: **_acme-challenge.domain.com**, value : dnsText of challenge for `domain.com`

##### Example no.2: 

You want to generate simple certificate with these subject names:
* `domain.com`
* `blog.domain.com` 
  
DNS TXT must contains 2 records :
* key: **_acme-challenge.domain.com**, value : dnsText of challenge for `domain.com`
* key: **_acme-challenge.blog.domain.com**, value : dnsText of challenge for `blog.domain.com` 

##### Example no.3: 

You want to generate wildcard certificate with these subject names:
* `domain.com`
* `*.domain.com` 
  
DNS TXT must contains 2 records:
* key: **_acme-challenge.domain.com**, value : dnsText of challenge for `domain.com`
* key: **_acme-challenge.domain.com**, value : dnsText of challenge for `*.domain.com`

**Yes, `*.domain.com` has the same key as `domain.com` !!!**

### Validation

All challenges must be validated:

```cs
foreach (var challenge in challenges)
{
    // Do a validation
    await acmeClient.ValidateChallengeAsync(account, challenge);

    // Verify status 
    var freshChallenge = await acmeClient.GetChallengeAsync(account, challenge);
    if (freshChallenge.Status == ChallengeStatus.Invalid)
    {
        throw new Exception("Something is wrong with your DNS TXT record(s)!");
    }
}
```

### Certificate

Finally, generate certificate:

```cs
var certificate = await acmeClient.GenerateCertificateAsync(account, order, "domain.com");
var password = "YourSuperSecretPassword";

// Generate certificate in pfx format
var pfx = certificate.GeneratePfx(password);

// Generate certificate in crt format
var crt = certificate.GenerateCrt(password);

// Generate certificate in PEM format 
var crtPem = certificate.GenerateCrtPem(password);

// Generate certificate private key in PEM format 
var keyPem = certificate.GenerateKeyPem();
```

**Enjoy! Any feedback is highly appreciated!**

---

## LetsEncrypt.ConsoleApp

Add your correct values to **.config** file :

```xml
<?xml version="1.0" encoding="utf-8" ?>
<configuration>
    <appSettings>
        <add key="ContactEmail" value="your@email.com" />
        <add key="Domains" value="domain.com, *.domain.com" />
        <add key="CertificateFileName" value="Domain.com" />
        <add key="CertificatePassword" value="YourSuperSecretPassword" />
    </appSettings>
</configuration>
```
and run console application **LetsEncrypt.ConsoleApp.exe**

**Enjoy!**

