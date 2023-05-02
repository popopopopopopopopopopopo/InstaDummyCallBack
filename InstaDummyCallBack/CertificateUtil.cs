using System;
using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;

public static class CertificateUtil
{
    public static X509Certificate2 CreateSelfSignedCertificate(string subjectName)
    {
        var rsa = RSA.Create(2048);
        var request = new CertificateRequest($"cn={subjectName}", rsa, HashAlgorithmName.SHA256, RSASignaturePadding.Pkcs1);

        // Set certificate properties
        request.CertificateExtensions.Add(
            new X509BasicConstraintsExtension(false, false, 0, true));

        var sanBuilder = new SubjectAlternativeNameBuilder();
        sanBuilder.AddDnsName(subjectName);
        request.CertificateExtensions.Add(sanBuilder.Build());

        var keyUsage = new X509KeyUsageExtension(X509KeyUsageFlags.KeyEncipherment | X509KeyUsageFlags.DigitalSignature, false);
        request.CertificateExtensions.Add(keyUsage);

        var enhancedKeyUsage = new X509EnhancedKeyUsageExtension(
            new OidCollection { new Oid("1.3.6.1.5.5.7.3.1") }, true);
        request.CertificateExtensions.Add(enhancedKeyUsage);

        // Sign the certificate with itself (root CA)
        var certificate = request.CreateSelfSigned(DateTimeOffset.Now, DateTimeOffset.Now.AddYears(1));

        // // Set friendly name
        // certificate.FriendlyName = friendlyName;

        // Export to PFX format and save to disk
        byte[] pfxBytes = certificate.Export(X509ContentType.Pfx);
        var certFile = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "cert.pfx");
        if (File.Exists(certFile))
        {
            File.Delete(certFile);
        }
        
        System.IO.File.WriteAllBytes(certFile, pfxBytes);

        // Load certificate from PFX file
        return new X509Certificate2(certFile, (string)null, X509KeyStorageFlags.MachineKeySet);
    }
}