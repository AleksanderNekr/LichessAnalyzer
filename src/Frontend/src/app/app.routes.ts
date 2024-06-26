import { Routes } from '@angular/router';
import { HomeComponent } from "./home/home.component";
import { LoginComponent } from "./login/login.component";
import { NotFoundComponent } from "./not-found/not-found.component";
import { AccountComponent } from "./account/account.component";
import { canActivate, canActivateChild } from "./auth/auth-guard";
import { AnalyticsListsComponent } from "./analytics-lists/analytics-lists.component";

export const routes: Routes = [
  { path: "", component: HomeComponent, pathMatch: "full" },
  { path: "account/login", component: LoginComponent },
  {
    path: "account/profile",
    component: AccountComponent,
    canActivate: [ canActivate ],
    canActivateChild: [ canActivateChild ]
  },
  { path: "lists", component: AnalyticsListsComponent },
  { path: "**", component: NotFoundComponent, pathMatch: "full" }
];
