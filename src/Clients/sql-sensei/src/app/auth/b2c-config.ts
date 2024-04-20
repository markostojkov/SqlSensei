import { BrowserCacheLocation, Configuration, LogLevel } from '@azure/msal-browser';
import { environment } from '../../environments/environment';

export const b2cPolicies = {
  names: {
    signIn: 'B2C_1_SignUpSignIn',
  },
  authorities: {
    signIn: {
      authority: 'https://sqlsensei.b2clogin.com/sqlsensei.onmicrosoft.com/B2C_1_SignUpSignIn',
    },
  },
  authorityDomain: 'sqlsensei.b2clogin.com',
};

export const msalConfig: Configuration = {
  auth: {
    clientId: 'f4a73c76-91f0-4551-9d24-73d39724fa9d',
    authority: b2cPolicies.authorities.signIn.authority,
    knownAuthorities: [b2cPolicies.authorityDomain],
    redirectUri: '/auth',
  },
  cache: {
    cacheLocation: BrowserCacheLocation.LocalStorage,
  },
  system: {
    allowRedirectInIframe: true,
    loggerOptions: {
      loggerCallback(logLevel: LogLevel, message: string): void {
        console.log(message);
      },
      logLevel: LogLevel.Error,
      piiLoggingEnabled: !environment.production,
    },
  },
};

export const apiScopes = ['https://sqlsensei.onmicrosoft.com/ce1ad9c4-bd03-495f-853c-c2d2a3cef411/SqlSensei.Full'];

export const loginRequest = {
  scopes: ['openid', 'profile', 'email', ...apiScopes],
  prompt: 'login',
};
