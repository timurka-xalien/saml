﻿using ComponentSpace.SAML2;
using ComponentSpace.SAML2.Configuration;
using System.Configuration;
using System;

namespace Cameyo.SamlPoc.WebApp.Services
{
    public static class SamlConfigurationManager
    {
        private const string ServiceProviderName = "saml:ServiceProvider:Name";
        private const string ServiceProviderDescription = "saml:ServiceProvider:Description";
        private const string ServiceProviderAssertionConsumerServiceUrl = "saml:ServiceProvider:AssertionConsumerServiceUrl";
        private const string ServiceProviderLocalCertificateFile = "saml:ServiceProvider:LocalCertificateFile";
        private const string ServiceProviderLocalCertificatePassword = "saml:ServiceProvider:LocalCertificatePassword";

        public static void ConfigureIdentityProviders(SamlIdentityProvidersRepository repository)
        {
            SAMLConfiguration samlConfiguration = new SAMLConfiguration();

            ConfigureServiceProvider(samlConfiguration);

            ConfigureIdentityProvidersUsingRepository(samlConfiguration, repository);
            // ConfigureIdentityProvidersUsingHardCodedConfiguration(samlConfiguration);

            SAMLController.Configuration = samlConfiguration;
        }

        private static void ConfigureServiceProvider(SAMLConfiguration samlConfiguration)
        {
            samlConfiguration.LocalServiceProviderConfiguration = new
                LocalServiceProviderConfiguration()
            {
                Name = ConfigurationManager.AppSettings[ServiceProviderName],
                Description = ConfigurationManager.AppSettings[ServiceProviderDescription],
                AssertionConsumerServiceUrl = ConfigurationManager.AppSettings[ServiceProviderAssertionConsumerServiceUrl],
                LocalCertificateFile = ConfigurationManager.AppSettings[ServiceProviderLocalCertificateFile],
                LocalCertificatePassword = ConfigurationManager.AppSettings[ServiceProviderLocalCertificatePassword]
            };
        }

        private static void ConfigureIdentityProvidersUsingRepository(
            SAMLConfiguration samlConfiguration, 
            SamlIdentityProvidersRepository repository)
        {
            var providers = repository.GetRegisteredIdentityProviders();

            foreach (var provider in providers)
            {
                samlConfiguration.AddPartnerIdentityProvider(
                 new PartnerIdentityProviderConfiguration()
                 {
                     Name = provider.Name,
                     Description = provider.Description,
                     SignAuthnRequest = provider.SignAuthnRequest,
                     SingleSignOnServiceUrl = provider.SingleSignOnUrl,
                     SingleLogoutServiceUrl = provider.SingleLogoutUrl,
                     PartnerCertificateFile = provider.CertificateFile,
                     UseEmbeddedCertificate = provider.UseEmbeddedCertificate,
                     DisableInboundLogout = !provider.SingleLogoutSupported,
                     DisableOutboundLogout = !provider.SingleLogoutSupported
                 });
            }
        }

        private static void ConfigureIdentityProvidersUsingHardcodedConfiguration(SAMLConfiguration samlConfiguration)
        {
            samlConfiguration.AddPartnerIdentityProvider(
             new PartnerIdentityProviderConfiguration()
             {
                 Name = "http://cone-idp",
                 Description = "Cone Identity Provider",
                 SignAuthnRequest = true,
                 SingleSignOnServiceUrl = "http://cone-idp/SAML/SSOService",
                 SingleLogoutServiceUrl = "http://cone-idp/SAML/SLOService",
                 PartnerCertificateFile = "Certificates\\idp.cer"
             });

            samlConfiguration.AddPartnerIdentityProvider(
                new PartnerIdentityProviderConfiguration()
                {
                    Name = "https://shib-idp/",
                    Description = "Shibboleth Identity Provider",
                    SignAuthnRequest = true,
                    SingleSignOnServiceUrl = "https://shib-idp/SAML/SSOService.aspx?binding=redirect",
                    PartnerCertificateFile = "Certificates\\idp.cer",
                    DisableInboundLogout = true,
                    DisableOutboundLogout = true
                });

            samlConfiguration.AddPartnerIdentityProvider(
                 new PartnerIdentityProviderConfiguration()
                 {
                     Name = "http://kentor-idp/Metadata",
                     Description = "Kentor Identity Provider",
                     SignAuthnRequest = true,
                     SingleSignOnServiceUrl = "http://kentor-idp/",
                     SingleLogoutServiceUrl = "http://kentor-idp/Logout",
                     UseEmbeddedCertificate = true
                 });
        }
    }
}