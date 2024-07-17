using System;
using System.IO;
using System.Security;
using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;
using System.Threading.Tasks;

namespace Bulk_Uploader_Electron;

    public class Certificate
    {
        public static void Certify()
        {
            CreateCertificateForServerAuthentication();

            CreateCertificateForClientAuthentication();
        }

        private static void CreateCertificateForServerAuthentication()
        {
            // Generate private-public key pair
            var rsaKey = RSA.Create(2048);

            // Describe certificate
            string subject = "CN=localhost";

            // Create certificate request
            var certificateRequest = new CertificateRequest(
                subject,
                rsaKey,
                HashAlgorithmName.SHA256,
                RSASignaturePadding.Pkcs1
            );

            certificateRequest.CertificateExtensions.Add(
                new X509BasicConstraintsExtension(
                    certificateAuthority: false,
                    hasPathLengthConstraint: false,
                    pathLengthConstraint: 0,
                    critical: true
                )
            );

            certificateRequest.CertificateExtensions.Add(
                new X509KeyUsageExtension(
                    keyUsages:
                        X509KeyUsageFlags.DigitalSignature
                        | X509KeyUsageFlags.KeyEncipherment,
                    critical: false
                )
            );

            certificateRequest.CertificateExtensions.Add(
                new X509SubjectKeyIdentifierExtension(
                    key: certificateRequest.PublicKey,
                    critical: false
                )
            );

            certificateRequest.CertificateExtensions.Add(
                new X509Extension(
                    new AsnEncodedData(
                        "Subject Alternative Name",
                        new byte[] { 48, 11, 130, 9, 108, 111, 99, 97, 108, 104, 111, 115, 116 }
                    ),
                    false
                )
            );

            var expireAt = DateTimeOffset.Now.AddYears(5);

            var certificate = certificateRequest.CreateSelfSigned(DateTimeOffset.Now, expireAt);

            // Export certificate with private key
            var exportableCertificate = new X509Certificate2(
                certificate.Export(X509ContentType.Cert),
                (string)null,
                X509KeyStorageFlags.Exportable | X509KeyStorageFlags.PersistKeySet
            ).CopyWithPrivateKey(rsaKey);

            exportableCertificate.FriendlyName = "Certificate For Server Authorization";

            // Create password for certificate protection
            var passwordForCertificateProtection = new SecureString();
            foreach (var @char in "p@ssw0rd")
            {
                passwordForCertificateProtection.AppendChar(@char);
            }

            // Export certificate to a file.
            File.WriteAllBytes(
                "certificateForServerAuthorization.pfx",
                exportableCertificate.Export(
                    X509ContentType.Pfx,
                    passwordForCertificateProtection
                )
            );

            // Test correctness of export
            var loadedCertificate = new X509Certificate2("certificateForServerAuthorization.pfx", passwordForCertificateProtection);

            Console.WriteLine(loadedCertificate.FriendlyName);
        }

        private static void CreateCertificateForClientAuthentication()
        {
            // Generate private-public key pair
            var rsaKey = RSA.Create(2048);

            // Describe certificate
            string subject = "CN=Ivan Yakimov";

            // Create certificate request
            var certificateRequest = new CertificateRequest(
                subject,
                rsaKey,
                HashAlgorithmName.SHA256,
                RSASignaturePadding.Pkcs1
            );

            certificateRequest.CertificateExtensions.Add(
                new X509BasicConstraintsExtension(
                    certificateAuthority: false,
                    hasPathLengthConstraint: false,
                    pathLengthConstraint: 0,
                    critical: true
                )
            );

            certificateRequest.CertificateExtensions.Add(
                new X509KeyUsageExtension(
                    keyUsages:
                        X509KeyUsageFlags.DigitalSignature
                        | X509KeyUsageFlags.KeyEncipherment,
                    critical: false
                )
            );

            certificateRequest.CertificateExtensions.Add(
                new X509SubjectKeyIdentifierExtension(
                    key: certificateRequest.PublicKey,
                    critical: false
                )
            );

            var expireAt = DateTimeOffset.Now.AddYears(5);

            var certificate = certificateRequest.CreateSelfSigned(DateTimeOffset.Now, expireAt);

            // Export certificate with private key
            var exportableCertificate = new X509Certificate2(
                certificate.Export(X509ContentType.Cert),
                (string)null,
                X509KeyStorageFlags.Exportable | X509KeyStorageFlags.PersistKeySet
            ).CopyWithPrivateKey(rsaKey);

            exportableCertificate.FriendlyName = "Certificate For Client Authorization";

            // Create password for certificate protection
            var passwordForCertificateProtection = new SecureString();
            foreach (var @char in "p@ssw0rd")
            {
                passwordForCertificateProtection.AppendChar(@char);
            }

            // Export certificate to a file.
            File.WriteAllBytes(
                "certificateForClientAuthorization.pfx",
                exportableCertificate.Export(
                    X509ContentType.Pfx,
                    passwordForCertificateProtection
                )
            );

            // Test correctness of export
            var loadedCertificate = new X509Certificate2("certificateForClientAuthorization.pfx", passwordForCertificateProtection);

            Console.WriteLine(loadedCertificate.FriendlyName);
        }
    }