import { NgModule } from '@angular/core';
import { Routes, RouterModule } from '@angular/router';
import { AuthGuard } from './guards/auth.guard';
import { FilterAddComponent } from './components/filter/filter-add/filter-add.component';
import { FilterEditComponent } from './components/filter/filter-edit/filter-edit.component';
import { HomeComponent } from './components/home/home.component';
import { LoginComponent } from './components/login/login.component';
import { StatisticsComponent } from './components/statistics/statistics.component';

const routes: Routes = [
  { path: 'home', component: HomeComponent, canActivate: [AuthGuard] },
  { path: 'add', component: FilterAddComponent, canActivate: [AuthGuard] },
  { path: 'stats', component: StatisticsComponent, canActivate: [AuthGuard] },
  { path: 'edit/:id', component: FilterEditComponent, canActivate: [AuthGuard] },
  { path: 'login', component: LoginComponent },
  { path: '', component: HomeComponent, canActivate: [AuthGuard] },
];

@NgModule({
  imports: [RouterModule.forRoot(routes)],
  exports: [RouterModule]
})
export class AppRoutingModule { }
