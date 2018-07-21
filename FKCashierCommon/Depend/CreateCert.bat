@echo off
del FKSSLCertSite.cer
del FKSSLCertSite.pfx
del FKSSLCertSite.pvk
del FKSSLCert.cer
del FKSSLCert.pvk
makecert -n "CN=FKSSLCert" -r -sv .\FKSSLCert.pvk .\FKSSLCert.cer
makecert -iv .\FKSSLCert.pvk -n "CN=FKSSLCertSite" -sv .\FKSSLCertSite.pvk -ic .\FKSSLCert.cer .\FKSSLCertSite.cer -sr LocalMachine -ss My -sky exchange -pe
pause