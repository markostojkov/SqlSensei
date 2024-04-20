import { Injectable } from '@angular/core';
import { MsalService, MsalBroadcastService } from '@azure/msal-angular';
import { InteractionStatus, AuthenticationResult, SilentRequest } from '@azure/msal-browser';
import { Subject, filter, takeUntil } from 'rxjs';
import { loginRequest } from './b2c-config';

@Injectable({
  providedIn: 'root',
})
export class AuthService {
  private readonly destroying$ = new Subject<void>();

  public constructor(private authService: MsalService, private msalBroadcastService: MsalBroadcastService) {}

  public initializeAuth(): void {
    this.msalBroadcastService.msalSubject$.subscribe((response) => {
      if (response.error && (response.error.message.includes('AADB2C90091') || response.error.message.includes('AADSTS700084'))) {
        this.authService.loginRedirect();
      }
    });

    this.msalBroadcastService.inProgress$
      .pipe(
        filter((status: InteractionStatus) => status === InteractionStatus.None),
        takeUntil(this.destroying$)
      )
      .subscribe(() => {
        this.checkAndSetActiveAccount();
      });
  }

  public logout(): void {
    const acc = this.authService.instance.getActiveAccount();

    if (acc) {
      const req: SilentRequest = {
        account: acc,
        scopes: loginRequest.scopes,
      };

      this.authService.acquireTokenSilent(req).subscribe({
        next: (result: AuthenticationResult) => {
          this.authService.logoutRedirect({ idTokenHint: result.idToken, account: acc });
        },
      });
    }
  }

  public onDestroy(): void {
    this.destroying$.next(undefined);
    this.destroying$.complete();
  }

  private checkAndSetActiveAccount(): void {
    const activeAccount = this.authService.instance.getActiveAccount();

    if (!activeAccount && this.authService.instance.getAllAccounts().length > 0) {
      const accounts = this.authService.instance.getAllAccounts();
      this.authService.instance.setActiveAccount(accounts[0]);
    }
  }
}
