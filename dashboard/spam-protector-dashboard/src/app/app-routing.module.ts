import { NgModule } from '@angular/core';
import { Routes, RouterModule } from '@angular/router';
import { FilterSendersComponent } from './components/filter/filter-senders/filter-senders.component';
import { AuthGuard } from './guards/auth.guard';
import { FilterSubjectsComponent } from './components/filter/filter-subjects/filter-subjects.component';
import { FilterDomainsComponent } from './components/filter/filter-domains/filter-domains.component';
import { FilterAddComponent } from './components/filter/filter-add/filter-add.component';
import { FilterEditComponent } from './components/filter/filter-edit/filter-edit.component';
import { HomeComponent } from './components/home/home.component';
import { LoginComponent } from './components/login/login.component';

const routes: Routes = [
  { path: 'home', component: HomeComponent, canActivate: [AuthGuard] },
  { path: 'add', component: FilterAddComponent, canActivate: [AuthGuard] },
  { path: 'edit/:id', component: FilterEditComponent, canActivate: [AuthGuard] },
  { path: 'login', component: LoginComponent },
  { path: '', component: HomeComponent, canActivate: [AuthGuard] },
];

@NgModule({
  imports: [RouterModule.forRoot(routes)],
  exports: [RouterModule]
})
export class AppRoutingModule { }
