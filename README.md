# Let's Encrypt C# library

Solution consist of 2 projects:
* **LetsEncrypt.Core** (.Net Standard Library - available as [nuget package](https://TODO))
* **LetsEncrypt.ConsoleApp** (.Net Core Console application)

#### LetsEncrypt.Core

LetsEncrypt.Core is simple and straightforward C# implementation of [ACME](https://en.wikipedia.org/wiki/Automated_Certificate_Management_Environment) client for [Let's Encrypt](https://letsencrypt.org/) certificates. Library is based on **.NET Standard 2.1+**.
It uses Let's Encrypt **v2 API** and this library is primary oriented for generation of **wildcard** certificates as .pfx. 

#### LetsEncrypt.ConsoleApp

LetsEncrypt.ConsoleApp is C# implementation|usage of previous LetsEncrypt.Core library based on **.NET Core 3.0**. It is simple **console application** which generates Let's Encrypt certificates. 


## LetsEncrypt.Core

#### Usage

Add [LetsEncrypt.Core](https://TODO) as nuget package (or manual **.dll reference**) to your project.

First step is to create client object. Then initialize enviroment ([staging](https://letsencrypt.org/docs/staging-environment/) or production ... use staging environment first to avoid [rate limits](https://letsencrypt.org/docs/rate-limits/)). And generate new or use existing comunication (private/public) key pair:

```cs
var acmeClient = new AcmeClient();
await acmeClient.InitAsync(ApiEnvironment.LetsEncryptV2Staging);
acmeClient.GenerateKeyPair();
```

... and let's start:

#### Account

Create new account: 
```cs
var account = await acmeClient.NewAccountAsync("your@email.com");
```

#### Order

When you want to generate wildcard certificate, I recomend to specify these 2 identifiers: `your.domain.com` and  `*.your.domain.com` as follows:
```cs
var order = await acmeClient.NewOrderAsync(account, new List<string> { "your.domain.com", "*.your.domain.com" });
```

#### Authorization

Wildcard certificates must by authorized by **DNS challange** only. So go one by one and create DNS TXT record. 
```cs
var challanges = await acmeClient.GetDnsChallenges(account, order);

foreach (var challange in challanges)
{  
    var dnsText = challange.VerificationValue;
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
* key: **_acme-challenge.domain.com**, value : dnsText of challange for `domain.com`

##### Example no.2: 

You want to generate simple certificate with these subject names:
* `domain.com`
* `blog.domain.com` 
  
DNS TXT must contains 2 records :
* key: **_acme-challenge.domain.com**, value : dnsText of challange for `domain.com`
* key: **_acme-challenge.blog.domain.com**, value : dnsText of challange for `blog.domain.com` 

##### Example no.3: 

You want to generate wildcard certificate with these subject names:
* `domain.com`
* `*.domain.com` 
  
DNS TXT must contains 2 records:
* key: **_acme-challenge.domain.com**, value : dnsText of challange for `domain.com`
* key: **_acme-challenge.domain.com**, value : dnsText of challange for `*.domain.com`

**Yes, `*.domain.com` has the same key as `domain.com` !!!**

---


#### Validation

All chalanges must be validated:

```cs
foreach (var challange in challanges)
{
    // Do a validation
    await acmeClient.ValidateChallengeAsync(account, challange);

    // Verify status 
    var freshChallange = await acmeClient.GetChallengeAsync(account, challange);
    if (freshChallange.Status == ChallengeStatus.Invalid)
    {
        throw new Exception("Something is wrong with your DNS TXT record(s)!");
    }
}
```

#### Certificate

Finally, generate certificate:

```cs
var certificate = await acmeClient.GenerateCertificateAsync(account, order, "your.domain.com", "YourSuperSecretPasswordToCertificate");

// Generate byte[] of certificate in pfx format
var pfx = certificate.GeneratePfx();
// Generate byte[] of certificate in crt format
var crt = certificate.GeneratePfx();
// Generate string of certificate in PEM format 
string crtPem = certificate.GenerateCrtPem();
// Generate string of certificate priate key in PEM format 
string keyPem = certificate.GenerateKeyPem();
```

**Enjoy! Any feedback is highly appreciated!**

---

## LetsEncrypt.ConsoleApp
...

