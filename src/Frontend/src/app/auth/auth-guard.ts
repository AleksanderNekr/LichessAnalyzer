import {
  ActivatedRouteSnapshot,
  CanActivateChildFn,
  CanActivateFn,
  Router,
  RouterStateSnapshot
} from "@angular/router";
import { inject } from "@angular/core";
import { AuthService } from "./auth.service";

export const canActivate: CanActivateFn = () => {
  const authService = inject(AuthService);
  const router = inject(Router);

  return authService.isAuthenticated() ? true : router.createUrlTree([ 'account/login' ])
};

export const canActivateChild: CanActivateChildFn =
  (route: ActivatedRouteSnapshot, state: RouterStateSnapshot) => canActivate(route, state);
